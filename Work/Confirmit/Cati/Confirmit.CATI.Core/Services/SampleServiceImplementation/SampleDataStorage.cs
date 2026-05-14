using System;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    class SampleDataStorage : ISampleDataStorage
    {
        private readonly bool _isUpdateMode;
        public int BatchID { get; set; }
        public int SurveySID { get; set; }
        public bool IsRandomCallDeliveryEnabled { get; set; }
        
        public int StartRangeOfInterviewId { get; private set; }
        public int FinishRangeOfInterviewId { get; private set; }
        
        public SurveySchedulingMode SchedulingMode { get; set; }
       
        //if storage was created during asyncOperation
        public int OperationId { get; set; }

        //
        // temporary storage of data for one(current) sample record.
        //
        public BvCallEntity Call { get; set; }
        public BvInterviewEntity Interview { get; set; }

        public bool IsCallDisabledByFCD { get; set; }

        //
        // temporary storage of all data.
        //
        private DataTable m_SvyScheduleTable = BvSvyScheduleAdapter.CreateDataTable();
        private DataTable m_InterviewTable = BvInterviewAdapter.CreateDataTable();
        private DataTable m_CallHistoryExTable = BvCallHistoryExAdapter.CreateDataTable();

        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly IRemoteDataCopier _remoteDataCopier;
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;

        public SampleDataStorage(
            ISurveyConnectionStringProvider surveyConnectionStringProvider,
            IRemoteDataCopier remoteDataCopier,
            ISurveyDatabaseEngine surveyDatabaseEngine,
            int batchID, 
            int surveySID, 
            SurveySchedulingMode schedulingMode, 
            bool isRandomCallDeliveryEnabled, 
            int startRangeOfInterviewId,
            bool isUpdateMode)
        {
            _surveyConnectionStringProvider = surveyConnectionStringProvider;
            _remoteDataCopier = remoteDataCopier;
            _surveyDatabaseEngine = surveyDatabaseEngine;

            _isUpdateMode = isUpdateMode;

            BatchID = batchID;
            SurveySID = surveySID;
            SchedulingMode = schedulingMode;
            IsRandomCallDeliveryEnabled = isRandomCallDeliveryEnabled;
            StartRangeOfInterviewId = startRangeOfInterviewId;

            if (isUpdateMode)
            {
                m_SvyScheduleTable.TableName = CreateUpdateTableName(m_SvyScheduleTable.TableName);
                m_InterviewTable.TableName = CreateUpdateTableName(m_InterviewTable.TableName);
                m_CallHistoryExTable.TableName = CreateUpdateTableName(m_CallHistoryExTable.TableName);
            }
        }

        public string CreateUpdateTableName(string tableName)
        {
            string date = DateTime.UtcNow.ToString("yyyy_mm_dd_ss");
            return "Update_" + date + "_" + tableName;
        }

        public void InsertInterview(BvInterviewEntity interview)
        {
            if (Interview != null)
            {
                throw new InternalErrorException(String.Format(
                                                     "Interview with SurveySID:{0}and ID = {1} already inserted. Inserted interview with SurveySID={2} and ID = {3}",
                                                     Interview.SurveySID, Interview.ID, interview.SurveySID, interview.ID));
            }

            if (interview.SurveySID != SurveySID)
            {
                throw new InternalErrorException(String.Format(
                                                     "Inserted interview have SurveySID = {0} and ID = {1}, but SampleDataStorage was created for SurveySID = {2}",
                                                     interview.SurveySID, interview.ID, SurveySID));
            }

            Interview = interview;
            FinishRangeOfInterviewId = Math.Max(FinishRangeOfInterviewId, interview.ID);
        }

        public void UpdateInterview(BvInterviewEntity interview)
        {
            if (Interview == null)
            {
                throw new InternalErrorException(String.Format(
                                                     "Interview with SurveySID = {0}, ID = {1} not found.",
                                                     interview.SurveySID, interview.ID));
            }

            if (Interview.SurveySID != interview.SurveySID || Interview.ID != interview.ID)
            {
                throw new InternalErrorException(String.Format(
                                                     "Current interview have SurveySID = {0}, ID = {1}, but updated interview have SurveySID = {2}, ID = {3}",
                                                     Interview.SurveySID, Interview.ID, interview.SurveySID, interview.ID));
            }

            Interview = interview;
        }

        public void DeleteInterview(int surveySID, int interviewID)
        {
            if (Interview == null)
            {
                throw new InternalErrorException(String.Format(
                                                     "Cann't delete interview with SurveySID = {0}, ID = {1}, because interview wasn't inserted",
                                                     surveySID, interviewID));
            }

            if (Interview.SurveySID != surveySID || Interview.ID != interviewID)
            {
                throw new InternalErrorException(String.Format(
                                                     "Cann't delete interview with SurveySID = {0}, ID = {1}, because current interview have SurveySID = {2}, ID = {3}",
                                                     surveySID, interviewID, Interview.SurveySID, Interview.ID));
            }

            Interview = null;
        }

        public void InsertCall(BvCallEntity call)
        {
            if (Call != null)
            {
                throw new InternalErrorException(String.Format(
                                                     "Call with SurveySID = {0}, IID = {1} already inserted. Inserted call with SurveySID = {2}, IID = {3}",
                                                     Call.SurveySID, Call.InterviewID, call.SurveySID, call.InterviewID));
            }

            if (call.SurveySID != SurveySID)
            {
                throw new InternalErrorException(String.Format(
                                                     "Inserted call have SurveySID = {0}, IID = {1}, but SampleDataStorage was created for SurveySID = {2}",
                                                     call.SurveySID, call.InterviewID, SurveySID));
            }

            Call = call;
        }

        public void UpdateCall(BvCallEntity call)
        {
            if (Call == null)
            {
                throw new InternalErrorException(String.Format(
                                                     "Call with SurveySID:{0} and IID = {1} not found.",
                                                     call.SurveySID, call.InterviewID));
            }

            if (Call.SurveySID != call.SurveySID || Call.InterviewID != call.InterviewID)
            {
                throw new InternalErrorException(String.Format(
                                                     "Current call have SurveySID = {0}, IID = {1}, but updated call have SurveySID = {2}, IID = {3}",
                                                     Call.SurveySID, Call.InterviewID, call.SurveySID, call.InterviewID));
            }

            Call = call;
        }

        public void DeleteCall(int surveySID, int interviewID)
        {
            if (Call != null)
            {
                if (Call.SurveySID != surveySID || Call.InterviewID != interviewID)
                {
                    throw new InternalErrorException(String.Format(
                        "Cann't delete call with SurveySID = {0}, IID = {1}, because current call have SurveySID = {2}, IID = {3}",
                        surveySID, interviewID, Call.SurveySID, Call.InterviewID));
                }
            }

            Call = null;
        }

        /// <summary>
        /// /This method store data of current record to DataTable objects and reset them
        /// </summary>
        public void SaveCurrentRecord()
        {
            if (Interview != null)
            {
                BvInterviewAdapter.SaveEntity2DataTable(m_InterviewTable, Interview);
            }

            if (Call != null)
            {
                var schedule = CallQueueService.ConvertCall2SvySchedule(Call);

                if (Interview != null && SchedulingMode == SurveySchedulingMode.CallGroup)
                {
                    schedule.ConditionValue = (int) Interview.TransientState;
                }

                if (IsRandomCallDeliveryEnabled)
                {
                    schedule.CallOrder = Randomizer.Next();
                }
                else
                {
                    schedule.CallOrder = (int) schedule.InterviewID;
                }

                BvSvyScheduleAdapter.SaveEntity2DataTable(m_SvyScheduleTable, schedule);
            }

            AddCallHistoryRecordIfNeeded();
          
            Interview = null;
            Call = null;
            IsCallDisabledByFCD = false;
        }

        private void InsertBulkWithoutFiringTriggers(DataTable data, bool keepIdentity)
        {
            InsertBulk(data, SqlBulkCopyOptions.Default, keepIdentity);
        }

        private void InsertBulkAndFireTriggers(DataTable data, bool keepIdentity)
        {
            InsertBulk(data, SqlBulkCopyOptions.FireTriggers, keepIdentity);
        }

        private void InsertBulk(DataTable data, SqlBulkCopyOptions bulkOptions, bool keepIdentity)
        {
            try
            {
                if (keepIdentity)
                {
                    // In the UPDATE mode temporary tables are created as a full copy of source tables so there is an IDENTITY column...
                    bulkOptions = bulkOptions | SqlBulkCopyOptions.KeepIdentity;
                }

                using (var transactionScope = new DatabaseTransactionScope("add sample through bulk"))
                {
                    using (var bulk = new SqlBulkCopy(transactionScope.Transaction.Connection, bulkOptions, transactionScope.Transaction)
                        {
                            BatchSize = 10000,
                            BulkCopyTimeout = 60*60,
                            DestinationTableName = data.TableName
                        })
                    {
                        foreach (DataColumn column in data.Columns)
                        {
                            if (!column.ExtendedProperties.Contains("Computed"))
                                bulk.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }

                        bulk.WriteToServer(data);
                    }
                    transactionScope.Commit();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(
                    "Bulk insert to '{0}' table for BatchID = {1} failed. Exception details: {2}",
                    data.TableName, BatchID, e);
                throw;
            }
        }

        /// <summary>
        /// This method commit accumulating data to DB througth BULK INSERT
        /// </summary>
        /// 
        public void Commit(IEventDetails eventDetails)
        {
            try
            {
                if (_isUpdateMode)
                {
                    CreateTemporaryTables();
                }

                using (var transaction = new DatabaseTransactionScope("AddSample.Commit"))
                {
                    if (m_InterviewTable.Rows.Count > 0)
                    {
                        InsertBulkAndFireTriggers(m_InterviewTable, _isUpdateMode);
                    }

                    if (m_SvyScheduleTable.Rows.Count > 0)
                    {
                        InsertBulkAndFireTriggers(m_SvyScheduleTable, _isUpdateMode);
                    }

                    if (m_CallHistoryExTable.Rows.Count > 0)
                    {
                        InsertBulkWithoutFiringTriggers(m_CallHistoryExTable, _isUpdateMode);
                    }

                    if (_isUpdateMode)
                    {
                        ApplySampleUpdate();
                    }

                    transaction.Commit();
                }

                if (_isUpdateMode)
                {
                    UpdateResponseControl();
                }

            }
            finally
            {
                if (_isUpdateMode)
                {
                    DropTemporaryTables();
                }
            }
        }

        private void UpdateResponseControl()
        {
            var remoteTempTableName = "#TempUpdatedInterviews";
            var copyDataQuery = $"SELECT [Id], [TransientState] FROM [{m_InterviewTable.TableName}]";
            var query = $@"
                UPDATE <Schema>.response_control
                SET ITS = [UpdatedInterviews].[TransientState]
                FROM [{remoteTempTableName}] as UpdatedInterviews
                WHERE respid = UpdatedInterviews.Id";

            using (var connectionScope = new ConnectionScope())
            {
                var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(SurveySID);
                using (var remoteConnectionProvider = new RemoteConnectionProvider(surveyConnectionInfo.ConnectionString))
                {
                    _remoteDataCopier.CopyDataToNewTable(connectionScope, remoteConnectionProvider, remoteTempTableName, copyDataQuery);

                    _surveyDatabaseEngine.ExecuteNonQuery(remoteConnectionProvider.Connection, SurveySID, query);
                }
            }
        }

        private void ApplySampleUpdate()
        {
            var dx = new DatabaseEngine();

            var query = string.Format(@"
DECLARE @ActiveCalls table([InterviewID] int, [SurveySID] int);  

INSERT INTO 
    @ActiveCalls 
SELECT 
    [InterviewID], [SurveySID] 
FROM 
    [dbo].[BvSvySchedule] 
WHERE 
    [SurveySID] = {3} AND
    [BvSvySchedule].[CallState] NOT IN ( 1,2,3 )



MERGE [dbo].[BvSvySchedule] AS target  
USING (SELECT
           call.[ApptID], 
           call.[ShiftTypeID], 
           interview.ID as [InterviewID],
           interview.[SurveySID],
           call.[CallState], 
           call.[Priority], 
           call.[TimeInShift], 
           call.[ExpireTime], 
           call.[ExplicitSID], 
           call.[ExplicitType], 
           call.[RuleNumber], 
           call.[CallOrder], 
           call.[OldPriority], 
           call.[ConditionValue], 
           call.[CellId], 
           call.[DialTypeId],
           call.InterviewId as NewCallId
       FROM [dbo].[{1}] as interview
       LEFT JOIN [dbo].[{0}] as call ON interview.SurveySID = call.SurveySID AND interview.ID = call.InterviewId)
      AS source (ApptID, ShiftTypeID, InterviewID, SurveySID, CallState, Priority, TimeInShift, ExpireTime, ExplicitSID, ExplicitType, RuleNumber, CallOrder, OldPriority, ConditionValue, CellId, DialTypeId, NewCallId)
ON 
    (target.SurveySID = source.SurveySID AND target.InterviewID = source.InterviewID)
WHEN MATCHED AND [target].[CallState] IN (1,2,3) AND [source].NewCallId IS NOT NULL THEN UPDATE SET
        [target].[ApptID]         = [source].[ApptID],
	    [target].[ShiftTypeID]    = [source].[ShiftTypeID],
        [target].[CallState]      = [source].[CallState],
        [target].[Priority]       = [source].[Priority],
        [target].[TimeInShift]    = [source].[TimeInShift],
        [target].[ExpireTime]     = [source].[ExpireTime],
        [target].[ExplicitSID]    = [source].[ExplicitSID],
        [target].[ExplicitType]   = [source].[ExplicitType],
        [target].[RuleNumber]     = [source].[RuleNumber],
        [target].[CallOrder]      = [source].[CallOrder],
        [target].[OldPriority]    = [source].[OldPriority],
        [target].[ConditionValue] = [source].[ConditionValue],
        [target].[CellId]         = [source].[CellId],
        [target].[DialTypeId]     = [source].[DialTypeId]
WHEN MATCHED AND [target].[CallState] IN (1,2,3) AND [source].NewCallId IS NULL THEN DELETE
WHEN NOT MATCHED BY TARGET AND [source].NewCallId IS NOT NULL THEN INSERT
(
		[ApptID],
        [ShiftTypeID],
        [InterviewID],
        [SurveySID],
        [CallState],
        [Priority],
        [TimeInShift],
        [ExpireTime],
        [ExplicitSID],
        [ExplicitType],
        [RuleNumber],
        [CallOrder],
        [OldPriority],
        [ConditionValue],
        [CellId],
        [DialTypeId]
) VALUES (
		[source].[ApptID],
        [source].[ShiftTypeID],
        [source].[InterviewID],
        [source].[SurveySID],
        [source].[CallState],
        [source].[Priority],
        [source].[TimeInShift],
        [source].[ExpireTime],
        [source].[ExplicitSID],
        [source].[ExplicitType],
        [source].[RuleNumber],
        [source].[CallOrder],
        [source].[OldPriority],
        [source].[ConditionValue],
        [source].[CellId],
        [source].[DialTypeId]
);

UPDATE [dbo].[BvInterview] SET
    [BvInterview].[TransientState]   = [Source].[TransientState],
    [BvInterview].[DialingMode]      = [Source].[DialingMode]
FROM
    [dbo].[BvInterview]
INNER JOIN 
    [dbo].[{1}] [Source] 
ON 
    [BvInterview].[ID] = [Source].[ID] AND [BvInterview].[SurveySID] = [Source].[SurveySID]
WHERE NOT EXISTS
    (SELECT NULL FROM @ActiveCalls [ActiveCalls] WHERE [BvInterview].[ID] = [ActiveCalls].[InterviewID] AND [BvInterview].[SurveySID] = [ActiveCalls].[SurveySID])

INSERT INTO BvCallHistoryEx 
    ([FiredTime], [ApptID], [ShiftTypeID], [InterviewID], [SurveyId], [ITS], [DialingMode], [CallState], [Priority], [TimeInShift], [ExpireTime], [ExplicitSID], [ExplicitType], [CellId], [OperationId], [OperationType], [CallCenterId], [DialTypeId]) 
SELECT 
    [Source].[FiredTime], [Source].[ApptID], [Source].[ShiftTypeID], [Source].[InterviewID], [Source].[SurveyId], [Source].[ITS], [Source].[DialingMode], [Source].[CallState], [Source].[Priority], [Source].[TimeInShift], [Source].[ExpireTime], [Source].[ExplicitSID], [Source].[ExplicitType], [Source].[CellId], [Source].[OperationId], [Source].[OperationType], [Source].[CallCenterId], [Source].[DialTypeId]
FROM 
    [{2}] [Source] 
WHERE NOT EXISTS
    (SELECT NULL FROM @ActiveCalls [ActiveCalls] WHERE [Source].[InterviewID] = [ActiveCalls].[InterviewID] AND [Source].[SurveyID] = [ActiveCalls].[SurveySID])
",
    m_SvyScheduleTable.TableName,
    m_InterviewTable.TableName,
    m_CallHistoryExTable.TableName,
    SurveySID);

            dx.ExecuteNonQuery(query, CommandType.Text);
        }

        private void AddCallHistoryRecordIfNeeded()
        {
            var call = Call ?? new BvCallEntity(){CallState = (short)CallState.ToBeDeleted};

            var callHistoryOperatorType = GetCallHistoryOperationType(Interview, call);

            if (callHistoryOperatorType != null)
            {
                var callEntity = CallQueueService.ConvertCall2SvySchedule(call);
                var history = new BvCallHistoryExEntity
                {
                    FiredTime = DateTime.UtcNow,
                    InterviewID = Interview.ID,
                    SurveyId = Interview.SurveySID,
                    ApptID = callEntity.ApptID,
                    ShiftTypeID = callEntity.ShiftTypeID,
                    ITS = (short) Interview.TransientState,
                    DialingMode = Interview.DialingMode,
                    CallState = (short) callEntity.CallState,
                    Priority = callEntity.Priority,
                    TimeInShift = callEntity.TimeInShift,
                    ExpireTime = callEntity.ExpireTime,
                    ExplicitSID = callEntity.ExplicitSID,
                    ExplicitType = (byte) callEntity.ExplicitType,
                    CellId = callEntity.CellId,
                    OperationId = OperationId,
                    OperationType = (byte)callHistoryOperatorType,
                    CallCenterId = 0,
                    DialTypeId = callEntity.DialTypeId
                };

                BvCallHistoryExAdapter.SaveEntity2DataTable(m_CallHistoryExTable, history);
            }
        }

        private OperationType? GetCallHistoryOperationType(BvInterviewEntity interview, BvCallEntity call)
        {
            if (interview.TransientState == (int) CallOutcome.FilteredByCallDelivery)
                return OperationType.DeleteByFcdDuringSample;

            //we should remove IsCallDisabledByFCD, if we support different call states from disbaled calls and disbaled by fcd calls
            if (call.CallState == (int)CallState.DisabledByFCD )
                return IsCallDisabledByFCD ? OperationType.DisableByFcdDuringSample : OperationType.DisableCalls;
            
            if (interview.TransientState == (int) CallOutcome.Blacklist)
                return OperationType.DeleteCallByBlacklistInAddSample;

            if ((_isUpdateMode) && (call.CallID != 0))
            {
                return OperationType.UpdateBySampleUpdate;
            }

            if (interview.TransientState == (int)CallOutcome.SynchronizedSample)
            {
                return OperationType.SynchronizeRespondents;
            }

            return null;
        }

        bool m_IsDisposed;

        /// <summary>
        /// Disposable interface will be implemented for correct rollback sample, if exception will be thrown
        /// </summary>
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                ServiceLocator.Resolve<ISampleDataStorageRepository>().Delete(BatchID);

                m_IsDisposed = true;

                // We should remove references on table objects 
                // before call GC, otherwise GC does not release memory
                m_SvyScheduleTable = null;
                m_InterviewTable = null;
                m_CallHistoryExTable = null;

                GC.Collect();
            }
        }

        public void CreateTemporaryTables()
        {
            CreateTemporaryTable(m_SvyScheduleTable.TableName, "BvSvySchedule");
            CreateTemporaryTable(m_InterviewTable.TableName, "BvInterview");
            CreateTemporaryTable(m_CallHistoryExTable.TableName, "BvCallHistoryEx");
        }

        public void CreateTemporaryTable(string tableName, string sourceTable)
        {
            var query = string.Format("SELECT * INTO {0} FROM {1} where 1<>1", tableName, sourceTable);
            var dx = new DatabaseEngine();
            dx.ExecuteNonQuery(query, CommandType.Text);
        }

        private void DropTemporaryTables()
        {
            DropTemporaryTable(m_SvyScheduleTable.TableName);
            DropTemporaryTable(m_InterviewTable.TableName);
            DropTemporaryTable(m_CallHistoryExTable.TableName);
        }

        private void DropTemporaryTable(string tableName)
        {
            var query =
                string.Format(
                    "if exists (select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}') drop table {0}",
                    tableName);
            var dx = new DatabaseEngine();
            dx.ExecuteNonQuery(query, CommandType.Text);
        }
    }
}