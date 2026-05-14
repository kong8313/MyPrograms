using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.Test.Common.Attributes;
using Newtonsoft.Json;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.ManagementServiceTest
{
    [TestClass]
    public class ManagementServiceTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private RespondentTools _respondentTools;
        private ISurveyStateService _surveyStateService;
        private IPersonDeferredMonitoringRepository _personDeferredMonitoringRepository;
        private ISurveyRepository _surveyRepository;
        
        private string ProjectId { get; set; }
        private string CfSurveyDbName { get; set; }

        private int _surveySid;
        private const string UserName = "testUser";
        private const string Password = "password";
        private const string ExtensionNumber = "101010";

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _respondentTools = new RespondentTools(_framework);

            ProjectId = _framework.TestSurveyName;
            CfSurveyDbName = _framework.TestSurveyDatabaseName;
            ServiceLocator.Resolve<ICallCenterRepository>();
            ServiceLocator.Resolve<ICallCenterService>();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _personDeferredMonitoringRepository = ServiceLocator.Resolve<IPersonDeferredMonitoringRepository>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        /// <summary>
        /// 1. Create survey using AddSurvey method
        /// 2. Check that created survey have SurveyState = Close
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_AddSurvey_Success()
        {
            _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            BvSurveyEntity survey = SurveyRepository.GetByName(ProjectId);
            Assert.AreEqual((int)SurveyState.Close, survey.State, "New survey has wrong state");
        }

        /// <summary>
        /// 1. Create survey using AddSurvey method
        /// 2. Check that created survey has valid SatateGroupId
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM"), Bug(40702)]
        public void ManagementServiceTest_AddSurvey_CheckStateGroupId()
        {
            _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            BvSurveyEntity survey = SurveyRepository.GetByName(ProjectId);

            Assert.AreEqual(StateGroupRepository.GetDefault().ID, survey.StateGroupID, "New survey has wrong state group ID");
        }

        /// <summary>
        /// Test checks that surveys with id more then 999999999 can be created
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void SurveyHasVeryLongName_SurveySuccessfullyCreated()
        {
            _backendTools.CreateSurvey("p1234567890", _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));
        }


        /// <summary>
        /// 1. Create survey using AddSurvey method
        /// 2. Delete survey using DeleteSurvey method
        /// 3. Check that no survey
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_DeleteSurvey_Success()
        {
            // Create survey
            _backendTools.CreateSurvey(
                ProjectId,
                _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            // Delete survey            
            BackendTools.DeleteSurvey(ProjectId);

            BvSurveyEntity survey = SurveyRepository.TryGetByName(ProjectId);
            Assert.IsNull(survey, "Survey isn't delete");
        }

        /// <summary>
        /// 1. Create survey using AddSurvey method
        /// 2. Delete survey Db
        /// 3. Delete survey using DeleteSurvey method
        /// 4. Check that no survey
        /// </summary>
        [TestMethod, Owner(@"FIRM\OlegZ")]
        public void ManagementServiceTest_DeleteSurveyIfSurveyDbDeleted_Success()
        {
            var surveyName = $"p{Process.GetCurrentProcess().Id + new Random().Next(1, 9)}";
            var surveyDbName = $"testSurvey_{surveyName}";

            var connectionString = _framework.GetConfirmitSqlServerConnectionString(surveyDbName);
            var master = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" }.ToString();
            var databaseTools = new DatabaseTools(master);
            var sqlObjectCreator = new SqlObjectCreator(_framework);

            // Create survey
            sqlObjectCreator.CreateTestSurveyDatabase(surveyDbName);
            _backendTools.CreateSurvey(surveyName, connectionString);

            var dbExists = databaseTools.IsDatabaseExists(surveyDbName);
            Assert.IsTrue(dbExists);

            // Delete survey db
            databaseTools.DropDatabase(surveyDbName);
            dbExists = databaseTools.IsDatabaseExists(surveyDbName);
            Assert.IsFalse(dbExists);

            // Delete survey            
            BackendTools.DeleteSurvey(surveyName);

            BvSurveyEntity survey = SurveyRepository.TryGetByName(surveyName);
            Assert.IsNull(survey, "Survey isn't delete");
        }


        /// <summary>
        /// 1. Create survey using AddSurvey method
        /// 2. Set permission for these interviewer and survey using UpdateSurveyAccessList method
        /// 3. Check that permission was set
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_UpdateSurveyAccessList_AddAccess()
        {
            const string userId = "grigoryk";

            // Create survey
            _backendTools.CreateSurvey(
                ProjectId,
                _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));
            BvSurveyEntity survey = SurveyRepository.GetByName(ProjectId);

            // Set access            
            new ManagementService().UpdateSurveyAccessList(userId, ProjectId, true);

            // Chek that permission was set
            List<BvUserSurveyPermissionEntity> items = BvUserSurveyPermissionAdapter.GetAll();

            Assert.AreEqual(1, items.Count, "BvUserSurveyPermission table has wrong rows count");
            Assert.AreEqual(survey.SID, items[0].SurveySID, "BvUserSurveyPermission table has wrong SurveySID parameter");
            Assert.AreEqual(userId, items[0].UserName, "BvUserSurveyPermission table has wrong UserName parameter");
        }


        /// <summary>
        /// 1. Create survey using AddSurvey method
        /// 2. Set permission for those interviewer and survey using UpdateSurveyAccessList method
        /// 3. Unset permission for these interviewer and survey using UpdateSurveyAccessList method
        /// 4. Check that permission was unset
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_UpdateSurveyAccessList_DeleteAccess()
        {
            const string userId = "grigoryk";

            // Create survey
            _backendTools.CreateSurvey(
                ProjectId,
                _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));
            SurveyRepository.GetByName(ProjectId);

            // Set access
            var managementService = new ManagementService();
            managementService.UpdateSurveyAccessList(userId, ProjectId, true);

            // Unset access
            managementService.UpdateSurveyAccessList(userId, ProjectId, false);

            // Chek that permission was unset
            List<BvUserSurveyPermissionEntity> items = BvUserSurveyPermissionAdapter.GetAll();

            Assert.AreEqual(0, items.Count, "BvUserSurveyPermission table has wrong rows count");
        }


        /// <summary>
        /// 1. Create survey using AddSurvey method
        /// 2. Change property of survey using UpdateSurveyProperties method
        /// 3/ Chek that properties was changed
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_UpdateSurveyProperties_Success()
        {
            // Create survey
            _backendTools.CreateSurvey(
                ProjectId,
                _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));
            BvSurveyEntity survey = SurveyRepository.GetByName(ProjectId);

            // Change property of survey
            const string confirmitProjectName = "TestConfirmitProjectName";
            const int dialingMode = 4;
            const bool openAndReview = true;
            const bool voiceRecording = true;
            const bool screenRecording = true;
            const bool supportBlacklist = true;
            const bool allowRespondentsDynamicCreation = true;
            const string notificationEmail = "test@firmsw.no";
            bool enforceHttps = !survey.EnforceHttps;
            new ManagementService().UpdateSurveyProperties(ProjectId,
                confirmitProjectName, dialingMode, openAndReview, voiceRecording, screenRecording, supportBlacklist, allowRespondentsDynamicCreation, notificationEmail, enforceHttps);

            // Check that properties was changed            
            BvSurveyEntity surveyCheck = SurveyRepository.GetByName(ProjectId);

            Assert.AreEqual((DialingMode)dialingMode, (DialingMode)surveyCheck.DialMode, "Dialing mode isn't set");
            Assert.AreEqual(confirmitProjectName, surveyCheck.Description, "Confirmit project name isn't set");
            Assert.AreEqual(openAndReview, Convert.ToBoolean(surveyCheck.ForceOpnRev), "Open and review parameter isn't set");
            Assert.AreEqual(voiceRecording, Convert.ToBoolean(surveyCheck.RecWholeInt), "Voice recording isn't set");
            Assert.AreEqual(screenRecording, Convert.ToBoolean(surveyCheck.InterviewScreenRecording), "Screen recording isn't set");
            Assert.AreEqual(supportBlacklist, Convert.ToBoolean(surveyCheck.IsTelephoneBlacklistSupported), "Support telephone blacklist isn't set");
            Assert.AreEqual(allowRespondentsDynamicCreation, Convert.ToBoolean(surveyCheck.IsRespondentsDynamicCreationAllowed), "Dynamic respondent creation isn't allowed");
            Assert.AreEqual(notificationEmail, surveyCheck.NotificationEmail, false, "NotificationEmail isn't set correctly.");
            Assert.AreEqual(enforceHttps, surveyCheck.EnforceHttps, "EnforceHttps flag isn't set correctly");
        }

        /// <summary>        
        /// 1. Create survey using AddSurvey method
        /// 2. Check that survey is closed using IsSurveyOpen (return False)        
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_IsSurveyOpen_ReturnFalseForNewSurvey()
        {
            _surveySid = _backendTools.CreateSurvey(
                ProjectId,
                _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            bool returnValue = new ManagementService().IsSurveyOpen(ProjectId);
            Assert.IsFalse(returnValue, "IsSurveyOpen return wrong value for new survey");
        }


        /// <summary>    
        /// 1. Create survey using AddSurvey method
        /// 2. Open survey using SurveyService.Open method
        /// 3. Execute IsSurveyOpen
        /// 4. Check that it returns True        
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_IsSurveyOpen_ReturnTrueForOpenedSurvey()
        {
            _surveySid = _backendTools.CreateSurvey(
                ProjectId,
                _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            _surveyStateService.Open(_surveySid);
            bool returnValue = new ManagementService().IsSurveyOpen(ProjectId);
            Assert.IsTrue(returnValue, "IsSurveyOpen return wrong value for opened survey");
        }


        /// <summary>    
        /// 1. Create survey using AddSurvey method
        /// 2. Open survey using SurveyService.Open method
        /// 3. Close survey using SurveyService.Close method
        /// 4. Execute IsSurveyOpen
        /// 5. Check that it returns False
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_IsSurveyOpen_ReturnFalseForClosedSurvey()
        {
            _surveySid = _backendTools.CreateSurvey(
                ProjectId,
                _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            _surveyStateService.Open(_surveySid);
            _surveyStateService.CloseSurvey(_surveySid);

            bool returnValue = new ManagementService().IsSurveyOpen(ProjectId);
            Assert.IsFalse(returnValue, "IsSurveyOpen return wrong value for closed survey");
        }

        /// <summary>
        /// 1. create and open survey
        /// 2. create interviewer and assign to survey
        /// 3. login interviewer
        /// 4. close survey
        /// 5. check that IsSurveyOpen returns True
        /// 6. logout interviewer
        /// 7. check that IsSurveyOpen returns False
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexeyN")]
        public void ManagementServiceTest_IsSurveyOpen_ReturnTrueWhenClosedSurveyHasTasks()
        {
            var managementService = new ManagementService();

            //
            // create and open survey
            _surveySid = _backendTools.CreateSurvey(
                ProjectId,
                _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            _surveyStateService.Open(_surveySid);

            //
            // create interviewer
            int personSid = PersonTools.CreatePerson("test person1");

            //
            // emulate interviewer login
            var task = new BvTasksEntity
            {
                SurveySID = _surveySid,
                PersonSID = personSid,
                StartSessionTime = DateTime.UtcNow,
                EncryptionKey = new byte[0],
                EncryptionIV = new byte[0]
            };

            BvTasksAdapter.Insert(task);

            //
            // close survey and check result
            _surveyStateService.CloseSurvey(_surveySid);

            bool returnValue = managementService.IsSurveyOpen(ProjectId);
            Assert.IsTrue(returnValue, "IsSurveyOpen return False but True is expected");

            //
            // delete task (emulate interviewer logout) and check result
            BvTasksAdapter.DeleteByCondition(
                "SurveySID = @SurveySID",
                new SqlParameter("@SurveySID", _surveySid));

            returnValue = managementService.IsSurveyOpen(ProjectId);
            Assert.IsFalse(returnValue, "IsSurveyOpen return True but False is expected");
        }

        /// <summary>
        /// Create survey
        /// Add interview with call
        /// Set appointment
        /// Call GetCATIAppointmentTime
        /// Check that correct appointment time was returned
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ManagementServiceTest_GetCATIAppointmentTime_ReturnCorrectAppointmentTime()
        {
            _backendTools.LaunchAllHoursScript();
            _surveySid = _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            const int tzID = 16; //Moscow tz
            TimezoneManager.AddTimezone(tzID);
            var interview = BackendTools.NewInterview(_surveySid);

            interview.TimezoneID = tzID;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            var setAppointmentTime = DateTime.Parse("10.10.2020 10:10:10.000");
            BackendTools.AddAppointment(interview.ID, _surveySid, setAppointmentTime);

            var getAppointmentTime = new ManagementService().GetCATIAppointmentTime(ProjectId, interview.ID);

            Assert.AreEqual(
                TimezoneManager.ConvertToTzLocalTime(tzID, setAppointmentTime),
                getAppointmentTime,
                "Wrong appointment time was returned.");
        }

        /// <summary>
        /// Create survey and person
        /// Add interview with call
        /// Set appointment
        /// Send completed call hotification
        /// check that history data contains correct appointmentId
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexeyN"), Bug(39092)]
        public void ManagementServiceTest_CompeteInterviewWithAppointment_AppointmentIdIsInHistory()
        {
            string userName = Guid.NewGuid().ToString();
            string password = userName;
            //
            // create survey and person
            _backendTools.LaunchAllHoursScript();
            _surveySid = _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            int personSid = PersonTools.CreatePerson(userName, password, AgentTaskChoiceMode.Manual);
            BackendTools.AssignCatiPersonToSurvey(_surveySid, personSid);

            //
            // add interview with call
            var interview = BackendTools.NewInterview(_surveySid);

            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            var appointmentTime1 = DateTime.UtcNow + TimeSpan.FromHours(2);
            BackendTools.AddAppointment(interview.ID, _surveySid, appointmentTime1);

            interview.TransientState = 1;
            InterviewRepository.Update(interview, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.Processed });

            var catiWsHelper = new CatiWsHelper(userName, password);

            var consoleDescriptor = new ConsoleDescription();

            catiWsHelper.ConsoleService.Login("", consoleDescriptor, out _, out _, out _);
            catiWsHelper.ConsoleService.StartInterview(ProjectId, interview.ID);
            //
            // add appointments
            var appointmentTime2 = DateTime.UtcNow + TimeSpan.FromHours(3) + TimeSpan.FromMinutes(30);
            BackendTools.AddAppointment(interview.ID, _surveySid, appointmentTime2);


            catiWsHelper.ConsoleService.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "1" });
            //
            // get appointment id
            var appointments = BvAppointmentAdapter.GetByCondition("State = 1"); // active appointment has call
            Assert.IsTrue(appointments.Count == 1, "there is no appointment that has call");

            var appointment = appointments.First();
            Assert.IsTrue(appointment.Time.Hour == appointmentTime2.Hour && appointment.Time.Minute == appointmentTime2.Minute);

            //
            // check data
            var history = BvHistoryAdapter.GetByCondition(
                "ITS = 1 and AppointmentID = @AppointmentID",
                new SqlParameter("@AppointmentID", appointment.ID));

            Assert.IsTrue(history.Count == 1, "there is not history record for specified appointment");

            var callHistory = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId", new SqlParameter("@SurveyId", _surveySid));
            Assert.AreEqual(1, history.Count);

            var callHistoryRecord = callHistory.Last();
            Assert.AreEqual(0, callHistoryRecord.OperationId);
            Assert.AreEqual((int)OperationType.Interview, callHistoryRecord.OperationType);
        }

        /// <summary>
        /// Create survey
        /// Add interview with call
        /// Do not set appointment
        /// Call GetCATIAppointmentTime
        /// Check that null was returned
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void ManagementServiceTest_GetCATIAppointmentTime_GetNullIfNoAppointmentExist()
        {
            _backendTools.LaunchAllHoursScript();
            _surveySid = _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            var interview = BackendTools.NewInterview(_surveySid);
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            var appTime = new ManagementService().GetCATIAppointmentTime(ProjectId, interview.ID);

            Assert.IsNull(appTime);
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void ManagementServiceTest_ActiveInterview_GetCATIDialingAttempts()
        {
            _backendTools.LaunchAllHoursScript();
            _surveySid = _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));
            _surveyStateService.Open(_surveySid);

            BackendTools.CreateInterviewsWithCalls(_surveySid, 1).Single();
            var console1 = TestCatiConsole.CreateAndLoginAsSA(_surveySid, "u1");
            var interview1 = console1.Start();
            
            //check return value for invalid interviewId
            var emptyDialingAttempts = new ManagementService().GetCatiInterviewDialingAttempts(ProjectId, 123);
            Assert.AreEqual(0, emptyDialingAttempts.Length);
            
            //check return value for active interview with no active dial
            emptyDialingAttempts = new ManagementService().GetCatiInterviewDialingAttempts(ProjectId, interview1.ID);
            Assert.AreEqual(0, emptyDialingAttempts.Length);
            
            var task = BvTasksAdapter.GetByCondition("SurveySId=@SurveyId AND InterviewId=@InterviewId", new SqlParameter("SurveyId", _surveySid), new SqlParameter("InterviewId", interview1.ID)).Single();
            var context = task.Context;
            context.ActiveDialId = 2;
            var activeDialStart = DateTime.Now;
            context.ActiveDialStart = activeDialStart;
            context.ActiveDialDialerCallerId = "TetDialerCallerId1";
            context.ActiveDialTelephoneNumber = "123454321";
            context.ActiveDialRingTime = 15;
            context.ActiveDialCallOutcomeMetadata = new Dictionary<string, string>() {
                { "custom1", "123" },
                { "custom2", "321" }
            };
            context.ActiveDialCallOutcome = CallOutcome.Connected;
            
            var previousDialStart = DateTime.Now.AddMinutes(-3);
            var previousDialEnd = DateTime.Now.AddMinutes(-1);
            context.DialHistories = new List<TaskDialHistory>() {
                new TaskDialHistory() {
                    DialerCallerId = "TetDialerCallerId2",
                    TelephoneNumber = "4321",
                    StartTime = previousDialStart,
                    FinishTime = previousDialEnd,
                    RingTime = 10,
                    DialId = 1,
                    CallOutcomeMetadata = new Dictionary<string, string>() {
                        { "custom1", "12345" },
                        { "custom2", "54321" }
                    },
                    DialerCallOutcome = CallOutcome.Busy
                }
            };
            
            task.JsonContext = JsonConvert.SerializeObject(context);
            BvTasksAdapter.Update(task);
            
            var dialingAttempts = new ManagementService().GetCatiInterviewDialingAttempts(ProjectId, interview1.ID);
            
            Assert.AreEqual(2, dialingAttempts.Length);

            var previousDial = dialingAttempts[0];
            var activeDial = dialingAttempts[1];
            
            Assert.AreEqual(2, activeDial.DialId);
            Assert.AreEqual(activeDialStart, activeDial.StartTime);
            Assert.AreEqual(null, activeDial.FinishTime);
            Assert.AreEqual(15, activeDial.RingTime);
            Assert.AreEqual("123", activeDial.CallOutcomeMetadata["custom1"]);
            Assert.AreEqual("321", activeDial.CallOutcomeMetadata["custom2"]);
            Assert.AreEqual("TetDialerCallerId1", activeDial.DialerCallerId);
            Assert.AreEqual("123454321", activeDial.TelephoneNumber);
            Assert.AreEqual((int)CallOutcome.Connected, activeDial.DialerCallOutcome);
            Assert.AreEqual("", activeDial.DialerTelephoneNumber);
            
            Assert.AreEqual(1, previousDial.DialId);
            Assert.AreEqual(previousDialStart, previousDial.StartTime);
            Assert.AreEqual(previousDialEnd, previousDial.FinishTime);
            Assert.AreEqual(10, previousDial.RingTime);
            Assert.AreEqual("12345", previousDial.CallOutcomeMetadata["custom1"]);
            Assert.AreEqual("54321", previousDial.CallOutcomeMetadata["custom2"]);
            Assert.AreEqual("TetDialerCallerId2", previousDial.DialerCallerId);
            Assert.AreEqual("4321", previousDial.TelephoneNumber);
            Assert.AreEqual((int)CallOutcome.Busy, previousDial.DialerCallOutcome);
            Assert.AreEqual("", previousDial.DialerTelephoneNumber);
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void ManagementServiceTest_CompletedInterview_GetCATIDialingAttempts()
        {
            var test = new TestCati2(true, true, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            var interviews = test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(test.SurveyName, interviews[0].ID);

            const int initiator = 0;
            var callOutcomeMetadata1 = new Dictionary<string, string>() {
                { "custom1", "123" },
                { "custom2", "321" }
            };
            var dialerCallerId1 = "testDialer1";
            var ringTime1 = 13;
            test.Dial(interview, initiator, true, CallOutcome.Connected, dialerCallerId1, ringTime1, callOutcomeMetadata1);
            var callOutcomeMetadata2 = new Dictionary<string, string>() {
                { "custom1", "123" },
                { "custom2", "321" }
            };
            var dialerCallerId2 = "testDialer2";
            var ringTime2 = 13;
            test.Redial(interview, CallOutcome.Connected, dialerCallerId2, ringTime2, callOutcomeMetadata2, "12345");
            var callOutcomeMetadata3 = new Dictionary<string, string>() {
                { "custom1", "12453" },
                { "custom2", "54321" }
            };
            var dialerCallerId3 = "testDialer3";
            var ringTime3 = 20;
            test.Redial(interview, CallOutcome.Connected, dialerCallerId3, ringTime3, callOutcomeMetadata3, "54321");
            interview.TransientState = TestCati2.ITS.FakeForComplete;
            test.CompleteInterview_Progressive(interview);

            var dialings = new ManagementService().GetCatiInterviewDialingAttempts(ProjectId, interview.ID);

            Assert.AreEqual(3, dialings.Length);

            Assert.AreEqual(dialerCallerId1, dialings[0].DialerCallerId);
            Assert.AreEqual(ringTime1, dialings[0].RingTime);
            Assert.AreEqual(callOutcomeMetadata1["custom1"], dialings[0].CallOutcomeMetadata["custom1"]);
            Assert.AreEqual(callOutcomeMetadata1["custom2"], dialings[0].CallOutcomeMetadata["custom2"]);
            Assert.AreEqual(interviews[0].TelephoneNumber, dialings[0].TelephoneNumber);

            Assert.AreEqual(dialerCallerId2, dialings[1].DialerCallerId);
            Assert.AreEqual(ringTime2, dialings[1].RingTime);
            Assert.AreEqual(callOutcomeMetadata2["custom1"], dialings[1].CallOutcomeMetadata["custom1"]);
            Assert.AreEqual(callOutcomeMetadata2["custom2"], dialings[1].CallOutcomeMetadata["custom2"]);
            Assert.AreEqual("12345", dialings[1].TelephoneNumber);

            Assert.AreEqual(dialerCallerId3, dialings[2].DialerCallerId);
            Assert.AreEqual(ringTime3, dialings[2].RingTime);
            Assert.AreEqual(callOutcomeMetadata3["custom1"], dialings[2].CallOutcomeMetadata["custom1"]);
            Assert.AreEqual(callOutcomeMetadata3["custom2"], dialings[2].CallOutcomeMetadata["custom2"]);
            Assert.AreEqual("54321", dialings[2].TelephoneNumber);
        }

        /// <summary>
        /// Create survey
        /// Create CATI person group
        /// Create CATI person
        /// Call GetCATIInterviewerName and GetCATIInterviewerDisplayName for this person
        /// Check that correct interviewer name and interviewer display name was returned
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_GetCATIInterviewerNameAndDisplayName_ReturnCorrectName()
        {
            const string setPersonName = "catiUser";
            const string setPersonDisplayName = "catiDisplayUser";
            
            _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            var groupId = PersonTools.CreatePersonGroup("catiGroup");
            var personId = PersonTools.CreatePerson(setPersonName, null, setPersonDisplayName, AgentTaskChoiceMode.Manual, null, CallCenterTools.DefaultId);
            
            PersonService.SetParentGroups(personId, new[] { groupId });

            var getPersonName = new ManagementService().GetCATIInterviewerName(personId);
            Assert.AreEqual(setPersonName, getPersonName, "Wrong cati interviewer name returned.");
            
            var getPersonDisplayName = new ManagementService().GetCatiInterviewerDisplayName(personId);
            Assert.AreEqual(setPersonDisplayName, getPersonDisplayName, "Wrong cati interviewer display name returned.");
        }
        
        /// <summary>
        /// Create survey
        /// Create 3 interviews with calls
        /// Call DeleteRespondentsAsync method and delete 2 interviews
        /// Check that 2 interviews and calls deleted - 1 interview and 1 call stay in db
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexeyN"), Bug(39228)]
        public void ManagementServiceTest_CreateInterviewsWithCalls_DeleteRespondents_CallsDeleted()
        {
            //
            // create survey and launch scheduling script
            _backendTools.LaunchAllHoursScript();
            int surveySid = _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            //
            // create 2 interviews with calls
            var interview1 = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview1);
            var call1 = BackendTools.NewCall(interview1);
            BackendTools.CreateCall(call1);

            var interview2 = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview2);
            var call2 = BackendTools.NewCall(interview2);
            BackendTools.CreateCall(call2);

            var interview3 = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview3);
            var call3 = BackendTools.NewCall(interview3);
            BackendTools.CreateCall(call3);

            //
            // delete respondents (calls resheduling should be called inside the method)
            _respondentTools.DeleteRespondentsAsync(
                ProjectId,
                new[] { interview1.ID, interview3.ID });

            BackendTools.RunSchedulingProcedure();

            //
            // check that interviews and calls were deleted
            var interviewsCount = _framework.DbEngine.ExecuteScalar<int>(
                "select count(*) from bvinterview",
                CommandType.Text);

            Assert.IsTrue(interviewsCount == 1, "interviews were not deleted properly");

            var callsCount = _framework.DbEngine.ExecuteScalar<int>(
                "select count(*) from bvsvyschedule where CallState != 0",
                CommandType.Text);

            Assert.IsTrue(callsCount == 1, "calls were not deleted properly");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(43842)]
        public void ManagementServiceTest_CreateInterviewsWithCalls_DeleteRespondents()
        {
            //
            // create survey and launch scheduling script
            _backendTools.LaunchAllHoursScript();
            int surveySid = _backendTools.CreateSurvey(ProjectId);
            _surveyStateService.Open(surveySid);

            var interview1 = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview1);
            var call1 = BackendTools.NewCall(interview1);
            BackendTools.CreateCall(call1);

            var interview2 = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview2);
            var call2 = BackendTools.NewCall(interview2);
            BackendTools.CreateCall(call2);
            //it is necessary for callid != interviewid
            CallQueueService.DeleteCall(surveySid, interview2.ID);
            BackendTools.RunSchedulingProcedure();
            call2 = BackendTools.NewCall(interview2);
            BackendTools.CreateCall(call2);

            var interview3 = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview3);
            var call3 = BackendTools.NewCall(interview3);
            call3.Priority = 1000;
            BackendTools.CreateCall(call3);

            BackendTools.AddAppointment(interview2.ID, surveySid, DateTime.UtcNow.AddHours(2));
            interview2.TransientState = 1;
            InterviewRepository.Update(
                interview2,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.Processed });

            int personSid = PersonTools.CreatePerson("u1");
            BackendTools.AssignResourceToInterview(surveySid, interview1.ID, personSid);
            BackendTools.AssignResourceToInterview(surveySid, interview3.ID, personSid);
            BackendTools.LoginPerson(personSid, "");
            var task = TaskService.LookupByPersonSid(personSid, surveySid);

            Assert.AreEqual(call3.CallID, task.CallID, "Incorrect calls was delivered to interviewier");

            // delete respondents (calls resheduling should be called inside the method)
            _respondentTools.DeleteRespondentsAsync(
              ProjectId,
              new[] { interview1.ID, interview2.ID, interview3.ID });

            BackendTools.RunSchedulingProcedure();

            TestAssert.AreEqual(new BvInterviewEntity[0], BvInterviewAdapter.GetAll());
            Assert.AreEqual(0, BvAppointmentAdapter.GetAll().Count, "There is should not be appointment");

            call3.CallState = -1;
            call3.Resource = personSid;
            TestAssert.AreEqual(new BvCallEntity[] { null, null, null },
                new[] { interview1, interview2, interview3 }.Select(x => CallQueueService.GetCallAndNoLock(surveySid, x.ID)));
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ManagementServiceTest_GetExtendedStatus_Success()
        {
            const string projectId = "p00000991";
            int surveySid = _backendTools.CreateSurvey(projectId);

            var interview = new BvInterviewEntity
            {
                ID = 1,
                SurveySID = surveySid,
                TransientState = (int)CallOutcome.FreshSample
            };

            BackendTools.CreateInterview(interview);

            var managementService = new ManagementService();
            var status = managementService.GetExtendedStatus(projectId, interview.ID);

            Assert.IsTrue(status == (int)CallOutcome.FreshSample);
        }

        [TestMethod, Owner(@"FIRM\DenisM")]
        public void ManagementServiceTest_GetExtendedStatusWithIncorrectRespondentId_ExceptionTrown()
        {
            const string projectId = "p00000991";
            _backendTools.CreateSurvey(projectId);
            var expectedExceptionThrown = false;

            var managementService = new ManagementService();

            try
            {
                managementService.GetExtendedStatus(projectId, -1);
            }
            catch (InternalErrorException ex)
            {
                Assert.AreEqual(string.Format("Interview {0} for survey {1} not found.", -1, projectId), ex.Message);
                expectedExceptionThrown = true;
            }

            Assert.IsTrue(expectedExceptionThrown, "expected exception has not been thrown");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SaveInterviewHistoryAndControlData_CallWithAssociatedDeferredRecord_ItsIsSetAndCallIdIsClearedInDeferredRecord()
        {
            var test = new TestCati2(false, _backendTools);

            const string user = "testUser";
            const string password = "password";

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                user,
                password,
                AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            test.Login(user, password, AgentTaskChoiceMode.Automatic, false);

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.InterviewScreenRecording = true;
            SurveyRepository.Update(survey);

            var interview = test.StartInterview_ManualOrPreview(null, 0);
            var call = CallQueueService.GetCallAndNoLock(interview.SurveySID, interview.ID);
            var deferredRecord = _personDeferredMonitoringRepository.GetByCallId(call.CallID);

            test.WS.WrapUp(interview.ID, false, 1, new CompletedInterviewDetails());

            var freshDeferredRecord = BvPersonDeferredMonitoringPartAdapterEx.GetByIdWithCheck(
                deferredRecord.ID, test.PersonSID);

            var freshInterview = InterviewRepository.GetById(test.SurveySID, interview.ID);

            Assert.IsNull(freshDeferredRecord.CallID);
            Assert.AreEqual(freshInterview.TransientState, freshDeferredRecord.ExtendedStatus);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SaveInterviewHistoryAndControlData_RepeatedGetStateDuringSingleInterview_ProperItsIsSetForDeferredRecord()
        {
            var test = new TestCati2(false, _backendTools);

            const string user = "testUser";
            const string password = "password";

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                user,
                password,
                AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            test.Login(user, password, AgentTaskChoiceMode.Automatic, false);

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.InterviewScreenRecording = true;
            SurveyRepository.Update(survey);

            var interview = test.StartInterview_ManualOrPreview(null, 0);
            var call = CallQueueService.GetCallAndNoLock(interview.SurveySID, interview.ID);
            var oldDeferredRecord = _personDeferredMonitoringRepository.GetByCallId(call.CallID);

            var state = test.StateWS.GetState();
            Assert.AreEqual(state.deferredRecordId, oldDeferredRecord.ID, "The same deferred record should be returned");

            test.WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "13", Status = "Complete" });

            var newDeferredRecord = BvPersonDeferredMonitoringPartAdapterEx.GetById(oldDeferredRecord.ID);
            Assert.AreEqual(TestCati2.ITS.FakeForComplete, newDeferredRecord.ExtendedStatus, "Interview should be completed");
        }

        /// <summary>
        /// Create survey, Add interview with call, Set appointment.
        /// Emulate dbo.respondent.tr_respondent_update trigger with null time zone.
        /// Check that timezoneId is changed in BvInterview table but wasn't changed in BvAppointment table.
        /// Call GetCATIAppointmentTime, check it does not crash.
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(78398)]
        public void ManagementServiceTest_RespondentTimezoneChangedToNull_AppointmentTimezoneIsNotChangedAndGetCATIAppointmentTimeSucceeds()
        {
            CheckAppointmentTimeZoneIsNotChangedAndGetCatiAppointmentTimeSucceeds(null);
        }

        /// <summary>
        /// Create survey, Add interview with call, Set appointment.
        /// Emulate dbo.respondent.tr_respondent_update trigger with zero time zone.
        /// Check that timezoneId is changed in BvInterview table but wasn't changed in BvAppointment table.
        /// Call GetCATIAppointmentTime, check it does not crash.
        /// </summary>
        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(78398)]
        public void ManagementServiceTest_RespondentTimezoneChangedToZero_AppointmentTimezoneIsNotChangedAndGetCATIAppointmentTimeSucceeds()
        {
            CheckAppointmentTimeZoneIsNotChangedAndGetCatiAppointmentTimeSucceeds(0);
        }

        private void CheckAppointmentTimeZoneIsNotChangedAndGetCatiAppointmentTimeSucceeds(int? timezoneId)
        {
            _backendTools.LaunchAllHoursScript();
            _surveySid = _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            const int tzID = 16; //Moscow tz
            TimezoneManager.AddTimezone(tzID);
            var interview = BackendTools.NewInterview(_surveySid);

            interview.TimezoneID = tzID;
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            var setAppointmentTime = DateTime.Parse("10.10.2020 10:10:10.000");
            BackendTools.AddAppointment(interview.ID, _surveySid, setAppointmentTime);

            var appointmentTimezoneId = AppointmentRepository.GetById(_surveySid, interview.ID).TZID;

            BvSpInterview_UpdateRespondentFieldsAdapter.ExecuteNonQuery(
                ProjectId, interview.ID, interview.TelephoneNumber, interview.RespondentName, interview.ExtensionNumber, timezoneId, (byte)DialType.Landline);

            new ManagementService().GetCATIAppointmentTime(ProjectId, interview.ID);

            Assert.AreEqual(
                appointmentTimezoneId,
                AppointmentRepository.GetById(_surveySid, interview.ID).TZID,
                "Appointment timezone is updated while interview timezone is changed to null or zero timezone.");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SaveInterviewHistoryAndControlData_RepeatedLoginWithIncompleteInterview_ProperItsIsSetForProperDeferredRecord()
        {
            var test = new TestCati2(false, _backendTools);

            const string user = "testUser";
            const string password = "password";

            test.CreateSurveyWithPerson(
                DialingMode.Manual,
                user,
                password,
                AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            test.Login(user, password, AgentTaskChoiceMode.Automatic, false);

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.InterviewScreenRecording = true;
            SurveyRepository.Update(survey);

            var interview = test.StartInterview_ManualOrPreview(null, 0);
            var call = CallQueueService.GetCallAndNoLock(interview.SurveySID, interview.ID);
            _personDeferredMonitoringRepository.GetByCallId(call.CallID);

            // repeated login
            SimulateReloginWithGetStateCall(user, password, test);

            test.WS.WrapUp(interview.ID, true, 1, new CompletedInterviewDetails { InterviewDuration = 0, Its = "2" });
            // checking deferred records status
            var deferredRecords = BvPersonDeferredMonitoringPartAdapterEx.GetByCondition(
                    "[InterviewID] = @InterviewID AND [SurveySID] = @SurveySID",
                    new SqlParameter("@InterviewID", interview.ID),
                    new SqlParameter("@SurveySID", interview.SurveySID)).OrderBy(record => record.TimeStamp).ToArray();

            Assert.AreEqual(2, deferredRecords.Length, "There should be 2 deferred records for the interview");
            var incompleteDeferredRecord = deferredRecords[0];
            Assert.IsNull(incompleteDeferredRecord.ExtendedStatus, "Incomplete interview shouldn't have extended status");

            var completedDeferredRecord = deferredRecords[1];
            Assert.AreEqual(TestCati2.ITS.FakeForBusy, completedDeferredRecord.ExtendedStatus, "Incorrect extended status for completed interview.");
        }


        [TestMethod]
        public void ManagementServiceTest_IsIvrCall_ReturnCorrectPersonType()
        {
            _backendTools.CreateSurvey(ProjectId, _framework.GetCatiSqlServerConnectionString(CfSurveyDbName));

            var groupId = PersonTools.CreatePersonGroup("catiGroup");
            var ivrPersonId = PersonTools.CreatePerson("ivrCATI", null, AgentTaskChoiceMode.Automatic, new[] { groupId }, CallCenterTools.DefaultId, AgentType.IvrAgent);
            var livePersonId = PersonTools.CreatePerson("liveAgent", null, AgentTaskChoiceMode.Automatic, new[] { groupId }, CallCenterTools.DefaultId, AgentType.LiveAgent);


            var ivrCheckResult = new ManagementService().IsIvrCall(ivrPersonId);
            var liveAgentCheckResult = new ManagementService().IsIvrCall(livePersonId);

            Assert.AreEqual(true, ivrCheckResult, "Wrong cati IVR agent type result returned.");
            Assert.AreEqual(false, liveAgentCheckResult, "Wrong cati interviewer type result returned.");
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void ManagementServiceTest_UpdateActiveQuestion_Success()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData{
                    Tag ="S1",
                    AssignsS = "P1",
                    Interviews = new []{new InterviewData { Tag="S1.I1", TelephoneNumber = "111", Call = new CallData() }}}},
                Persons = new[] { new PersonData { Tag = "P1" } }
            }.Create();

            var interviewer = context.GetPerson("P1");
            interviewer.Login().Start().Wait();

            var interview = context.GetInterview("S1.I1");

            new ManagementService().UpdateActiveQuestion(interview.Survey.Model.ProjectId, interviewer.Id, "q1").Wait();

            var tasks = BvTasksAdapter.GetAll();

            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("q1", tasks[0].State);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_GetSurveyCallCentersWithWrongProjectIdAndNoSupervisorName_DefaultCallCenterNameIsReturned()
        {
            var result = new ManagementService().GetSurveyCallCenters("p1", null);

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("Default", result[0]);
        }
        
        private void SimulateReloginWithGetStateCall(string user, string password, TestCati2 test)
        {
            PersonInfo personInfo;
            DiallerInfo dialerInfo;
            CatiConsolePropertiesContainer properties;
            var consoleServiceHelper = new CatiWsHelper(user, password);

            var consoleDescriptor = new ConsoleDescription();

            consoleServiceHelper.ConsoleService.Login(test.StationId, consoleDescriptor, out personInfo, out dialerInfo, out properties);
            test.StateWS.GetState();
        }


        [TestMethod, Owner(@"FIRM\OlegZ")]
        public void ManagementServiceTest_IsTimeInShift_Success()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData{
                    Tag ="S1",
                    AssignsS = "P1",
                    SchedulingScript = "SS1",
                    Interviews = new []{new InterviewData { Tag="S1.I1", TelephoneNumber = "111", Call = new CallData() , TimeZoneId = "16"}}}

                },
                Persons = new[] { new PersonData { Tag = "P1" } },
                Scripts = new[]
                {
                    new ScriptData
                    {
                        Tag="SS1", Script = new TestScript(new SubRule(Guid.NewGuid(), new Action[0]))
                        {
                            Shifts = new List<Shift>
                            {
                                new Shift(0, 1, "1.10:00:00", "1.18:59:59"), // Monday
                                new Shift(1, 1, "2.10:00:00", "2.18:59:59"), // Tuesday
                                new Shift(2, 1, "3.10:00:00", "3.18:59:59"), // Wednesday
                                new Shift(3, 1, "4.10:00:00", "4.18:59:59"), // Thursday
                                new Shift(4, 1, "5.10:00:00", "5.18:59:59"), // Friday
                                new Shift(5, 1, "6.10:00:00", "6.18:59:59"), // Saturday
                                new Shift(6, 1, "0.10:00:00", "0.18:59:59") // Sunday
                            }
                        }
                    }
                },

            }.Create();

            TimezoneService.Activate(16);

            var survey = context.GetSurvey("S1");
            var dateTime = DateTime.Parse("2019-12-30 11:30:00");
            var timezoneId = 16; // default

            var utcDateTime = TimezoneService.ConvertTimeToUtc(timezoneId, dateTime);
            var target = new ManagementService().IsTimeInShift(survey.Data.ProjectId, timezoneId, utcDateTime);

            Assert.IsTrue(target);

            dateTime = DateTime.Parse("2019-12-30 06:00:00");
            utcDateTime = TimezoneService.ConvertTimeToUtc(timezoneId, dateTime);
            target = new ManagementService().IsTimeInShift(survey.Data.ProjectId, timezoneId, utcDateTime);

            Assert.IsFalse(target);
        }

        [TestMethod, Owner(@"FIRM\OlegZ")]
        public void ManagementServiceTest_AreTimesInShift_Success()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData{
                    Tag ="S1",
                    AssignsS = "P1",
                    SchedulingScript = "SS1",
                    Interviews = new []{new InterviewData { Tag="S1.I1", TelephoneNumber = "111", Call = new CallData() , TimeZoneId = "16"}}}

                },
                Persons = new[] { new PersonData { Tag = "P1" } },
                Scripts = new[]
                {
                    new ScriptData
                    {
                        Tag="SS1", Script = new TestScript(new SubRule(Guid.NewGuid(), new Action[0]))
                        {
                            Shifts = new List<Shift>
                            {
                                new Shift(0, 1, "1.10:00:00", "1.18:59:59"), // Monday
                                new Shift(1, 1, "2.10:00:00", "2.18:59:59"), // Tuesday
                                new Shift(2, 1, "3.10:00:00", "3.18:59:59"), // Wednesday
                                new Shift(3, 1, "4.10:00:00", "4.18:59:59"), // Thursday
                                new Shift(4, 1, "5.10:00:00", "5.18:59:59"), // Friday
                                new Shift(5, 1, "6.10:00:00", "6.18:59:59"), // Saturday
                                new Shift(6, 1, "0.10:00:00", "0.18:59:59") // Sunday
                            }
                        }
                    }
                },

            }.Create();

            TimezoneService.Activate(16);

            var survey = context.GetSurvey("S1");
            var dateTimes = new[] { DateTime.Parse("2019-12-30 11:30:00"), DateTime.Parse("2019-12-30 06:00:00") };
            var timezoneId = 16; // default

            var utcDateTimes = dateTimes.Select(x => TimezoneService.ConvertTimeToUtc(timezoneId, x)).ToArray();
            var target = new ManagementService().AreTimesInShift(survey.Data.ProjectId, timezoneId, utcDateTimes);

            Assert.IsTrue(target.Any());
            Assert.IsTrue(target[0].IsInShift);
            Assert.IsFalse(target[1].IsInShift);
        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void IsCatiGroupMember_CreateGroupAndTwoPersons_AddOnePersonToGroup_CheckThatMethodReturnsCorrectResultForBothPersons()
        {
            const string groupName = "PersonGroup";

            // arrange
            var context = new TestData
            {
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1", Name = groupName }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1" },
                    new PersonData { Tag = "P2", Memberships = "PG1" }
                }
            }.Create();

            var inter1Id = context.GetPerson("P1").Id;
            var inter2Id = context.GetPerson("P2").Id;

            // act
            var service = new ManagementService();
            var resultInter1 = service.IsCatiGroupMember(inter1Id, groupName);
            var resultInter2 = service.IsCatiGroupMember(inter2Id, groupName);

            var resultWrongInterId = service.IsCatiGroupMember(inter1Id * 100, groupName);
            var resultWrongGroupName = service.IsCatiGroupMember(inter2Id, groupName + "WrongName");

            // assert
            Assert.IsFalse(resultInter1);
            Assert.IsTrue(resultInter2);

            Assert.IsFalse(resultWrongInterId);
            Assert.IsFalse(resultWrongGroupName);

        }

        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void ManagementServiceTest_AddToCATIBlacklist_TelephoneNumberWithNonDigitsSymbols_NormalizedNumberInBlacklist()
        {
            const string telephoneNumber = "(123) 456-78<string to ignore>90";
            const string normalizedTelephoneNumber = "1234567890";

            var managementService = new ManagementService();
            managementService.AddToCATIBlacklist(telephoneNumber, null, 0);

            var numbers = ServiceLocator.Resolve<ITelephoneBlacklistRepository>().GetAll();

            Assert.AreEqual(1, numbers.Count);
            Assert.AreEqual(normalizedTelephoneNumber, numbers[0].TelephoneNumber);
        }
        
        [TestMethod, Owner(@"FIRM\Grigoryk")]
        public void ManagementServiceTest_AddToCATIBlacklist_CommentAndCreationDateAreCorrect()
        {
            const string telephoneNumber = "12345";
            var context = new TestData
            {
                Surveys = new[]{new SurveyData{
                    Tag ="S1",
                    Interviews = new []{new InterviewData { Tag="S1.I1", TelephoneNumber = "111", Call = new CallData() , TimeZoneId = "16"}}}

                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            var surveyFromDb = _surveyRepository.GetByProjectId(survey.Data.ProjectId);
            surveyFromDb.Description = "Very long description of test survey to test that all works fine";
            _surveyRepository.Update(surveyFromDb);
            
            var managementService = new ManagementService();
            managementService.AddToCATIBlacklist(telephoneNumber, surveyFromDb.ProjectId, interview.Id);

            var numbers = ServiceLocator.Resolve<ITelephoneBlacklistRepository>().GetAll();

            Assert.AreEqual(1, numbers.Count);
            Assert.AreEqual($"Added from {surveyFromDb.ProjectId} Very long description of test survey to test th [{interview.Id}]", numbers[0].Comment);
            Assert.IsNotNull(numbers[0].Timestamp);
            var currentTime = DateTime.UtcNow;
            Assert.IsTrue(currentTime.Subtract(numbers[0].Timestamp) < TimeSpan.FromSeconds(1));
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void ManagementServiceTest_SoftDeleteAndRestoreSoftDeletedSurvey_Success()
        {
            var context = new TestData
            {
                Surveys = new[] { new SurveyData { Tag ="S1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var savedIsCacheEnabled = BackendInstance.Current.IsCacheEnabled;

            try
            {
                BackendInstance.Current.IsCacheEnabled = false;
                
                var state = SurveyRepository.GetById(survey.Id);
                Assert.AreEqual((int)SurveyState.Open, state.State);

                new ManagementService().SoftDeleteSurvey(survey.Data.ProjectId);

                state = SurveyRepository.GetById(survey.Id);
                Assert.AreEqual((int)SurveyState.SoftDeleted, state.State);

                new ManagementService().RestoreSoftDeletedSurvey(survey.Data.ProjectId);

                state = SurveyRepository.GetById(survey.Id);
                Assert.AreEqual((int)SurveyState.Close, state.State);
            }
            finally
            {
                BackendInstance.Current.IsCacheEnabled = savedIsCacheEnabled;
            }
        }
    }
}
