using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Misc.CP.Fakes;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.SurveyTest
{
    [TestClass]
    public class UserSurveyListTest : BaseMockedIntegrationTest
    {
        public override void OnPostTestInitialize()
        {
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Insert_SingleSurvey_ListIsCorrect()
        {
            var context = new TestData()
            {
                Surveys = new []
                {
                    new SurveyData() {Tag = "S1", CallCenters=new[] {"CC1"}}
                },
                CallCenters = new []
                {
                    new CallCenterData(){Tag = "CC1"}
                },
                Supervisors = new[]
                {
                    new SupervisorData() { Tag = "SV1", CurrentCallCenter="CC1", Surveys=new[]{"S1"} }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var supervisor = context.GetSupervisor("SV1");

            TestingFramework.RegistryStub<ISupervisorNameProvider, StubISupervisorNameProvider>().NameGet =
                () => supervisor.Name;
            
            var repository = ServiceLocator.Resolve<IUserSurveyListRepository>();

            repository.Insert(UserSurveyListType.Recent, survey.Id);

            var actual = repository.GetList(UserSurveyListType.Recent).ToArray();

            Assert.AreEqual(1, actual.Length, "Wrong list size");
            Assert.AreEqual(survey.Id, actual[0].SID, "Wrong surveyId");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Insert_SingleSurveyWithoutSupervisorPermission_ListIsEmpty()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", CallCenters=new[] {"CC1"}}
                },
                CallCenters = new[]
                {
                    new CallCenterData(){Tag = "CC1"}
                },
                Supervisors = new[]
                {
                    new SupervisorData() { Tag = "SV1", CurrentCallCenter="CC1" }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var supervisor = context.GetSupervisor("SV1");

            TestingFramework.RegistryStub<ISupervisorNameProvider, StubISupervisorNameProvider>().NameGet =
                () => supervisor.Name;

            var repository = ServiceLocator.Resolve<IUserSurveyListRepository>();

            repository.Insert(UserSurveyListType.Recent, survey.Id);

            var actual = repository.GetList(UserSurveyListType.Recent).ToArray();

            Assert.AreEqual(0, actual.Length, "Wrong list size");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Insert_SingleSurveyWithoutCallCenterAssignment_ListIsEmpty()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1"}
                },
                CallCenters = new[]
                {
                    new CallCenterData() {Tag = "CC1"}
                },
                Supervisors = new[]
                {
                    new SupervisorData() {Tag = "SV1", CurrentCallCenter = "CC1", Surveys = new[] {"S1"}}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var supervisor = context.GetSupervisor("SV1");

            TestingFramework.RegistryStub<ISupervisorNameProvider, StubISupervisorNameProvider>().NameGet =
                () => supervisor.Name;

            var repository = ServiceLocator.Resolve<IUserSurveyListRepository>();

            repository.Insert(UserSurveyListType.Recent, survey.Id);

            var actual = repository.GetList(UserSurveyListType.Recent).ToArray();

            Assert.AreEqual(0, actual.Length, "Wrong list size");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Insert_TwoSurveysExist_ListIsReordered()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", CallCenters=new[] {"CC1"}},
                    new SurveyData() {Tag = "S2", CallCenters=new[] {"CC1"}}
                },
                CallCenters = new[]
                {
                    new CallCenterData() {Tag = "CC1"}
                },
                Supervisors = new[]
                {
                    new SupervisorData() {Tag = "SV1", CurrentCallCenter = "CC1", Surveys = new[] {"S1", "S2"}}
                }
            }.Create();

            var survey1 = context.GetSurvey("S1");
            var survey2 = context.GetSurvey("S2");
            var supervisor = context.GetSupervisor("SV1");

            TestingFramework.RegistryStub<ISupervisorNameProvider, StubISupervisorNameProvider>().NameGet =
                () => supervisor.Name;

            var repository = ServiceLocator.Resolve<IUserSurveyListRepository>();

            repository.Insert(UserSurveyListType.Recent, survey1.Id);
            repository.Insert(UserSurveyListType.Recent, survey2.Id);

            var actual = repository.GetList(UserSurveyListType.Recent).Select(x => x.SID).ToArray();
            var expected = context.GetSurveys("S2", "S1").Select(x => x.Id).ToArray();

            CollectionAssert.AreEqual(expected, actual);

            repository.Insert(UserSurveyListType.Recent, survey1.Id);

            actual = repository.GetList(UserSurveyListType.Recent).Select(x => x.SID).ToArray();
            expected = context.GetSurveys("S1", "S2").Select(x => x.Id).ToArray();

            CollectionAssert.AreEqual(expected, actual);

        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Insert_TwoSurveysExist_ListIsNotReorderedForFirstSupervisor()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() {Tag = "S1", CallCenters=new[] {"CC1"}},
                    new SurveyData() {Tag = "S2", CallCenters=new[] {"CC1"}}
                },
                CallCenters = new[]
                {
                    new CallCenterData() {Tag = "CC1"}
                },
                Supervisors = new[]
                {
                    new SupervisorData() {Tag = "SV1", CurrentCallCenter = "CC1", Surveys = new[] {"S1", "S2"}},
                    new SupervisorData() {Tag = "SV2", CurrentCallCenter = "CC1", Surveys = new[] {"S1", "S2"}}
                }
            }.Create();

            var survey1 = context.GetSurvey("S1");
            var survey2 = context.GetSurvey("S2");
            var supervisor1 = context.GetSupervisor("SV1");
            var supervisor2 = context.GetSupervisor("SV2");


            var supervisorNameProvider = TestingFramework.RegistryStub<ISupervisorNameProvider, StubISupervisorNameProvider>();
            supervisorNameProvider.NameGet = () => supervisor1.Name;

            var repository = ServiceLocator.Resolve<IUserSurveyListRepository>();

            repository.Insert(UserSurveyListType.Recent, survey1.Id);
            repository.Insert(UserSurveyListType.Recent, survey2.Id);

            var actual = repository.GetList(UserSurveyListType.Recent).Select(x => x.SID).ToArray();
            var expected = context.GetSurveys("S2", "S1").Select(x => x.Id).ToArray();

            CollectionAssert.AreEqual(expected, actual);


            supervisorNameProvider.NameGet = () => supervisor2.Name;

            repository.Insert(UserSurveyListType.Recent, survey1.Id);

            actual = repository.GetList(UserSurveyListType.Recent).Select(x => x.SID).ToArray();
            expected = context.GetSurveys("S1").Select(x => x.Id).ToArray();

            CollectionAssert.AreEqual(expected, actual);


            supervisorNameProvider.NameGet = () => supervisor1.Name;

            actual = repository.GetList(UserSurveyListType.Recent).Select(x => x.SID).ToArray();
            expected = context.GetSurveys("S2", "S1").Select(x => x.Id).ToArray();

            CollectionAssert.AreEqual(expected, actual);

        }

    }
}
