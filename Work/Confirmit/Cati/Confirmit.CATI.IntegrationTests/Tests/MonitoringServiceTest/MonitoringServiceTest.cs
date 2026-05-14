using System;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Monitoring;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.IntegrationTests.Tests.MonitoringTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.MonitoringServiceTest
{
    [TestClass]
    public class MonitoringServiceTest : BaseMonitoringTest
    {
        private IMonitoringService _monitoringService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _monitoringService = ServiceLocator.Resolve<IMonitoringService>();
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
        /// 8.  Start task using TaskService.CreateDirectByPersonSid method
        /// 9.  Start monitoring using StartMonitoring method
        /// 10. Check that monitoring was started using GetActiveMonitoring method
        /// 11. Stop monitoring using StopMonitoring method
        /// 12. Check that monitoring was stoped using GetActiveMonitoring method
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyL")]
        public void MonitoringServiceTest_StartAndStopMonitoring_CheckWithGetActiveMonitoringMethod()
        {
            CreateSurveyPersonInterviewCall();

            // Start monitoring
            long monitoringSessionId = _monitoringService.StartMonitoring(PersonId, "grigoryk", "surveyId");

            Assert.AreNotEqual(0, monitoringSessionId, "monitoringSessionId is zero");

            // Check that monitoring was started
            FusionMonitoringDescription activeMonitoring = _monitoringService.GetActiveMonitoring(PersonId);
            Assert.IsNotNull(activeMonitoring, "GetActiveMonitoring don't return active monitoring");
            Assert.AreEqual(monitoringSessionId, activeMonitoring.MonitoringSessionId, "GetActiveMonitoring return monitoring with wrong MonitoringSessionID");
            Assert.IsFalse(activeMonitoring.IsWebMonitoring);
            Assert.IsFalse(activeMonitoring.IsLiveMonitoringEnabled);

            // Stop monitoring
            _monitoringService.StopMonitoring(PersonId, monitoringSessionId, "grigoryk");

            // Check that monitoring was stoped
            activeMonitoring = _monitoringService.GetActiveMonitoring(PersonId);
            Assert.IsNull(activeMonitoring, "GetActiveMonitoring return wrong active monitoring");
        }

        [TestMethod, Owner(@"FIRM\ElenaKs")]
        public void MonitoringServiceTest_StartAndStopMonitoringForWebConsole_IsWebConsoleFlagIsSet()
        {
            CreateSurveyPersonInterviewCall();

            // Start monitoring
            long monitoringSessionId = _monitoringService.StartMonitoring(PersonId, "grigoryk", "surveyId", null, true);

            // Check web monitoring
            var activeMonitoring = _monitoringService.GetActiveMonitoring(PersonId);
            Assert.IsTrue(activeMonitoring.IsWebMonitoring);

            // Stop monitoring
            _monitoringService.StopMonitoring(PersonId, monitoringSessionId, "grigoryk");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void MonitoringIsNotStarted_IsMonitored_ReturnsFalse()
        {
            CreateSurveyPersonInterviewCall();

            var isMonitored = _monitoringService.IsMonitored(PersonId);

            Assert.IsFalse(isMonitored, "Monitoring should not be started.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void MonitoringIsStarted_IsMonitored_ReturnsTrue()
        {
            const string supervisorName = "Test Supervisor";

            CreateSurveyPersonInterviewCall();

            _monitoringService.StartMonitoring(PersonId, supervisorName, "surveyId");

            var isMonitored = _monitoringService.IsMonitored(PersonId);

            Assert.IsTrue(isMonitored, "Monitoring should be started.");
        }


        [TestMethod, Owner(@"FIRM\alm")]
        public void MonitoringIsNotStarted_IsActiveMonitoringSession_ReturnsFalse()
        {
            CreateSurveyPersonInterviewCall();

            var isActiveMonitoringSession = _monitoringService.IsActiveMonitoringSession(
                PersonId,
                monitoringSessionId: DateTime.UtcNow.Ticks); // Dummy Id

            Assert.IsFalse(isActiveMonitoringSession, "Monitoring should not be active.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void MonitoringIsStarted_IsActiveMonitoringSession_ReturnsTrue()
        {
            const string supervisorName = "Test Supervisor";

            CreateSurveyPersonInterviewCall();

            var monitoringSessionId = _monitoringService.StartMonitoring(PersonId, supervisorName, "surveyId");

            var isActiveMonitoringSession = _monitoringService.IsActiveMonitoringSession(PersonId, monitoringSessionId);

            Assert.IsTrue(isActiveMonitoringSession, "Monitoring should be active.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void MonitoringIsStarted_IsActiveMonitoringSessionWithWrongSessionId_ReturnsFalse()
        {
            const string supervisorName = "Test Supervisor";

            CreateSurveyPersonInterviewCall();

            var monitoringSessionId = _monitoringService.StartMonitoring(PersonId, supervisorName, "surveyId");

            var isActiveMonitoringSession = _monitoringService.IsActiveMonitoringSession(PersonId, monitoringSessionId + 1);

            Assert.IsFalse(isActiveMonitoringSession, "Monitoring should not be active.");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        [ExpectedException(typeof(UserMessageException))]
        public void MonitoringIsAlreadyStarted_StartMonitoringWithTheSameSupervisor_UserMessageExceptionIsThrown()
        {
            const string supervisorName = "Test Supervisor";

            CreateSurveyPersonInterviewCall();

            _monitoringService.StartMonitoring(PersonId, supervisorName, "surveyId");

            // Start again with the same supervisor name
            _monitoringService.StartMonitoring(PersonId, supervisorName, "surveyId");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        [ExpectedException(typeof(UserMessageException))]
        public void MonitoringIsAlreadyStarted_StartMonitoringWithAnotherSupervisor_UserMessageExceptionIsThrown()
        {
            const string supervisorName = "Test Supervisor";

            CreateSurveyPersonInterviewCall();

            _monitoringService.StartMonitoring(PersonId, supervisorName, "surveyId");

            // Start again with another supervisor name
            _monitoringService.StartMonitoring(PersonId, supervisorName + " Another Name", "surveyId");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        [ExpectedException(typeof(ArgumentException))]
        public void StopMonitoringWithWrongSessionId_ArgumentExceptionIsThrown()
        {
            const string supervisorName = "Test Supervisor";

            CreateSurveyPersonInterviewCall();

            var monitoringSessionId = _monitoringService.StartMonitoring(PersonId, supervisorName, "surveyId");

            _monitoringService.StopMonitoring(PersonId, monitoringSessionId + 1, supervisorName);
        }

        [TestMethod, Owner(@"FIRM\alm")]
        [ExpectedException(typeof(UserMessageException))]
        public void StopMonitoringWithWrongSupervisorName_UserMessageExceptionIsThrown()
        {
            const string supervisorName = "Test Supervisor";

            CreateSurveyPersonInterviewCall();

            var monitoringSessionId = _monitoringService.StartMonitoring(PersonId, supervisorName, "surveyId");

            _monitoringService.StopMonitoring(PersonId, monitoringSessionId, supervisorName + " Another Name");
        }


        [TestMethod, Owner(@"FIRM\alm")]
        public void NoActiveMonitoring_StopMonitoring_NoExceptionIsThrown()
        {
            const string supervisorName = "Test Supervisor";

            CreateSurveyPersonInterviewCall();

            _monitoringService.StopMonitoring(PersonId, monitoringSessionId: 0, supervisorName: supervisorName);
        }
    }
}
