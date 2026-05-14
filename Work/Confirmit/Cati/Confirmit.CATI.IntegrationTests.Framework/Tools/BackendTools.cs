using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;

using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using InterviewControlData = Confirmit.CATI.Core.ManagementService.InterviewControlData;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class BackendTools
    {
        private readonly IntegrationTestingFramework _framework;

        //default parameter for adding survey
        private const string SurveyName = "";

        public BackendTools(IntegrationTestingFramework framework)
        {
            _framework = framework;
        }

        public void LaunchAllHoursScript()
        {
            var schedule = ScheduleRepository.GetByName(AllHoursSchedule.Name);
            int scriptId;
            if (schedule != null)
            {
                scriptId = schedule.ScheduleID;
                ScheduleService.Launch(scriptId);
                return;
            }
            schedule = new BvScheduleEntity
            {
                Name = AllHoursSchedule.Name,
                XmlUnderDev = AllHoursSchedule.Xml,
            };
            scriptId = ScheduleRepository.Insert(schedule);
            ScheduleService.Launch(scriptId);
        }

        public void LaunchScript(int surveySid, TestScript script)
        {
            var survey = SurveyRepository.GetById(surveySid);
            int scheduleId = script.Create(null);
            survey.ScheduleID = scheduleId;

            SurveyRepository.Update(survey);
        }

        public static void ExecuteAllAsyncOperations()
        {
            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            var executeNext = true;
            while (executeNext)
            {
                executeNext = executor.DequeueAndExecute();
            }

        }

        /// <summary>
        /// Returns Default schedule ID.
        /// </summary>
        public static int GetDefaultScheduleID()
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            var scriptID = framework.DbEngine.ExecuteScalar<int>(
                    @"SELECT MIN( ScheduleID ) FROM BvSchedule",
                CommandType.Text
                );

            return scriptID;
        }

        /// <summary>
        /// Returns All Hours schedule ID.
        /// </summary>
        public static int GetAllHoursID()
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            var scriptID = framework.DbEngine.ExecuteScalar<int>(
                    $"SELECT MAX( ScheduleID  ) FROM BvSchedule WHERE [Name] = '{AllHoursSchedule.Name}'",
                CommandType.Text
                );

            return scriptID;
        }

        /// <summary>
        /// Returns shift type work ID for specified schedule.
        /// </summary>
        /// <param name="shiftTypeID">Shift type ID (in schedule)</param>
        /// <returns></returns>
        public static int GetShiftTypeWorkID(int shiftTypeID)
        {
            var shiftTypes = BvShiftTypeAdapter.GetByCondition("ID = @ID", new SqlParameter("@ID", shiftTypeID));
            if (shiftTypes.Count == 1)
                return shiftTypes[0].ObjectID;
            return (int)CallShiftType.None;
        }

        public static int GetDefaultStateGroup()
        {
            var entities = BvStateGroupAdapter.GetByCondition(
                "[Name] = @Name",
                new SqlParameter("@Name", "Default group"));

            return entities.FirstOrDefault().ID;
        }

        public static string GenerateSurveyName()
        {
            return "p" + GenerateCompaingId().ToString("D10");
        }

        public static long GenerateCompaingId()
        {
            return Int32.MaxValue + (long)Randomizer.Next(10000000);
        }

        public int CreateSurvey(TestScript script)
        {
            return CreateSurvey(script, null, null);
        }

        public int CreateSurvey(TestScript script, string name)
        {
            return CreateSurvey(script, name, null);
        }

        public int CreateSurvey(string name, bool assignToDefaultCallCenter = false, bool isRespondentsDynamicCreationAllowed = false, string scheduleName = AllHoursSchedule.Name)
        {
            return CreateSurvey(null, name, null, assignToDefaultCallCenter, isRespondentsDynamicCreationAllowed, 0, scheduleName);
        }

        public int CreateSurvey(string confirmitProjectID, string cfSqlServerConnectionString, string scheduleName = AllHoursSchedule.Name)
        {
            return CreateSurvey(null, confirmitProjectID, cfSqlServerConnectionString, false, false, 0, scheduleName);
        }

        public int CreateSurvey(
            TestScript script,
            string name,
            string cfSqlServerConnectionString,
            bool assignToDefaultCallCenter = false,
            bool isRespondentsDynamicCreationAllowed = false,
            int openEndReview = 0,
            string scheduleName = "")
        {
            if (String.IsNullOrEmpty(name))
                name = GenerateSurveyName();

            if (String.IsNullOrEmpty(cfSqlServerConnectionString))
            {
                cfSqlServerConnectionString = IntegrationTestingFramework.Instance.DbEngine.ConnectionString;
            }

            _framework.RegisterSurveyDbName(name, cfSqlServerConnectionString);

            var sqlServerName =
                new SqlConnectionStringBuilder(IntegrationTestingFramework.Instance.DbEngine.ConnectionString)
                    .DataSource;

            ServiceLocator.Resolve<ISurveyService>().CreateSurvey(name, SurveyName, cfSqlServerConnectionString, string.Empty, sqlServerName);

            BvSurveyEntity survey = SurveyRepository.GetByName(name);

            ServiceLocator.Resolve<ICallCenterService>().AssignSurvey(CallCenterTools.DefaultId, survey.SID);

            // Stub of replicated data table.
            // We use plain SQL instead of SMO to improve performance.
            new DatabaseEngine().ExecuteNonQuery(
                string.Format(
                    @"IF OBJECT_ID('{0}', 'U') is  null
                    BEGIN
                        CREATE TABLE [dbo].[{0}]([respid] [int],	[CallAttemptCount] [int])
                    END",
                    ReplicationSchemaService.GetDestinationTableName(survey.SID)),
                CommandType.Text);

            survey.DestinationTableName = ReplicationSchemaService.GetDestinationTableName(survey.SID);
            survey.IsRespondentsDynamicCreationAllowed = isRespondentsDynamicCreationAllowed;
            survey.IsQuotaInCatiDb = false;

            survey.ForceOpnRev = openEndReview;

            if (!String.IsNullOrEmpty(scheduleName))
            {
                var schedule = ScheduleRepository.GetByName(scheduleName);
                if (schedule != null)
                {
                    survey.ScheduleID = schedule.ScheduleID;
                }
            }

            SurveyRepository.Update(survey);

            if (script != null)
            {
                LaunchScript(survey.SID, script);
            }

            if (assignToDefaultCallCenter)
            {
                ServiceLocator.Resolve<ICallCenterService>().AssignSurvey(CallCenterTools.DefaultId, survey.SID);
            }

            return survey.SID;
        }

        public static AsyncOperationResult DeleteSurvey(string projectId)
        {
            int operationId = new ManagementService().DeleteSurvey(projectId);

            var operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operationId);

            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            return executor.ExecuteOperationSync(operation);
        }

        private static Dictionary<int, int> _Survey2interviewID = new Dictionary<int, int>();

        /// <summary>
        /// Resets interview identifier which is used by NewInterview() function to
        /// initial value 1.
        /// </summary>
        public static void ResetInterviewId()
        {
            _Survey2interviewID = new Dictionary<int, int>();
        }

        public static int GetNewInterviewId(int surveyId)
        {
            lock (_Survey2interviewID)
            {
                int result;
                if (!_Survey2interviewID.TryGetValue(surveyId, out result))
                {
                    result = 0;
                }

                _Survey2interviewID[surveyId] = ++result;

                return result;
            }
        }

        public static BvInterviewWithOriginEntity NewInterview(int surveySID, DialType dialType = DialType.Landline)
        {
            return new BvInterviewWithOriginEntity(
                new BvInterviewEntity
                {
                    ID = GetNewInterviewId(surveySID),
                    SurveySID = surveySID,
                    TransientState = (int)CallOutcome.FreshSample,
                    DialTypeId = (byte)dialType
                });
        }

        public static void CreateInterview(BvInterviewEntity interview)
        {
            ServiceLocator.Resolve<IInterviewRepository>().InsertOnly(interview);
            new DatabaseEngine().ExecuteBatch(String.Format("INSERT INTO BvReplicatedData_{0}(respid)values({1})", interview.SurveySID, interview.ID));
        }

        public static void UpdateFieldInReplicatedTable(BvInterviewEntity interview, string columnName, string value)
        {
            new DatabaseEngine().ExecuteBatch(String.Format("UPDATE BvReplicatedData_{0} SET {2}={3} WHERE respid={1}", interview.SurveySID, interview.ID, columnName, value));
        }

        public static void CheckInterview(BvInterviewEntity interview)
        {
            var actualInterview = InterviewRepository.GetById(interview.SurveySID, interview.ID);
            TestAssert.AreEqual(interview, actualInterview);
        }

        public static BvCallEntity NewCall(BvInterviewEntity interview)
        {
            return new BvCallEntity
            {
                InterviewID = interview.ID,
                SurveySID = interview.SurveySID,
                CallState = 2,
                ShiftID = (int)(CallShiftType.None),//None
                DialTypeId = interview.DialTypeId
            };
        }

        public static void CreateCall(BvCallEntity call)
        {
            // TODO: Huge perf bottleneck, ideally change to BvSvyScheduleAdapter.Insert
            call.CallID = CallQueueService.AddCallToDb(call, 0, null);
        }

        public static BvInterviewEntity CreateInterviewWithCall(int surveyId, int? interviewID)
        {
            var interview = NewInterview(surveyId);
            interview.ID = interviewID ?? interview.ID;
            CreateInterview(interview);

            var call = NewCall(interview);

            var entity = CallQueueService.ConvertCall2SvySchedule(call);
            BvSvyScheduleAdapter.Insert(entity);

            return interview;
        }

        public static BvInterviewEntity CreateInterviewWithCall(int surveyId)
        {
            return CreateInterviewWithCall(surveyId, null);
        }

        public static List<BvInterviewEntity> CreateInterviewsWithCalls(int surveyId, int count)
        {
            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;

            CreateInterviewsWithCalls(surveyId, count, out interviews, out calls);

            return interviews;
        }

        public static void CreateInterviewsWithCalls(int surveyId, int count, out List<BvInterviewEntity> interviews, out List<BvCallEntity> calls)
        {
            interviews = new List<BvInterviewEntity>();
            calls = new List<BvCallEntity>();

            while (count-- > 0)
            {
                var interview = NewInterview(surveyId);
                CreateInterview(interview);

                var call = NewCall(interview);
                CreateCall(call);

                interviews.Add(interview);
                calls.Add(call);
            }
        }

        public static void CheckInterviewsWithCalls(IEnumerable<BvInterviewEntity> interviews, IEnumerable<BvCallEntity> calls)
        {
            foreach (var interview in interviews)
            {
                CheckInterview(interview);
            }

            foreach (var call in calls)
            {
                CheckCall(call);
            }
        }

        public static void FireEvent(BvInterviewWithOriginEntity interview)
        {
            FireEvent(interview, DateTime.UtcNow);
        }

        public static void FireEvent(BvInterviewWithOriginEntity interview, DateTime eventTime)
        {
            //
            // we need make a copy because InterviewRepository.Update can modify entity inside
            InterviewRepository.Update(CloneBvInterviewEntity(interview), new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, EventTime = eventTime, opType = OperationType.MovedAndReschedule });
        }

        public static void CheckCall(BvCallEntity call)
        {
            var actualCall = CallQueueService.GetCallAndNoLock(call.SurveySID, call.InterviewID);
            TestAssert.AreEqual(call, actualCall);
        }

        public static void CheckCall(BvCallEntity call, DateTime startShiftTime, DateTime finishShiftTime)
        {
            var actualCall = CallQueueService.GetCallAndNoLock(call.SurveySID, call.InterviewID);
            TestAssert.AreEqual(call, actualCall, startShiftTime, finishShiftTime);
        }

        public static bool IsCallExists(int surveySID, int interviewID)
        {
            var call = CallQueueService.GetCallAndNoLock(surveySID, interviewID);

            return call != null;
        }

        public static void AssignCatiPersonToSurvey(int surveySID, int personSID)
        {
            AssignmentService.AssignResourceToSurvey(surveySID, personSID, CallCenterTools.DefaultId);
        }

        public static void DeassignCatiPersonFromSurvey(int surveySID, int personSID)
        {
            AssignmentService.DeassignResourceFromSurvey(surveySID, personSID, CallCenterTools.DefaultId);
        }

        public static void DeassignCatiPersonFromSurveyCalls(int surveySID, int personSID)
        {
            ServiceLocator.Resolve<IAssignmentService>().DeassignResourcesFromSurveyCalls(surveySID, new[] { personSID }, CallCenterTools.DefaultId);
        }

        public static void AddAppointmentAndLinkItWithCall(int interviewSid, int surveySid, DateTime appTime)
        {
            AddAppointment(interviewSid, surveySid, appTime, false);

            var interview = InterviewRepository.GetById(surveySid, interviewSid);
            interview.TransientState = (int)CallOutcome.Appointment;
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions() { ExecutionReason = SchedulingScriptExecutionReason.Processed });
        }

        public static void CopyCallHistoryExToCallHistory(int callHistoryExId)
        {
            var query = @"
INSERT INTO dbo.BvCallHistory (FiredTime,ApptID,ShiftTypeID,InterviewID,SurveyId,ITS,DialingMode,CallState,Priority,TimeInShift,ExpireTime,ExplicitSID,ExplicitType,CellId,OperationId,OperationType,CallCenterId,DialTypeId)
SELECT FiredTime,ApptID,ShiftTypeID,InterviewID,SurveyId,ITS,DialingMode,CallState,Priority,TimeInShift,ExpireTime,ExplicitSID,ExplicitType,CellId,OperationId,OperationType,CallCenterId,DialTypeId
FROM dbo.BvCallHistoryEx
WHERE Id >= @callHistoryExId;
DELETE FROM dbo.BvCallHistoryEx WHERE Id >= @callHistoryExId";
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            framework.DbEngine.ExecuteNonQuery(query, new SqlParameter("@callHistoryExId", callHistoryExId));
        }
        
        public static void AddAppointment(int interviewSid, int surveySid, DateTime appTime)
        {
            AddAppointment(interviewSid, surveySid, appTime, false);
        }

        
        public static void AddAppointment(
            int interviewSid,
            int surveySid,
            DateTime appTime,
            bool allowApptOutsideShifts,
            int? timeZoneId = null)
        {
            ServiceLocator.Resolve<IInterviewService>().AddAppointments(
                surveySid,
                interviewSid,
                0,
                new[]
                {
                    new Appointment
                    {
                        time = appTime,
                        expirationTime = appTime.AddDays(1),
                        contactName = "interviewSID=" + interviewSid,
                        appointmentTimeZone = timeZoneId != null ? new Timezone() { Id = timeZoneId.Value } : null
                    }
                },
                allowApptOutsideShifts);
        }

        public static int CountHistoryRecordsForInterview(BvInterviewEntity interview, DateTime minFiredTime)
        {
            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;
            return framework.DbEngine.ExecuteScalar<int>(
                    @"SELECT COUNT(*) from BvHistory WHERE InterviewID = @IID AND FiredTime >= @minFiredTime",
                    CommandType.Text,
                    new SqlParameter("@IID", interview.ID),
                    new SqlParameter("@minFiredTime", minFiredTime)
                );
        }

        private void AddSampleWithoutErrorChecking(string projectID, int batchID, int mode)
        {
            var managementService = new ManagementService();

            managementService.AddSample(projectID, batchID, mode, 0);
        }

        private void ProcessSampleWithoutErrorChecking(string projectID, int batchID, int sampleMode, int schedulingMode)
        {
            var managementService = new ManagementService();

            managementService.ProcessSample(projectID, batchID, sampleMode, schedulingMode);
        }

        public void AddSample(string projectID, int batchID, int mode, int startRespId, int count, IEnumerable<int> timeZones, int[] resources = null)
        {
            IRespondentBatchObtainer respondentBatchObtainer = new FakeRespondentBatchObtainer(startRespId, count, timeZones, resources);

            ServiceLocator.RegisterInstance(respondentBatchObtainer);

            AddSampleWithoutErrorChecking(projectID, batchID, mode);
            AddSampleValidateState(projectID, batchID);
        }

        public void ProcessSample(string projectID, int batchID, int sampleMode, int schedulingMode, int startRespId, int count, IEnumerable<int> timeZones, int[] resources = null)
        {
            IRespondentBatchObtainer respondentBatchObtainer = new FakeRespondentBatchObtainer(startRespId, count, timeZones, resources);
            ServiceLocator.RegisterInstance(respondentBatchObtainer);

            ProcessSampleWithoutErrorChecking(projectID, batchID, sampleMode, schedulingMode);
            ProcessSampleValidateState(projectID, batchID, sampleMode);
        }

        public void AddSample(string projectID, int batchID, int mode)
        {
            IRespondentBatchObtainer respondentBatchObtainer = ServiceLocator.Resolve<IRespondentBatchObtainer>();
            ServiceLocator.RegisterInstance(respondentBatchObtainer);

            AddSampleWithoutErrorChecking(projectID, batchID, mode);
            AddSampleValidateState(projectID, batchID);
        }

        public void ProcessSample(string projectID, int batchID, int sampleMode, int schedulingMode)
        {
            IRespondentBatchObtainer respondentBatchObtainer = ServiceLocator.Resolve<IRespondentBatchObtainer>();
            ServiceLocator.RegisterInstance(respondentBatchObtainer);

            ProcessSampleWithoutErrorChecking(projectID, batchID, sampleMode, schedulingMode);
            ProcessSampleValidateState(projectID, batchID, sampleMode);
        }

        private void AddSampleValidateState(string projectID, int batchID)
        {
            ExecuteAllAsyncOperations();

            var managementService = new ManagementService();
            var sampleState = managementService.AddSampleGetState(batchID, out _);
            
            if (sampleState == (int)ProcessSampleAsyncResult.InProgress)
            {
                throw new Exception(
                    String.Format(
                        "AddSample async operation is not executed synchronously, ProjectID = {0}, BatchID = {1}",
                        projectID,
                        batchID));
            }

            if (sampleState == (int)ProcessSampleAsyncResult.Error)
            {
                throw new Exception(
                    String.Format(
                        "AddSample async operation failed, ProjectID = {0}, BatchID = {1}",
                        projectID,
                        batchID));
            }
        }


        private void ProcessSampleValidateState(string projectID, int batchID, int sampleMode)
        {
            ExecuteAllAsyncOperations();

            var managementService = new ManagementService();
            var sampleState = managementService.ProcessSampleGetState(batchID, sampleMode, out _);
            
            if (sampleState == (int)ProcessSampleAsyncResult.InProgress)
            {
                throw new Exception(
                    String.Format(
                        "ProcessSample async operation is not executed synchronously, ProjectID = {0}, BatchID = {1}",
                        projectID,
                        batchID));
            }

            if (sampleState == (int)ProcessSampleAsyncResult.Error)
            {
                throw new Exception(
                    String.Format(
                        "ProcessSample async operation failed, ProjectID = {0}, BatchID = {1}",
                        projectID,
                        batchID));
            }
        }
        
        /// <summary>
        /// Enables change tracking for the survey database and the specified tables in it.
        /// </summary>
        /// <param name="cfSurveyDb"></param>
        /// <param name="tablesInfo"></param>
        public static void EnableChangeTracking(DatabaseEngine cfSurveyDb, TableInfo[] tablesInfo)
        {
            var commandText =
                String.Format(
                    @"ALTER DATABASE {0}
                    set change_tracking=on
                    (
                       change_retention = 1 minutes , --days hours minutes
                       auto_cleanup = on
                    )",
                    cfSurveyDb.DatabaseName);

            cfSurveyDb.ExecuteNonQuery(commandText, CommandType.Text);

            commandText = String.Join(
                Environment.NewLine,
                tablesInfo.Select(
                    x =>
                    String.Format(
                        @"use [{0}]
                             alter table {1}
                             enable change_tracking
                             WITH (TRACK_COLUMNS_UPDATED = ON)",
                        cfSurveyDb.DatabaseName,
                        x.Name)).ToArray());

            cfSurveyDb.ExecuteNonQuery(commandText, CommandType.Text);
        }

        public static void AssertAggregateData(int surveySid, int interviewCount, int callCount)
        {
            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();
            BvSpAggregateSurveyProcessDeltaAdapter.ExecuteNonQuery();

            IntegrationTestingFramework framework = IntegrationTestingFramework.Instance;

            const string query = "SELECT ScheduledCallsCount, SuspendedCallsCount " +
                "FROM BvAggregateSurvey " +
                "WHERE SID = @SID";

            var dataTable = framework.DbEngine.ExecuteDataTable<DataTable>(query,
                CommandType.Text,
                new SqlParameter("@SID", surveySid));

            Assert.AreEqual(1, dataTable.Rows.Count, "there is no record in BvAggregateSurvey for this survey");
            Assert.AreEqual(interviewCount, (int)dataTable.Rows[0]["SuspendedCallsCount"], "SuspendedCallsCount is incorrect");
            Assert.AreEqual(callCount, (int)dataTable.Rows[0]["ScheduledCallsCount"], "ScheduledCallsCount is incorrect");
        }

        public static void SetInterviewItsAppointment(int surveySID, int interviewSID)
        {
            var options = new SchedulingScriptExecutionOptions()
            {
                ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled,
                ITS = 1,
                opType = OperationType.MovedAndReschedule
            };
            InterviewService.Schedule(surveySID, interviewSID, options);
        }


        public void CreateHistoryRecords(int surveryId, int personId, DateTime[] firedTimes, int startInterviewId = 1, int duration = 100, int waitingTime = 5, byte extendedStatus = 13, int callCenterId = 0)
        {
            for (int i = 0; i < firedTimes.Length; i++)
                BvHistoryAdapter.Insert(new BvHistoryEntity
                {
                    SurveyId = surveryId,
                    Duration = duration,
                    FiredTime = firedTimes[i],
                    InterviewId = startInterviewId + i,
                    WaitingTime = waitingTime,
                    PersonSID = personId,
                    ITS = extendedStatus,
                    RoleID = 2,
                    CallCenterID = callCenterId
                });
        }

        public void SaveInterviewHistoryAndControlData(InterviewHistoryData historyData, InterviewControlData controlData, BvInterviewTimings timings = null, int? linkedInterviewSessionId = null)
        {
            var survey = SurveyRepository.GetByName(historyData.projectID);
            timings = timings ?? new BvInterviewTimings() { InterviewDurationTime = historyData.totalDuration };

            ServiceLocator.Resolve<IInterviewHistoryAndDataProcessor>().SaveHistoryAndControlData(false, historyData, controlData, timings, survey, null, null, true, null);
        }

        public void SaveInterviewHistoryAndControlDataWithScheduling(InterviewHistoryData historyData, InterviewControlData controlData, BvSurveyEntity survey, BvInterviewTimings timings = null, int? linkedInterviewSessionId = null)
        {
            ServiceLocator.Resolve<IInterviewHistoryAndDataProcessor>().SaveHistoryAndControlData(true, historyData, controlData, timings, survey, linkedInterviewSessionId, null, true, null);
        }


        public static BvInterviewWithOriginEntity CloneBvInterviewEntity(BvInterviewWithOriginEntity interview)
        {
            return new BvInterviewWithOriginEntity(interview.Origin)
            {
                ID = interview.ID,
                SurveySID = interview.SurveySID,
                TelephoneNumber = interview.TelephoneNumber,
                RespondentName = interview.RespondentName,
                TimezoneID = interview.TimezoneID,
                TransientState = interview.TransientState,
                LastCallTime = interview.LastCallTime,
                LastCallPersonSID = interview.LastCallPersonSID,
                Duration = interview.Duration,
                ExtensionNumber = interview.ExtensionNumber,
                ConfirmitSid = interview.ConfirmitSid,
                BatchID = interview.BatchID,
                LastChannelID = interview.LastChannelID
            };
        }


        public static void SyncResponseControl(DatabaseEngine db, int surveySid)
        {
            db.ExecuteNonQuery(
                    @"insert into response_control(respid, its) 
                        select ID, TransientState 
                        from BvInterview 
                            WHERE SurveySID = @SurveySID",
                 CommandType.Text,
                 new SqlParameter("@SurveySID", surveySid));
        }

        public static void CheckResponseControl(DatabaseEngine db, int surveySid)
        {
            var count = db.ExecuteScalar<int>(
                    @"select COUNT(*) from response_control rc
                        left join BvInterview i
                        on rc.respid = i.ID and i.SurveySID = @SurveySID 
                        WHERE rc.ITS <> i.TransientState",
                 CommandType.Text,
                 new SqlParameter("@SurveySID", surveySid));

            Assert.AreEqual(0, count, "We have several interviews in response_control and bvInterview table which have different ITS");
        }

        public static string ReadAllText(string path)
        {
            path = Path.Combine(IntegrationTestingFramework.Instance.Cfg.TestDataPath, path);
            return File.ReadAllText(path);
        }

        public static void WriteAllText(string path, string text)
        {
            path = Path.Combine(IntegrationTestingFramework.Instance.Cfg.TestDataPath, path);
            File.WriteAllText(path, text);
        }

        public static void AssignResourceToInterview(int surveyId, int interviewId, int personId)
        {
            AssignmentService.AssignResourceToInterview(surveyId, interviewId, personId, CallCenterTools.DefaultId);
        }

        public static void AssignCatiPersonsToSurvey(int surveyId, int[] resources)
        {
            AssignmentService.AssignResourcesToSurvey(surveyId, resources, CallCenterTools.DefaultId);
        }

        public static void LoginPerson(int personId, string stationId)
        {
            var stationInfo = new StationIdParser().Parse(stationId);
            if (stationInfo.DialerId == 0)
            {
                // Dialer id is 1 by default
                stationInfo.DialerId = 1;
            }

            PersonService.LoginPerson(personId, stationInfo);
            RunSchedulingProcedure();
        }

        public static void ForceProcessingAsyncTriggers()
        {
            BvSpSvyScheduleRuntimeStatistics_ProcessDeltaAdapter.ExecuteNonQuery();
            BvSpAggregateSurveyProcessDeltaAdapter.ExecuteNonQuery();
            BvSpSampleStatusSummaryProcessDeltaAdapter.ExecuteNonQuery();
        }

        public static void TraceQuery(DatabaseEngine db, string note, string query)
        {
            var table = db.ExecuteDataTable<DataTable>(query, CommandType.Text);
            var result = FormatDataTable(table);
            Trace.TraceInformation("Trace query with note '{0}': {1}", note, result);
        }

        public static string FormatDataTable(DataTable table)
        {
            var sb = new StringBuilder();
            sb.AppendLine();

            var orderColumns = table.Columns.Cast<DataColumn>().OrderBy(x => x.Ordinal).Select(x => x.ColumnName).ToArray();
            var column2Len = new Dictionary<string, int>();

            foreach (var columnName in orderColumns)
            {
                var len = table.Rows.Cast<DataRow>().Max(x => GetFormatValue(x[columnName]).Length);
                len = Math.Max(len, columnName.Length) + 1;
                column2Len[columnName] = len;
                sb.Append(columnName.PadRight(len));
            }

            foreach (DataRow row in table.Rows)
            {
                sb.AppendLine();
                foreach (var columnName in orderColumns)
                {
                    var value = GetFormatValue(row[columnName]);
                    sb.Append(value.PadRight(column2Len[columnName]));
                }
            }

            return sb.ToString();
        }

        public static string Format<T>(IEnumerable<T> collection)
        {
            var formatter = TestAssert.CreateFormatter<T>();
            return formatter.Format(collection.ToArray());
        }

        private static string GetFormatValue(object value)
        {
            if (value is DBNull)
                return "NULL";
            if (value is DateTime)
            {
                var result = ((DateTime)value).ToString("MM/dd/yyyy HH:mm:ss.ffff", CultureInfo.InvariantCulture);

                return result;
            }

            if (value is byte[] bytes)
            {
                return "0x" + BitConverter.ToString(bytes).Replace("-", "");
            }

            return value.ToString();
        }

        public AsyncOperationResult LaunchSurvey(string projectId, LaunchSurveyParameters parameters)
        {
            _framework.RegisterSurveyDbName(projectId, parameters.SurveyProperties.CfSqlServerConnectionString);

            var operationId = new ManagementService().LaunchSurvey(projectId, parameters);
            var operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operationId);
            var result = ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);
            ExecuteAllAsyncOperations();
            return result;
        }

        public AsyncOperationResult ExecuteRoutineMaintenance()
        {
            var parameters = new Core.AsyncOperations.Operations.ExecuteRoutineMaintenance.Parameters();

            var operation = ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                0,
                "ExecuteRoutineMaintenance",
                true,
                parameters,
                AsyncOperationConstants.NormalPriority,
                "system");


            var result = ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);
            ExecuteAllAsyncOperations();
            return result;
        }

        public static BvAppointmentEntity NewAppointment(BvInterviewEntity interview)
        {
            return new BvAppointmentEntity()
            {
                InterviewSID = interview.ID,
                SurveySID = interview.SurveySID,
                State = 1,
                TZID = interview.TimezoneID,
                Time = DateTime.UtcNow,
                RespondentName = "respName",
                ContactName = "Contact name",
            };
        }

        public static void CreateAppointment(BvAppointmentEntity appointment)
        {
            int appId;
            BvSpAppointmentUpdateAdapter.ExecuteNonQuery(
                appointment.ID,
                appointment.SurveySID,
                appointment.InterviewSID,
                appointment.Time,
                appointment.ExpTime,
                appointment.ContactName,
                appointment.State,
                appointment.TZID, out appId);
            appointment.ID = appId;

            AppointmentRepository.InsertUpdate(appointment);
        }

        public static void SetCallsStateToSentToDialer(DatabaseEngine db, int surveySid, int startId, int endId)
        {
            SetCallsState(db, surveySid, startId, endId, -2);
        }

        public static void SetCallsStateToSoftDeleted(DatabaseEngine db, int surveySid, int startId, int endId)
        {
            SetCallsState(db, surveySid, startId, endId, 0);
        }
        
        public static void SetCallsState(DatabaseEngine db, int surveySid, int startId, int endId, int callState)
        {
            db.ExecuteNonQuery(
                    @"UPDATE bvsvyschedule SET CallState = @CallState 
                            WHERE SurveySID = @SurveySID AND ID BETWEEN @Start AND @End",
                 CommandType.Text,
                 new SqlParameter("@SurveySID", surveySid),
                 new SqlParameter("@Start", startId),
                 new SqlParameter("@End", endId),
                 new SqlParameter("@CallState", callState));
        }

        public static string Format(TestDataContext context, string expected)
        {
            // This used to find and parse following construnctions: {  tag:filed }
            // where ':field' is optional
            var pattern = new Regex(@"{[\s]*(?<content>(?<tag>[\w\.]+)(:(?<field>[\w]+))?)[\s]*}");

            var cache = new Dictionary<string, string>();
            var controllers = context.GetAll().ToArray();

            expected = pattern.Replace(expected, (match) =>
            {
                var content = match.Groups["content"].Value;
                string result;

                if (!cache.TryGetValue(content, out result))
                {
                    var tag = match.Groups["tag"].Value;
                    string field = match.Groups["field"].Value;

                    var controller = controllers.FirstOrDefault(x => x.Tag == tag);
                    if (controller == null)
                        throw new Exception($"Controller with Tag='{tag}' not found.");

                    if (String.IsNullOrEmpty(field))
                    {
                        result = controller.Id.ToString();
                    }
                    else
                    {
                        var value = controller.Model.GetType().GetProperty(field).GetValue(controller.Model);
                        result = value == null ? "<NULL>" : value.ToString();
                    }

                    cache[content] = result;
                }

                return new String(' ', Math.Max(0, match.Value.Length - result.Length)) + result;
            });
            return expected;
        }

        public static void RunSchedulingProcedure(DateTime? time = null)
        {
            ServiceLocator.Resolve<ICallQueueService>().Schedule(time);
        }
    }
}
