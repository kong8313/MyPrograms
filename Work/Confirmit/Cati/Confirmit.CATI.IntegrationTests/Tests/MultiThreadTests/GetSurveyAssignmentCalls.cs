using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.MultiThreadTests
{
    [TestClass]
    public class GetSurveyAssignmentCalls
    {
        private const string SurveyName = "p000001";
        private const int CreatedCallsCount = 2;

        readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools = new BackendTools(IntegrationTestingFramework.Instance);

        private int _surveySid;
        private int _personSid;
        private List<BvInterviewEntity> _interviews;

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();

            _backendTools.LaunchAllHoursScript();
            _surveySid = _backendTools.CreateSurvey(SurveyName);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _surveyStateService.Open(_surveySid);

            _interviews = new List<BvInterviewEntity>(CreatedCallsCount);
            for (int i = 0; i < CreatedCallsCount; i++)
            {
                var interview = BackendTools.NewInterview(_surveySid);
                BackendTools.CreateInterview(interview);
                _interviews.Add(interview);

                var call = BackendTools.NewCall(interview);
                BackendTools.CreateCall(call);
            }

            _personSid = PersonTools.CreateAssignAndLoginPersonOnSurvey(_surveySid, "userName", AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SimultaneouslyCall_SurveyAssignment()
        {
            const int threadCount = 2;
            Test(threadCount);
        }

        private void Test(int threadCount)
        {
            var totalCalls = new BlockingCollection<int>();
            Exception exception = null;

            Parallel.For(0, threadCount, (x) =>
            {
                exception = GetAndSaveCallsReturnedForSurveyAssignment(totalCalls);
            });

            if (exception != null)
                Assert.Fail(exception.ToString());

            Assert.AreEqual(2, totalCalls.Count);
        }

        private Exception GetAndSaveCallsReturnedForSurveyAssignment(BlockingCollection<int> storage)
        {
            Exception result = null;
            try
            {
                var call = BvSpLookUpByPerson_ForAssignmentModeAdapter.ExecuteEntity(_surveySid, _personSid, DateTime.UtcNow);
                storage.Add(call.InterviewId.Value);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            return result;
        }
    }
}
