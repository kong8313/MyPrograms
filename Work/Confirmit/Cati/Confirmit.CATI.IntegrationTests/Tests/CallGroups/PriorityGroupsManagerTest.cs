using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.ITSs;
using Confirmit.CATI.Supervisor.Core.PriorityGroups;
using Confirmit.CATI.Supervisor.Resources;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CallGroups
{
    [TestClass]
    public class PriorityGroupsManagerTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private int _personId;
        private BvCallGroupEntity _callGroup;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize(false);

            _personId = PersonTools.CreatePerson("test", "password", AgentTaskChoiceMode.Manual);
            _callGroup = new BvCallGroupEntity
                         {
                             Name = "my call group"
                         };
            ServiceLocator.Resolve<ICallGroupRepository>().Insert(_callGroup);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetPersonsPageNotInGroup_SinglePersonWithoutGroup_PersonIsReturned()
        {
            int totalCount;
            var result = PriorityGroupsManager.GetPersonsPageNotInGroup(
                _callGroup.Id, new PagingArgs(string.Empty, true), out totalCount);

            Assert.AreEqual(1, totalCount, "Single record should be returned");
            Assert.AreEqual(1, result.Count(), "Single record should be returned");
            Assert.AreEqual(_personId, result.ElementAt(0).PersonSID, "Wrong person id");
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetPersonsPageNotInGroup_SinglePersonAssignedToGroup_EmptyCollection()
        {
            ServiceLocator.Resolve<ICallGroupService>().SetPersonsAssignment(new List<int> { _personId }, _callGroup.Id);

            int totalCount;
            var result = PriorityGroupsManager.GetPersonsPageNotInGroup(
                _callGroup.Id, new PagingArgs(string.Empty, true), out totalCount);

            Assert.AreEqual(0, totalCount, "Collection should be empty");
            Assert.AreEqual(0, result.Count(), "Collection should be empty");
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetNotIncludedStatuses_GroupWithDefaultStatuses_EmptyCollection()
        {
            var its = 31;
            var stateGroupId = StateGroupRepository.GetDefault().ID;
            ServiceLocator.Resolve<IServiceRegistrator>().Register<IPriorityGroupsManager, PriorityGroupsManager>();

            var states = ServiceLocator.Resolve<IPriorityGroupsManager>().GetNotIncludedStatuses(_callGroup.Id);

            var expectedState = StateRepository.GetById(stateGroupId, its);
            var actualState = states.Single(x => x.Key == its);
            
            Assert.AreEqual(expectedState.Name, actualState.Value);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetStatusesByGroupId_GroupWithDefaultStatuses_EmptyCollection()
        {
            var its = 31;
            var stateGroupId = StateGroupRepository.GetDefault().ID;
            ServiceLocator.Resolve<IServiceRegistrator>().Register<IPriorityGroupsManager, PriorityGroupsManager>();
            
            var priorityGroupsManager = ServiceLocator.Resolve<IPriorityGroupsManager>();
            priorityGroupsManager.AddStatuses(_callGroup.Id, new[] {its});

            var states = priorityGroupsManager.GetStatusesByGroupId(_callGroup.Id);
            
            var expectedState = StateRepository.GetById(stateGroupId, its);
            var actualState = states.Single(x => x.Id == its);

            Assert.AreEqual(expectedState.Name, actualState.Name);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetNotIncludedStatuses_GroupWithSpecificStatuses_EmptyCollection()
        {
            var its = 31;
            var stateGroupId = CreateNotDefaultStateGroupAndAssignToCallGroup(_callGroup.Id, its);
            ServiceLocator.Resolve<IServiceRegistrator>().Register<IPriorityGroupsManager, PriorityGroupsManager>();

            var states = ServiceLocator.Resolve<IPriorityGroupsManager>().GetNotIncludedStatuses(_callGroup.Id);

            var expectedState = StateRepository.GetById(stateGroupId, its);
            var actualState = states.Single(x => x.Key == its);
            var defaultState = StateRepository.GetById(StateGroupRepository.GetDefault().ID, its);

            Assert.AreEqual(expectedState.Name, actualState.Value);
            Assert.AreNotEqual(defaultState.Name, actualState.Value);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetStatusesByGroupId_GroupWithSpecificStatuses_EmptyCollection()
        {
            var its = 31;
            var stateGroupId = CreateNotDefaultStateGroupAndAssignToCallGroup(_callGroup.Id, its);
            ServiceLocator.Resolve<IServiceRegistrator>().Register<IPriorityGroupsManager, PriorityGroupsManager>();
            
            var priorityGroupsManager = ServiceLocator.Resolve<IPriorityGroupsManager>();
            priorityGroupsManager.AddStatuses(_callGroup.Id, new[] { its });

            var states = priorityGroupsManager.GetStatusesByGroupId(_callGroup.Id);

            var expectedState = StateRepository.GetById(stateGroupId, its);
            var actualState = states.Single(x => x.Id == its);
            var defaultState = StateRepository.GetById(StateGroupRepository.GetDefault().ID, its);

            Assert.AreEqual(expectedState.Name, actualState.Name);
            Assert.AreNotEqual(defaultState.Name, actualState.Name);
        }

        private int CreateNotDefaultStateGroupAndAssignToCallGroup(int callGroupId, int changedIts)
        {
            var groupId = StateGroupsManager.CopyStateGroup("Test group", StateGroupRepository.GetDefault().ID);
            var state = StateRepository.GetById(groupId, changedIts);
            state.Name += "(changed)";
            StateRepository.Update(state);

            var callGroupRepository = ServiceLocator.Resolve<ICallGroupRepository>();

            var group = callGroupRepository.Get(callGroupId);
            
            group.DesignStateGroupID = groupId;
            
            callGroupRepository.Update(group);

            return groupId;
        }
    }
}
