using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Framework.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.CallCenter
{
    [TestClass]
    public class CallCenterServiceTest
    {
        const string Supervisor = "supervisor";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        private ICallCenterRepository _repository;
        private ICallCenterService _service;
        private BvCallCenterEntity _callCenter1;
        private BvCallCenterEntity _callCenter2;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _repository = ServiceLocator.Resolve<ICallCenterRepository>();
            _service = ServiceLocator.Resolve<ICallCenterService>();

            _callCenter1 = new BvCallCenterEntity { Name = "callCenter2", Description = "Description 1", LocalTimezoneId = 1 };
            _repository.Insert(_callCenter1);

            _callCenter2 = new BvCallCenterEntity { Name = "callCenter2", Description = "Description 2", LocalTimezoneId = 1 };
            _repository.Insert(_callCenter2);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetSurveyAssignments_NoSurveys_ResultIsEmpty()
        {
            var actual = _service.GetSurveyAssignments(_callCenter1.ID).Count;
            Assert.AreEqual(0, actual);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetSurveyAssignments_SurveyWithoutAssignemnt_ResultIsEmpty()
        {
            _backendTools.CreateSurvey("p10000001");

            var actual = _service.GetSurveyAssignments(_callCenter1.ID).Count;
            Assert.AreEqual(0, actual);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetSurveyAssignments_SurveyWithAssignemnt_ResultIsCorrect()
        {
            var surveyId = _backendTools.CreateSurvey("p10000001");

            _service.AssignSurvey(_callCenter1.ID, surveyId);

            var actual = _service.GetSurveyAssignments(_callCenter1.ID).ToArray();

            CollectionAssert.AreEqual(new[] { surveyId }, actual);

            var count = _service.GetSurveyAssignments(_callCenter2.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AssignSurvey_SurveyWithoutAssignemnt_AssignmentIsCreated()
        {
            var surveyId = _backendTools.CreateSurvey("p10000001");

            _service.AssignSurvey(_callCenter1.ID, surveyId);

            var actual = _service.GetSurveyAssignments(_callCenter1.ID).ToArray();

            CollectionAssert.AreEqual(new[] { surveyId }, actual);

            var count = _service.GetSurveyAssignments(_callCenter2.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AssignSurvey_SurveyWithAssignemnt_AssignmentIsNotModified()
        {
            var surveyId = _backendTools.CreateSurvey("p10000001");

            _service.AssignSurvey(_callCenter1.ID, surveyId);
            _service.AssignSurvey(_callCenter1.ID, surveyId);

            var actual = _service.GetSurveyAssignments(_callCenter1.ID).ToArray();

            CollectionAssert.AreEqual(new[] { surveyId }, actual);

            var count = _service.GetSurveyAssignments(_callCenter2.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DeassignSurvey_SurveyWithAssignemnt_AssignmentIsDeleted()
        {
            var surveyId = _backendTools.CreateSurvey("p10000001");

            _service.AssignSurvey(_callCenter1.ID, surveyId);

            var count = _service.GetSurveyAssignments(_callCenter1.ID).Count;
            Assert.AreEqual(1, count);

            _service.DeassignSurvey(_callCenter1.ID, surveyId);

            count = _service.GetSurveyAssignments(_callCenter1.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DeassignSurvey_SurveyWithPersonAssignemnt_AssignmentIsDeleted()
        {
            var surveyId = _backendTools.CreateSurvey("p10000001");
            var cs = new CallCenterWrapper(_callCenter1, _backendTools);
            var person = cs.CreatePerson("p1");

            _service.AssignSurvey(_callCenter1.ID, surveyId);
            cs.AssignResourceToSurvey(surveyId, person.Entity.SID);

            Assert.AreEqual(1, BvPersonOrGroupAssignmentOnSurveyAdapter.GetAll().Count(x => x.PersonOrGroupId == person.Entity.SID));
            Assert.AreEqual(1, BvPersonRelAdapter.GetAll().Count(x => x.PersonSID == person.Entity.SID && x.Type == 2));

            _service.DeassignSurvey(_callCenter1.ID, surveyId);

            Assert.AreEqual(0, BvPersonOrGroupAssignmentOnSurveyAdapter.GetAll().Count(x => x.PersonOrGroupId == person.Entity.SID));
            Assert.AreEqual(0, BvPersonRelAdapter.GetAll().Count(x => x.PersonSID == person.Entity.SID && x.Type == 2));
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DeassignSurvey_SurveyWithoutAssignemnt_AssignmentIsDeleted()
        {
            var surveyId = _backendTools.CreateSurvey("p10000001");

            _service.DeassignSurvey(_callCenter1.ID, surveyId);

            var count = _service.GetSurveyAssignments(_callCenter1.ID).Count;
            Assert.AreEqual(0, count);

            count = _service.GetSurveyAssignments(_callCenter2.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AssignSupervisor_SupervisorWithoutAssignemnt_AssignmentIsCreated()
        {
            _service.AssignSupervisors(_callCenter1.ID, Supervisor);

            var actual = _service.GetSupervisorAssignments(_callCenter1.ID).ToArray();

            CollectionAssert.AreEqual(new[] { Supervisor }, actual);

            var count = _service.GetSupervisorAssignments(_callCenter2.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AssignSupervisor_SupervisorWithAssignemntToOtherCallCenter_AssignmentIsModified()
        {
            _service.AssignSupervisors(_callCenter1.ID, Supervisor);
            _service.AssignSupervisors(_callCenter2.ID, Supervisor);

            var actual = _service.GetSupervisorAssignments(_callCenter2.ID).ToArray();

            CollectionAssert.AreEqual(new[] { Supervisor }, actual);

            var count = _service.GetSupervisorAssignments(_callCenter1.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DeassignSupervisor_SupervisorWithAssignemnt_AssignmentIsDeleted()
        {
            _service.AssignSupervisors(_callCenter1.ID, Supervisor);
            _service.DeassignSupervisor(_callCenter1.ID, Supervisor);

            var count = _service.GetSupervisorAssignments(_callCenter1.ID).Count;

            Assert.AreEqual(0, count);

            count = _service.GetSupervisorAssignments(_callCenter2.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DeassignSupervisor_SupervisorWithAssignemntToOtherCallCenter_AssignmentIsNotDeleted()
        {
            _service.AssignSupervisors(_callCenter1.ID, Supervisor);
            _service.DeassignSupervisor(_callCenter2.ID, Supervisor);

            var actual = _service.GetSupervisorAssignments(_callCenter1.ID).ToArray();

            CollectionAssert.AreEqual(new[] { Supervisor }, actual);

            var count = _service.GetSupervisorAssignments(_callCenter2.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DeassignSupervisor_SupervisorWithoutAssignemnt_AssignmentIsDeleted()
        {
            _service.DeassignSupervisor(_callCenter2.ID, Supervisor);

            var count = _service.GetSupervisorAssignments(_callCenter1.ID).Count;

            Assert.AreEqual(0, count);

            count = _service.GetSupervisorAssignments(_callCenter2.ID).Count;
            Assert.AreEqual(0, count);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignSurveys_SeveralCallCentersAndSeveralSurveys_AssignmentsAreCreated()
        {
            int survey1Id = _backendTools.CreateSurvey("p000001");
            int survey2Id = _backendTools.CreateSurvey("p000002");

            BvSurveyAssignmentOnCallCenterAdapter.DeleteByCondition("1=1");

            var expectedAssignments = new[]
                                          {
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey1Id },
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey2Id },
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey1Id },
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey2Id }
                                          };

            _service.AssignSurveys(new[] { _callCenter1.ID, _callCenter2.ID }, new[] { survey1Id, survey2Id });

            AssertEquivalentAllSurveyAssignments(expectedAssignments);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignSurveys_AddingNewAssignWithExistingAssign_ExistingAssignAreNotChanged()
        {
            int survey1Id = _backendTools.CreateSurvey("p000001");
            int survey2Id = _backendTools.CreateSurvey("p000002");

            BvSurveyAssignmentOnCallCenterAdapter.DeleteByCondition("1=1");

            BvSurveyAssignmentOnCallCenterAdapter.Insert(new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey1Id });

            var expectedAssignments = new[]
                                          {
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey1Id },
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey2Id }
                                          };

            _service.AssignSurveys(new[] { _callCenter2.ID }, new[] { survey2Id });

            AssertEquivalentAllSurveyAssignments(expectedAssignments);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignSurveys_AddingOnlyExistingAssign_ExistingAssignAreNotDuplicated()
        {
            int survey1Id = _backendTools.CreateSurvey("p000001");
            int survey2Id = _backendTools.CreateSurvey("p000002");

            BvSurveyAssignmentOnCallCenterAdapter.DeleteByCondition("1=1");

            BvSurveyAssignmentOnCallCenterAdapter.Insert(new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey1Id });
            BvSurveyAssignmentOnCallCenterAdapter.Insert(new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey2Id });

            var expectedAssignments = new[]
                                          {
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey1Id },
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey2Id }
                                          };

            _service.AssignSurveys(new[] { _callCenter2.ID }, new[] { survey2Id });

            AssertEquivalentAllSurveyAssignments(expectedAssignments);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ReassignSurveys_WithExistingSurveyAssignments_ExistingSurveyAssignmentsAreRemoved()
        {
            int survey1Id = _backendTools.CreateSurvey("p000001");
            int survey2Id = _backendTools.CreateSurvey("p000002");

            BvSurveyAssignmentOnCallCenterAdapter.Insert(new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey1Id });
            BvSurveyAssignmentOnCallCenterAdapter.Insert(new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey2Id });

            var expectedAssignments = new[]
                                          {
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey1Id },
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey2Id }
                                          };

            _service.ReassignSurveys(new[] { _callCenter1.ID }, new[] { survey1Id, survey2Id });

            AssertEquivalentAllSurveyAssignments(expectedAssignments);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ReassignSurveys_WithExistingSurveyAssignments_OnlyForGivenSurveysExistingAssignmentsAreRemoved()
        {
            int survey1Id = _backendTools.CreateSurvey("p000001");
            int survey2Id = _backendTools.CreateSurvey("p000002");

            BvSurveyAssignmentOnCallCenterAdapter.DeleteByCondition("1=1");

            BvSurveyAssignmentOnCallCenterAdapter.Insert(new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey1Id });
            BvSurveyAssignmentOnCallCenterAdapter.Insert(new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey2Id });
            BvSurveyAssignmentOnCallCenterAdapter.Insert(new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey2Id });

            var expectedAssignments = new[]
                                          {
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter2.ID, SurveyId = survey1Id },
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey2Id }
                                          };

            _service.ReassignSurveys(new[] { _callCenter1.ID }, new[] { survey2Id });

            AssertEquivalentAllSurveyAssignments(expectedAssignments);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void ReassignSurveys_NoExistingSurveyAssignments_AssignmentsAreCreated()
        {
            int survey1Id = _backendTools.CreateSurvey("p000001");
            int survey2Id = _backendTools.CreateSurvey("p000002");

            var expectedAssignments = new[]
                                          {
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey1Id },
                                              new BvSurveyAssignmentOnCallCenterEntity { CallCenterId = _callCenter1.ID, SurveyId = survey2Id }
                                          };

            _service.ReassignSurveys(new[] { _callCenter1.ID }, new[] { survey1Id, survey2Id });

            AssertEquivalentAllSurveyAssignments(expectedAssignments);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(ArgumentException))]
        public void AssignSupervisors_CallCenterDoesNotExist_ExceptionIsThrown()
        {
            _service.AssignSupervisors(0, new[] {"super"});
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignSupervisors_NoExistingAssignments_SupervisorsAreAssigned()
        {
            const string super1 = "super1";
            const string super2 = "super2";
            const string super3 = "super3";

            var expectedAssignments = new[]
                                          {
                                              new BvSupervisorAssignmentEntity{CallCenterId = _callCenter1.ID, Name = super1},
                                              new BvSupervisorAssignmentEntity{CallCenterId = _callCenter1.ID, Name = super2},
                                              new BvSupervisorAssignmentEntity{CallCenterId = _callCenter1.ID, Name = super3}
                                          };

            _service.AssignSupervisors(_callCenter1.ID, new[] { super1, super2, super3 });

            AssertEquivalentAllSuperAssignments(expectedAssignments);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignSupervisors_WithExistingAssignments_SupervisorsAssignmentIsChanged()
        {
            const string super1 = "super1";

            BvSupervisorAssignmentAdapter.Insert(new BvSupervisorAssignmentEntity { CallCenterId = _callCenter1.ID, Name = super1 });

            var expectedAssignments = new[]
                                          {
                                              new BvSupervisorAssignmentEntity{CallCenterId = _callCenter2.ID, Name = super1}
                                          };

            _service.AssignSupervisors(_callCenter2.ID, new[] { super1 });

            AssertEquivalentAllSuperAssignments(expectedAssignments);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void AssignSupervisors_WithExistingAssignments_OnlyGivenSupervisorAssignmentIsChanged()
        {
            const string super1 = "super1";
            const string super2 = "super2";

            BvSupervisorAssignmentAdapter.Insert(new BvSupervisorAssignmentEntity { CallCenterId = _callCenter1.ID, Name = super1 });
            BvSupervisorAssignmentAdapter.Insert(new BvSupervisorAssignmentEntity { CallCenterId = _callCenter1.ID, Name = super2 });

            var expectedAssignments = new[]
                                          {
                                              new BvSupervisorAssignmentEntity{CallCenterId = _callCenter2.ID, Name = super1},
                                              new BvSupervisorAssignmentEntity{CallCenterId = _callCenter1.ID, Name = super2}
                                          };

            _service.AssignSupervisors(_callCenter2.ID, new[] { super1 });

            AssertEquivalentAllSuperAssignments(expectedAssignments);
        }

        private static void AssertEquivalentAllSurveyAssignments(
            IEnumerable<BvSurveyAssignmentOnCallCenterEntity> expectedAssignments)
        {
            AssertEquivalentSurveyAssignments(expectedAssignments, BvSurveyAssignmentOnCallCenterAdapter.GetAll());
        }

        private static void AssertEquivalentSurveyAssignments(
            IEnumerable<BvSurveyAssignmentOnCallCenterEntity> expectedAssignments,
            IEnumerable<BvSurveyAssignmentOnCallCenterEntity> actualAssignments)
        {
            TestCollectionAssert.AreEquivalent(
                expectedAssignments,
                actualAssignments,
                (item1, item2) => item1.CallCenterId == item2.CallCenterId && item1.SurveyId == item2.SurveyId);
        }

        private static void AssertEquivalentAllSuperAssignments(
            IEnumerable<BvSupervisorAssignmentEntity> expectedAssignments)
        {
            AssertEquivalentSuperAssignments(expectedAssignments, BvSupervisorAssignmentAdapter.GetAll());
        }

        private static void AssertEquivalentSuperAssignments(
            IEnumerable<BvSupervisorAssignmentEntity> expectedAssignments,
            IEnumerable<BvSupervisorAssignmentEntity> actualAssignments)
        {
            TestCollectionAssert.AreEquivalent(
                expectedAssignments,
                actualAssignments,
                (item1, item2) => item1.CallCenterId == item2.CallCenterId && item1.Name == item2.Name);
        }
    }
}
