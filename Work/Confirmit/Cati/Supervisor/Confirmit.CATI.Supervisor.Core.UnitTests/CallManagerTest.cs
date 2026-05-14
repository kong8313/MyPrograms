using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Framework.Fakes;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Misc.CP.Fakes;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.UnitTests;
using Confirmit.CATI.IntegrationTests.Framework.ServiceLocatorRegistry;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.CallCenters.Fakes;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class CallManagerTest : BaseTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();

            /*var sl = new ServiceLocator();
            sl.Cleanup();
            sl.Initialize();*/
            ServiceLocator.RegisterSingleton<ICallCenterProvider, TestCallCenterProvider>();
        }

        private static void MockAsyncOperations()
        {
            var survey = new BvSurveyEntity() { Name = "p00001", Description = "Test Survey" };

            ISurveyRepository surveyRepositoryStub = new StubISurveyRepository 
            {
                Inner = ServiceLocator.Resolve<ISurveyRepository>(),
                GetByIdInt32 = sid => survey
            };
            ServiceLocator.RegisterInstance(surveyRepositoryStub);

            // Mock CallManager.StartAsyncOperation
            ServiceLocator.RegisterInstance<ISupervisorNameProvider>(new StubISupervisorNameProvider());
            ServiceLocator.RegisterInstance<ICallCenterProvider>(new StubICallCenterProvider());
            ServiceLocator.RegisterInstance<IAsyncOperationQueue>(new StubIAsyncOperationQueue());
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void MoveCalls_NoSurvey_Exception()
        {
            CallManager.MoveCalls(0, 1, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void MoveCalls_WrongITSID_Exception()
        {
            CallManager.MoveCalls(52, 0, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteCalls_UninitializedCallList_ExceptionThrows()
        {
            CallManager.DeleteCalls(1, null);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void DeleteCalls_SurveyIdIsInvalid_ExceptionThrows()
        {
            CallManager.DeleteCalls(0, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void MoveAndRescheduleCalls_SurveyIdIsInvalid_ExceptionThrows()
        {
            CallManager.MoveAndRescheduleCalls(0, 1, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void MoveAndRescheduleCalls_ItsIdIsInvalid_ExceptionThrows()
        {
            CallManager.MoveAndRescheduleCalls(1, 0, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void MoveAndRescheduleCalls_FilteredInterviews_Success()
        {
            MockAsyncOperations();
            CallManager.MoveAndRescheduleCalls(1, 1, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignCalls_SurveyIdIsInvalid_ExceptionThrows()
        {
            CallManager.AssignCalls(0, new[] { 1 }, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignCalls_PersonOrGroupIdIsInvalid_ExceptionThrows()
        {
            CallManager.AssignCalls(1, new int[] { }, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignCalls_Interviews_Success()
        {
            MockAsyncOperations();

            CallManager.AssignCalls(1, new[] { 1 }, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeCallsShiftType_SurveyIdIsInvalid_ExceptionThrows()
        {
            CallManager.ChangeShiftTypeOfCalls(0, 1, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeCallsShiftType_ShiftTypeIdIsInvalid_ExceptionThrows()
        {
            CallManager.ChangeShiftTypeOfCalls(1, -2, new SelectedBatchParameters(new[] { 1 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ChangeCallsShiftType_FilteredInterviews_Success()
        {

            MockAsyncOperations();

            CallManager.ChangeShiftTypeOfCalls(1, (int)CallShiftType.None, new FilteredBatchParameters(1, 1, 1, CallStates.Scheduled, null));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ChangeCallsShiftType_ListOfInterviews_Success()
        {
            int[] callIds = new int[] { 1 };

            MockAsyncOperations();

            CallManager.ChangeShiftTypeOfCalls(1, (int)CallShiftType.None, new SelectedBatchParameters(callIds));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void ChangeCallsPriority_SurveyIdIsInvalidForFiltered_ExceptionThrows()
        {
            CallManager.ChangeCallsPriority(0, 1, new SelectedBatchParameters(new[] { 0 }));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ChangeCallsPriority_BatchIsNull_ExceptionThrows()
        {
            CallManager.ChangeCallsPriority(1, 1, null);
        }

        /*
		[TestMethod, Owner(@"FIRM\SergeyC")]
		public void ChangeCallsPriority_ListOfInterviews_Success()
		{
			int[] callIds = new int[] { 1 };

			using (RecordExpectations recorder = new RecordExpectations())
			{
				Survey survey = new Survey(1);
				recorder.CheckArguments();
				survey.Obj.ChangeCallsPriority(callIds, (int)E_GENERATE_MODE.ACTIVECALLID, 0);
				recorder.CheckArguments(
					Check.CustomChecker(
						new ParameterCheckerEx(TestUtility.CheckFusionCompatibleArray),
						callIds
					));

				survey.Dispose();
			}

			CallManager.ChangeCallsPriority(1, callIds, CallStates.Active, 0);

			MockManager.Verify();
		}

		#endregion

		#region AddCall()

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AddCall_CallIsNull_ExceptionThrows()
		{
			CallManager.AddCall(null);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void AddCall_SurveyIdIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
				recorder.ExpectAndReturn(call.SurveySID, -1);
				recorder.RepeatAlways();
				recorder.ExpectAndReturn(call.InterviewID, 1);
				recorder.ExpectAndReturn(call.CallState, 1);
				recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
			}

			CallManager.AddCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
		public void AddCall_InterviewIdIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
				recorder.ExpectAndReturn(call.SurveySID, 1);
                recorder.ExpectAndReturn(call.InterviewID, -1);
				recorder.RepeatAlways();
				recorder.ExpectAndReturn(call.CallState, 1);
				recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
			}

			CallManager.AddCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void AddCall_PhaseIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1);
                recorder.ExpectAndReturn(call.SurveySID, 1);
				recorder.ExpectAndReturn(call.CallState, 0);
				recorder.RepeatAlways();
                recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
			}

			CallManager.AddCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void AddCall_RoleIdIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1);
                recorder.ExpectAndReturn(call.SurveySID, 1);
				recorder.ExpectAndReturn(call.CallState, 1);
                recorder.ExpectAndReturn(call.RoleID, 0);
				recorder.RepeatAlways();
				recorder.ExpectAndReturn(call.Priority, 1);
			}

			CallManager.AddCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void AddCall_PriorityIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1);
                recorder.ExpectAndReturn(call.SurveySID, 1);
				recorder.ExpectAndReturn(call.CallState, 1);
                recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 0);
				recorder.RepeatAlways();
			}

			CallManager.AddCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		public void AddCall_CorrectCall_Success()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);

			using (RecordExpectations recorder = new RecordExpectations())
			{
				recorder.ExpectAndReturn(call.SurveySID, 1).Repeat(2);
                recorder.ExpectAndReturn(call.InterviewID, 1).Repeat(2);
				recorder.ExpectAndReturn(call.CallState, 1);
				recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
			    recorder.ExpectAndReturn(CallManager.IsInterviewExists(1, 1), true);

                CallQueueService service = new CallQueueService();
                service.AddCall(call, 0);
                recorder.CheckArguments();
			}

			CallManager.AddCall(call);
		}

		#endregion

		#region UpdateCall()

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void UpdateCall_CallIsNull_ExceptionThrows()
		{
			CallManager.UpdateCall(null);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateCall_SurveyIdIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, -1);
				recorder.RepeatAlways();
                recorder.ExpectAndReturn(call.InterviewID, 1);
				recorder.ExpectAndReturn(call.CallID, 1);
				recorder.ExpectAndReturn(call.CallState, 1);
                recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
			}

			CallManager.UpdateCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateCall_InterviewIdIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1);
                recorder.ExpectAndReturn(call.InterviewID, -1);
				recorder.RepeatAlways();
				recorder.ExpectAndReturn(call.CallID, 1);
				recorder.ExpectAndReturn(call.CallState, 1);
                recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
			}

			CallManager.UpdateCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateCall_CallIdIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1).Repeat(2);
                recorder.ExpectAndReturn(call.SurveySID, 1).Repeat(2);
				recorder.ExpectAndReturn(call.CallID, -1);
				recorder.RepeatAlways();
				recorder.ExpectAndReturn(call.CallState, 1);
                recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
                recorder.ExpectAndReturn(CallManager.IsInterviewExists(1, 1), true);
			}

			CallManager.UpdateCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateCall_PhaseIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1);
                recorder.ExpectAndReturn(call.SurveySID, 1);
				recorder.ExpectAndReturn(call.CallID, 1);
				recorder.ExpectAndReturn(call.CallState, 0);
				recorder.RepeatAlways();
                recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
			}

			CallManager.UpdateCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateCall_RoleIdIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1);
                recorder.ExpectAndReturn(call.SurveySID, 1);
				recorder.ExpectAndReturn(call.CallID, 1);
				recorder.ExpectAndReturn(call.CallState, 1);
                recorder.ExpectAndReturn(call.RoleID, 0);
				recorder.RepeatAlways();
				recorder.ExpectAndReturn(call.Priority, 1);
			}

			CallManager.UpdateCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateCall_PriorityIsInvalid_ExceptionThrows()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);
			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1);
                recorder.ExpectAndReturn(call.SurveySID, 1);
				recorder.ExpectAndReturn(call.CallID, 1);
				recorder.ExpectAndReturn(call.CallState, 1);
				recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 0);
				recorder.RepeatAlways();
			}

			CallManager.UpdateCall(call);
		}

		[TestMethod, Owner(@"FIRM\SergeyC")]
		public void UpdateCall_CorrectCall_Success()
		{
            BvCallEntity call = RecorderManager.CreateMockedObject<BvCallEntity>(Constructor.Mocked);

			using (RecordExpectations recorder = new RecordExpectations())
			{
                recorder.ExpectAndReturn(call.SurveySID, 1).Repeat(2);
                recorder.ExpectAndReturn(call.InterviewID, 1).Repeat(2);
				recorder.ExpectAndReturn(call.CallID, 1);
				recorder.ExpectAndReturn(call.CallState, 1);
				recorder.ExpectAndReturn(call.RoleID, 1);
				recorder.ExpectAndReturn(call.Priority, 1);
                recorder.ExpectAndReturn(CallManager.IsInterviewExists(1, 1), true);

                CallQueueService service = new CallQueueService();
                service.UpdateCall(call, 0);
                recorder.CheckArguments();
			}

			CallManager.UpdateCall(call);
		}

		#endregion*/
    }
}
