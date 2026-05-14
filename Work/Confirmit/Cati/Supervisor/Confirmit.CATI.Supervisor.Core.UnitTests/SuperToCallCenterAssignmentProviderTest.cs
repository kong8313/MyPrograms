using System.Linq;
using System.Collections.Generic;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.CallCenters;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class SuperToCallCenterAssignmentProviderTest
    {
        class CallCenterRepository : ICallCenterRepository
        {
            private readonly List<BvCallCenterEntity> _bvCallCenterEntities = new List<BvCallCenterEntity>
                                                                                  {
                                                                                      new BvCallCenterEntity
                                                                                          {
                                                                                              ID = 1,
                                                                                              Name = "Default",
                                                                                              CanBeDeleted = false
                                                                                          },
                                                                                      new BvCallCenterEntity
                                                                                          {
                                                                                              ID = 2,
                                                                                              Name = "Second",
                                                                                              CanBeDeleted = true
                                                                                          }
                                                                                  };

            public BvCallCenterEntity Get(int id)
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntity Default
            {
                get
                {
                    return _bvCallCenterEntities[0];   
                }
            }

            public List<BvCallCenterEntity> GetAssignedToSurvey(int surveyId)
            {
                throw new System.NotImplementedException();
            }

            public List<BvCallCenterEntity> GetAll()
            {
                return _bvCallCenterEntities;
            }

            public void Insert(BvCallCenterEntity entity)
            {
                throw new System.NotImplementedException();
            }

            public void Update(BvCallCenterEntity entity)
            {
                throw new System.NotImplementedException();
            }

            public void Delete(int id, int moveToCallCenterId, InterviewerActionOnCallCenterDelete interviewerAction)
            {
                throw new System.NotImplementedException();
            }

            public BvCallCenterEntityWithDialerIds GetCallCenterWithDialers(int id)
            {
                throw new System.NotImplementedException();
            }

            public void Insert(BvCallCenterEntityWithDialerIds entity)
            {
                throw new System.NotImplementedException();
            }

            public void Update(BvCallCenterEntityWithDialerIds entity, int[] newDialerIds, int[] oldDialerIds)
            {
                throw new System.NotImplementedException();
            }

            public List<BvCallCenterEntityWithDialerIds> GetAllWithDialerIds()
            {
                throw new System.NotImplementedException();
            }
        }

        class ConfrimitSupersProvider : ICachedConfirmitSupervisorProvider
        {
            public IEnumerable<CatiSupervisor> GetConfirmitCatiSupervisors()
            {
                return new[]
                           {
                               new CatiSupervisor {Login = "super1", FirstName = string.Empty, LastName = string.Empty, FullName = string.Empty},
                               new CatiSupervisor {Login = "super2", FirstName = string.Empty, LastName = string.Empty, FullName = string.Empty},
                               new CatiSupervisor {Login = "super3", FirstName = string.Empty, LastName = string.Empty, FullName = string.Empty}
                           };
            }

            public void ClearCache()
            {
                throw new System.NotImplementedException();
            }
        }

        class CatiAssignmentsProvider : ICallCenterService
        {
            private IEnumerable<BvSupervisorAssignmentEntity> _assignments = new List<BvSupervisorAssignmentEntity>();

            public CatiAssignmentsProvider() { }

            public CatiAssignmentsProvider(IEnumerable<BvSupervisorAssignmentEntity> assignments)
            {
                _assignments = assignments;
            }

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
                throw new System.NotImplementedException();
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
                return _assignments;
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

        private ICallCenterRepository _callCenterRepository;
        private ICachedConfirmitSupervisorProvider _superProvider;
        private ICallCenterService _emptyCatiAssignments;
        private ICallCenterService _filledCatiAssignments;

        [TestInitialize]
        public void TestInitialize()
        {
            _callCenterRepository = new CallCenterRepository();
            _superProvider = new ConfrimitSupersProvider();
            _emptyCatiAssignments = new CatiAssignmentsProvider();
            _filledCatiAssignments = new CatiAssignmentsProvider(new[]
                                                                     {
                                                                         new BvSupervisorAssignmentEntity
                                                                             {
                                                                                 CallCenterId = 2,
                                                                                 Name = "super1"
                                                                             },
                                                                         new BvSupervisorAssignmentEntity
                                                                             {
                                                                                 CallCenterId = 2,
                                                                                 Name = "super2"
                                                                             },
                                                                         new BvSupervisorAssignmentEntity
                                                                             {
                                                                                 CallCenterId = 2,
                                                                                 Name = "super3"
                                                                             }
                                                                     });
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetAllAssignments_NoRecordsInCatiAssignmentTable_SupersAreAssignedToDefaultCallCenter()
        {
            var provider = new SuperToCallCenterAssignmentProvider(
                _callCenterRepository, _emptyCatiAssignments, _superProvider);

            var result = provider.GetAllAssignments();

            var expected = new[]
                               {
                                   new SupervisorToCallCenterAssignment("super1", string.Empty, 1, "Default"),
                                   new SupervisorToCallCenterAssignment("super2", string.Empty, 1, "Default"),
                                   new SupervisorToCallCenterAssignment("super3", string.Empty, 1, "Default")
                               };

            CollectionAssert.AreEquivalent(expected, result.ToArray());
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetAllAssignments_ThereAreDataInCatiAssignmentTable_AssignmentsFromCatiTableAreReturned()
        {
            var provider = new SuperToCallCenterAssignmentProvider(
                _callCenterRepository, _filledCatiAssignments, _superProvider);

            var result = provider.GetAllAssignments();

            var expected = new[]
                               {
                                   new SupervisorToCallCenterAssignment("super1", string.Empty, 2, "Second"),
                                   new SupervisorToCallCenterAssignment("super2", string.Empty, 2, "Second"),
                                   new SupervisorToCallCenterAssignment("super3", string.Empty, 2, "Second")
                               };

            CollectionAssert.AreEquivalent(expected, result.ToArray());
        }
    }
}
