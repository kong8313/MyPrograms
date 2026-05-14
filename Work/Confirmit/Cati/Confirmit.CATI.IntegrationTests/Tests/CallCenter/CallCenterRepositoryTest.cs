using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.CallCenters;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Framework.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CallCenter
{
    [TestClass]
    public class CallCenterRepositoryTest
    {
        readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private ICallCenterRepository _repository;
        private BvCallCenterEntity _entity;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _repository = ServiceLocator.Resolve<ICallCenterRepository>();
            _entity = new BvCallCenterEntity
                          {
                              Name = "call Center name",
                              Description = "call center description",
                              LocalTimezoneId = _repository.Default.LocalTimezoneId
                          };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Empty_Insert_CallCenterIsCreated()
        {
            _repository.Insert(_entity);

            Assert.IsTrue(_entity.ID > 0, "Created call center should have ID greater then 0" );

            var actual = _repository.Get(_entity.ID);

            Assert.AreEqual(_entity.ID, actual.ID);
            Assert.AreEqual(_entity.Name, actual.Name);
            Assert.AreEqual(_entity.Description, actual.Description);
            Assert.AreEqual(_entity.LocalTimezoneId, actual.LocalTimezoneId);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallCenter_Update_CallCenterIsUpdated()
        {
            _repository.Insert(_entity);

            var expectedTimezoneId = _repository.Default.LocalTimezoneId + 1;
            TimezoneService.Activate(expectedTimezoneId);
            _entity.Name = "new call center name";
            _entity.Description = "new call center description";
            _entity.LocalTimezoneId = expectedTimezoneId;

            _repository.Update(_entity);

            var actual = _repository.Get(_entity.ID);

            Assert.AreEqual(_entity.ID, actual.ID);
            Assert.AreEqual(_entity.Name, actual.Name);
            Assert.AreEqual(_entity.Description, actual.Description);
            Assert.AreEqual(expectedTimezoneId, actual.LocalTimezoneId);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallCenter_DeleteWithPersonRemoving_CallCenterWithPersonsAreDeleted()
        {
            _repository.Insert(_entity);

            var defCC = new CallCenterWrapper(_repository.Default, _backendTools);
            var defSurvey = defCC.CreateAndAssignSurvey("p00000001");
            var defPerson = defCC.CreatePerson("p1");
            defCC.AssignResourceToSurvey(defSurvey, defPerson.Entity.SID);
            defCC.AssignResourceToSurvey(defSurvey, PersonGroupService.RootGroupId);

            var delCC = new CallCenterWrapper(_entity, _backendTools);
            var delSurvey = delCC.CreateAndAssignSurvey("p0000002");
            var delPerson = delCC.CreatePerson("p2");
            delCC.AssignResourceToSurvey(delSurvey, delPerson.Entity.SID);
            delCC.AssignResourceToSurvey(delSurvey, PersonGroupService.RootGroupId);

            _repository.Delete(_entity.ID, defCC.Entity.ID, InterviewerActionOnCallCenterDelete.Delete);

            //callCenter is deleted
            Assert.IsNull(_repository.Get(_entity.ID));

            //person is deleted
            CollectionAssert.AreEqual(new[] { defPerson.Entity.SID }, BvPersonAdapter.GetAll().Select(x => x.SID).ToArray());

            //survey is deleted
            CollectionAssert.AreEqual(new[] { defSurvey, delSurvey }, BvSurveyAdapter.GetAll().Select(x => x.SID).ToArray());

            //assignment are deleted
            var assignments = BvPersonOrGroupAssignmentOnSurveyAdapter.GetAll().OrderBy(x => x.PersonOrGroupId).ToArray();
            
            Assert.AreEqual(2, assignments.Length);
            
            Assert.AreEqual(PersonGroupService.RootGroupId, assignments[0].PersonOrGroupId);
            Assert.AreEqual(defCC.Entity.ID, assignments[0].CallCenterID);
            Assert.AreEqual(defSurvey, assignments[0].SurveyId);

            Assert.AreEqual(defPerson.Entity.SID, assignments[1].PersonOrGroupId);
            Assert.AreEqual(defCC.Entity.ID, assignments[1].CallCenterID);
            Assert.AreEqual(defSurvey, assignments[1].SurveyId);

            //check that assignment of person is deleted
            Assert.AreEqual(0, BvPersonRelAdapter.GetAll().Count(x => x.PersonSID == delPerson.Entity.SID));
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallCenter_DeleteWithPersonMoving_CallCenterWithPersonsAreMoved()
        {
            _repository.Insert(_entity);

            var defCC = new CallCenterWrapper(_repository.Default, _backendTools);
            var defSurvey = defCC.CreateAndAssignSurvey("p00000001");
            var defPerson = defCC.CreatePerson("p1");
            defCC.AssignResourceToSurvey(defSurvey, defPerson.Entity.SID);
            defCC.AssignResourceToSurvey(defSurvey, PersonGroupService.RootGroupId);

            var delCC = new CallCenterWrapper(_entity, _backendTools);
            var delSurvey = delCC.CreateAndAssignSurvey("p0000002");
            var delPerson = delCC.CreatePerson("p2");
            delCC.AssignResourceToSurvey(delSurvey, delPerson.Entity.SID);
            delCC.AssignResourceToSurvey(delSurvey, PersonGroupService.RootGroupId);

            _repository.Delete(_entity.ID, defCC.Entity.ID, InterviewerActionOnCallCenterDelete.MoveToSelectedCallCenter);

            //callCenter is deleted
            Assert.IsNull(_repository.Get(_entity.ID));

            //person is deleted
            CollectionAssert.AreEqual(new[] { defPerson.Entity.SID, delPerson.Entity.SID }, BvPersonAdapter.GetAll().Select(x => x.SID).ToArray());

            //survey is deleted
            CollectionAssert.AreEqual(new[] { defSurvey, delSurvey }, BvSurveyAdapter.GetAll().Select(x => x.SID).ToArray());

            //assignment are deleted
            var assignments = BvPersonOrGroupAssignmentOnSurveyAdapter.GetAll().OrderBy(x => x.PersonOrGroupId).ToArray();

            Assert.AreEqual(2, assignments.Length);

            Assert.AreEqual(PersonGroupService.RootGroupId, assignments[0].PersonOrGroupId);
            Assert.AreEqual(defCC.Entity.ID, assignments[0].CallCenterID);
            Assert.AreEqual(defSurvey, assignments[0].SurveyId);

            Assert.AreEqual(defPerson.Entity.SID, assignments[1].PersonOrGroupId);
            Assert.AreEqual(defCC.Entity.ID, assignments[1].CallCenterID);
            Assert.AreEqual(defSurvey, assignments[1].SurveyId);

            //check BvPersonRel for check assign moved person to syrvey through parent group
            Assert.IsNotNull(BvPersonRelAdapter.GetAll().SingleOrDefault(x => x.PersonSID == delPerson.Entity.SID && x.Type == 2 && x.ObjectSID == defSurvey));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(InternalErrorException))]
        public void Insert_CallCenterWithInactiveTimezone_ExceptionIsThrown()
        {
            var entity = new BvCallCenterEntity {Name = "CallCenter", LocalTimezoneId = _repository.Default.LocalTimezoneId + 1};

            _repository.Insert(entity);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        [ExpectedException(typeof(InternalErrorException))]
        public void Update_CallCenterWithInactiveTimezone_ExceptionIsThrown()
        {
            var entity = _repository.Default;
            entity.LocalTimezoneId = entity.LocalTimezoneId + 1;

            _repository.Update(entity);
        }
    }
}
