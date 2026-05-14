using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class CallManagermentChangeDialModeTests
    {
        private const int Count = 4;
        const string ProjectId = "p004466";
        int _surveySid, _localTimezoneId;
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private readonly BvInterviewEntity[] _interviews = new BvInterviewEntity[Count];

        

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _surveySid = _backendTools.CreateSurvey(ProjectId);            
            _localTimezoneId = ServiceLocator.Resolve<ICallCenterRepository>().Default.LocalTimezoneId;                        
        }

        [TestCleanup]
        public void Cleanup()
        {            
            _framework.TestCleanup();            
        }
        
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDialingMode_SetSelected_ModeIsChangedCorrectly()
        {
            PrepareDataForSetDialingMode();

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid, 
                                                                                new SelectedBatchParameters(_interviews.Where(item => item.ID <= 2).Select(item => item.ID)), 
                                                                                DialingMode.Preview);
            
            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[0].ID).DialingMode);
            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[1].ID).DialingMode);
            Assert.AreEqual(0, InterviewRepository.GetById(_surveySid, _interviews[2].ID).DialingMode);
            Assert.AreEqual(0, InterviewRepository.GetById(_surveySid, _interviews[3].ID).DialingMode);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDialingMode_SetSelected_SetDialingModeEventIsLogged()
        {
            const ManagementEvent expectedEventType = ManagementEvent.ChangeDialModeOfSelectedInterviews;
            const string expectedEventName = "ChangeDialModeOfSelectedInterviewsEvent";
            
            PrepareDataForSetDialingMode();
            TestAssert.ManagementActivityEventDoesntExist(expectedEventType, expectedEventName, _surveySid);            

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                new SelectedBatchParameters(_interviews.Where(item => item.ID <= 2).Select(item => item.ID)),
                                                                                DialingMode.Preview);
            

            TestAssert.ManagementActivityEventExists(expectedEventType, expectedEventName, _surveySid);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDialingMode_ResetSelected_ModeIsReset()
        {
            PrepareDataForSetDialingMode(DialingMode.Preview);
            
            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                new SelectedBatchParameters(_interviews.Where(item => item.ID > 2).Select(item => item.ID)),
                                                                                null);
            
            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[0].ID).DialingMode);
            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[1].ID).DialingMode);
            Assert.AreEqual(0, InterviewRepository.GetById(_surveySid, _interviews[2].ID).DialingMode);
            Assert.AreEqual(0, InterviewRepository.GetById(_surveySid, _interviews[3].ID).DialingMode);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDialingMode_ResetSelected_ResetDialingModeEventIsLogged()
        {
            const ManagementEvent expectedEventType = ManagementEvent.ChangeDialModeOfSelectedInterviews;
            const string expectedEventName = "ChangeDialModeOfSelectedInterviewsEvent";

            PrepareDataForSetDialingMode();
            TestAssert.ManagementActivityEventDoesntExist(expectedEventType, expectedEventName, _surveySid);

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                new SelectedBatchParameters(_interviews.Select(item => item.ID)),
                                                                                DialingMode.Preview);
            

            TestAssert.ManagementActivityEventExists(expectedEventType, expectedEventName, _surveySid);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDialingMode_SetFiltered_ModeIsChangedCorrectly()
        {
            PrepareDataForSetDialingMode();

            var batchParameters = new FilteredBatchParameters(_surveySid,0, _localTimezoneId, CallStates.All, 
                                                              new SearchParameterCollection
                                                                {
                                                                    new SearchParameter
                                                                        {
                                                                            ColumnName = "InterviewID",
                                                                            ColumnType = SearchColumnType.Number,
                                                                            Operator = SearchOperator.LessThanOrEqual,
                                                                            Value = 3
                                                                        }});

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                batchParameters,
                                                                                DialingMode.Preview);

            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[0].ID).DialingMode);
            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[1].ID).DialingMode);
            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[2].ID).DialingMode);
            Assert.AreEqual(0, InterviewRepository.GetById(_surveySid, _interviews[3].ID).DialingMode);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDialingMode_SetFiltered_SetFilteredDialingModeEventIsLogged()
        {
            const ManagementEvent expectedEventType = ManagementEvent.ChangeDialModeOfFilteredInterviews;
            const string expectedEventName = "ChangeDialModeOfFilteredInterviewsEvent";

            PrepareDataForSetDialingMode();
            TestAssert.ManagementActivityEventDoesntExist(expectedEventType, expectedEventName, _surveySid);

            var batchParameters = new FilteredBatchParameters(_surveySid, 0, _localTimezoneId, CallStates.All, new SearchParameterCollection());

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                batchParameters,
                                                                                DialingMode.Preview);

            TestAssert.ManagementActivityEventExists(expectedEventType, expectedEventName, _surveySid);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDialingMode_ResetFiltered_ModeIsReset()
        {
            PrepareDataForSetDialingMode(DialingMode.Preview);

            var batchParameters = new FilteredBatchParameters(_surveySid, 0, _localTimezoneId, CallStates.All,
                                                              new SearchParameterCollection
                                                                {
                                                                   new SearchParameter
                                                                    {
                                                                        ColumnName = "InterviewID",
                                                                        ColumnType = SearchColumnType.Number,
                                                                        Operator = SearchOperator.Greater,
                                                                        Value = 3
                                                                    }});

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                batchParameters,
                                                                                null);

            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[0].ID).DialingMode);
            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[1].ID).DialingMode);
            Assert.AreEqual((int)DialingMode.Preview, InterviewRepository.GetById(_surveySid, _interviews[2].ID).DialingMode);
            Assert.AreEqual(0, InterviewRepository.GetById(_surveySid, _interviews[3].ID).DialingMode);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void SetDialingMode_ResetFiltered_ResetFilteredDialingModeEventIsLogged()
        {
            const ManagementEvent expectedEventType = ManagementEvent.ChangeDialModeOfFilteredInterviews;
            const string expectedEventName = "ChangeDialModeOfFilteredInterviewsEvent";

            PrepareDataForSetDialingMode();
            TestAssert.ManagementActivityEventDoesntExist(expectedEventType, expectedEventName, _surveySid);

            var batchParameters = new FilteredBatchParameters(_surveySid, 0, _localTimezoneId, CallStates.All, new SearchParameterCollection());

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                batchParameters,
                                                                                null);

            TestAssert.ManagementActivityEventExists(expectedEventType, expectedEventName, _surveySid);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void SetDialingMode_SetAutomaticMode_NoCallsInBvSvySchedule_BvCallHistoryHasRecords_CallValuesAreNull()
        {

            PrepareDataForSetDialingMode();

            var batchParameters = new FilteredBatchParameters(_surveySid, 0, _localTimezoneId, CallStates.All, new SearchParameterCollection());

            var result = new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                batchParameters,
                                                                                DialingMode.Automatic);

            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId", new SqlParameter("@SurveyId", _surveySid));

            var first = history.First();
            Assert.AreEqual(Count, history.Count);
            Assert.AreEqual(result.Id, first.OperationId);
            Assert.AreEqual((int)OperationType.ChangeDiallingMode, (int)first.OperationType);
            Assert.AreEqual((int) CallOutcome.FreshSample, (int) first.ITS);
            Assert.AreEqual(0, first.CallCenterId);
            Assert.AreEqual((byte) DialingMode.Automatic, first.DialingMode );
            Assert.IsNull(first.CallState);
            Assert.IsNull(first.TimeInShift);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void SetDialingMode_SetAutomaticMode_CallsInBvSvySchedule_BvCallHistoryHasRecordsWithCallValues()
        {

            PrepareDataForSetDialingMode();

            var batchParameters = new FilteredBatchParameters(_surveySid, 0, _localTimezoneId, CallStates.All, new SearchParameterCollection());

            CallTools.ActivateCalls(_surveySid, 5, CallStates.All, new int[] {}, (int) CallShiftType.None, DateTime.UtcNow, false, new int[] {1,2,3,4});

            var result = new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(_surveySid,
                                                                                batchParameters,
                                                                                DialingMode.Automatic);

            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId ORDER BY ID", new SqlParameter("@SurveyId", _surveySid));

            var first = history.Last();
            Assert.AreEqual(Count*2, history.Count);
            Assert.AreEqual(result.Id, first.OperationId);
            Assert.AreEqual((int)OperationType.ChangeDiallingMode, (int)first.OperationType);
            Assert.AreEqual((int)CallOutcome.FreshSample, (int)first.ITS);
            Assert.AreEqual(0, first.CallCenterId);
            Assert.AreEqual((byte)DialingMode.Automatic, first.DialingMode);
            Assert.IsNotNull(first.CallState);
            Assert.IsNotNull(first.TimeInShift);
            Assert.AreEqual(5, first.Priority);
        }

        private void PrepareDataForSetDialingMode(DialingMode? initialDialingMode = null)
        {
            _framework.ClearConfirmlogDatabase();

            SurveyService.SetDialingMode(_surveySid, DialingMode.Predictive);

            for (int i = 0; i < Count; i++)
            {
                int id = i + 1;
                _interviews[i] = new BvInterviewEntity { ID = id, SurveySID = _surveySid };
                if (initialDialingMode.HasValue)
                {
                    _interviews[i].DialingMode = (byte)initialDialingMode;
                }

                _interviews[i].TransientState = 16;
                BackendTools.CreateInterview(_interviews[i]);
            }
        }


    }
}
