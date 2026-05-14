using System;
using System.Collections.Generic;
using System.Data;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Random;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.Activity;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class ActivityManagerTest : BaseMockedIntegrationTest
    {
        private int _surveySid;
        private int _personId;

        private readonly BvInterviewEntity[] _interviews = new BvInterviewEntity[5];
        private readonly BvCallEntity[] _calls = new BvCallEntity[5];
        private readonly DateTime[] _appointmentTimes = new DateTime[5];

        private ISurveyStateService _surveyStateService;
        private IActivityManager _activityManager;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _activityManager = ServiceLocator.Resolve<IActivityManager>();
        }
        /// <summary>
        /// Max value, that could be set in alert.
        /// Currently MinutesSpentWorkingOnSurvey alert values are multiplied by 60 so larger values will cause with overflow.
        /// </summary>
        private const int MaxAlertValue = Int32.MaxValue / 60 - 1;

        private static SurveyAlertInfo GetAlertByTypeId(int thresholdTypeId)
        {
            return ActivityManager.GetSurveyAlertsList().Find(x => x.ThresholdsTypeId == thresholdTypeId);
        }

        /// <summary>
        /// Add survey, open survey
        /// Launch ALL HOURS script and assignes it to survey
        /// Add user i1, assign it to survey 1.
        /// </summary>
        private void PrepareDataForTest()
        {
            BackendToolsObject.LaunchAllHoursScript();
            _surveySid = BackendToolsObject.CreateSurvey("p000001");
            _surveyStateService.Open(_surveySid);

            _personId = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Manual);

            BackendTools.AssignCatiPersonToSurvey(_surveySid, _personId);
            BackendTools.LoginPerson(_personId, "");
        }

        /// <summary>
        ///  Creates the sample and appointment
        /// </summary>
        /// <param name="i">Interview / Call / Appointment number to work on.</param>
        private void CreateSampleAndAppointment(int i)
        {
            CreateSample(i);
            CreateAppointment(i);
        }


        /// <summary>
        /// Creates the sample
        /// </summary>
        /// <param name="i">Interview / Call / Appointment number to work on.</param>        
        private void CreateSample(int i)
        {
            _interviews[i] = BackendTools.NewInterview(_surveySid);
            BackendTools.CreateInterview(_interviews[i]);

            _calls[i] = BackendTools.NewCall(_interviews[i]);
            BackendTools.CreateCall(_calls[i]);
            _calls[i].CallID = CallQueueService.GetCallAndNoLock(_surveySid, _interviews[i].ID).CallID;
        }

        /// <summary>
        /// Adds appointment.
        /// Sets appointment state for the interview specified by order.
        /// Runs scheduling rules.
        /// Checks that call and appointment exist in DB.
        /// </summary>
        /// <param name="i">Interview / Call / Appointment number to work on.</param>
        private void CreateAppointment(int i)
        {
            BackendTools.AddAppointment(_interviews[i].ID, _surveySid, _appointmentTimes[i]);
            BackendTools.SetInterviewItsAppointment(_surveySid, _interviews[i].ID);
            CallTools.CheckAppointmentState(_surveySid, _interviews[i].ID, 1);
            CallTools.CheckCallExistsInBvSvySchedule(_calls[i].CallID);
        }

        /// <summary>
        /// Runs BvSpAlert_RecalculateAll.
        /// In general this stored procedure can be called once in 15 seconds.
        /// This method allows to run it more often.
        /// </summary>
        private void RecalculateAll()
        {
            BvSpAlert_RecalculateAllAdapter.ExecuteNonQuery(DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the next appointment time for current survey.
        /// </summary>
        private DateTime GetNextAppointmentTime()
        {
            RecalculateAll();
            List<SurveyActivityInfo> activityData = _activityManager.GetSurveyActivityData(String.Empty, true, false, new[] { _surveySid }, false);
            Assert.AreEqual(1, activityData.Count);
            Assert.IsTrue(activityData[0].NextAppointment.HasValue);
            return activityData[0].NextAppointment.Value;
        }

        /// <summary>
        /// The following actions performed for all survey alert types, alert values are generated randomly.
        /// 
        /// 1. Create alert.
        /// 2. Verify that alert is set with valid values.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void SetAlert_CheckAlertIsSetWithRightValues()
        {
            foreach (BvThresholdType thresholdType in ActivityManager.SurveyListThresholdTypes)
            {
                int amber = Randomizer.Next(MaxAlertValue);
                int red = Randomizer.Next(MaxAlertValue);
                var thresholdTypeId = (int)thresholdType;

                ActivityManager.SetAlert(new SurveyAlertInfo(0, amber, red, thresholdTypeId));

                SurveyAlertInfo alert = GetAlertByTypeId(thresholdTypeId);

                Assert.IsNotNull(alert, "Alert {0} was not set", thresholdType.ToString());
                Assert.AreEqual(amber, alert.Amber, "Amber value for alert {0} is wrong", thresholdType.ToString());
                Assert.AreEqual(red, alert.Red, "Red value for alert {0} is wrong", thresholdType.ToString());
            }
        }

        /// <summary>
        /// The following actions performed for all survey alert types, alert values are generated randomly.
        /// 
        /// 1. Create alert.
        /// 2. Update alert with new values.
        /// 3. Verify that alert is updated with new values.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void UpdateAlert_CheckAlertIsUpdatedWithRightValues()
        {
            var r = new Random();

            foreach (BvThresholdType thresholdType in ActivityManager.SurveyListThresholdTypes)
            {
                int amber = r.Next(MaxAlertValue);
                int red = r.Next(MaxAlertValue);
                var thresholdTypeId = (int)thresholdType;

                ActivityManager.SetAlert(new SurveyAlertInfo(0, 1, 1, thresholdTypeId));

                ActivityManager.SetAlert(new SurveyAlertInfo(0, amber, red, thresholdTypeId));

                SurveyAlertInfo alert = GetAlertByTypeId(thresholdTypeId);

                Assert.IsNotNull(alert, "Alert {0} was not set", thresholdType.ToString());
                Assert.AreEqual(amber, alert.Amber, "Amber value for alert {0} is wrong", thresholdType.ToString());
                Assert.AreEqual(red, alert.Red, "Red value for alert {0} is wrong", thresholdType.ToString());
            }
        }

        /// <summary>
        /// The following actions performed for all survey alert types.
        /// 
        /// 1. Create alert.
        /// 2. Delete created alert.
        /// 3. Verify that alert not exists.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void DeleteAlert_CheckAlertIsDeleted()
        {
            foreach (BvThresholdType thresholdType in ActivityManager.SurveyListThresholdTypes)
            {
                var thresholdTypeId = (int)thresholdType;

                ActivityManager.SetAlert(new SurveyAlertInfo(0, 10, 20, thresholdTypeId));

                ActivityManager.DeleteAlert(0, thresholdTypeId);

                SurveyAlertInfo alert = GetAlertByTypeId(thresholdTypeId);

                Assert.IsNull(alert, "Alert {0} was not deleted", thresholdType.ToString());
            }
        }

        /// <summary>
        /// 1.	Add a survey 1
        /// 2.	Assign standard All hours to the survey 1 
        /// 3.	Add user 1
        /// 4.	Assign user 1 to survey 1
        /// 5.	Add a sample record for survey 1 (call 1)
        /// 6.	For interview record 1 set Appointment, appointment time = current time + 2hours
        /// 7.	Check that there is an appropriate record exist in bvappointment  and there is a call in bvsvyschedule
        /// 8.	Check that Next appointment value for the survey 1(the one from the CP -> Activity Views- Survey List) is the appointment time for call 1
        /// 9.	Add 5 sample records for survey 1 (call 2, call 3, call 4, call 5)
        /// 10.	For interview record 2 set Appointment, appointment time = current time + 10min
        /// 11.	For interview record 3 set Appointment, appointment time = current time + 20min
        /// 12.	For interview record 4 set Appointment, appointment time = current time + 30min
        /// 13.	For interview record 5 set Appointment, appointment time = current time + 40min
        /// 14.	Check that there are appropriate records exist in bvappointment  and there are calls in bvsvyschedule for interviews 1-5
        /// 15.	Check that Next appointment value for the survey 1(the one from the CP -> Activity Views- Survey List) is the appointment time for call 2
        /// 16.	Delete call 2
        /// 17.	Check that Next appointment value for the survey 1(the one from the CP -> Activity Views- Survey List) is the appointment time for call 3
        /// 18.	Make an interview for call 3 then set Appointment, appointment time = current time + 8hours
        /// 19.	Check that Next appointment value for the survey 1(the one from the CP -> Activity Views- Survey List) is the appointment time for call 4
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void CreateAppointments_GetNextSurveyAppointment_ValidAppointmentTimeGiven()
        {
            PrepareDataForTest();

            var now = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                                   DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);

            _appointmentTimes[0] = now.AddHours(2);
            _appointmentTimes[1] = now.AddMinutes(10);
            _appointmentTimes[2] = now.AddMinutes(20);
            _appointmentTimes[3] = now.AddMinutes(30);
            _appointmentTimes[4] = now.AddMinutes(40);

            CreateSampleAndAppointment(0);
            Assert.AreEqual(_appointmentTimes[0], GetNextAppointmentTime());

            for (int i = 1; i < 5; i++)
            {
                CreateSampleAndAppointment(i);
            }
            Assert.AreEqual(_appointmentTimes[1], GetNextAppointmentTime());

            CallTools.DeleteCalls(_surveySid, new[] { _interviews[1].ID });
            Assert.AreEqual(_appointmentTimes[2], GetNextAppointmentTime());

            BackendTools.AddAppointment(_interviews[2].ID, _surveySid, now.AddHours(8));
            BackendTools.SetInterviewItsAppointment(_surveySid, _interviews[2].ID);
            Assert.AreEqual(_appointmentTimes[3], GetNextAppointmentTime());
        }

        /// <summary>
        /// 1.	Add a survey 1
        /// 2.	Assign standard All hours to the survey 1 
        /// 3.	Add user 1
        /// 4.	Assign user 1 to survey 1
        /// 5.	Add a sample record for survey 1 (call 1)
        /// 6.	add appointment
        /// 7.	Get the same interview 
        /// 8.	reschedule interview so call will be created on appointment
        /// 9.  Check ScheduledCallsCount
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyActivity_SetAppointmentTwiceForCall_ScheduledCallsCountIsValid()
        {
            PrepareDataForTest();

            var now = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                                        DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);

            _appointmentTimes[0] = now;

            CreateSampleAndAppointment(0);

            TaskService.LookupByPersonSid(_personId, _surveySid, _interviews[0].ID);

            BackendTools.AddAppointment(_interviews[0].ID, _surveySid, now.AddHours(2));
            BackendTools.SetInterviewItsAppointment(_surveySid, _interviews[0].ID);

            BackendTools.ForceProcessingAsyncTriggers();

            var actual = TestingFramework.DbEngine.ExecuteScalar<int>(
            "select ScheduledCallsCount from BvAggregateSurvey where SID = " + _surveySid,
            CommandType.Text);

            Assert.AreEqual(1, actual);
        }


        /// <summary>
        /// 1.  Create survey using SurveyRepository.Insert method
        /// 2.  Launch ALL HOURS script using LaunchAllHoursScript method
        /// 3.  Open survey using SurveyService.Open method
        /// 4.  Add interviewers using PersonTools.CreatePerson method
        /// 5.  Assign interviewers to survey using AssignmentService.AssignResourceToSurvey method
        /// 6.  Create interview using BackendTools.NewInterview 
        ///     and BackendTools.CreateInterview methods
        /// 7.  Create call using BackendTools.NewCall and BackendTools.CreateCall methods
        /// 8.  Sets appointment state for the interview specified by order
        ///     using BackendTools.AddAppointment method
        /// 9.  Runs scheduling rules using CallTools.SetInterviewItsAppointment method
        /// 10. Checks that call and appointment exist in DB using CallTools.CheckAppointmentState
        ///     and CallTools.CheckCallExistsInBvSvySchedule methods
        /// 11. Start task using TaskService.CreateDirectByPersonSid method
        /// 12. Terminate task using TerminateTaskByPerson method
        /// 13. Get history of interview changes using GetInterviewHistoryList method
        /// 14. Check that return two changes with correct ITS id
        /// 15. Get extended call history of interview changes using GetExtendedCallHistoryList method
        /// 16. Check that return two changes with correct transient state
        /// </summary>
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CallQueueService_GetInterviewHistoryList_Success()
        {
            // Create survey, launch and open it. Create interviewers and assign it to survey
            PrepareDataForTest();

            // Create the sample            
            _appointmentTimes[0] = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                                        DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second);
            CreateSampleAndAppointment(0);

            // Start task
            var task = TaskService.LookupByPersonSid(_personId, _surveySid, _interviews[0].ID);
            TaskRepository.Update(task);

            // Terminate task
            TaskService.TerminateTask(
                _personId,
                new DatabaseTransactionOptions("TerminateTask", DeadlockPriority.Normal));

            // Copy some rows from BvCallHistoryEx to BvCallHistory to check that BvSpGetExtendedCallHistory gets data from both tables
            BackendTools.CopyCallHistoryExToCallHistory(2);
            
            // Get history about interview
            List<BvSpCallHistory_ListEntity> historyList = CallQueueService.GetInterviewHistoryList(_surveySid, _interviews[0].ID, CallCenterTools.DefaultId);

            // Check that GetInterviewHistoryList return correct data 
            Assert.AreEqual(2, historyList.Count, "GetInterviewHistoryList return wrong history records count");
            Assert.AreEqual((int)CallOutcome.Appointment, historyList[0].ITS_ID, "GetInterviewHistoryList return first row with incorrect ITS id");
            Assert.AreEqual((int)CallOutcome.InterruptedBySystem, historyList[1].ITS_ID, "GetInterviewHistoryList return second row with incorrect ITS id");
            
            // Get extended call history about interview
            List<BvSpGetExtendedCallHistoryEntity> extendedCallHistoryList = CallQueueService.GetExtendedCallHistoryList(_surveySid, _interviews[0].ID, CallCenterTools.DefaultId);

            // Check that GetExtendedCallHistoryList return correct data 
            Assert.AreEqual(2, extendedCallHistoryList.Count, "GetExtendedCallHistoryList return wrong history records count");
            Assert.AreEqual("Appointment", extendedCallHistoryList[0].TransientState, "GetExtendedCallHistoryList return first row with incorrect transient state");
            Assert.AreEqual("Interrupted by system", extendedCallHistoryList[1].TransientState, "GetExtendedCallHistoryList return second row with incorrect transient state");
        }
    }
}
