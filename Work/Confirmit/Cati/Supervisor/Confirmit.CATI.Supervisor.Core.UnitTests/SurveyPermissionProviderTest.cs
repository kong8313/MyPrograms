using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class SurveyPermissionProviderTest
    {
        private ISurveyPermissionProvider _provider;
        private CallCenterServiceTest _callCenterServiceTest;
        private CallCenterProviderTest _callCenterProviderTest;

        class UserSurveyPermissionRepositoryTest : IUserSurveyPermissionRepository
        {
            public List<BvSpUserSurveyPermission_GetEntity> GetListByUserName(string userName)
            {
                return new List<BvSpUserSurveyPermission_GetEntity>
                           {
                               new BvSpUserSurveyPermission_GetEntity {SurveySID = 1},
                               new BvSpUserSurveyPermission_GetEntity {SurveySID = 2},
                               new BvSpUserSurveyPermission_GetEntity {SurveySID = 3}
                           };
            }

            public void Insert(string userName, string projectId)
            {
                throw new System.NotImplementedException();
            }

            public void Delete(string userName, string projectId)
            {
                throw new System.NotImplementedException();
            }

            public void Delete(string userName)
            {
                throw new System.NotImplementedException();
            }

            public void DeleteAllForSpecificSurvey(int surveyId)
            {
                throw new System.NotImplementedException();
            }
        }

        class CallCenterServiceTest : ICallCenterService
        {
            public int GivenCallCenterId = -1;

            public void AssignSurvey(int callCenterId, int surveyId)
            {
                throw new System.NotImplementedException();
            }

            public bool DeassignSurvey(int callCenterId, int surveyId)
            {
                throw new System.NotImplementedException();
            }

            public void AssignSurveys(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<BvSurveyAssignmentOnCallCenterEntity> ReassignSurveys(IEnumerable<int> callCenterIds, IEnumerable<int> surveyIds)
            {
                throw new System.NotImplementedException();
            }

            public void AssignSupervisors(int callCenterId, params string[] name)
            {
                throw new System.NotImplementedException();
            }

            public void DeassignSupervisor(int callCenterId, string name)
            {
                throw new System.NotImplementedException();
            }

            public void ClearSupervisorAssignment(string name)
            {
                throw new System.NotImplementedException();
            }

            public List<int> GetSurveyAssignments(int callCenterId)
            {
                GivenCallCenterId = callCenterId;

                return new List<int> {2, 3, 4, 5};
            }

            public IEnumerable<BvSurveyAssignmentOnCallCenterEntity> GetAssignmentsBySurvey(int surveyId)
            {
                throw new System.NotImplementedException();
            }

            public List<string> GetSupervisorAssignments(int callCenterId)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<BvSupervisorAssignmentEntity> GetAllSupervisorCallCenterAssignments()
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntity GetSupervisorCallCenter(string superName)
            {
                throw new System.NotImplementedException();
            }

            public bool IsNameAlreadyInUse(string callCenterName)
            {
                throw new System.NotImplementedException();
            }

            public bool HasLoggedInPersons(int callCenterId, int? surveyId)
            {
                throw new System.NotImplementedException();
            }

            public bool IsNeedToHidePii()
            {
                throw new System.NotImplementedException();
            }
        }

        class CallCenterProviderTest : ICallCenterProvider
        {
            public readonly int CallCenterId;

            public CallCenterProviderTest(int callCenterId)
            {
                CallCenterId = callCenterId;
            }

            public int GetCurrentId()
            {
                return CallCenterId;
            }

            public BvCallCenterEntity GetCurrent()
            {
                throw new System.NotImplementedException();
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _callCenterServiceTest = new CallCenterServiceTest();
            _callCenterProviderTest = new CallCenterProviderTest(10);

            _provider = new SurveyPermissionProvider(
                new UserSurveyPermissionRepositoryTest(), 
                _callCenterServiceTest,
                _callCenterProviderTest);
        }

        [TestMethod, Owner(@"FIRM/SergeyC")]
        public void GetUserSurveyPermission_PermittedSurveysAndAssignments_AssignmentsAreTakenForCorrectCallCenter()
        {
            _provider.GetUserSurveyPermission(string.Empty);

            Assert.AreEqual(_callCenterProviderTest.CallCenterId, _callCenterServiceTest.GivenCallCenterId, "Survey assignments were taken for incorrect call center");
        }

        [TestMethod, Owner(@"FIRM/SergeyC")]
        public void GetUserSurveyPermission_PermittedSurveysAndAssignments_IntersectionIsReturned()
        {
            var expected = new[] {2, 3};

            var actual = _provider.GetUserSurveyPermission(string.Empty);

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
