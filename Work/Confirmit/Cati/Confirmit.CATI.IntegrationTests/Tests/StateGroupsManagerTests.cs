using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.ITSs;

namespace Confirmit.CATI.IntegrationTests.Tests
{
    [TestClass]
    public class StateGroupsManagerTests
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        [TestMethod]
        [Owner(@"FIRM\LiubovK")]
        public void AddNewStateGroup_CheckNewGroupAdded()
        {
            StateGroupsManager.AddStateGroup("testNewGroup");
            Assert.IsNotNull(StateGroupRepository.GetByName("testNewGroup"), "There is no new group with name 'testNewGroup'");
        }

        [TestMethod]
        [Owner(@"FIRM\LiubovK")]
        public void CopyStateGroup_CheckStateGroupSuccessfullyDuplicated()
        {
            var copyId = StateGroupsManager.CopyStateGroup("testGroup", StateGroupRepository.GetDefault().ID);
            var defaultStateGroupStates = StateRepository.GetAll(StateGroupRepository.GetDefault().ID);

            AssertIfStatesAreEqual(defaultStateGroupStates, StateRepository.GetAll(copyId));
        }

        [TestMethod]
        [Owner(@"FIRM\LiubovK")]
        public void CopyToDefaultGroup_CustomGroupSuccessfullyCopiedToDefaultGroup()
        {
            var customGroupId = CreateAndPrepareNewStateGroup("testGroup");

            var defaultStateGroup = StateGroupRepository.GetDefault();
            var defaultStateGroupStatesBeforeCopyToDefault = StateRepository.GetAll(defaultStateGroup.ID);

            StateGroupsManager.CopyToDefaultGroup(customGroupId, DateTime.UtcNow);

            var backupStateGroup = StateGroupRepository.GetAll().SingleOrDefault(x => x.ID != customGroupId && x.ID != defaultStateGroup.ID);

            Assert.IsNotNull(backupStateGroup);

            AssertIfStatesAreEqual(defaultStateGroupStatesBeforeCopyToDefault, StateRepository.GetAll(backupStateGroup.ID));

            AssertIfStatesAreEqual(StateRepository.GetAll(customGroupId), StateRepository.GetAll(StateGroupRepository.GetDefault().ID));
        }

        [TestMethod]
        [Owner(@"FIRM\LiubovK"), ExpectedException(typeof(UserMessageException))]
        public void CopyToDefaultGroup_TryCopyDefaultGroupIntoItself()
        {
            StateGroupsManager.CopyToDefaultGroup(StateGroupRepository.GetDefault().ID, DateTime.UtcNow);
        }
        private void AssertIfStatesAreEqual(List<BvStateEntity> originalStates, List<BvStateEntity> newStates)
        {
            foreach (var state in newStates)
            {
                var originalState = originalStates.FirstOrDefault(x => x.StateID == state.StateID);
                Assert.AreEqual(originalState?.Name, state.Name);
                Assert.AreEqual(originalState?.Priority, state.Priority);
                Assert.AreEqual(originalState?.DA, state.DA);
                Assert.AreEqual(originalState?.FcdAction, state.FcdAction);
            }
        }

        private int CreateAndPrepareNewStateGroup(string name)
        {
            var customGroupId = StateGroupsManager.AddStateGroup(name);
            var customStateGroupStates = StateRepository.GetAll(customGroupId);

            var i = 1;
            foreach (var state in customStateGroupStates)
            {
                state.Name += i;
                state.Priority = i;
                state.DA = state.DA == 1 ? 0 : 1;
                state.FcdAction = state.FcdAction != true;
                StateRepository.Update(state);
                i++;
            }

            return customGroupId;
        }
    }
}
