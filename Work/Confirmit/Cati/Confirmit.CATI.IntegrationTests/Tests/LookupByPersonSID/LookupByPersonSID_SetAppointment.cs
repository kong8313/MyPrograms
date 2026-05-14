using System;
using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.LookupByPersonSID
{
    [TestClass]
    public class LookupByPersonSIDSetAppointment : BaseMockedIntegrationTest
    {
        private int _surveySID;
        private int _personID; // persons ID
        private readonly BvInterviewEntity[] _interviews = new BvInterviewEntity[5]; // interviews
        private readonly BvCallEntity[] _calls = new BvCallEntity[5]; // test calls

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        #region Support methods

        /// <summary>
        /// Add survey, open survey
        /// Launch ALL HOURS script and assignes it to survey
        /// Add user i1
        /// Add 5 sample records
        /// </summary>
        private void PrepareDataForTest()
        {
            BackendToolsObject.LaunchAllHoursScript();
            _surveySID = BackendToolsObject.CreateSurvey("p000001");

            _surveyStateService.Open(_surveySID);

            _personID = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic);

            for (int i = 0; i < 5; i++)
            {
                _interviews[i] = BackendTools.NewInterview(_surveySID);
                BackendTools.CreateInterview(_interviews[i]);

                _calls[i] = BackendTools.NewCall(_interviews[i]);

                BackendTools.CreateCall(_calls[i]);
                _calls[i].CallID = CallQueueService.GetCallAndNoLock(_surveySID, _interviews[i].ID).CallID;
            }
        }

        /// <summary>
        /// Adds appointment for interview specified by order and checks appointment state.
        /// </summary>
        /// <param name="order">Interview order in in the interviews array</param>
        /// <param name="appTime">Appointment time to set</param>
        private void AddAppointmentAndCheckState(int order, DateTime appTime)
        {
            BackendTools.AddAppointment(_interviews[order].ID, _surveySID, appTime);
            CallTools.CheckAppointmentState(_surveySID, _interviews[order].ID, 0);
        }

        /// <summary>
        /// Sets appointment state for the interview specified by order.
        /// Runs scheduling rules.
        /// Checks appointment state.
        /// Checks that call time equals time specified by callTime.
        /// </summary>
        /// <param name="order">Interview order in in the interviews array</param>
        /// <param name="callTime">Call time to be checked</param>
        private void RunSchedulingRulesAndCheckCallTime(DateTime callTime, int order)
        {
            // set ITS =1 (Appointment) and run scheduling rules
            var options = new SchedulingScriptExecutionOptions
            {
                ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled,
                ITS = 1,
                LastCallPersonSID = _personID,
                IsLogToHistory = false
            };

            InterviewService.Schedule(_surveySID, _interviews[order].ID, options);

            CallTools.CheckAppointmentState(_surveySID, _interviews[order].ID, 1);
            CallTools.CheckCallTimeInBvSvySchedule(_calls[order].CallID, callTime);
        }

        /// <summary>
        /// Checks that call is given and checks appointment state after it.
        /// </summary>
        /// <param name="order">Interview order in in the interviews array</param>
        private void AssertCallWasGivenAndCheckAppointmentState(int order)
        {
            CallTools.AssertCallWasGiven(_personID, _calls[order].CallID, 0);
            CallTools.CheckAppointmentState(_surveySID, _interviews[order].ID, 2);
        }

        /// <summary>
        /// Checks that call is given.
        /// </summary>
        /// <param name="order">Interview order in in the interviews array</param>
        private void AssertCallWasGiven(int order)
        {
            CallTools.AssertCallWasGiven(_personID, _calls[order].CallID, _surveySID);
        }

        #endregion

        /// <summary>
        /// Add survey, open survey
        /// Launch ALL HOURS script and assignes it to survey
        /// Add user i1
        /// Add 5 sample records
        /// Check appointment state = 0
        /// Set appointment time to now + 1min for all calls
        /// Check appointment state = 1
        /// Wait for 2 minutes
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// i1      |   1
        /// i1      |   2
        /// i1      |   3
        /// i1      |   4
        /// i1      |   5
        /// Check appointment state = 2
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SetAppointment_GetCalls_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            DateTime now = DateTime.UtcNow.TrimMiliseconds().ChangeKind(DateTimeKind.Unspecified);

            new DateTimeMocker(TestingFramework).MockDate(now);

            DateTime appTime = now.AddMinutes(1);

            for (int i = 0; i < 5; i++)
            {
                AddAppointmentAndCheckState(i, appTime);
            }

            for (int i = 0; i < 5; i++)
            {
                RunSchedulingRulesAndCheckCallTime(appTime, i);
            }

            BackendTools.AssignCatiPersonToSurvey(_surveySID, _personID);
            BackendTools.LoginPerson(_personID, "");

            new DateTimeMocker(TestingFramework).MockDate(now.AddMinutes(2));

            for (int i = 0; i < 5; i++)
            {
                AssertCallWasGivenAndCheckAppointmentState(i);
            }
        }

        /// <summary>
        /// Add survey, open survey
        /// Launch ALL HOURS script and assignes it to survey
        /// Add user i1
        /// Add 5 sample records
        /// Check appointment state = 0
        /// Set appointment time to now + 2min for all calls
        /// Check appointment state = 1
        /// Wait for 1 minute
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// i1      |   1
        /// i1      |   2
        /// i1      |   3
        /// i1      |   4
        /// i1      |   5
        /// Check appointment state = 2
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SetAppointmentToSomeCalls_GetCalls_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            DateTime now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).ToUniversalTime();
            DateTime appTime = now.AddMinutes(1);

            for (int i = 2; i < 5; i++)
            {
                AddAppointmentAndCheckState(i, appTime);
            }

            for (int i = 2; i < 5; i++)
            {
                RunSchedulingRulesAndCheckCallTime(appTime, i);
            }

            BackendTools.AssignCatiPersonToSurvey(_surveySID, _personID);

            BackendTools.LoginPerson(_personID, "");

            AssertCallWasGivenAndCheckAppointmentState(2);
            AssertCallWasGivenAndCheckAppointmentState(3);
            AssertCallWasGivenAndCheckAppointmentState(4);
            AssertCallWasGiven(0);
            AssertCallWasGiven(1);
        }

        /// <summary>
        /// Add survey, open survey
        /// Launch ALL HOURS script and assignes it to survey
        /// Add user i1
        /// Add 5 sample records
        /// Check appointment state = 0
        /// Set appointment time to next day for all calls 1-4
        /// Check appointment state = 1
        /// Execute LookupByPerson for user and get call 5
        /// Check appointment state = 2
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void SetAppointmentToNextDay_GetCall_NotAppointmentCallGiven()
        {
            PrepareDataForTest();

            DateTime now = DateTime.Now;
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).ToUniversalTime();
            DateTime appTime = now.AddDays(1);

            for (int i = 0; i < 4; i++)
            {
                AddAppointmentAndCheckState(i, appTime);
            }

            for (int i = 0; i < 4; i++)
            {
                RunSchedulingRulesAndCheckCallTime(appTime, i);
            }

            BackendTools.AssignCatiPersonToSurvey(_surveySID, _personID);

            BackendTools.LoginPerson(_personID, "");

            CallTools.AssertCallWasGiven(_personID, _calls[4].CallID, 0);
        }
    }
}
