using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.Survey.Fakes;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.UnitTests
{
    [TestClass]
    public class PredictiveHelperTest
    {
        private IServiceRegistrator _serviceRegistrator;

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _serviceRegistrator = UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
        }

        private void PrepareAndMock(List<int> personSids, bool onlySurveySelectionInters, bool noPredictiveSurveys)
        {
            BvPersonEntity person1 = new BvPersonEntity();
            BvPersonEntity person2 = new BvPersonEntity();
            BvPersonEntity person3 = new BvPersonEntity();

            person1.SID = personSids[0];
            person2.SID = personSids[1];
            person3.SID = personSids[2];

            person1.Name = "I1";
            person2.Name = "I2";
            person3.Name = "I3";

            if (onlySurveySelectionInters)
            {
                person1.ManualSelection = (int)AgentTaskChoiceMode.CampaignAssignment;
                person2.ManualSelection = (int)AgentTaskChoiceMode.CampaignAssignment;
            }
            else
            {
                person1.ManualSelection = (int)AgentTaskChoiceMode.Automatic;
                person2.ManualSelection = (int)AgentTaskChoiceMode.Manual;
            }

            person3.ManualSelection = (int)AgentTaskChoiceMode.CampaignAssignment;

            BvSurveyEntity survey1 = new BvSurveyEntity();
            BvSurveyEntity survey2 = new BvSurveyEntity();
            BvSurveyEntity survey3 = new BvSurveyEntity();
            BvSurveyEntity survey4 = new BvSurveyEntity();

            survey1.Name = "p0000001";
            survey2.Name = "p0000002";
            survey3.Name = "p0000003";
            survey4.Name = "p0000004";

            survey1.Description = "name1";
            survey2.Description = "name2";
            survey3.Description = "name3";
            survey4.Description = "name4";

            survey1.DialMode = (byte)DialingMode.Automatic;
            survey2.DialMode = (byte)DialingMode.Manual;
            survey3.DialMode = (byte)DialingMode.Predictive;
            survey4.DialMode = (byte)DialingMode.Preview;

            var personRepository = new StubIPersonRepository();
            var surveyRepository = new StubISurveyRepository();
            var surveyService = new StubISurveyService();
            _serviceRegistrator.RegisterInstance<IPersonRepository>(personRepository);
            _serviceRegistrator.RegisterInstance<ISurveyRepository>(surveyRepository);
            _serviceRegistrator.RegisterInstance<ISurveyService>(surveyService);

            var persons = new Queue<BvPersonEntity>();

            persons.Enqueue(person1);
            persons.Enqueue(person2);
            persons.Enqueue(person3);
            personRepository.GetByIdInt32 = sid => persons.Dequeue();
            surveyService.GetDialingModeInt32 = sid =>
            {
                if (sid == 3 && !noPredictiveSurveys) return DialingMode.Predictive;
                return DialingMode.Automatic;
            };
            surveyRepository.GetByIdInt32 = sid =>
            {
                if (sid == 1) return survey1;
                if (sid == 2) return survey2;
                if (!noPredictiveSurveys)
                {
                    if (sid == 3) return survey3;
                    if (sid == 9) return survey4;
                }
                else
                {
                    if (sid == 3) return survey4;
                }

                return null;
            };
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetPredictiveSurveyAssignmentWarning_AllTypesOfSurveysAndInters_GetWarningMessage()
        {
            List<int> surveySids = new List<int> { 1, 2, 3, 9 };
            List<int> personSids = new List<int> { 4, 5, 6 };

            PrepareAndMock(personSids, false, false);

            string warning = new PredictiveHelper().GetPredictiveSurveyAssignmentWarning(surveySids, personSids);
            Assert.IsNotNull(warning);

            string expectedString = BaseForm.GetResString(
                        "WrongTaskChoiceDuringAssignmentOnPredictiveSurvey",
                        "name3 (p0000003)",
                        "I1, I2");
            Assert.AreEqual(expectedString, warning);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetPredictiveSurveyAssignmentWarning_OnlySurveySelectionInters_GetNull()
        {
            List<int> surveySids = new List<int> { 1, 2, 3, 9 };
            List<int> personSids = new List<int> { 4, 5, 6 };

            PrepareAndMock(personSids, true, false);

            string warning = new PredictiveHelper().GetPredictiveSurveyAssignmentWarning(surveySids, personSids);
            Assert.IsNull(warning);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetPredictiveSurveyAssignmentWarning_NoPredictiveSurveys_GetNull()
        {
            List<int> surveySids = new List<int> { 1, 2, 3 };
            List<int> personSids = new List<int> { 4, 5, 6 };

            PrepareAndMock(personSids, false, true);

            string warning = new PredictiveHelper().GetPredictiveSurveyAssignmentWarning(surveySids, personSids);
            Assert.IsNull(warning);
        }
    }
}