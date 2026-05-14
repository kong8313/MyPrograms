using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public class SampleService : ISampleService
    {
        private readonly IReplicationService _replicationService;
        private readonly ISystemSettings _systemSettings;
        private readonly IRetryingService _retryingService;
        private readonly IRespondentBatchObtainer _respondentBatchObtainer;
        private readonly ISchedulingScriptNotificator _schedulingScriptNotificator;
        private readonly IRemoteDataCopier _remoteDataCopier;
        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;

        /// <summary>
        /// Gets the timeout (in seconds) for sample upload SQL commands.
        /// </summary>
        public const int SampleCommandExecutionTimeout = 10 * 60;

        public void AddSampleRecord(
            int batchId,
            int surveySid,
            ProcessSampleMode processSampleMode,
            ProcessSampleAsyncResult sampleState)
        {
            using (var tx = new DatabaseTransactionScope("ProcessSample"))
            {
                BvSamplesAdapter.DeleteByCondition(
                    "BatchID = @BatchID AND SampleType = @SampleType",
                    new SqlParameter("@BatchID", batchId),
                    new SqlParameter("@SampleType", (int)processSampleMode));

                var sampleEntity = new BvSamplesEntity
                {
                    BatchID = batchId,
                    State = (int)sampleState,
                    StateDescription = "",
                    SurveySID = surveySid,
                    CountInterviews = 0,
                    StartedTime = DateTime.UtcNow,
                    SampleType = (int)processSampleMode
                };

                if (sampleState == ProcessSampleAsyncResult.Success)
                {
                    sampleEntity.FinishedTime = sampleEntity.StartedTime;
                }

                BvSamplesAdapter.Insert(sampleEntity);

                tx.Commit();
            }
        }
        
        public SampleService(
            IReplicationService replicationService,
            ISystemSettings systemSettings,
            IRetryingService retryingService,
            IRespondentBatchObtainer respondentBatchObtainer,
            ISchedulingScriptNotificator schedulingScriptNotificator,
            IRemoteDataCopier remoteDataCopier,
            ISurveyConnectionStringProvider surveyConnectionStringProvider,
            ISurveyDatabaseEngine surveyDatabaseEngine)
        {
            _replicationService = replicationService;
            _systemSettings = systemSettings;
            _retryingService = retryingService;
            _respondentBatchObtainer = respondentBatchObtainer;
            _schedulingScriptNotificator = schedulingScriptNotificator;
            _remoteDataCopier = remoteDataCopier;
            _surveyConnectionStringProvider = surveyConnectionStringProvider;
            _surveyDatabaseEngine = surveyDatabaseEngine;
        }

        public void ProcessSample(BvSurveyEntity survey, int batchId, ProcessSampleMode processSampleMode, SchedulingMode schedulingMode,
            Action<string> taskLog, Action<int, int, int> updateProgress, AsyncOperationResult result, CancellationToken cancellationToken)
        {
            var evt = new ProcessSampleEvent
            {
                Details =
                {
                    ProjectdId = survey.ProjectId,
                    ProjectName = survey.Description,
                    BatchId = batchId,
                    ProcessSampleMode = processSampleMode,
                    SchedulingMode = schedulingMode
                }
            };

            using (new EventDetailsScope(evt.Details))
            {
                var context = new SampleContext
                {
                    Survey = survey,
                    BatchId = batchId,
                    ProcessSampleMode = processSampleMode,
                    SchedulingMode = schedulingMode,
                    AddedRecords = 0,
                    PartitionSize = _systemSettings.AsyncOperation.AddSamplePortionSize,
                    IgnoredItsByFcd = GetIgnoredItsByFCD(survey.StateGroupID),
                    EventDetails = evt.Details,
                    TimeZoneReolver = new TimezoneResolver(),
                    SampleDataStorageRepository = ServiceLocator.Resolve<ISampleDataStorageRepository>(),
                    StateContainer = new SampleProcessingStateContainer(survey.SID, batchId),
                    RespondentBatchObtainer = _respondentBatchObtainer
                };

                ProcessSample(context, taskLog, updateProgress, result, cancellationToken);

                evt.Details.ProcessedRecords = context.AddedRecords;

                _schedulingScriptNotificator.Notify(context.SchedulingScriptNotificatorExceptions, batchId, survey.SID, survey.ScheduleID);

                evt.Finish();
            }
        }

        private void ReplicateRespondentsData(SampleContext context, CancellationToken cancellationToken)
        {
            if (context.ProcessSampleMode == ProcessSampleMode.Add)
            {
                // Insert data in to the replication tables
                // We're NOT inserting data in the replication procedure, just in the sample addition.
                _replicationService.UploadSampleDataToReplicatedTable(context.Survey.SID, context.BatchId, cancellationToken);
                context.EventDetails.AddTiming("UploadSampleDataToReplicatedTable");

            }
            else
            {
                // Once sample is updated we need to replicate new values
                _replicationService.RunForceReplication(context.Survey.SID, cancellationToken);
                context.EventDetails.AddTiming("RunForceReplication");
            }
        }

        private void ProcessSampleInBatches(SampleContext context, Action<string> taskLog, Action<int, int, int> updateProgress, AsyncOperationResult result, 
            CancellationToken cancellationToken, ref bool hasFailedPartition, ref bool hasSuccessfulPartition)
        {
            var startRangeOfInterviewId = 0;
            var currentPartition = 0;
            
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    hasFailedPartition = true;
                    taskLog("Cancelling operation...");
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var processor = ServiceLocator.Resolve<ISampleBatchProcessor>();
                currentPartition++;
                taskLog($"Uploading interview {startRangeOfInterviewId}...");

                try
                {
                    _retryingService.Retry("Processing sample batch", () => { processor.Process(context, startRangeOfInterviewId); });

                    result.ProcessedItemsCount = context.AddedRecords;

                    hasSuccessfulPartition = true;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        "ProcessAddSample operation with batchID = {0}, mode = {1}, recordsCount = {2}, SID = {3}. Add partition {4} with size = {5} of batch is failed: {6}",
                        context.BatchId,
                        context.SchedulingMode,
                        context.RecordsCount,
                        context.Survey.SID,
                        currentPartition,
                        processor.Records == null ? "<NULL>" : processor.Records.Length.ToString(),
                        ex);

                    taskLog($"Uploading interviews {startRangeOfInterviewId} - {startRangeOfInterviewId + processor.Records?.Length ?? 0} failed. Error: {ex.Message}");

                    result.FailedItemsCount += processor.Records?.Length ?? 0;
                    hasFailedPartition = true;
                }

                updateProgress(context.AddedRecords + result.FailedItemsCount, context.AddedRecords, result.FailedItemsCount);

                if (processor.Records == null)
                {
                    break;
                }

                if (processor.Records.Length < context.PartitionSize)
                {
                    break;
                }

                startRangeOfInterviewId = processor.Records.Max(x => x.InterviewId) + 1;
            }
        }

        private void ProcessSample(SampleContext context, Action<string> taskLog, Action<int, int, int> updateProgress, AsyncOperationResult result, CancellationToken cancellationToken)
        {
            var hasFailedPartition = false;
            var hasSuccessfulPartition = false;

            try
            {
                ReplicateRespondentsData(context, cancellationToken);

                ProcessSampleInBatches(context, taskLog, updateProgress, result, cancellationToken, ref hasFailedPartition, ref hasSuccessfulPartition);
            }
            catch (OperationCanceledException)
            {
                CleanFailedRecordsAndSetState(context, true, taskLog);
                throw;
            }

            context.EventDetails.AddTiming("Process Sample Cycle");

            if (!hasSuccessfulPartition)
            {
                DeleteFullBatchFromReplicatedTable(context);
                throw new Exception("all sample partitions were failed");
            }

            CleanFailedRecordsAndSetState(context, hasFailedPartition, taskLog);
        }

        private void CleanFailedRecordsAndSetState(SampleContext context, bool hasFailedPartition, Action<string> taskLog)
        {

            SurveyService.UpdateLastTouchTime(context.Survey.SID);
            context.EventDetails.AddTiming("UpdateLastTouchTime");

            var warnings = new List<string>();

            if (context.TimeZoneReolver.HasError)
            {
                warnings.Add("Warning! Respondent data contained invalid timezone values, these have been assigned the default timezone value.");
            }

            if (hasFailedPartition)
            {
                var ranges = String.Join(",", GetRangesOfNotAddedInterviewsAndCleanReplicatedTable(context).Select(x => String.Format("[{0}-{1}]", x.Item1, x.Item2)));
                warnings.Add(String.Format("Warning! Sample is partially added. Following interviews weren't added: {0}", ranges));
            }

            if (context.StateContainer.AreInvalidRecordsFound())
            {
                warnings.Add("Warning! " + context.StateContainer.GetWarningMessage());
            }

            if (warnings.Count > 0)
            {
                var warningMessages = string.Join(Environment.NewLine, warnings);

                Trace.TraceWarning(
                    "ProcessSample operation with batchID = {0}, mode = {1}, recordsCount = {2}, SID ={3}. {4}",
                    context.BatchId,
                    context.SchedulingMode,
                    context.RecordsCount,
                    context.Survey.SID,
                    warningMessages);

                SetState(context.BatchId, context.ProcessSampleMode, ProcessSampleAsyncResult.Success, warningMessages, context.AddedRecords);
                taskLog(warningMessages);
            }
            else
            {
                taskLog($"Successfully uploaded {context.RecordsCount} interviews");
                SetState(context.BatchId, context.ProcessSampleMode, ProcessSampleAsyncResult.Success, String.Empty, context.AddedRecords);
            }
        }

        public ProcessSampleAsyncResult GetState(
            int batchId,
            ProcessSampleMode processSampleMode,
            out string stateDescription)
        {
            var sampleEntity = BvSamplesAdapter.GetByCondition(
                "BatchID = @BatchID AND SampleType = @SampleType",
                new SqlParameter("@BatchID", batchId),
                new SqlParameter("@SampleType", (int)processSampleMode)).FirstOrDefault();

            stateDescription = sampleEntity.StateDescription;

            return (ProcessSampleAsyncResult)sampleEntity.State;
        }
        
        public static void SetState(
            int batchId,
            ProcessSampleMode processSampleMode,
            ProcessSampleAsyncResult state,
            string stateDescription,
            int addedRecords = -1)
        {
            var sampleEntity = BvSamplesAdapter.GetByCondition(
                "BatchID = @BatchID AND SampleType = @SampleType",
                new SqlParameter("@BatchID", batchId),
                new SqlParameter("@SampleType", (int)processSampleMode)).FirstOrDefault();

            if (sampleEntity != null)
            {
                sampleEntity.State = (int)state;
                sampleEntity.StateDescription = stateDescription ?? sampleEntity.StateDescription;
                sampleEntity.FinishedTime = DateTime.UtcNow;
                if (addedRecords >= 0)
                    sampleEntity.CountInterviews = addedRecords;

                BvSamplesAdapter.Update(sampleEntity);
            }
        }

        private int[] GetIgnoredItsByFCD(int stateGroupId)
        {
            var ignoredItsByFcd =
                StateRepository.GetAll(stateGroupId)
                    .Where(x => x.FcdAction)
                    .Select(y => y.StateID)
                    .ToArray();
            return ignoredItsByFcd;
        }

        private void DeleteFullBatchFromReplicatedTable(SampleContext context)
        {
            var queryMinMax = $"SELECT MIN(respid) as minId, MAX(respid) as maxId FROM <Schema>.respondent WHERE batchId = {context.BatchId}";
            var resMinMax = _surveyDatabaseEngine.ExecuteQuery(context.Survey.SID, queryMinMax);
            var minId = resMinMax.Rows[0]["minId"];
            var maxId = resMinMax.Rows[0]["maxId"];
            var deleteQuery = $@"
                  DELETE FROM BvReplicatedData_{context.Survey.SID}
                         WHERE BvReplicatedData_{context.Survey.SID}.respid >= {minId} AND BvReplicatedData_{context.Survey.SID}.respid <= {maxId}
                  ";
            new DatabaseEngine().ExecuteNonQuery(deleteQuery, CommandType.Text);
        }

        private IEnumerable<Tuple<int, int>> GetRangesOfNotAddedInterviewsAndCleanReplicatedTable(SampleContext context)
        {
            var copyDataQuery = $"SELECT respid FROM <Schema>.respondent WHERE batchId = {context.BatchId} order by respid";
            var catiDbQuery = $@"
                  CREATE TABLE #failedIds(respid INT PRIMARY KEY)
                  INSERT INTO #failedIds SELECT respid FROM #ids
                         LEFT JOIN BvInterview i ON #ids.respid = i.ID AND i.SurveySID = {context.Survey.SID}
                         WHERE i.ID IS NULL 
                  DELETE FROM BvReplicatedData_{context.Survey.SID} 
                         FROM BvReplicatedData_{context.Survey.SID} r
                         JOIN #failedIds i ON r.respid = i.respid
                  SELECT respid from #failedIds ORDER BY respid
                  ";

            List<int> respIds;
            using (var connectionScope = new ConnectionScope())
            {
                var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(context.Survey.SID);
                _remoteDataCopier.CopyDataToNewTable(
                    surveyConnectionInfo.ConnectionString, connectionScope, "#ids", copyDataQuery, surveyConnectionInfo.SchemaName);
                respIds = new DatabaseEngine().ExecuteScalarList<int>(catiDbQuery, CommandType.Text);
            }

            var startRangeRespId = 0;
            var lastRespId = int.MinValue;
            foreach (var respId in respIds)
            {
                if (respId != lastRespId + 1)
                {
                    if (lastRespId != int.MinValue)
                    {
                        yield return Tuple.Create(startRangeRespId, lastRespId);
                    }

                    startRangeRespId = respId;
                }

                lastRespId = respId;
            }

            if (lastRespId != -1)
            {
                yield return Tuple.Create(startRangeRespId, lastRespId);
            }
        }
    }
}
