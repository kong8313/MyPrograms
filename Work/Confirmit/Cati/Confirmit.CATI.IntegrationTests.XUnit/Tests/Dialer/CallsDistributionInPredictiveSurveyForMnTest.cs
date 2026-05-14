using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Dialer
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait2)]
    public class CallsDistributionInPredictiveSurveyForMnTest : BaseMockedIntegrationTest
    {
        private readonly ISurveyStateService _surveyStateService;
        
        private readonly int _surveySid;
        const string ProjectId = "p09284375";
        
        public CallsDistributionInPredictiveSurveyForMnTest()
        {
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            BackendToolsObject.LaunchAllHoursScript();
            _surveySid = BackendToolsObject.CreateSurvey(ProjectId);
            _surveyStateService.Open(_surveySid);
        }

        private IEnumerable<BvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity> GetBvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity(
            IEnumerable<int> interviewIds,
            int objectSid,
            int personSid)
        {
            return interviewIds.Select(x => new BvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity
            {
                DiallingMode = 0,
                ID = x,
                InterviewID = x,
                GroupID = objectSid,
                SurveySid = _surveySid,
                TelephoneNumber = null,
                TimeInShift = BvCallEntity.TimeInsteadNowTimeToCall,
                ExplicitSid = personSid
            });
        }

        private bool CompareBvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity(
            BvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity expected,
            BvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity actual)
        {
            Assert.AreEqual(expected.ID, actual.ID, "different ids of call");
            Assert.AreEqual(expected.InterviewID, actual.InterviewID, "different InterviewID of call");
            Assert.AreEqual(expected.SurveySid, actual.SurveySid, "different SurveyId of call");
            Assert.AreEqual(expected.GroupID, actual.GroupID, "different GroupID of call");
            Assert.AreEqual(expected.ExplicitSid, actual.ExplicitSid, "different ExplicitSid of call");
            Assert.AreEqual(expected.DiallingMode, actual.DiallingMode, "different DiallingMode of call");
            Assert.AreEqual(expected.TelephoneNumber, actual.TelephoneNumber, "different TelephoneNumber of call");
            Assert.AreEqual(expected.TimeInShift, actual.TimeInShift, "different TimeInShift of call");
            return true;
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void CallsDistributionInPredictiveSurveyMN_AlonePersonShouldGetAllCalls_AllCallsIsDistributed(DialType dialType)
        {
            var interviewIds = Enumerable.Range(1, 50);

            int surveySid2 = BackendToolsObject.CreateSurvey("p0998877");
            _surveyStateService.Open(surveySid2);

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews);
            var interviews2 = FusionLibTestTools.CreateInterviewsForTest(surveySid2, interviewIds, dialType).ToList();
            FusionLibTestTools.CreateCallsForTest(interviews2);

            int group1 = PersonTools.CreateAndAssignPersonGroupOnSurvey(_surveySid, "g1");
            BackendTools.AssignCatiPersonToSurvey(surveySid2, group1);

            int person1 = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u1", AgentTaskChoiceMode.Automatic, dialType);
            BackendTools.AssignCatiPersonToSurvey(surveySid2, person1);
            PersonService.SetParentGroups(person1, new[] { group1 });

            int person2 = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "u2", AgentTaskChoiceMode.Automatic, dialType);
            BackendTools.AssignCatiPersonToSurvey(surveySid2, person2);

            CallTools.AssignCalls(_surveySid, interviewIds.Take(5), person1);
            CallTools.AssignCalls(surveySid2, interviewIds.Take(15), person1);

            CallTools.AssignCalls(_surveySid, interviewIds.Skip(5).Take(5), group1);
            CallTools.AssignCalls(surveySid2, interviewIds.Skip(15).Take(5), group1);

            CallTools.AssignCalls(_surveySid, interviewIds.Skip(10).Take(15), person2);
            CallTools.AssignCalls(surveySid2, interviewIds.Skip(20).Take(1), person2);

            BackendTools.RunSchedulingProcedure();
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            var res = BvSpGetCachedCallsForPredictiveSurveyByPersonGroupAdapter.ExecuteEntityList(
                _surveySid,
                group1,
                3,
                currentTime,
                (int)dialType);

            var expected = GetBvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity(
                interviewIds.Skip(7).Take(3),
                group1,
                0);

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity);

            PredictiveTools.CheckUpdatingPhase(_surveySid, expected.Select(x => (int)x.InterviewID));
            res = BvSpGetCachedCallsForPredictiveSurveyByPersonGroupAdapter.ExecuteEntityList(
                _surveySid,
                _surveySid,
                300,
                currentTime, (int)dialType);

            expected = GetBvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity(
                interviewIds.Reverse().Take(25),
                _surveySid,
                0);

            TestAssert.AreEqual(expected.OrderBy(x => x.InterviewID),
                res.OrderBy(x => x.InterviewID),
                CompareBvSpGetCachedCallsForPredictiveSurveyByPersonGroupEntity);
        }
    }
}
