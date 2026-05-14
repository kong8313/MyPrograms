using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.LookupByPersonSID
{
    [TestClass]
    public class LookupByPersonSIDTwoSurveys
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private const int Count = 3; // calls count in tests
        private readonly int[] _surveyIDs = new int[2]; //  surdey iD
        private readonly int[] _personIDs = new int[3]; // persons IDs
        private readonly int[] _groupIDs = new int[2]; // groups IDs
        private readonly BvInterviewEntity[] _firstSurveyInterviews = new BvInterviewEntity[Count]; // interviews for the first survey
        private readonly BvCallEntity[] _firstSurveyCalls = new BvCallEntity[Count]; // test calls for the first survey
        private readonly BvInterviewEntity[] _secondSurveyInterviews = new BvInterviewEntity[Count]; // interviews for the second survey
        private readonly BvCallEntity[] _secondSurveyCalls = new BvCallEntity[Count]; // test calls for the second survey
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            var stubITelephony = new StubITelephony
            {
                SetGroupsInt32Int64StringArrayOfInt32 = (id, campaignId, agentId, groups) => DialerErrorCode.Success
            };
            Stubs.ExtendExistingITelephonyStub(stubITelephony);

            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        

        /// <summary>
        /// Creates 2 surveys.
        /// Launches ALL HOURS scheduling script and assignes it to surveys.
        /// </summary>
        private void PrepareDataForTest()
        {
            const string firstProjectId = "p0000001";
            const string secondProjectId = "p0000002";

            _surveyIDs[0] = _backendTools.CreateSurvey(firstProjectId);

            _surveyIDs[1] = _backendTools.CreateSurvey(secondProjectId);

            _backendTools.LaunchAllHoursScript();
        }

        /// <summary>
        /// Creates call with given priority for the first survey.
        /// </summary>
        /// <param name="order">Call order in calls aray</param>
        /// <param name="priority">Call priority</param>
        /// <param name="timeInShift">Time in shift</param>
        private void CreateCallForFirstSurvey(int order, short priority, DateTime timeInShift)
        {
            _firstSurveyInterviews[order] = BackendTools.NewInterview(_surveyIDs[0]);
            BackendTools.CreateInterview(_firstSurveyInterviews[order]);

            _firstSurveyCalls[order] = BackendTools.NewCall(_firstSurveyInterviews[order]);
            _firstSurveyCalls[order].TimeInShift = timeInShift.ToUniversalTime();
            _firstSurveyCalls[order].Priority = priority;
            BackendTools.CreateCall(_firstSurveyCalls[order]);
            _firstSurveyCalls[order].CallID = CallQueueService.GetCallAndNoLock(_surveyIDs[0], _firstSurveyInterviews[order].ID).CallID;
        }

        /// <summary>
        /// Creates call with given priority for the second survey.
        /// </summary>
        /// <param name="order">Call order in calls aray</param>
        /// <param name="priority">Call priority</param>
        /// <param name="timeInShift">Time in shift</param>
        private void CreateCallForSecondSurvey(int order, short priority, DateTime timeInShift)
        {
            _secondSurveyInterviews[order] = BackendTools.NewInterview(_surveyIDs[1]);
            BackendTools.CreateInterview(_secondSurveyInterviews[order]);

            _secondSurveyCalls[order] = BackendTools.NewCall(_secondSurveyInterviews[order]);
            _secondSurveyCalls[order].TimeInShift = timeInShift.ToUniversalTime();
            _secondSurveyCalls[order].Priority = priority;
            BackendTools.CreateCall(_secondSurveyCalls[order]);
            _secondSurveyCalls[order].CallID = CallQueueService.GetCallAndNoLock(_surveyIDs[1], _secondSurveyInterviews[order].ID).CallID;
        }

        /// <summary>
        /// create 2 surveys - survey1, survey2
        /// open surveys
        /// create 3 persons i1, i2, i3
        /// Add 1 sample record to survey 1 ( call 1 )
        /// Add 1 sample record to survey 2 ( call 2 )
        /// Add 1 sample record to survey 1 ( call 3 )
        /// Add 1 sample record to survey 2 ( call 4 )
        /// Add 1 sample record to survey 1 ( call 5 )
        /// Add 1 sample record to survey 2 ( call 6 )
        /// Set time to call for call 1 = Now() – 10 min
        /// Set time to call for call 2 = Now() – 10 min + 1 min
        /// Set time to call for call 3 = Now() – 10 min + 2 min
        /// Set time to call for call 4 = Now() – 10 min + 3 min
        /// Set time to call for call 5 = Now() – 10 min + 4 min 
        /// Set time to call for call 6 = Now() – 10 min + 5 min
        /// Assign i1, i2, i3 to survey 1
        /// Assign i1, i2, i3 to survey 2
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// i1      |   1
        /// i2      |   2
        /// i3      |   3
        /// i1      |   4
        /// i2      |   5
        /// i3      |   6
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void DifferentTimeForCallsAssignedTwoSurveys_LookupByPersonSID_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            _surveyStateService.Open(_surveyIDs[0]);
            _surveyStateService.Open(_surveyIDs[1]);

            _personIDs[0] = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic);
            _personIDs[1] = PersonTools.CreatePerson("i2", "password", AgentTaskChoiceMode.Automatic);
            _personIDs[2] = PersonTools.CreatePerson("i3", "password", AgentTaskChoiceMode.Automatic);

            const int priority = 1;
            DateTime now = DateTime.Now;
            CreateCallForFirstSurvey(0, priority, now.AddMinutes(-10));
            CreateCallForSecondSurvey(0, priority, now.AddMinutes(-9));
            CreateCallForFirstSurvey(1, priority, now.AddMinutes(-8));
            CreateCallForSecondSurvey(1, priority, now.AddMinutes(-7));
            CreateCallForFirstSurvey(2, priority, now.AddMinutes(-6));
            CreateCallForSecondSurvey(2, priority, now.AddMinutes(-5));

            for (int i = 0; i < 3; i++)
            {
                BackendTools.AssignCatiPersonToSurvey(_surveyIDs[0], _personIDs[i]);
                BackendTools.AssignCatiPersonToSurvey(_surveyIDs[1], _personIDs[i]);
            }

            BackendTools.LoginPerson(_personIDs[0], "");
            BackendTools.LoginPerson(_personIDs[1], "");
            BackendTools.LoginPerson(_personIDs[2], "");

            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[0].CallID);
            CallTools.AssertCallWasGiven(_personIDs[1], _secondSurveyCalls[0].CallID);
            CallTools.AssertCallWasGiven(_personIDs[2], _firstSurveyCalls[1].CallID);
            CallTools.AssertCallWasGiven(_personIDs[0], _secondSurveyCalls[1].CallID);
            CallTools.AssertCallWasGiven(_personIDs[1], _firstSurveyCalls[2].CallID);
            CallTools.AssertCallWasGiven(_personIDs[2], _secondSurveyCalls[2].CallID);
        }

        /// <summary>
        /// 
        /// Test creates two surveys. Assigns surveys to groups.
        /// Test checks that LookupByPersonSID returns calls in right order.
        /// Order depends on calls priority and surveys openning and closing order.
        ///
        /// create 2 surveys - survey1, survey2
        /// Add 2 users – i1, i2
        /// Create 2 groups g1, g2
        /// Make i1 member of g1, i2 of g2
        /// Add 3 sample records for survey1 (calls 1-3)
        /// Add 3 sample records for survey2 (calls 4-6)
        /// Assign g1 to survey1
        /// Assign g2 to survey2
        /// Set time to now for all calls
        /// Increase priority for calls 2,3 of survey2 to 5
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// Open survey1
        /// i1      |   1
        /// i2      |   -
        /// Open survey2
        /// i2      |   5
        /// Make i1 also a member of g2
        /// i1      |   6
        /// Close survey2
        /// i1      |   2
        /// i2      |   -
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void GroupsAssignedToTwoSurveys_LookupByPersonSID_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            _groupIDs[0] = PersonTools.CreatePersonGroup("g1");
            _personIDs[0] = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic, new[] { _groupIDs[0] });
            _groupIDs[1] = PersonTools.CreatePersonGroup("g2");
            _personIDs[1] = PersonTools.CreatePerson("i2", "password", AgentTaskChoiceMode.Automatic, new[] { _groupIDs[1] });

            short priority = 1;
            CreateCallForFirstSurvey(0, priority, DateTime.Now);
            CreateCallForFirstSurvey(1, priority, DateTime.Now);
            CreateCallForFirstSurvey(2, priority, DateTime.Now);

            CreateCallForSecondSurvey(0, priority, DateTime.Now);
            priority = 5;
            CreateCallForSecondSurvey(1, priority, DateTime.Now);
            CreateCallForSecondSurvey(2, priority, DateTime.Now);

            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[0], _groupIDs[0]);
            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[1], _groupIDs[1]);

            _surveyStateService.Open(_surveyIDs[0]);
            BackendTools.LoginPerson(_personIDs[0], "");
            BackendTools.LoginPerson(_personIDs[1], "");

            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[0].CallID);
            CallTools.AssertNoCallWasGiven(_personIDs[1]);

            _surveyStateService.Open(_surveyIDs[1]);

            CallTools.AssertCallWasGiven(_personIDs[1], _secondSurveyCalls[1].CallID);

            // make i1 member of g2
            List<int> parentGroups = PersonService.GetParentGroups(_personIDs[0]).ToList();
            parentGroups.Add(_groupIDs[1]);
            PersonService.SetParentGroups(_personIDs[0], parentGroups.ToArray());

            CallTools.AssertCallWasGiven(_personIDs[0], _secondSurveyCalls[2].CallID);

            _surveyStateService.CloseSurvey(_surveyIDs[1]);
            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[1].CallID);
            CallTools.AssertNoCallWasGiven(_personIDs[1]);
        }

        /// <summary>
        /// 
        /// Test creates two surveys. Assigns first survey to group. Activates second survey calls to interviewer.
        /// Test checks that LookupByPersonSID returns calls in right order.
        /// Order depends on calls priority and surveys openning and closing order.
        /// 
        /// create 2 surveys - survey1, survey2
        /// Add user i1
        /// Create group g1
        /// Make i1 member of i1
        /// Add 3 sample records for survey1 (calls 1-3)
        /// Add 3 sample records for survey2 (calls 4-6)
        /// Assign g1 to survey1
        /// Set time to now for all calls
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// Open survey1
        /// i1      |   1
        /// Open Survey2
        /// i1      |   2
        /// Activate calls of surveys2 (priority = 5, resource = i1)
        /// i1      |   4
        /// Close Survey2
        /// i1      |   3
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void FirstSurveyAssignedToGroupSecondSurveyCallsActivated_LookupByPersonSID_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            _groupIDs[0] = PersonTools.CreatePersonGroup("g1");
            _personIDs[0] = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic, new[] { _groupIDs[0] });

            short priority = 1;
            for (int i = 0; i < Count; i++)
            {
                CreateCallForFirstSurvey(i, priority, DateTime.Now);
            }

            for (int i = 0; i < Count; i++)
            {
                CreateCallForSecondSurvey(i, priority, DateTime.Now);
            }

            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[0], _groupIDs[0]);
            BackendTools.LoginPerson(_personIDs[0], "");

            _surveyStateService.Open(_surveyIDs[0]);
            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[0].CallID);

            _surveyStateService.Open(_surveyIDs[1]);
            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[1].CallID);

            priority = 5;
            DateTime now = DateTime.Now.ToUniversalTime();
            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                _surveyIDs[1], new[]
                                    {
                                        _secondSurveyCalls[0].InterviewID,
                                        _secondSurveyCalls[1].InterviewID,
                                        _secondSurveyCalls[2].InterviewID
                                    },
                priority, _personIDs[0], (int)CallShiftType.None, now, CallStates.Scheduled, false);

            CallTools.AssertCallWasGiven(_personIDs[0], _secondSurveyCalls[0].CallID);

            _surveyStateService.CloseSurvey(_surveyIDs[1]);
            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[2].CallID);
        }

        /// <summary>
        /// 
        /// Test creates two surveys and assigns them to single interviewer.
        /// Test checks that LookupByPersonSID returns calls in right order.
        /// Order depends on calls priority and surveys openning and closing order.
        /// 
        /// create 2 surveys - survey1, survey2
        /// Add user i1
        /// Add 3 sample records for survey1 (calls 1-3)
        /// Add 3 sample records for survey2 (calls 4-6)
        /// Assign i1 to survey1 and survey 2
        /// Set time to now for all calls
        /// Increase priority for call 3 to 9
        /// Increase priority for call 4 of Survey2 to 8
        /// Increase priority for call 6 of Survey2 to 10
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// Open survey1 and survey2
        /// i1      |   6
        /// i1      |   3
        /// i1      |   4
        /// Close Survey2
        /// i1      |   1
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void TwoSurveysAssignedToSingleInterviewer_LookupByPersonSID_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            _personIDs[0] = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic);

            short priority = 1;
            CreateCallForFirstSurvey(0, priority, DateTime.Now);
            CreateCallForFirstSurvey(1, priority, DateTime.Now);
            priority = 9;
            CreateCallForFirstSurvey(2, priority, DateTime.Now);

            priority = 8;
            CreateCallForSecondSurvey(0, priority, DateTime.Now);
            priority = 1;
            CreateCallForSecondSurvey(1, priority, DateTime.Now);
            priority = 10;
            CreateCallForSecondSurvey(2, priority, DateTime.Now);

            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[0], _personIDs[0]);
            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[1], _personIDs[0]);

            _surveyStateService.Open(_surveyIDs[0]);
            _surveyStateService.Open(_surveyIDs[1]);
            BackendTools.LoginPerson(_personIDs[0], "");
            BackendTools.LoginPerson(_personIDs[0], "");

            CallTools.AssertCallWasGiven(_personIDs[0], _secondSurveyCalls[2].CallID);
            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[2].CallID);
            CallTools.AssertCallWasGiven(_personIDs[0], _secondSurveyCalls[0].CallID);

            _surveyStateService.CloseSurvey(_surveyIDs[1]);
            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[0].CallID);
        }

        /// <summary>
        /// 
        /// Test creates two surveys and assigns them to two interviewers.
        /// Test checks that LookupByPersonSID for different interviewers returns calls in right order.
        /// Order depends on calls priority and surveys openning order.
        /// 
        /// create 2 surveys - survey1, survey2
        /// Add 2 users – i1, i2
        /// Add 3 sample records for survey1 (calls 1-3)
        /// Add 3 sample records for survey2 (calls 4-6)
        /// Assign i1 to survey1
        /// Assign i2 to survey2
        /// Set time to now for all calls
        /// Increase priority for calls 4,5 to 5
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// Open survey1 and survey2
        /// i1      |   1
        /// i2      |   4
        /// Assign i1 also to Survey2
        /// i1      |   5
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void InterviewersAssignedToTwoSurveys_LookupByPersonSID_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            _personIDs[0] = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic);
            _personIDs[1] = PersonTools.CreatePerson("i2", "password", AgentTaskChoiceMode.Automatic);

            short priority = 1;
            CreateCallForFirstSurvey(0, priority, DateTime.Now);
            CreateCallForFirstSurvey(1, priority, DateTime.Now);
            CreateCallForFirstSurvey(2, priority, DateTime.Now);

            priority = 5;
            CreateCallForSecondSurvey(0, priority, DateTime.Now);
            CreateCallForSecondSurvey(1, priority, DateTime.Now);
            priority = 1;
            CreateCallForSecondSurvey(2, priority, DateTime.Now);

            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[0], _personIDs[0]);
            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[1], _personIDs[1]);

            _surveyStateService.Open(_surveyIDs[0]);
            _surveyStateService.Open(_surveyIDs[1]);

            BackendTools.LoginPerson(_personIDs[0], "");
            BackendTools.LoginPerson(_personIDs[1], "");

            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[0].CallID);
            CallTools.AssertCallWasGiven(_personIDs[1], _secondSurveyCalls[0].CallID);

            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[1], _personIDs[0]);
            CallTools.AssertCallWasGiven(_personIDs[0], _secondSurveyCalls[1].CallID);
        }

        /// <summary>
        /// 
        /// Test creates two surveys and assigns them to two interviewers and group.
        /// Test checks that LookupByPersonSID returns calls in right order
        /// in cases when person is deassigned from survey or
        /// person is excluded from a group assigned to survey.
        /// 
        /// create 2 surveys - survey1, survey2
        /// Add 2 users – i1, i2
        /// Add 1 group - g1
        /// Add 3 sample records for survey1 (calls 1-3)
        /// Add 3 sample records for survey2 (calls 4-6)
        /// Assign g1 to survey1
        /// Assign i1 to survey2
        /// Assign i2 to survey1, survey2
        /// Set time to now for all calls
        /// Execute LookupByPerson for user and get call
        /// user    |   call
        /// Open survey1 and survey2
        /// i1      |   1
        /// i2      |   2
        /// Exclude i1 from g1
        /// i1      |   4
        /// Deassign i2 from Survey1
        /// i2      |   5
        /// </summary>
        [TestMethod, Owner(@"FIRM\SvetlanaT")]
        public void InterviewerDeassignesFromSurvey_LookupByPersonSID_CallsGivenInRightOrder()
        {
            PrepareDataForTest();

            _groupIDs[0] = PersonTools.CreatePersonGroup("g1");
            _personIDs[0] = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic, new[] { _groupIDs[0] });
            _personIDs[1] = PersonTools.CreatePerson("i2", "password", AgentTaskChoiceMode.Automatic);

            const short priority = 1;
            CreateCallForFirstSurvey(0, priority, DateTime.Now);
            CreateCallForFirstSurvey(1, priority, DateTime.Now);
            CreateCallForFirstSurvey(2, priority, DateTime.Now);

            //priority = 5;
            CreateCallForSecondSurvey(0, priority, DateTime.Now);
            CreateCallForSecondSurvey(1, priority, DateTime.Now);
            //priority = 1;
            CreateCallForSecondSurvey(2, priority, DateTime.Now);

            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[0], _groupIDs[0]);
            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[1], _personIDs[0]);
            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[0], _personIDs[1]);
            BackendTools.AssignCatiPersonToSurvey(_surveyIDs[1], _personIDs[1]);

            _surveyStateService.Open(_surveyIDs[0]);
            _surveyStateService.Open(_surveyIDs[1]);

            BackendTools.LoginPerson(_personIDs[0], "");
            BackendTools.LoginPerson(_personIDs[1], "");

            CallTools.AssertCallWasGiven(_personIDs[0], _firstSurveyCalls[0].CallID);
            CallTools.AssertCallWasGiven(_personIDs[1], _firstSurveyCalls[1].CallID);

            // exclude i1 from groups
            List<int> parentGroups = PersonService.GetParentGroups(_personIDs[0]).ToList();
            parentGroups.Clear();
            PersonService.SetParentGroups(_personIDs[0], parentGroups.ToArray());

            CallTools.AssertCallWasGiven(_personIDs[0], _secondSurveyCalls[0].CallID);

            // deassign i2 from the first survey
            BackendTools.DeassignCatiPersonFromSurvey(_surveyIDs[0], _personIDs[1]);

            CallTools.AssertCallWasGiven(_personIDs[1], _secondSurveyCalls[1].CallID);
        }
    }
}
