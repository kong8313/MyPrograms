using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.Services
{
    public class CallQueueService : ICallQueueService
    {
        private readonly IAuthoringService _authoringService;
        private readonly ITimeZoneBalancingSettings _tzBalancingSettings;
        private readonly ITimeService _timeService;
        private readonly IInterviewerApiClient _interviewerApiClient;
        private readonly ICompanyInfo _companyInfo;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;

        public static readonly DateTime DefaultTimeInShift = new DateTime(1899, 12, 30, 0, 0, 0);
        public static readonly DateTime ExpirationDateNever = new DateTime(9999, 1, 1);

        public CallQueueService(
            IAuthoringService authoringService, 
            ITimeZoneBalancingSettings tzBalancingSettings, 
            ITimeService timeService,
            IInterviewerApiClient interviewerApiClient,
            ICompanyInfo companyInfo, 
            IDatabaseLockTimeouts databaseLockTimeouts)
        {
            _authoringService = authoringService;
            _tzBalancingSettings = tzBalancingSettings;
            _timeService = timeService;
            _interviewerApiClient = interviewerApiClient;
            _companyInfo = companyInfo;
            _databaseLockTimeouts = databaseLockTimeouts;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="call"></param>
        /// <param name="batchID"></param>
        /// <param name="its"></param>
        /// <param name="reason"></param>
        /// <returns>
        /// If call is being added during loading sample It will be always created.
        /// Otherwise it can fall into closed cell and won't be created.
        /// Return value is true if call have been created/updated. 
        /// </returns>
        public static bool AddCall([NotNull] BvCallEntity call, int batchID, int its, SchedulingScriptExecutionReason reason = SchedulingScriptExecutionReason.Unspecified)
        {
            if (reason == SchedulingScriptExecutionReason.AddedBySample)
            {
                ServiceLocator.Resolve<ISampleDataStorageRepository>().Get(batchID).InsertCall(call);
                return true;
            }

            var callId = AddCallToDb(call, its, null);

            return (callId > 0);
        }

        public static bool AddCall([NotNull] BvCallEntity call, int batchID, BvInterviewEntity interview,
            SchedulingScriptExecutionReason reason = SchedulingScriptExecutionReason.Unspecified)
        {
            if (reason == SchedulingScriptExecutionReason.AddedBySample)
            {
                ServiceLocator.Resolve<ISampleDataStorageRepository>().Get(batchID).InsertCall(call);
                return true;
            }

            var callId = AddCallToDb(call, interview.TransientState, interview.TimezoneID);

            return (callId > 0);
        }

        bool ICallQueueService.AddCall([NotNull] BvCallEntity call)
        {
            var callId = AddCallToDb(call, 0, null);

            return (callId > 0);
        }

        public static int AddCallToDb(BvCallEntity call, int its, int? callTz)
        {
            int callId;
            BvSpSvySch_InsertAdapter.ExecuteNonQuery(
                call.CallID,
                call.ApptID,
                call.SurveySID,
                call.InterviewID,
                call.CallState,
                call.ShiftID,
                call.Priority,
                call.TimeInShift,
                call.TimeToExpire,
                call.Resource,
                call.RuleNumber,
                ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId(),
                ServiceLocator.Resolve<IFCDSettings>().BehaviorType,
                its,
                call.DialTypeId,
                (byte)call.Type,
                call.DialerId,
                call.ActiveDialId,
                callTz,
                out callId);
            return callId;
        }

        public static void UpdateCall([NotNull] BvCallEntity call, int batchID)
        {
            if (batchID != 0)
            {
                ServiceLocator.Resolve<ISampleDataStorageRepository>().Get(batchID).UpdateCall(call);
            }
            else
            {
                BvSpSvySch_InsertAdapter.ExecuteNonQuery(
                    call.CallID,
                    call.ApptID,
                    call.SurveySID,
                    call.InterviewID,
                    call.CallState,
                    call.ShiftID,
                    call.Priority,
                    call.TimeInShift,
                    call.TimeToExpire,
                    call.Resource,
                    call.RuleNumber,
                    ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId(),
                    ServiceLocator.Resolve<IFCDSettings>().BehaviorType,
                    0,
                    call.DialTypeId,
                    call.Type,
                    call.DialerId,
                    call.ActiveDialId,
                    null
               );
            }
        }

        public static void DeleteCall([NotNull] BvCallEntity call, int batchID)
        {
            if (batchID != 0)
            {
                ServiceLocator.Resolve<ISampleDataStorageRepository>().Get(batchID).DeleteCall(call.SurveySID, call.InterviewID);
            }
            else
            {
                DeleteCall(call.SurveySID, call.InterviewID);
            }
        }

        public static void DeleteCall(int surveySID, int interviewID)
        {
            BvSpSvySch_DeleteAdapter.ExecuteNonQuery(
                surveySID,
                interviewID);
        }

        public static void DeleteCalls(int surveySID, int batchId)
        {
            BvSpCalls_Delete_BatchAdapter.ExecuteNonQuery(
                surveySID,
                batchId);
        }

        public static void ReleaseCall(int surveySid, int interviewId)
        {
            BvSpReleaseCallAdapter.ExecuteNonQuery(surveySid, interviewId);
        }


        public static void FinalDeleteCall(int surveySID, int interviewID, int batchId)
        {
            if (batchId != 0)
            {
                ServiceLocator.Resolve<ISampleDataStorageRepository>().Get(batchId).DeleteCall(surveySID, interviewID);
            }
            else
            {
                BvSpSvySch_DeleteAdapter.ExecuteNonQuery(surveySID, interviewID);
            }
        }

        [CanBeNull]
        public static BvCallEntity GetCallAndNoLock(int surveySID, int interviewID, int batchID, bool isSampleUpdateMode)
        {
            //
            // If batchID is not eq 0 then we're inside sample addition
            // So, there is no call and we could simply return null
            //
            if ((batchID != 0) && (!isSampleUpdateMode))
                return null;

            return GetCallAndNoLock(surveySID, interviewID);
        }

        [CanBeNull]
        public static BvCallEntity GetCallAndNoLock(int surveySid, int interviewId)
        {
            using (IDataReader dr = BvSpCall_GetAdapter.ExecuteReader(surveySid,
                interviewId,
                (int)CallLockMode.NoLock,
                (int)CallMode.Live))
            {
                return BvCallAdapter.Read(dr);
            }
        }

        [CanBeNull]
        public BvCallEntity GetCallWithTryLock(int surveySid, int interviewId, out bool isCallLocked)
        {
            int isLocked;
            using (IDataReader dr = BvSpCall_GetAdapter.ExecuteReader(surveySid,
                interviewId,
                (int)CallLockMode.TryLockOnlyNotLive,
                (int)CallMode.Live,
                out isLocked))
            {
                isCallLocked = isLocked > 0;
                return BvCallAdapter.Read(dr);
            }
        }

        [CanBeNull]
        public BvCallEntity GetCallWithTryLockAny(int surveySid, int interviewId, out bool isCallLocked)
        {
            int isLocked;
            using (IDataReader dr = BvSpCall_GetAdapter.ExecuteReader(surveySid,
                interviewId,
                (int)CallLockMode.TryLockAny,
                (int)CallMode.Live,
                out isLocked))
            {
                isCallLocked = isLocked > 0;
                return BvCallAdapter.Read(dr);
            }
        }

        public BvCallEntity GetExpiredCallAndLock(int lastId, DateTime now)
        {
            using (IDataReader dr = BvSpCall_GetExpiredAndLockAdapter.ExecuteReader(lastId, now))
            {
                return BvCallAdapter.Read(dr);
            }
        }

        public void ScheduleAndRemoveDeletedCalls(CancellationToken cancellationToken = default(CancellationToken))
        {
            var evt = new ScheduleEvent();
            
            RemoveDeletedCalls(cancellationToken);
            evt.AddTiming("RemoveDeletedCalls");
            
            if (!cancellationToken.IsCancellationRequested)
                ScheduleInternal(_timeService.GetUtcNow(), evt);
            
            evt.Finish();
        }

        public void Schedule(DateTime? utcNow = null)
        {
            var evt = new ScheduleEvent();

            ScheduleInternal(utcNow.GetValueOrDefault(_timeService.GetUtcNow()), evt);
            
            evt.Finish();
        }
        
        private void ScheduleInternal(DateTime utcNow, ScheduleEvent evt)
        {
            using (var dbLock = DatabaseLockService.CreateLock(
                DatabaseLockTimeoutsAndRecourceNames.ScheduleResourceName,
                "CallQueueService.Schedule",
                0))
            {
                if (dbLock.TryEnterLock())
                {
                    evt.AddTiming("GettingLock");

                    SyncRuntimeStatistics(DeadlockPriority.PeriodicalThread);
                    evt.AddTiming("BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter");

                    using (var transaction = new DatabaseTransactionScope("ScheduleFillCache",
                                                                       DeadlockPriority.SchedulingProcedure))
                    {
                        BvSpQueueUpSheduleTask3Adapter.ExecuteNonQuery(
                            utcNow,
                            ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId(),
                            _tzBalancingSettings.EndOfShiftThreshold,
                            60 * 2 /*Timeout in sec(2Min)*/,
                            out _);

                        transaction.Commit();
                    }
                    evt.AddTiming("BvSpQueueUpScheduleTask3Adapter");

                    _interviewerApiClient.NotifyScheduling(_companyInfo.CompanyId);
                    evt.AddTiming("InterviewerApiClient.NotifyScheduling");
                }
            }
        }

        bool ICallQueueService.IsResourceLoggedIn(int resourceId, int surveySid)
        {
            var effectiveResourceId = resourceId != 0
                ? resourceId
                : surveySid;

            var count = BvSpAssignment_IsLoggedInAdapter.ExecuteScalar<int>(effectiveResourceId, surveySid);

            return count > 0;
        }

        public void SyncRuntimeStatistics(DeadlockPriority deadlockPriority)
        {
            using (var transaction =
                new DatabaseTransactionScope(new DatabaseTransactionOptions("BvSvyScheduleRuntimeStatistics", deadlockPriority )))
            {
                BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();

                transaction.Commit();
            }
        }

        public static List<BvSpCallHistory_ListEntity> GetInterviewHistoryList(
            int surveySid,
            int interviewId,
            int callCenterId)
        {
            return BvSpCallHistory_ListAdapter.ExecuteEntityList(
                interviewId,
                surveySid,
                callCenterId);
        }

        public static List<BvSpGetExtendedCallHistoryEntity> GetExtendedCallHistoryList(
            int surveySid,
            int interviewId,
            int callCenterId)
        {
            return BvSpGetExtendedCallHistoryAdapter.ExecuteEntityList(
                interviewId,
                surveySid,
                callCenterId);
        }

        public static BvSvyScheduleEntity ConvertCall2SvySchedule([NotNull] BvCallEntity call)
        {
            int callExplicitSID;
            int callExplicitType;
            int callShiftTypeID;

            if (call.Resource != 0)
            {
                callExplicitType = (int)CallExplicitType.PersonOrPersonGroup;
                callExplicitSID = call.Resource;
            }
            else
            {
                callExplicitType = (int)CallExplicitType.Survey;
                callExplicitSID = call.SurveySID;
            }

            if (call.ShiftID > 0)// specific shift
            {
                //if Call.ShiftID is shift type id, then convert them to shiftzone id
                callShiftTypeID = GetShiftZoneID(call.ShiftID, call.TimeZoneID).Value;
            }
            else if (call.ShiftID == (int)CallShiftType.None) // None
            {
                callShiftTypeID = (int)CallShiftType.None;
            }
            else//(int)CallShiftType.AnyValid
            {
                callShiftTypeID = -call.TimeZoneID;
            }

            return new BvSvyScheduleEntity
            {
                ID = call.CallID,
                ApptID = call.ApptID,
                InterviewID = call.InterviewID,
                SurveySID = call.SurveySID,
                CallState = call.CallState,
                Priority = call.Priority,
                TimeInShift = call.TimeInShift.HasValue ? call.TimeInShift.Value : DefaultTimeInShift,
                ExpireTime = call.TimeToExpire.HasValue ? call.TimeToExpire.Value : ExpirationDateNever,
                RuleNumber = call.RuleNumber,
                ExplicitSID = callExplicitSID,
                ExplicitType = callExplicitType,
                ShiftTypeID = callShiftTypeID,
                OldPriority = call.OldPriority,
                ConditionValue = call.ConditionValue,
                CellId = call.CellId,
                DialTypeId = call.DialTypeId,
                Type = call.Type
            };
        }

        public static int? GetShiftZoneID(int shiftTypeID, int tzID)
        {
            var entities = BvShiftZonesAdapter.GetByCondition(
                "ShiftTypeID = @ShiftTypeID AND TimeZoneID = @TimeZoneID",
                new SqlParameter("@ShiftTypeID", shiftTypeID),
                new SqlParameter("@TimeZoneID", tzID));

            if (entities.Count != 1)
                return null;

            return entities[0].ID;
        }

        public static BvCallEntity GetCallInfo(long callId)
        {
            using (IDataReader dr = BvSpCall_GetInfoAdapter.ExecuteReader((int)callId))
            {
                return BvCallAdapter.Read(dr);
            }
        }

        public BvCallEntity GetCall(long callId)
        {
            return GetCallInfo(callId);
        }

        private static void LogErrorMessageForExpiredCall([NotNull] BvCallEntity call, Exception exeption, string message)
        {
            Trace.TraceError("Error during of processing of expied call:" + Environment.NewLine +
                "   Call details: SurveySID = '{0}', InterviewID = '{1}'" + Environment.NewLine +
                "   Message: {2}" + Environment.NewLine +
                "   Exception: {3}",
                call.SurveySID,
                call.InterviewID,
                message ?? "None",
                exeption != null ? exeption.ToString() : "None");


        }

        private void ProcessExpiredCall([NotNull] BvCallEntity call)
        {
            //
            // try schedule interview in normal mode
            //
            try
            {
                //
                // if CF survey database doesn't available, then try attach this database
                //

                var survey = SurveyRepository.GetWithNoCache(call.SurveySID);

                var interview = InterviewRepository.GetById(call.SurveySID, call.InterviewID);
                if (interview == null)
                {
                    //if interview doesn't exist, we cann't do anything 
                    LogErrorMessageForExpiredCall(call, null, "Interview not found");
                    return;
                }

                if (survey.ReplicationStatus != true)
                {
                    // Attach database
                    // Note: CF will get DB version from survey Db. So CF will attach survey database, if database is detached
                    _authoringService.GetDBVersion(survey.Name);

                    Trace.TraceInformation("CF survey database for survey with SID = {0} and Name = {1} was attached",
                        survey.SID,
                        survey.Name);
                }

                //
                // Schedule interview
                //
                using (var transaction = new DatabaseTransactionScope("ScheduleExpiredCall", DeadlockPriority.PeriodicalThread))
                {
                    var options = new SchedulingScriptExecutionOptions
                    {
                        ExecutionReason = SchedulingScriptExecutionReason.Expired,
                        CallProvider = new CallMemoryProvider(call),
                        opType = OperationType.ExpiredCall,
                        IsLogToHistory = false
                    };

                    InterviewRepository.Update(interview, options);

                    transaction.Commit();
                }
            }
            catch (Exception exScheduling)
            {
                LogErrorMessageForExpiredCall(call, exScheduling, "Scheduling of interview was failed");

                //
                // if interview isn't schedule, we should delete call and update interview with ITS = 30/*Error*/
                // Note: we should reread interview, because during excution of scheuling script interview object may be changed.

                var interview = InterviewRepository.GetById(call.SurveySID, call.InterviewID);

                if (interview == null)
                {
                    //if interview doesn't exist, we cann't do anything 
                    LogErrorMessageForExpiredCall(call, null, "Interview not found");
                    return;
                }

                interview.TransientState = (int)CallOutcome.Error;

                //
                // if scheduling is failed we should delete call, because this call have phase = -1
                // 
                try
                {
                    using (var transaction = new DatabaseTransactionScope("FinalDeleteExpiredCall", DeadlockPriority.PeriodicalThread))
                    {
                        FinalDeleteCall(call.SurveySID, call.InterviewID, 0);
                        transaction.Commit();
                    }
                }
                catch (Exception exFinalDeleteCall)
                {
                    LogErrorMessageForExpiredCall(call, exFinalDeleteCall, "FinalDeleteCall was failed for this call");
                }

                //
                // Safe update interview */
                //
                try
                {
                    using (var transaction = new DatabaseTransactionScope("SafeUpdateExpiredInterview", DeadlockPriority.PeriodicalThread))
                    {
                        var options = new SchedulingScriptExecutionOptions()
                        {
                            IsExecuteSchedulingScript = false,
                            IsLogToHistory = false
                        };
                        InterviewRepository.Update(interview, options);
                        transaction.Commit();
                    }

                    Trace.TraceWarning("Interview with SurveySID = {0} and InterviewID = {1} for expired calls was updated safely with ITS = 30, expied calls wasn't schedule and was deleted",
                        interview.SurveySID, interview.ID);
                }
                catch (Exception exSafeUpdate)
                {
                    LogErrorMessageForExpiredCall(call, exSafeUpdate, "Safe update interview was failed");

                    //
                    // if safe update of interview is failed we should try to do direct update of interview
                    //
                    try
                    {
                        using (var transaction = new DatabaseTransactionScope("DirectUpdateExpiredInterview", DeadlockPriority.PeriodicalThread))
                        {
                            BvInterviewAdapter.Update(interview);
                            transaction.Commit();
                        }
                    }
                    catch (Exception exDirectUpdate)
                    {
                        LogErrorMessageForExpiredCall(call, exDirectUpdate, "Direct update of interview with ITS = 30 was failed");
                        return;
                    }
                    Trace.TraceWarning("Interview with SurveySID = {0} and InterviewID = {1} for expired calls was updated directly with ITS = 30, expied calls wasn't schedule and was deleted",
                    interview.SurveySID, interview.ID);
                }
            }
        }

        public static int GetCountOfScheduledInterviews(int surveyId)
        {
            return AggregateSurveyRepository.GetById(surveyId).ScheduledCallsCount;
        }

        public static int GetCountOfScheduledInterviewsWithSpecificIts(int surveyId, int[] itsList)
        {
            if (itsList.Length <= 0)
            {
                return 0;
            }

            var query = String.Format(
                @"  SELECT COUNT(*) FROM BvReplicatedData_{0} r
                    INNER JOIN BvSvySchedule c
    	            ON r.respId = c.InterviewID AND c.SurveySID = {0}
	                INNER JOIN BvInterview i
	                ON r.respId = i.ID AND i.SurveySID = {0}
	                WHERE ( c.CallState > 0 OR c.CallState = -2 ) AND i.TransientState IN ( {1} )
	                ",
                surveyId,
                String.Join(",", itsList.Select(x => x.ToString()).ToArray())
                );

            return new DatabaseEngine().ExecuteScalar<int>(query, CommandType.Text);
        }

        public void ExpireAllCalls(CancellationToken cancellationToken = default(CancellationToken))
        {
            var evt = new ExpiredCallsEvents();

            int lastId = 0;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                
                using (var dbLock = DatabaseLockService.CreateLock(
                           DatabaseLockTimeoutsAndRecourceNames.ScheduleResourceName,
                           "CallQueueService.ExpireAllCalls",
                           _databaseLockTimeouts.DefaultLockTimeoutInMs))
                {
                    if (!dbLock.TryEnterLock())
                    {
                        break;
                    }

                    using (var transaction = new DatabaseTransactionScope("RemoveExpiredCalls", DeadlockPriority.PeriodicalThread))
                    {
                        BvCallEntity call = GetExpiredCallAndLock(lastId, _timeService.GetUtcNow());

                        if (call == null)
                        {
                            transaction.Commit();
                            break;
                        }

                        lastId = call.CallID;

                        evt.AddExpiredCall(call.SurveySID, call.InterviewID);
                        ProcessExpiredCall(call);

                        transaction.Commit();

                    }

                }
            }

            evt.Finish();
        }

        public static void RemoveDeletedCalls(CancellationToken cancellationToken)
        {
            var batchSize = 3000;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                
                var query = @"
                    WITH Calls AS (
                        SELECT TOP(@BatchSize) SurveySid, InterviewId 
                        FROM BvSvySchedule
                        JOIN BvSurvey ON SurveySid = BvSurvey.Sid
                        WHERE CallState = 0
                    ) 
                    DELETE FROM BvSvySchedule FROM Calls 
                    WHERE BvSvySchedule.SurveySID = calls.SurveySID AND BvSvySchedule.InterviewID = calls.InterviewID
                    SELECT @@ROWCOUNT AS DELETED";

                var rowsDeleted = new DatabaseEngine().ExecuteScalar<int>(query, CommandType.Text, new SqlParameter("BatchSize", batchSize));
                if (rowsDeleted < batchSize)
                    return;
            }
        }

        public void ForceCallDelivery(BvCallEntity call = null)
        {
            Schedule();

            var exclusiveDatabaseLockFactory = new ExclusiveDatabaseLockFactory("CallQueueService.ScheduleCall", 60000);

            using (var dbLock = exclusiveDatabaseLockFactory.Create(DatabaseLockTimeoutsAndRecourceNames.IvrThreadResourceName))
            {
                if (dbLock.TryEnterLock())
                {
                    ServiceLocator.Resolve<IIvrConsoleService>().ExecutePeriodicalWork();
                }
            }
        }

        public bool IsSurveyCallsShouldBeReassignedManually(int surveyId)
        {
            var query = @"IF EXISTS(SELECT * FROM BvSvySchedule WHERE SurveySID=@SurveySID AND ShiftTypeID > 0)
                            SELECT CAST(1 AS BIT) 
                          ELSE 
                            SELECT CAST(0 AS BIT)";

            return new DatabaseEngine().ExecuteScalar<bool>(query, CommandType.Text, new SqlParameter("SurveySID", surveyId));
        }

        public List<BvShiftTypeChange> GetShiftTypesThatNeedChange(int newScheduleId, int surveySid)
        {
            var query = @"SELECT DISTINCT shiftTypesInner.ShiftTypeName as ShiftTypeCurrent, ObjectID as ShiftTypeAnalogId 
                         FROM [BvSvySchedule] as calls
                         INNER JOIN [BvViewInnerShiftType] as shiftTypesInner ON calls.ShiftTypeID = shiftTypesInner.ShiftTypeId
                         LEFT JOIN [BvShiftType] as shiftTypes ON Name = shiftTypesInner.ShiftTypeName AND OwnerSID = @OwnerSID
                         WHERE calls.ShiftTypeID > 0 AND SurveySID = @SurveyId";

            using (IDataReader dataReader = new DatabaseEngine().ExecuteReaderInNewConnection(
                       query, 
                       CommandType.Text, 
                       new SqlParameter("@SurveyId", surveySid), 
                       new SqlParameter("@OwnerSID", newScheduleId)))
            {
                var result = new List<BvShiftTypeChange>();
                while (dataReader.Read())
                {
                    var shiftTypeCurrent = (string)dataReader["ShiftTypeCurrent"];

                    var shiftTypeAnalogId = dataReader.IsDBNull(dataReader.GetOrdinal("ShiftTypeAnalogId"))
                        ? -1
                        : (int)dataReader["ShiftTypeAnalogId"];

                    result.Add(new BvShiftTypeChange() { ShiftTypeCurrent = shiftTypeCurrent, ShiftTypeAnalogId = shiftTypeAnalogId });
                }
                return result;
            }
        }
    }
}
