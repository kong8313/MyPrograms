using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class CallChangingPriorityTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            _backendTools.LaunchAllHoursScript();

            _surveySid = _backendTools.CreateSurvey(ProjectId);
            _surveyStateService.Open(_surveySid);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        

        private const string ProjectId = "p004466";
        private const short NewPriority = 134;
        private int _surveySid;

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ChangeCallsPriority_CallsExist_PriorityChanged()
        {
            var interviewIds = new[] { 1, 2, 3 };
            var interviewWithCallIds = new[] { 2, 3 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviewWithCallIds.Select(x => interviews[x-1])).ToList();

            CallTools.ChangeCallsPriority(_surveySid, new[] { calls[0].InterviewID }, CallStates.Scheduled, NewPriority);
            calls[0].Priority = NewPriority;

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls.Select(x => x.Priority), BvSvyScheduleAdapter.GetAll().OrderBy(x => x.ID).Select(x => x.Priority));

            CallTools.ChangeCallsPriority(_surveySid, new[] { calls[0].InterviewID, calls[1].InterviewID }, CallStates.Scheduled, NewPriority + 1);

            calls[0].Priority = NewPriority+1;
            calls[1].Priority = NewPriority + 1;

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls.Select(x => x.Priority), BvSvyScheduleAdapter.GetAll().OrderBy(x => x.ID).Select(x => x.Priority));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ChangeCallsPriority_CallsExistAndCustomFiltered_PriorityChanged()
        {
            var interviewIds = new[] { 1, 2 };

            int filterSid = FusionLibTestTools.CreateFilterForTest("ID", FilterOperator.Less, "2");

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();
            BackendTools.RunSchedulingProcedure();

            CallTools.ChangeCallsPriority(_surveySid, filterSid, CallStates.Scheduled, NewPriority);
            calls[0].Priority = NewPriority;

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls.Select(x => x.Priority), BvSvyScheduleAdapter.GetAll().OrderBy(x => x.ID).Select(x => x.Priority));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ChangeCallsPriority_CallsExistAndDefaultFiltered_PriorityChanged()
        {
            var interviewIds = new[] { 1, 2 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();
            BackendTools.RunSchedulingProcedure();

            CallTools.ChangeCallsPriority(_surveySid, 0, CallStates.Scheduled, NewPriority);

            TestAssert.AreEqual(
                calls.Select(x => { x.Priority = NewPriority; return x; }), 
                interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ChangeCallsPriority_CallsHaveNegativePhase_PrioritysAreNotChanged()
        {
            var interviewIds = new[] { 1, 2 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            calls[0].CallState = (int)PhaseState.ProcessedCall;
            CallQueueService.UpdateCall(calls[0], 0);
            calls[1].CallState = (int)PhaseState.PreparedForPredictiveCall;
            CallQueueService.UpdateCall(calls[1], 0);

            CallTools.ChangeCallsPriority(_surveySid, 0, CallStates.Scheduled, NewPriority + 1);

            TestAssert.AreEqual(
                calls.Select(x => x.Priority),
                BvSvyScheduleAdapter.GetAll().OrderBy(x => x.InterviewID).Select(x => x.Priority));

            CallTools.ChangeCallsPriority(_surveySid, 0, CallStates.Scheduled, NewPriority);

            TestAssert.AreEqual(calls, interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)));

            calls[0].CallState = (int)PhaseState.DefaultState;
            CallQueueService.UpdateCall(calls[0], 0);
            calls[1].CallState = (int)PhaseState.DefaultState;
            CallQueueService.UpdateCall(calls[1], 0);
            BackendTools.RunSchedulingProcedure();
        }
    }
}
