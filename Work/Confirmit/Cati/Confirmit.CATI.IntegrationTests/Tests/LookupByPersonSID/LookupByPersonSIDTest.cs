using System;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Confirmit.CATI.Supervisor.Core.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using System.Linq;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.LookupByPersonSID
{
    [TestClass]
    public class LookupByPersonSidTest : BaseMockedIntegrationTest
    {
        private const int CallsCount = 10; // calls count in tests
        private int _surveySid;
        private const string ProjectId = "p0000111";
        private readonly int[] _personIDs = new int[3]; // persons IDs
        private readonly int[] _groupIDs = new int[2]; // groups IDs
        private readonly BvInterviewEntity[] _interviews = new BvInterviewEntity[CallsCount]; // interviews
        private readonly BvCallEntity[] _calls = new BvCallEntity[CallsCount]; // test calls

        private ISurveyStateService _surveyStateService;
        private IInterviewRepository _interviewRepository;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
        }

        /// <summary>
        /// Creates survey.
        /// Opens survey.
        /// Adds scheduling script.
        /// Creates 10 interviews.
        /// Creates 10 calls.
        /// Creates persons.
        /// </summary>
        private void PrepareDataForTest()
        {
            BackendToolsObject.LaunchAllHoursScript();
            _surveySid = BackendToolsObject.CreateSurvey(ProjectId);

            _surveyStateService.Open(_surveySid);

            //create 10 calls:
            for (int i = 0; i < CallsCount; i++)
            {
                _interviews[i] = new BvInterviewEntity { ID = (i + 1), SurveySID = _surveySid, TransientState = 16 };
                _interviewRepository.InsertOnly(_interviews[i]);

                _calls[i] = BackendTools.NewCall(_interviews[i]);
                _calls[i].TimeInShift = DateTime.Now.ToUniversalTime();
                if (i < 3 || i > 5)
                    _calls[i].Priority = 5000;
                BackendTools.CreateCall(_calls[i]);
                _calls[i].CallID = CallQueueService.GetCallAndNoLock(_surveySid, _interviews[i].ID).CallID;
            }

            // create groups and persons
            _groupIDs[0] = PersonTools.CreatePersonGroup("g1");
            _personIDs[0] = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic, new[] { _groupIDs[0] });
            _groupIDs[1] = PersonTools.CreatePersonGroup("g2");
            _personIDs[1] = PersonTools.CreatePerson("i2", "password", AgentTaskChoiceMode.Automatic, new[] { _groupIDs[1] });
            _personIDs[2] = PersonTools.CreatePerson("i3", "password", AgentTaskChoiceMode.Automatic, new[] { PersonManager.GetCatiRootID() });
        }

        /// <summary>
        /// Check that 1-10 calls phase = 2.
        /// Set ITS = Completed to 1-5 calls.
        /// Set ITS = Busy to 5-10 calls.
        /// Run scheduling procedure.
        /// Check that 1-5 calls do not exist.
        /// Check that 5-10 calls exist and phase = 2.
        /// </summary>
        private void MoveAndRescheduleAndCheckCallsExist()
        {
            var historyData = _interviews.Take(5).Select(x => x.ID)
                .Select(x => new InterviewHistoryData
                {
                    projectID = ProjectId,
                    respondentPhone = "1",
                    time = DateTime.UtcNow,
                    interviewID = x,
                    status = "13",
                    appointmentID = 0,
                    grossDuration = 0,
                    netDuration = 0,
                    totalDuration = 0,
                    interviewerID = _personIDs[0],
                    roleID = 2 /* CATI */
                });

            var controlData = _interviews.Take(5).Select(x => x.ID)
                .Select(x => new InterviewControlData
                {
                    projectID = ProjectId,
                    interviewID = x,
                    status = "13",
                    respondentName = "q",
                    respondentPhone = "1",
                    interviewerID = _personIDs[0],
                    lastCallTime = DateTime.UtcNow,
                    roleID = 2, /* CATI */
                    totalDuration = 0
                });

            foreach (var data in controlData.Join(historyData, x => x.interviewID, y => y.interviewID, (x, y) => new { x, y }))
            {
                BackendToolsObject.SaveInterviewHistoryAndControlDataWithScheduling(data.y, data.x, SurveyRepository.GetById(_surveySid), new BvInterviewTimings());
            }

            historyData = _interviews.Skip(5).Take(5).Select(x => x.ID)
                .Select(x => new InterviewHistoryData
                {
                    projectID = ProjectId,
                    respondentPhone = "1",
                    time = DateTime.UtcNow,
                    interviewID = x,
                    status = "2",
                    appointmentID = 0,
                    grossDuration = 0,
                    netDuration = 0,
                    totalDuration = 0,
                    interviewerID = _personIDs[0],
                    roleID = 2 /* CATI */
                });

            controlData = _interviews.Skip(5).Take(5).Select(x => x.ID)
                .Select(x => new InterviewControlData
                {
                    projectID = ProjectId,
                    interviewID = x,
                    status = "2",
                    respondentName = "q",
                    respondentPhone = "1",
                    interviewerID = _personIDs[0],
                    lastCallTime = DateTime.UtcNow,
                    roleID = 2, /* CATI */
                    totalDuration = 0
                });

            foreach (var data in controlData.Join(historyData, x => x.interviewID, y => y.interviewID, (x, y) => new { x, y }))
            {
                BackendToolsObject.SaveInterviewHistoryAndControlDataWithScheduling(data.y, data.x, SurveyRepository.GetById(_surveySid), new BvInterviewTimings());
            }

            BackendTools.RunSchedulingProcedure();

            for (int i = 0; i < 5; i++)
            {
                Assert.IsFalse(BackendTools.IsCallExists(_surveySid, _interviews[i].ID));
                CallTools.CheckCallNotExistInbvSvySchedule(_calls[i].CallID);
            }
            for (int i = 5; i < 10; i++)
            {
                CallTools.CheckCallPhaseInBvSvySchedule(_calls[i].CallID, 2);
            }
        }

        /// <summary>
        /// Add survey, open survey.
        /// Add 3 users – i1,i2,i3
        /// Create two groups g1, g2
        /// Make i1 member of g1, i2 of g2
        /// Add 10 sample records
        /// Assign  g1 to a survey
        /// Assign g2 to a survey
        /// Increase priority for calls 1-3 and 7-10 to 5000
        /// Set time to now for all calls
        /// Assign i3 to survey
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// i1      |   1
        /// i2      |   2
        /// i3      |   3
        /// i1      |   7
        /// i2      |   8
        /// i3      |   9
        /// i1      |   10
        /// i2      |   4
        /// i3      |   5
        /// i3      |   6
        /// For calls 1-5 set its = complete ( 13) and run scheduling rules
        /// For calls 6-10 set its-busy and run scheduling rules
        /// Run scheduling procedure
        /// Check that 1-5 calls do not exist.
        /// Check that 5-10 calls exist and phase = 2
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void AssignGroupsToSurvey_GetCalls_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            // assign groups g1, g2 to survey
            BackendTools.AssignCatiPersonToSurvey(_surveySid, _groupIDs[0]);
            BackendTools.AssignCatiPersonToSurvey(_surveySid, _groupIDs[1]);
            // assign person i3 to survey
            BackendTools.AssignCatiPersonToSurvey(_surveySid, _personIDs[2]);

            BackendTools.LoginPerson(_personIDs[0], "");
            BackendTools.LoginPerson(_personIDs[1], "");
            BackendTools.LoginPerson(_personIDs[2], "");

            CallTools.AssertCallWasGiven(_personIDs[0], _calls[0].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[1], _calls[1].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[2], _calls[2].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[0], _calls[6].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[1], _calls[7].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[2], _calls[8].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[0], _calls[9].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[1], _calls[3].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[2], _calls[4].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[2], _calls[5].CallID, _surveySid);

            MoveAndRescheduleAndCheckCallsExist();
        }

        /// <summary>
        /// Add survey, open survey.
        /// Add 3 users – i1,i2,i3
        /// Create two groups g1, g2
        /// Make i1 member of g1, i2 of g2
        /// Add 10 sample records
        /// Assign 1-3 calls to g1
        /// Assign 4-6 calls to g2
        /// Increase priority for calls 1-3 and 7-10 to 5000
        /// Set time to now for all calls
        /// Assign i3 to survey
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// i1      |   1
        /// i2      |   4
        /// i3      |   7
        /// i1      |   2
        /// i2      |   5
        /// i3      |   8
        /// i1      |   3
        /// i2      |   6
        /// i3      |   9
        /// i3      |   10
        /// For calls 1-5 set its = complete ( 13) and run scheduling rules
        /// For calls 6-10 set its-busy and run scheduling rules
        /// Run scheduling procedure
        /// Check that 1-5 calls do not exist.
        /// Check that 5-10 calls exist and phase = 2
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void AssignGroupsToCalls_GetCalls_CallsGivenInRightOrder()
        {
            PrepareDataForTest();
            // assign group g1 to calls 1-3
            CallTools.AssignCalls(_surveySid, new[]{
                                               _interviews[0].ID,
                                               _interviews[1].ID,
                                               _interviews[2].ID},
                                    _groupIDs[0]);
            // assign group g1 to calls 4-6
            CallTools.AssignCalls(_surveySid, new[]{
                                               _interviews[3].ID,
                                               _interviews[4].ID,
                                               _interviews[5].ID},
                                   _groupIDs[1]);
            // assign person i3 to survey
            BackendTools.AssignCatiPersonToSurvey(_surveySid, _personIDs[2]);
            BackendTools.LoginPerson(_personIDs[0], "");
            BackendTools.LoginPerson(_personIDs[1], "");
            BackendTools.LoginPerson(_personIDs[2], "");

            CallTools.AssertCallWasGiven(_personIDs[0], _calls[0].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[1], _calls[3].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[2], _calls[6].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[0], _calls[1].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[1], _calls[4].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[2], _calls[7].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[0], _calls[2].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[1], _calls[5].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[2], _calls[8].CallID, _surveySid);
            CallTools.AssertCallWasGiven(_personIDs[2], _calls[9].CallID, _surveySid);

            MoveAndRescheduleAndCheckCallsExist();
        }
    }
}
