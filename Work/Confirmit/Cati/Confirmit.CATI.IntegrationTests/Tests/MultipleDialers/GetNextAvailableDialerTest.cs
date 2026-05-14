using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.MultipleDialers
{
    [TestClass]
    public class GetNextAvailableDialerTest : BaseMockedIntegrationTest
    {

        private IDialersRepository _dialersRepository;
        private ISurveyRepository _surveyRepository;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
        }


        [TestMethod, Owner(@"FIRM\Leonids")]
        public void GetNextAvailableDialer_TwoConnectedAndActiveDialers_DialersSelectedInRoundRobin()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", 
                }},
                Dialers = new[]
                {
                    new DialerData { Tag = "D1" },
                    new DialerData { Tag = "D2" },
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;

            var id = _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);
            BvSurveyDialerAdapter.DeleteByCondition("1 = 1");
            var id1 = _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);
            BvSurveyDialerAdapter.DeleteByCondition("1 = 1");
            var id2 = _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);
            BvSurveyDialerAdapter.DeleteByCondition("1 = 1");
            var id3 = _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);

            Assert.IsTrue(id == id2, "id not equal to id2");
            Assert.IsTrue(id1 == id3, "id1 not equal to id3");
            Assert.IsTrue(id != id1, "id should not be equal to id1");
        }

        [TestMethod, Owner(@"FIRM\Leonids")]
        public void GetNextAvailableDialer_TwoDialers_OneActiveAndConnected_OtherConnecteNonActive_SurveyAssignedToCorrectDialer()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", 
                }},
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", IsActive = false},
                    new DialerData { Tag = "D2" },
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;
            _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);

            BvSurveyCache.Instance.OnTableChanged();
            Assert.AreEqual(context.GetDialer("D2").Id, BvSurveyDialerAdapter.GetAll().Single(x => x.SurveyId == surveyId && x.DialTypeId == (int)DialType.Landline).DialerId);

        }

        [TestMethod, Owner(@"FIRM\Leonids")]
        public void GetNextAvailableDialer_TwoDialers_OneActiveAndConnected_OtherActiveAndNotConnected_SurveyAssignedToCorrectDialer()
        {
            var context = new TestData
            {
                Surveys = new[]{
                new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", 
                },
                new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S2", 
                }},
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", IsConnected = false },
                    new DialerData { Tag = "D2" },
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;
            _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);

            BvSurveyCache.Instance.OnTableChanged();
            var survey1DialerId = BvSurveyDialerAdapter.GetAll().Single(x => x.SurveyId == surveyId && x.DialTypeId == (int) DialType.Landline).DialerId;
            Assert.AreEqual(context.GetDialer("D2").Id, survey1DialerId);
            var survey2Dialer = BvSurveyDialerAdapter.GetAll().SingleOrDefault(x => x.SurveyId == context.GetSurvey("S2").Id && x.DialTypeId == (int)DialType.Landline);
            Assert.IsNull(survey2Dialer, "DialerId should be null");
        }

        [TestMethod, Owner(@"FIRM\Leonids")]
        public void GetNextAvailableDialer_TwoNotActiveDialers_NoDialerReturned()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", 
                }},
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", IsActive = false},
                    new DialerData { Tag = "D2", IsActive = false},
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;

            var id = _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);

            Assert.IsNull(id);
        }

        [TestMethod, Owner(@"FIRM\Leonids")]
        public void GetNextAvailableDialer_TwoNotConnectedDialers_NoDialerReturned()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", 
                }},
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", IsConnected = false },
                    new DialerData { Tag = "D2", IsConnected = false },
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;

            var id = _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);

            Assert.IsNull(id);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetNextAvailableDialer_TwoDialersWithDifferentTypes_BvSurveyDialerstableAreCorrect()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1",
                }},
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", DialType = DialType.Landline},
                    new DialerData { Tag = "D2", DialType = DialType.Cellphone },
                }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;

            var id = _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Landline);

            Assert.AreEqual(context.GetDialer("D1").Id, id, "Incorrect dialer id");

            id = _dialersRepository.GetNextAvailableDialer(surveyId, DialType.Cellphone);

            Assert.AreEqual(context.GetDialer("D2").Id, id, "Incorrect dialer id");

            var surveyDialers = BvSurveyDialerAdapter.GetAll();
            var automaticSurveyDialer = surveyDialers.Single(x => x.DialTypeId == (int) DialType.Landline);
            var manualSurveyDialer = surveyDialers.Single(x => x.DialTypeId == (int)DialType.Cellphone);

            Assert.AreEqual(context.GetDialer("D1").Id, automaticSurveyDialer.DialerId, "Incorrect dialer id");
            Assert.AreEqual(context.GetDialer("D2").Id, manualSurveyDialer.DialerId, "Incorrect dialer id");
        }


        [TestMethod, Owner(@"FIRM\Leonids")]
        public void GetNextAvailableDialer_TwoConnectedDialers_OnlySecondIsActive_InterLoginToSecondDialer()
        {
            var context = new TestData
            {
                Surveys = new[]{new SurveyData
                {
                    IsOpen = true,DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData {Tag="S1.I1", Call = new CallData ()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", IsActive = false},
                    new DialerData { Tag = "D2" },
                }
            }.Create();

            var person = context.GetPerson("P1");
            var survey = context.GetSurvey("S1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            var task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual(context.GetDialer("D2").Id, task.DialerId, "Dialer Id is not correct.");
        }
    }
}
