using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class AssignCallsAsyncOperationTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignCalls_AssignSelectedCalls_OperationCompleted()
        {
            const string surveyName = "p000001";
            const int its = 13;

            // Create survey
            int surveySID = BackendToolsObject.CreateSurvey(surveyName);

            // Open survey
            _surveyStateService.Open(surveySID);

            var interview1 = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview1);

            var interview2 = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview2);

            var operationResult = CallTools.MoveCalls(surveySID, new[] { interview1.ID, interview2.ID }, its);

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operationResult.Id);
            Assert.IsNotNull(operationEntity, "BvAsyncOperations records wasn't created");
            Assert.AreEqual((int)AsyncOperationState.Completed, operationEntity.State);

            interview1 = InterviewRepository.GetById(surveySID, interview1.ID);
            Assert.AreEqual(its, interview1.TransientState);

            interview2 = InterviewRepository.GetById(surveySID, interview2.ID);
            Assert.AreEqual(its, interview2.TransientState);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignCalls_AssignFilteredCalls_OperationCompleted()
        {
            const string surveyName = "p000001";
            const int its = 13;

            // Create survey
            int surveySID = BackendToolsObject.CreateSurvey(surveyName);

            // Open survey
            _surveyStateService.Open(surveySID);

            var interview1 = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview1);
            var oldInterview1Its = interview1.TransientState;

            var interview2 = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview2);

            var interview3 = BackendTools.NewInterview(surveySID);
            BackendTools.CreateInterview(interview3);

            var operationResult = CallTools.MoveCalls(
                surveySID, 0, CallStates.All, its, 1,
                new SearchParameterCollection
                    {
                        new SearchParameter
                            {
                                ColumnName = "InterviewID",
                                ColumnType = SearchColumnType.Number,
                                Operator = SearchOperator.Greater,
                                Value = interview1.ID
                            }
                    });

            Assert.AreEqual(AsyncOperationState.Completed, operationResult.State, "Operation failed");

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operationResult.Id);
            Assert.IsNotNull(operationEntity, "BvAsyncOperations records wasn't created");

            interview1 = InterviewRepository.GetById(surveySID, interview1.ID);
            Assert.AreEqual(oldInterview1Its, interview1.TransientState, "First interview its shouldn't be changed");

            interview2 = InterviewRepository.GetById(surveySID, interview2.ID);
            Assert.AreEqual(its, interview2.TransientState);

            interview3 = InterviewRepository.GetById(surveySID, interview3.ID);
            Assert.AreEqual(its, interview3.TransientState);
        }

        [TestMethod]
        public void AssignCalls_ActivateOnNotExistingMultipleAssginment_ActivationSuccessed()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1", Call = new CallData() }}
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData {Tag = "PG1", Name = "PersonGroup1"},
                    new PersonGroupData {Tag = "PG2", Name = "PersonGroup2"},
                    new PersonGroupData {Tag = "PG3", Name = "PersonGroup3"}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            var group1 = context.GetResource("PG1");
            var group2 = context.GetResource("PG2");

            CallTools.AssignCalls(survey.Id,new[] { interview.Id }, new [] {group1.Id, group2.Id});

            var assignment = BvAssignmentResourceAdapter.GetAll().FirstOrDefault(a => a.Name == "PersonGroup1,PersonGroup2" && a.Qualifier == group1.Id + "," + group2.Id);
            Assert.IsNotNull(assignment, "Multuple assginment is not created.");

            context.GetCall("S1.I1").Assert.IsTrue(c => c.Resource == assignment.ID);
        }
    }
}
