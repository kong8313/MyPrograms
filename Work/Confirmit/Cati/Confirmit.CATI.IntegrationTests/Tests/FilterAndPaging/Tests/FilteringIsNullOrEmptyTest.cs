using System;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using ConfirmitDialerInterface;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class FilteringIsNullOrEmptyTest
    {
        private const string _interName = "inter1";

        
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private int _timezoneId;
        private BackendTools _backendTools;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _backendTools.LaunchAllHoursScript();
            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();

            _framework.SetTestHttpContextCurrentWithSupervisorPrincipal();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.ClearTestHttpContextCurrent();

            _framework.TestCleanup();
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.FilteringIsNullOrEmpty)]
        public void FilterForEmptyResource_TwoOfThreeRecordsReturned()
        {
            int totalCount;

            var context = CreateSurveyWithCallsAndReplicateData();

            var searchArgs = SearchTools.SearchBy("Resource", SearchColumnType.Text, SearchOperator.IsNullOrEmpty, String.Empty);

            var actualRecordSet = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(2, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.FilteringIsNullOrEmpty)]
        public void FilterForEmptyBackgroundVariables_TwoOfThreeRecordsReturned()
        {
            int totalCount;

            var context = CreateSurveyWithCallsAndReplicateData();

            var searchArgs = SearchTools.SearchBy("Varq1", SearchColumnType.Text, SearchOperator.IsNullOrEmpty, string.Empty);

            var actualRecordSet = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false, "q1");

            Assert.AreEqual(1, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.FilteringIsNullOrEmpty)]
        public void FilterForEmptyTelephoneNumber_TwoOfThreeRecordsReturned()
        {
            int totalCount;

            var context = CreateSurveyWithCallsAndReplicateData();
            var surveyId = context.GetSurvey("S1").Id;

            new DatabaseEngine().ExecuteNonQuery(@"UPDATE BvInterview SET TelephoneNumber = null WHERE SurveySID = @SurveyId AND ID = @ID", CommandType.Text,
                    new[] {new SqlParameter("@SurveyId", surveyId), new SqlParameter("@ID",context.GetInterview("S1.I1").Id), });

            new DatabaseEngine().ExecuteNonQuery(@"UPDATE BvInterview SET TelephoneNumber = '' WHERE SurveySID = @SurveyId AND ID = @ID", CommandType.Text,
                new[] { new SqlParameter("@SurveyId", surveyId), new SqlParameter("@ID", context.GetInterview("S1.I2").Id), });

            var searchArgs = SearchTools.SearchBy("TelephoneNumber", SearchColumnType.Text, SearchOperator.IsNullOrEmpty, string.Empty);

            var actualRecordSet = CallHelper.GetCallsPage(surveyId, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(2, actualRecordSet.Rows.Count);
        }


        private TestDataContext CreateSurveyWithCallsAndReplicateData()
        {
            var context = new TestData
            {
                Surveys = new[]
                { 
                    new SurveyData 
                    { Tag="S1", IsUseDb = true,
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}, SqlType = SqlDataType.Char, TableName = "respondent"},
                        },
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Data="q1=1", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2, Resource = "P1"}},
                            new InterviewData(){Tag="S1.I2", Data="q1=2", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                            new InterviewData(){Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData(){CallState = 2}},
                        }
                   }
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1", Name = _interName}
                }
            }.Create();

            RereadAllSurveyReplicatedData(context.GetSurvey("S1").Id);

            return context;
        }

        private void RereadAllSurveyReplicatedData(int surveyId)
        {
            var param = new Core.AsyncOperations.Operations.RereadSurveyReplicatedData.Parameters()
            {
                SurveyId = surveyId
            };
            var operationEntity = ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                0,
                $"Reread Survey Replicated Data for survey {surveyId}",
                false,
                param,
                AsyncOperationConstants.HighPriority,
                "");

            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operationEntity);
        }


    }
}
