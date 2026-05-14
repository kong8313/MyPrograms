using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public class SampleBatchProcessor : ISampleBatchProcessor
    {
        private readonly ISampleRecordProcessorFactory _sampleRecordProcessorFactory;
        private readonly IFCDSettings _fcdSettings;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;

        public RespondentRecord[] Records { get; private set; }

        public SampleBatchProcessor(
            ISampleRecordProcessorFactory sampleRecordProcessorFactory,
            IFCDSettings fcdSettings,
            IDatabaseLockTimeouts databaseLockTimeouts)
        {
            _sampleRecordProcessorFactory = sampleRecordProcessorFactory;
            _fcdSettings = fcdSettings;
            _databaseLockTimeouts = databaseLockTimeouts;
        }

        public void Process(SampleContext context, int startRangeOfInterviewId)
        {
            using (
                var dbLock =
                    ExclusiveDatabaseLock.CreateLock(
                        DatabaseLockTimeoutsAndRecourceNames.GetFcdResourceName(context.Survey.SID), "SampleProc", _databaseLockTimeouts.DefaultLockTimeoutInMs))
            {
                dbLock.EnterLock();

                SampleService.SetState(context.BatchId, context.ProcessSampleMode, ProcessSampleAsyncResult.InProgress, "Fetching sample records");

                bool isUpdateMode = context.ProcessSampleMode == ProcessSampleMode.Update;

                Records = context.RespondentBatchObtainer.GetRespondentBatchPartition(
                    context.Survey,
                    context.BatchId,
                    startRangeOfInterviewId,
                    context.PartitionSize,
                    isUpdateMode);
                
                if (Records.Length <= 0)
                    return;

                SampleService.SetState(context.BatchId, context.ProcessSampleMode, ProcessSampleAsyncResult.InProgress, "Processing sample records");

                using (var storage = context.SampleDataStorageRepository.Create(
                           context.BatchId,
                           context.Survey,
                           startRangeOfInterviewId,
                           isUpdateMode))
                {
                    ProcessSampleRecords(context, storage, Records);

                    SampleService.SetState(context.BatchId, context.ProcessSampleMode, ProcessSampleAsyncResult.InProgress, "Saving sample data to database");

                    storage.Commit(context.EventDetails);
                }

                context.AddedRecords += Records.Length;
            }
        }

        private void ProcessSampleRecords(SampleContext context, ISampleDataStorage storage, IEnumerable<RespondentRecord> batchPartition)
        {
            var recordProcessor = _sampleRecordProcessorFactory.Create(context);

            foreach (var record in batchPartition)
            {
                record.TimeZoneId = context.TimeZoneReolver.Resolve(record.TimeZoneId);
                var interview = InterviewService.GetInterviewFromRespondentRecord(context.Survey.SID, context.BatchId, record, context);

                recordProcessor.Process(storage, context.StateContainer, record, interview, context.ProcessSampleMode);

                if (context.StateContainer.AreInvalidRecordsFound())
                {
                    Trace.TraceWarning(context.StateContainer.GetWarningMessage());
                }

                if (storage.Call != null)
                {
                    storage.Call.CellId = record.ClusteredCellId;

                    if (record.IsTelephoneInBlackList)
                    {
                        storage.Call = null;
                        storage.Interview.TransientState = (int)CallOutcome.Blacklist;
                    }
                    else if (IsInterviewFilteredByFcd(context, record, interview))
                    {
                        switch (_fcdSettings.AlgorithmType)
                        {
                            case FcdAlgorithmType.DeleteCalls:
                                storage.Call = null;
                                storage.Interview.TransientState = (int)CallOutcome.FilteredByCallDelivery;
                                break;
                            case FcdAlgorithmType.DisableCallsWithReenabling:
                                storage.Call.CallState = (int)CallState.DisabledByFCD;
                                storage.IsCallDisabledByFCD = true;
                                break;
                            default:
                                throw new Exception("Unknown FCD algorithm.");
                        }
                    }
                }

                storage.SaveCurrentRecord();
            }

            recordProcessor.OnCompleted();
        }

        private bool IsInterviewFilteredByFcd(SampleContext context, RespondentRecord record, BvInterviewEntity interview)
        {
            return record.IsClosedCell && !context.IgnoredItsByFcd.Contains(interview.TransientState);
        }
    }
}