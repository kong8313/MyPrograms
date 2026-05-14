using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Dialer
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait2)]
    public sealed class CallsDistributionInPredictiveSurveyForProtsTest : BaseMockedIntegrationTest
    {
        private readonly ISurveyStateService _surveyStateService;

        private readonly int _surveySid;
        const string ProjectId = "p0928475";
        
        public CallsDistributionInPredictiveSurveyForProtsTest()
        {
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            BackendToolsObject.LaunchAllHoursScript();
            _surveySid = BackendToolsObject.CreateSurvey(ProjectId);
            _surveyStateService.Open(_surveySid);
        }
        
        private IEnumerable<BvSpGetCachedCallsForPredictiveSurveyBySurveyEntity> GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForGroup(
            IEnumerable<int> interviewIds,
            int groupId)
        {
            return interviewIds.Select(x => new BvSpGetCachedCallsForPredictiveSurveyBySurveyEntity
            {
                DiallingMode = 0,
                ID = x,
                InterviewID = x,
                GroupID = (groupId == _surveySid ? 0 : groupId),
                SurveySid = _surveySid,
                TelephoneNumber = null,
                TimeInShift = BvCallEntity.TimeInsteadNowTimeToCall,
                ExplicitSid = 0
            });
        }

        private IEnumerable<BvSpGetCachedCallsForPredictiveSurveyBySurveyEntity> GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(
            IEnumerable<int> interviewIds,
            int personSid,
            int? surveySid = null)
        {
            return interviewIds.Select(x => new BvSpGetCachedCallsForPredictiveSurveyBySurveyEntity
            {
                DiallingMode = 0,
                ID = BvSvyScheduleAdapter.GetByCondition("SurveySid="+(surveySid ?? _surveySid)+" and interviewid="+x).First().ID,
                InterviewID = x,
                GroupID = 0,
                SurveySid = surveySid ?? _surveySid,
                TelephoneNumber = null,
                TimeInShift = BvCallEntity.TimeInsteadNowTimeToCall,
                ExplicitSid = personSid
            });
        }

        private IEnumerable<BvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity> GetCachedCallsForPredictiveSurveyByPersonGroup(
            IEnumerable<int> interviewIds, int groupId)
        {
            return interviewIds.Select(x => new BvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity
            {
                DiallingMode = 0,
                ID = x,
                InterviewID = x,
                GroupID = groupId,
                SurveySid = _surveySid,
                TelephoneNumber = null,
                TimeInShift = BvCallEntity.TimeInsteadNowTimeToCall,
                ExplicitSid = 0
            });
        }

        private IEnumerable<BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyEntity> GetCachedCallsForPredictiveSurveyAssignedToSurveyOnly(
            IEnumerable<int> interviewIds)
        {
            return interviewIds.Select(x => new BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyEntity
            {
                DiallingMode = 0,
                ID = x,
                InterviewID = x,
                GroupID = 0,
                SurveySid = _surveySid,
                TelephoneNumber = null,
                TimeInShift = BvCallEntity.TimeInsteadNowTimeToCall,
                ExplicitSid = 0
            });
        }

        private IEnumerable<BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssignedEntity> GetCachedCallsForPredictiveSurveyExplicitlyAssignedForPerson(
            IEnumerable<int> interviewIds,
            int personSid)
        {
            return interviewIds.Select(x => new BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssignedEntity
            {
                DiallingMode = 0,
                ID = x,
                InterviewID = x,
                GroupID = 0,
                SurveySid = _surveySid,
                TelephoneNumber = null,
                TimeInShift = BvCallEntity.TimeInsteadNowTimeToCall,
                ExplicitSid = personSid
            });
        }

        private IEnumerable<BvSpGetCachedCallsForPredictiveSurveyBySurveyEntity> GetRandomCalls(int top)
        {
            return BvSvyScheduleAdapter.GetAll().OrderBy(x => x.CallOrder).Take(top).
                Select(x => new BvSpGetCachedCallsForPredictiveSurveyBySurveyEntity
            {
                DiallingMode = 0,
                ID = x.ID,
                InterviewID = x.InterviewID,
                GroupID = 0,
                SurveySid = x.SurveySID,
                TelephoneNumber = null,
                TimeInShift = BvCallEntity.TimeInsteadNowTimeToCall,
                ExplicitSid = 0
            });
        }

        private bool CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity(
            BvSpGetCachedCallsForPredictiveSurveyBySurveyEntity expected,
            BvSpGetCachedCallsForPredictiveSurveyBySurveyEntity actual)
        {
            Assert.AreEqual(expected.ID, actual.ID, "different ids of call");
            Assert.AreEqual(expected.InterviewID, actual.InterviewID, "different InterviewID of call");
            Assert.AreEqual(expected.SurveySid, actual.SurveySid, "different SurveyId of call");
            Assert.AreEqual(expected.GroupID, actual.GroupID, "different SurveyGroupId of call");
            Assert.AreEqual(expected.ExplicitSid, actual.ExplicitSid, "different ExplicitSid of call");
            Assert.AreEqual(expected.DiallingMode, actual.DiallingMode, "different DiallingMode of call");
            Assert.AreEqual(expected.TelephoneNumber, actual.TelephoneNumber, "different TelephoneNumber of call");
            Assert.AreEqual(expected.TimeInShift, actual.TimeInShift, "different TimeInShift of call");
            return true;
        }

        private bool CompareGetCachedCallsForPredictiveSurveyExplicitlyAssignedEntity(
            BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssignedEntity expected,
            BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssignedEntity actual)
        {
            Assert.AreEqual(expected.ID, actual.ID, "different ids of call");
            Assert.AreEqual(expected.InterviewID, actual.InterviewID, "different InterviewID of call");
            Assert.AreEqual(expected.SurveySid, actual.SurveySid, "different SurveySid of call");
            Assert.AreEqual(expected.GroupID, actual.GroupID, "different SurveyGroupId of call");
            Assert.AreEqual(expected.ExplicitSid, actual.ExplicitSid, "different ExplicitSid of call");
            Assert.AreEqual(expected.DiallingMode, actual.DiallingMode, "different DiallingMode of call");
            Assert.AreEqual(expected.TelephoneNumber, actual.TelephoneNumber, "different TelephoneNumber of call");
            Assert.AreEqual(expected.TimeInShift, actual.TimeInShift, "different TimeInShift of call");
            return true;
        }

        private bool CompareGetCachedCallsForPredictiveSurveyByPersonGroupEntity(
            BvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity expected,
            BvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity actual)
        {
            Assert.AreEqual(expected.ID, actual.ID, "different ids of call");
            Assert.AreEqual(expected.InterviewID, actual.InterviewID, "different InterviewID of call");
            Assert.AreEqual(expected.SurveySid, actual.SurveySid, "different SurveySid of call");
            Assert.AreEqual(expected.GroupID, actual.GroupID, "different SurveyGroupId of call");
            Assert.AreEqual(expected.ExplicitSid, actual.ExplicitSid, "different ExplicitSid of call");
            Assert.AreEqual(expected.DiallingMode, actual.DiallingMode, "different DiallingMode of call");
            Assert.AreEqual(expected.TelephoneNumber, actual.TelephoneNumber, "different TelephoneNumber of call");
            Assert.AreEqual(expected.TimeInShift, actual.TimeInShift, "different TimeInShift of call");
            return true;
        }

        private bool CompareGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyEntity(
            BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyEntity expected,
            BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyEntity actual)
        {
            Assert.AreEqual(expected.ID, actual.ID, "different ids of call");
            Assert.AreEqual(expected.InterviewID, actual.InterviewID, "different InterviewID of call");
            Assert.AreEqual(expected.SurveySid, actual.SurveySid, "different SurveySid of call");
            Assert.AreEqual(expected.GroupID, actual.GroupID, "different SurveyGroupId of call");
            Assert.AreEqual(expected.ExplicitSid, actual.ExplicitSid, "different ExplicitSid of call");
            Assert.AreEqual(expected.DiallingMode, actual.DiallingMode, "different DiallingMode of call");
            Assert.AreEqual(expected.TelephoneNumber, actual.TelephoneNumber, "different TelephoneNumber of call");
            Assert.AreEqual(expected.TimeInShift, actual.TimeInShift, "different TimeInShift of call");
            return true;
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void CallsDistributionInPredictiveSurveyProTS_AlonePersonShouldGetAllCalls_AllCallsIsDistributed(DialType dialType)
        {
            var interviewIds = Enumerable.Range(1, 6).ToArray();

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            int person1 = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.CampaignAssignment, dialType);

            CallTools.AssignCalls(_surveySid, interviewIds, person1);

            BackendTools.RunSchedulingProcedure();

            const int count = 6;
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            var res = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(_surveySid, 1, count, currentTime, (int)dialType);

            var expected = GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(
                interviewIds,
                person1).ToArray();

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity);

            PredictiveTools.CheckUpdatingPhase(_surveySid, expected.Select(x => (int)x.InterviewID));
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void CallsDistributionInPredictiveSurveyProTS_TwoPersonsGetCorrectPartOfCalls_GettingCallsCorrect(DialType dialType)
        {
            const int interviewsCountForPerson1 = 10;
            const int interviewsCountForPerson2 = 20;

            var interviewIds = Enumerable.Range(1, interviewsCountForPerson1 + interviewsCountForPerson2).ToArray();
            var person1InterviewIds = Enumerable.Range(1, interviewsCountForPerson1).ToArray();
            var person2InterviewIds = Enumerable.Range(interviewsCountForPerson1 + 1, interviewsCountForPerson2).ToArray();

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            int person1 = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.CampaignAssignment, dialType);
            int person2 = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u2", AgentTaskChoiceMode.CampaignAssignment, dialType);

            CallTools.AssignCalls(_surveySid, person1InterviewIds, person1);
            CallTools.AssignCalls(_surveySid, person2InterviewIds, person2);

            BackendTools.RunSchedulingProcedure();

            const int count = (interviewsCountForPerson1 + interviewsCountForPerson2) / 2;
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            var res = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(_surveySid, 1, count, currentTime, (int)dialType);

            //we get half of calls with high priority
            var expectedCallsForPerson1 = person1InterviewIds.Skip(interviewsCountForPerson1 / 2);
            var expectedCallsForPerson2 = person2InterviewIds.Skip(interviewsCountForPerson2 / 2);

            var expected = GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(expectedCallsForPerson1, person1).
                Concat(GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(expectedCallsForPerson2, person2)).ToArray();

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity);

            PredictiveTools.CheckUpdatingPhase(_surveySid, expected.Select(x => (int)x.InterviewID));
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void CallsDistributionInPredictiveSurveyProTS_PersonGetAllCall_GettingCallsCorrect(DialType dialType)
        {
            var interviewIds = Enumerable.Range(1, 27).ToArray();
            var person1InterviewIds = Enumerable.Range(1, 1).ToArray();
            var person2InterviewIds = Enumerable.Range(2, 6).ToArray();

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            int person1 = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.CampaignAssignment, dialType);
            int person2 = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u2", AgentTaskChoiceMode.CampaignAssignment, dialType);

            CallTools.AssignCalls(_surveySid, person1InterviewIds, person1);
            CallTools.AssignCalls(_surveySid, person2InterviewIds, person2);

            BackendTools.RunSchedulingProcedure();

            int count = interviewIds.Count() / 2;
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            var res = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(_surveySid, 1, count, currentTime, (int)dialType);

            var expected = GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(person1InterviewIds, person1).
                Concat(GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(person2InterviewIds.Skip(1), person2)).
                Concat(GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForGroup(interviewIds.Skip(17), _surveySid)).ToArray();

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity);

            PredictiveTools.CheckUpdatingPhase(_surveySid, expected.Select(x => (int)x.InterviewID));
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void CallsDistributionInPredictiveSurveyProTS_OnlyCallsWithPhase2AreReturned_GettingCallsCorrect(DialType dialType)
        {
            var interviewIds = Enumerable.Range(1, 9).ToArray();

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            int person = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.CampaignAssignment, dialType);
            int group = PersonTools.CreateAndAssignPersonGroupOnSurvey(_surveySid, "g1");
            PersonService.SetParentGroups(person, new[] { group });

            CallTools.AssignCalls(_surveySid, interviewIds.Reverse().Take(2), person);
            CallTools.AssignCalls(_surveySid, interviewIds.Take(1), group);

            BackendTools.RunSchedulingProcedure();

            TaskService.LookupByPersonSid(person, _surveySid);

            BackendTools.RunSchedulingProcedure();

            int count = interviewIds.Count();
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            var res = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(_surveySid, 1, count, currentTime, (int)dialType);

            var expected = GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(interviewIds.Reverse().Skip(1).Take(1), person).
                Concat(GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForGroup(interviewIds.Take(1), group)).
                Concat(GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForGroup(interviewIds.Skip(1).Take(interviewIds.Count() - 3), _surveySid)).ToArray();

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity);

            PredictiveTools.CheckUpdatingPhase(_surveySid, expected.Select(x => (int)x.InterviewID));
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void CallsDistributionInPredictiveSurveyProTS_SomeSurveysGetCallsForOneOfThem_GettingCallsCorrect(DialType dialType)
        {
            int surveySid2 = BackendToolsObject.CreateSurvey("p0998877");
            _surveyStateService.Open(surveySid2);

            var interviewIds = Enumerable.Range(1, 20).ToArray();

            var interviews2 = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews2);
            var interviews1 = FusionLibTestTools.CreateInterviewsForTest(surveySid2, interviewIds.Take(10), dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews1);
            
            int person1 = PersonTools.CreateAssignAndLoginPersonOnSurvey(surveySid2, "u1", AgentTaskChoiceMode.CampaignAssignment, dialType);
            BackendTools.AssignCatiPersonToSurvey(_surveySid, person1);
            CallTools.AssignCalls(surveySid2, interviewIds.Take(10), person1);
            CallTools.AssignCalls(_surveySid, interviewIds.Skip(10), person1);

            int person2 = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u2", AgentTaskChoiceMode.CampaignAssignment, dialType);
            CallTools.AssignCalls(_surveySid, interviewIds.Take(10), person2);

            BackendTools.RunSchedulingProcedure();

            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            var res = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(_surveySid, 1, 100, currentTime, (int)dialType);

            var expected = GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(interviewIds.Take(10), person2);

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity);

            PredictiveTools.CheckUpdatingPhase(_surveySid, expected.Select(x => (int)x.InterviewID));

            res = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(surveySid2, 1, 100, currentTime, (int)dialType);

            expected = GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(interviewIds.Take(10), person1, surveySid2);

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity);

            PredictiveTools.CheckUpdatingPhase(_surveySid, expected.Select(x => (int)x.InterviewID));
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void CallsDistributionInPredictiveSurveyProTS_RandomCallDeliveryEnable_CallSAreDeliveredCorrectly(DialType dialType)
        {
            SurveyService.SetCallDeliveryMode(_surveySid, CallDeliveryMode.Random);

            var interviewIds = Enumerable.Range(1, 40);

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews, 1);

            int person = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.CampaignAssignment, dialType);
            BackendTools.AssignCatiPersonToSurvey(_surveySid, person);
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();

            BackendTools.RunSchedulingProcedure();

            var res = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(_surveySid, 1, 10, currentTime, (int)dialType);

            TestAssert.AreEqual(GetRandomCalls(10),
                res,
                CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity);
        }

        [Theory, Owner(@"FIRM\AlexanderL"), Cr(70770)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetCachedCallsForPredictiveSurveyBySurvey_TimeShouldBeSpecifiedOnlyForAppointments(DialType dialType)
        {
            var interviewIds = Enumerable.Range(1, 2).ToArray();

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            var date = DateTime.UtcNow.AddHours(1).CutMilliseconds();
            BackendTools.AddAppointmentAndLinkItWithCall(interviews[0].ID, _surveySid, date);

            int person = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.CampaignAssignment, dialType);
            CallTools.AssignCalls(_surveySid, interviewIds, person);
            BackendTools.RunSchedulingProcedure(date);

            var res = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(_surveySid, 1, 100, date, (int)dialType);

            var expected = GetBvSpGetCachedCallsForPredictiveSurveyBySurveyEntityForPerson(interviewIds, person);
            expected = expected.Select(x =>
                {
                    x.TimeInShift = (x.InterviewID == interviews[0].ID ? date : x.TimeInShift);
                    return x;
                });

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyBySurveyEntity);
        }

        [Theory, Owner(@"FIRM\AlexanderL"), Cr(70770)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetCachedCallsForPredictiveSurveyExplicitly_TimeShouldBeSpecifiedOnlyForAppointments(DialType dialType)
        {
            var interviewIds = Enumerable.Range(1, 2).ToArray();

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            var date = DateTime.UtcNow.AddHours(1).CutMilliseconds();
            BackendTools.AddAppointmentAndLinkItWithCall(interviews[0].ID, _surveySid, date);

            int person = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.CampaignAssignment, dialType);
            CallTools.AssignCalls(_surveySid, interviewIds, person);
            BackendTools.RunSchedulingProcedure(date);

            var res = BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssignedAdapter.ExecuteEntityList(_surveySid, 1, 100, date, (int)dialType);

            var expected = GetCachedCallsForPredictiveSurveyExplicitlyAssignedForPerson(interviewIds, person);
            expected = expected.Select(x =>
            {
                x.TimeInShift = (x.InterviewID == interviews[0].ID ? date : x.TimeInShift);
                return x;
            });

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareGetCachedCallsForPredictiveSurveyExplicitlyAssignedEntity);
        }

        [Theory, Owner(@"FIRM\AlexanderL"), Cr(70770)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetCachedCallsForPredictiveSurveyByPersonGroup_TimeShouldBeSpecifiedOnlyForAppointments(DialType dialType)
        {
            var interviewIds = Enumerable.Range(1, 2).ToArray();

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            var date = DateTime.UtcNow.AddHours(1).CutMilliseconds();
            BackendTools.AddAppointmentAndLinkItWithCall(interviews[0].ID, _surveySid, date);

            int person = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.Automatic, dialType);
            var group = PersonTools.CreatePersonGroup("Group");
            PersonService.SetParentGroups(person, new[] {group});

            CallTools.AssignCalls(_surveySid, interviewIds, group);
            BackendTools.RunSchedulingProcedure(date);

            var res = BvSpGetCachedCallsForPredictiveSurveyByPersonGroupAdapter.ExecuteEntityList(_surveySid, group, 100, date, (int)dialType);

            var expected = GetCachedCallsForPredictiveSurveyByPersonGroup(interviewIds, group);
            expected = expected.Select(x =>
            {
                x.TimeInShift = (x.InterviewID == interviews[0].ID ? date : x.TimeInShift);
                return x;
            });

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareGetCachedCallsForPredictiveSurveyByPersonGroupEntity);
        }

        [Theory, Owner(@"FIRM\AlexanderL"), Cr(70770)]
        [ClassData(typeof(PredictiveDialTypes))]
        public void GetCachedCallsForPredictiveSurveyAssignedToSurveyOnly_TimeShouldBeSpecifiedOnlyForAppointments(DialType dialType)
        {
            var interviewIds = Enumerable.Range(1, 2).ToArray();

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);

            var date = DateTime.UtcNow.AddHours(1).CutMilliseconds();
            BackendTools.AddAppointmentAndLinkItWithCall(interviews[0].ID, _surveySid, date);

            PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.Automatic);
            BackendTools.RunSchedulingProcedure(date);

            var res = BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyAdapter.ExecuteEntityList(_surveySid, 100, date, (int)dialType);

            var expected = GetCachedCallsForPredictiveSurveyAssignedToSurveyOnly(interviewIds);
            expected = expected.Select(x =>
            {
                x.TimeInShift = (x.InterviewID == interviews[0].ID ? date : x.TimeInShift);
                return x;
            });

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyEntity);
        }
    }
}
