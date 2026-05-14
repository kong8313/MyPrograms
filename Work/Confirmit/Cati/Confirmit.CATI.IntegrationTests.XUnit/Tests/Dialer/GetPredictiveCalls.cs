using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Survey;
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
    public class GetPredictiveCalls: BaseMockedIntegrationTest
    {
        private const string SurveyName = "p000001";
        private const int CreatedCallsCount = 100;
        private const int CallsCountPerCall = 10;

        private readonly int _surveySid;
        private int _personSid;
        private List<BvInterviewEntity> _interviews;

        public GetPredictiveCalls()
        {
            BackendToolsObject.LaunchAllHoursScript();
            _surveySid = BackendToolsObject.CreateSurvey(SurveyName);
            
            var surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            surveyStateService.Open(_surveySid);
        }
        
        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void SimultaneouslyCall_CallsAssignedToCampaignOnly(DialType dialType)
        {
            InitPersonAndInterviews(dialType);
            const int threadCount = 2;
            Test(threadCount, CallsSelectionAlgorithm.CallsAssignedToCampaignOnly, dialType);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void SimultaneouslyCall_ByPersonGroup(DialType dialType)
        {
            InitPersonAndInterviews(dialType);
            AssignAllCallsToPerson();
            const int threadCount = 2;
            Test(threadCount, CallsSelectionAlgorithm.ByPersonGroup, dialType);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void SimultaneouslyCall_CallsAssignedToAgentsExplicitly(DialType dialType)
        {
            InitPersonAndInterviews(dialType);
            AssignAllCallsToPerson();
            const int threadCount = 2;
            Test(threadCount, CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly, dialType);
        }

        [Theory, Owner(@"FIRM\AlexanderL")]
        [ClassData(typeof(PredictiveDialTypes))]
        public void SimultaneouslyCall_ByCampaign(DialType dialType)
        {
            InitPersonAndInterviews(dialType);
            const int threadCount = 3;
            Test(threadCount, CallsSelectionAlgorithm.ByCampaign, dialType);
        }

        private void AssignAllCallsToPerson()
        {
            foreach (var interview in _interviews)
                BackendTools.AssignResourceToInterview(_surveySid, interview.ID, _personSid);

            BackendTools.RunSchedulingProcedure();
        }

        private void Test(int threadCount, CallsSelectionAlgorithm algorithm, DialType dialType)
        {
            var totalCalls = new BlockingCollection<PredictiveCall>();
            Exception exception = null;

            Parallel.For(0, threadCount, x =>
            {
                exception = GetAndSaveCallsReturnedForPredictive(totalCalls, algorithm, dialType);
            });

            if (exception != null)
                Assert.Fail(exception.ToString());

            Assert.AreEqual(Math.Min(CreatedCallsCount, CallsCountPerCall*threadCount), totalCalls.Count, String.Join(",", totalCalls.Select(x => x.ID)));
            Assert.IsFalse(totalCalls.GroupBy(x => x.ID).Any(x => x.Count() > 1), String.Join(",", totalCalls.Select(x => x.ID)));
        }
        
        private void InitPersonAndInterviews(DialType dialType)
        {
            _interviews = new List<BvInterviewEntity>(CreatedCallsCount);
            for (int i = 0; i < CreatedCallsCount; i++)
            {
                var interview = BackendTools.NewInterview(_surveySid, dialType);
                BackendTools.CreateInterview(interview);
                _interviews.Add(interview);

                var call = BackendTools.NewCall(interview);
                BackendTools.CreateCall(call);
            }

            _personSid = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "userName", AgentTaskChoiceMode.CampaignAssignment, dialType);
        }

        private Exception GetAndSaveCallsReturnedForPredictive(BlockingCollection<PredictiveCall> storage, CallsSelectionAlgorithm algorithm, DialType dialType)
        {
            Exception result = null;
            try
            {
                var calls = PredictiveTools.GetCallsForPredictive(_surveySid, _personSid, algorithm, CallsCountPerCall, dialType);
                foreach (var call in calls)
                    storage.Add(call);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        }
    }
}
