using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.CATIConsoleService
{
    [TestClass]
    public class GetOpenedSurveysTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void GetOpenedSurveys_CreateTwoSurveysWithDifferentValueOfIsRespondentsDynamicCreationAllowedProperty_CorrectValuesAreReturned()
        {
            const string project1 = "p87547584";
            const string project2 = "p948573342";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1, false, true);
            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Manual);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var ws = new CatiWsHelper(personName, personPassword);

            var surveys = ws.ConsoleService.GetOpenedSurveys();

            Assert.AreEqual(2, surveys.Length);

            Assert.AreEqual(project1, surveys[0].id);
            Assert.AreEqual("", surveys[0].name);
            Assert.AreEqual(true, surveys[0].IsRespondentsDynamicCreationAllowed);

            Assert.AreEqual(project2, surveys[1].id);
            Assert.AreEqual("", surveys[1].name);
            Assert.AreEqual(false, surveys[1].IsRespondentsDynamicCreationAllowed);

        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void GetOpenedSurveys_AssistedDialTypePerson_AutomaticSurveyIsNotReturned()
        {
            const string projectManual = "p10000001";
            const string projectAutomatic = "p10000002";
            const string projectPredictive = "p10000003";
            const string assistedPersonName = "assistedUser";
            const string assistedPersonPassword = "assistedUser";
            const string landlinePersonName = "landlineUser";
            const string landlinePersonPassword = "landlineUser";

            int surveyIdManual = BackendToolsObject.CreateSurvey(projectManual);
            int surveyIdAutomatic = BackendToolsObject.CreateSurvey(projectAutomatic);
            int surveyIdPredictive = BackendToolsObject.CreateSurvey(projectPredictive);

            var surveyManual = SurveyRepository.GetById(surveyIdManual);
            surveyManual.DialMode = (byte)DialingMode.Manual;
            SurveyRepository.Update(surveyManual);

            var surveyAutomatic = SurveyRepository.GetById(surveyIdAutomatic);
            surveyAutomatic.DialMode = (byte)DialingMode.Automatic;
            SurveyRepository.Update(surveyAutomatic);

            var surveyPredictive = SurveyRepository.GetById(surveyIdPredictive);
            surveyPredictive.DialMode = (byte)DialingMode.Predictive;
            SurveyRepository.Update(surveyPredictive);

            _surveyStateService.Open(surveyIdManual);
            _surveyStateService.Open(surveyIdAutomatic);
            _surveyStateService.Open(surveyIdPredictive);

            int assistedPersonId = PersonTools.CreatePerson(assistedPersonName, assistedPersonPassword, AgentTaskChoiceMode.Manual, DialType.Assisted);
            int landlinePersonId = PersonTools.CreatePerson(landlinePersonName, landlinePersonPassword, AgentTaskChoiceMode.Manual, DialType.Landline);

            BackendTools.AssignCatiPersonToSurvey(surveyIdManual, assistedPersonId);
            BackendTools.AssignCatiPersonToSurvey(surveyIdAutomatic, assistedPersonId);
            BackendTools.AssignCatiPersonToSurvey(surveyIdPredictive, assistedPersonId);

            BackendTools.AssignCatiPersonToSurvey(surveyIdManual, landlinePersonId);
            BackendTools.AssignCatiPersonToSurvey(surveyIdAutomatic, landlinePersonId);
            BackendTools.AssignCatiPersonToSurvey(surveyIdPredictive, landlinePersonId);

            var assistedWs = new CatiWsHelper(assistedPersonName, assistedPersonPassword);
            var assistedSurveys = assistedWs.ConsoleService.GetOpenedSurveys();

            Assert.AreEqual(2, assistedSurveys.Length);
            var assistedSurveyIds = assistedSurveys.Select(s => s.id).ToArray();
            CollectionAssert.Contains(assistedSurveyIds, projectManual);
            CollectionAssert.Contains(assistedSurveyIds, projectPredictive);
            CollectionAssert.DoesNotContain(assistedSurveyIds, projectAutomatic);

            var landlineWs = new CatiWsHelper(landlinePersonName, landlinePersonPassword);
            var landlineSurveys = landlineWs.ConsoleService.GetOpenedSurveys();

            Assert.AreEqual(3, landlineSurveys.Length);
            var landlineSurveyIds = landlineSurveys.Select(s => s.id).ToArray();
            CollectionAssert.Contains(landlineSurveyIds, projectManual);
            CollectionAssert.Contains(landlineSurveyIds, projectAutomatic);
            CollectionAssert.Contains(landlineSurveyIds, projectPredictive);
        }
    }
}
