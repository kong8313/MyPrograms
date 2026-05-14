using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.RestApi
{
    [TestClass]
    public class ReviewerServiceTests : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            ServiceLocator.RegisterInstance<ITimeService>(new TestTimeService(new DateTime(2017, 1, 1)));
        }

        [TestMethod, Owner(@"FIRM\VictorR")]
        public void CreateSessionForReview_TwoInterviews_NewSessionWasCreated()
        {          
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                            new InterviewData() { Tag = "S1.I2", Call = new CallData() }
                        }
                    }
                }
            }.Create();

            var reviewerSettings = TestingFramework.RegistryStub<IReviewerSettings, StubIReviewerSettings>();
            reviewerSettings.SessionUrlTemplateGet = () => "www.reviewer.com/{0}";

            var stubIResponseReviewerApiClient = new StubIResponseReviewerApiClient
            {
                AddSessionSessionModel = 
                    sessionModel => Task.FromResult(new SessionModel
                        {
                            SessionId = 3,
                            ProjectId = "P123456",
                            Name = "New session",
                            CreatedByUser = "user",
                            CreatedDate = DateTime.Now
                        })
            };
            ServiceLocator.RegisterInstance<IResponseReviewerApiClient>(stubIResponseReviewerApiClient);
            
            var reviewerService = ServiceLocator.Resolve<IReviewerService>();
            var calls = context.GetCalls("S1.I1", "S1.I2");
            var batchParameters = new SelectedBatchParameters(calls.Select(x => x.Id));
            var surveyId = context.GetSurvey("S1").Id;

            // act
            var sessionUrl = reviewerService.CreateSessionForReview("Session name", surveyId, "Administrator", batchParameters);

            // assert
            Assert.AreEqual(string.Format(reviewerSettings.SessionUrlTemplateGet(), 3), sessionUrl);
        }

        [TestMethod]
        public void CreateSessionForReview_WithException_Should_Behave_Correctly()
        {
            // arrange
            TestingFramework.RegistryStub<IReviewerSettings, StubIReviewerSettings>();
            TestingFramework.RegistryStub<IResponseReviewerApiClient, StubIResponseReviewerApiClient>();
            var reviewerService = ServiceLocator.Resolve<IReviewerService>();
            var batchFactory = ServiceLocator.Resolve<IBatchFactory>();
            var batchParameters = new SelectedBatchParameters(new [] { 1, 2 });
            // act
            var currentBatchId = batchFactory.CreateDatabaseBatch(batchParameters).Id;
            try
            {
                reviewerService.CreateSessionForReview("Session name", int.MaxValue, "Administrator", batchParameters);
            }
            catch (Exception e)
            {
                Assert.IsNotNull(e);
            }
            // assert

            // Check batch id changed if an error has occured
            var batchIdAfterServiceExecution = BvTransferBatchesAdapter.GetByCondition(null).First().LastBatchID;
            Assert.AreEqual(currentBatchId, batchIdAfterServiceExecution - 1); // one increment was during CreateSessionForReview execution
            // cleanup was successful 
            Assert.AreEqual(0, BvTransferArraysAdapter.GetByCondition("BatchID = @BatchID", new SqlParameter("@BatchID", batchIdAfterServiceExecution)).Count);
        }
    }
}
