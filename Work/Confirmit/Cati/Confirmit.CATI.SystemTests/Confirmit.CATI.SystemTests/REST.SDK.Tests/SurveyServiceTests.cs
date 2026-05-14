using System;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;
using Confirmit.CATI.REST.SDK.Services;
using Confirmit.SystemTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.SystemTests.REST.SDK.Tests
{
    [TestClass]
    public class SurveyServiceTests : BaseSystemTests
    {
        private ISurveyService _surveyService;

        [TestInitialize]
        public void Initialize()
        {
            TestsGroupName = "Rest.Sdk";

            TestInitialize();

            _surveyService = new SurveyService(Confirmit.Cati.RestClient);
        }

        [TestMethod]
        public async Task GetSurveysList()
        {
            await _surveyService.GetAsync("");
        }

        [TestMethod]
        public async Task GetSurveysListWithOrderAndTop()
        {
            await _surveyService.GetAsync("?$orderby=SampleSize&$top=10");
        }

        [TestMethod]
        public async Task OpenCloseSurvey()
        {
            try
            {
                ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
                Confirmit.Surveys[ProjectId].Launch();

                var survey = await _surveyService.GetAsyncByKey(ProjectId);
                Assert.AreEqual(SurveyState.Closed, survey.State);

                await _surveyService.Open(ProjectId);

                survey = await _surveyService.GetAsyncByKey(ProjectId);
                Assert.AreEqual(SurveyState.Open, survey.State);

                await _surveyService.Close(ProjectId);

                survey = await _surveyService.GetAsyncByKey(ProjectId);
                Assert.AreEqual(SurveyState.Closed, survey.State);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                Cleanup();
            }
        }

        [TestMethod]
        public async Task OpenShutdownSurvey()
        {
            try
            {
                ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
                Confirmit.Surveys[ProjectId].Launch();

                var survey = await _surveyService.GetAsyncByKey(ProjectId);
                Assert.AreEqual(SurveyState.Closed, survey.State);

                await _surveyService.Open(ProjectId);

                var openedSurvey = await _surveyService.GetAsyncByKey(ProjectId);
                Assert.AreEqual(SurveyState.Open, openedSurvey.State);

                await _surveyService.Shutdown(ProjectId);

                survey = await _surveyService.GetAsyncByKey(ProjectId);
                Assert.AreEqual(SurveyState.Closed, survey.State);
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                Cleanup();
            }
        }

        [TestMethod]
        public async Task GetSurveyAssignments()
        {
            try
            {
                ProjectId = Confirmit.Surveys.ImportFromFile(PathToSurvey);
                Confirmit.Surveys[ProjectId].Launch();

                var surveys = await _surveyService.GetAsync("");
                int repeatCnt = Math.Min(50, surveys.Count);

                for (int i = 0; i < repeatCnt; i++)
                {
                    var assignments = await _surveyService.GetAssignments(surveys[i].SurveyId, Constants.DefaultCallCenterId);

                    Assert.IsNotNull(assignments);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail($"An exception occured. Message: {ex}\r\nStack trace: {ex.StackTrace}");
            }
            finally
            {
                Cleanup();
            }
        }
    }
}
