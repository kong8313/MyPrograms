using System.Collections.Generic;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.DatabaseUpdateLibrary.UnitTests
{
    [TestClass]
    public class UpdateScriptsProviderTest
    {
        private StubIResources _resources;
        private StubIUpdateScriptDatabaseWorker _updateScriptDatabaseWorker;        
        private UpdateScriptsProvider _updateScriptsProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _resources = new StubIResources();
            _updateScriptDatabaseWorker = new StubIUpdateScriptDatabaseWorker();

            _updateScriptsProvider = new UpdateScriptsProvider(_resources, _updateScriptDatabaseWorker);
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToValidate_NoExecutedScripts_AllScriptsAreReturned()
        {
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => new UpdateScriptInfo[0];

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_04", "Description", false)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToRollBack = _updateScriptsProvider.GetScriptsToValidate("");
            Assert.AreEqual(2, scriptsToRollBack.Count);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToApply_NoExecutedScripts_AllScriptsAreReturned()
        {
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => new UpdateScriptInfo[0];

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_04", "Description", false)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToCommit = _updateScriptsProvider.GetScriptsToApply("");
            Assert.AreEqual(2, scriptsToCommit.Count);
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToValidate_NoUnsafeScripts_AllScriptsAreReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_04", "Description", false)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;
            
            var scriptsToRollback = _updateScriptsProvider.GetScriptsToValidate("");
            Assert.AreEqual(2, scriptsToRollback.Count);
            Assert.AreEqual("_2017-01-01_01_01_03", scriptsToRollback[0].Name);
            Assert.AreEqual("_2017-01-01_01_01_04", scriptsToRollback[1].Name);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToValidate_OneUnsafeScriptAtTheEnd_AllScriptsExceptOneAreReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_04", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_05", "Description", true)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToRollback = _updateScriptsProvider.GetScriptsToValidate("");
            Assert.AreEqual(2, scriptsToRollback.Count);
            Assert.AreEqual("_2017-01-01_01_01_03", scriptsToRollback[0].Name);
            Assert.AreEqual("_2017-01-01_01_01_04", scriptsToRollback[1].Name);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToValidate_AllUnsafeScriptAtTheEnd_NoScriptsAreReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", true),
                new UpdateScriptInfo("_2017-01-01_01_01_04", "Description", true)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToRollback = _updateScriptsProvider.GetScriptsToValidate("");
            Assert.AreEqual(0, scriptsToRollback.Count);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToValidate_NotAllUnsafeScriptAtTheEnd_AllScriptsAreReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", true),
                new UpdateScriptInfo("_2017-01-01_01_01_04", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_05", "Description", true)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToRollback = _updateScriptsProvider.GetScriptsToValidate("");
            Assert.AreEqual(3, scriptsToRollback.Count);
            Assert.AreEqual("_2017-01-01_01_01_03", scriptsToRollback[0].Name);
            Assert.AreEqual("_2017-01-01_01_01_04", scriptsToRollback[1].Name);
            Assert.AreEqual("_2017-01-01_01_01_05", scriptsToRollback[2].Name);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToValidate_AllScriptsAreAlreadyExecuted_NoScriptsReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", true),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", true)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", true),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", true)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToCommit = _updateScriptsProvider.GetScriptsToValidate("");
            Assert.AreEqual(0, scriptsToCommit.Count);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToValidate_TwoScriptsFromThreeAreExecuted_OneScriptReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", false)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToCommit = _updateScriptsProvider.GetScriptsToValidate("");
            Assert.AreEqual(1, scriptsToCommit.Count);
            Assert.AreEqual("_2017-01-01_01_01_03", scriptsToCommit[0].Name);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToApply_TwoNormalScripts_BothTwoAreReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_04", "Description", false)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToCommit = _updateScriptsProvider.GetScriptsToApply("");
            Assert.AreEqual(2, scriptsToCommit.Count);
            Assert.AreEqual("_2017-01-01_01_01_03", scriptsToCommit[0].Name);
            Assert.AreEqual("_2017-01-01_01_01_04", scriptsToCommit[1].Name);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToApply_OneNormalAndOneUnsafeScriptAtTheEnd_BothTwoAreReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_04", "Description", true)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToCommit = _updateScriptsProvider.GetScriptsToApply("");
            Assert.AreEqual(2, scriptsToCommit.Count);
            Assert.AreEqual("_2017-01-01_01_01_03", scriptsToCommit[0].Name);
            Assert.AreEqual("_2017-01-01_01_01_04", scriptsToCommit[1].Name);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToApply_AllScriptsAreAlreadyExecuted_NoScriptsReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToCommit = _updateScriptsProvider.GetScriptsToApply("");
            Assert.AreEqual(0, scriptsToCommit.Count);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScriptsToApply_OneScriptFromTwoAreExceutedWithDifferentOrder_OneScriptReturned()
        {
            UpdateScriptInfo[] executedUpdateScriptInfos =
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false)
            };
            _updateScriptDatabaseWorker.GetAppliedUpdateScriptInfosString = databaseName => executedUpdateScriptInfos;

            UpdateScriptInfo[] updateScriptInfos =
            {                
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false)
            };
            _resources.UpdateScriptInfosGet = () => updateScriptInfos;

            var scriptsToCommit = _updateScriptsProvider.GetScriptsToApply("");
            Assert.AreEqual(1, scriptsToCommit.Count);
            Assert.AreEqual("_2017-01-01_01_01_02", scriptsToCommit[0].Name);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CheckAllLastUpdateScriptsAreUnsafe_AllScriptsAreNoraml_ReturnFalse()
        {
            List<UpdateScriptInfo> scriptsToExecute = new List<UpdateScriptInfo>
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", false)
            };

            Assert.IsFalse(_updateScriptsProvider.CheckAllLastUpdateScriptsAreUnsafe(scriptsToExecute));
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CheckAllLastUpdateScriptsAreUnsafe_AllScriptsAreUnsafe_ReturnTrue()
        {
            List<UpdateScriptInfo> scriptsToExecute = new List<UpdateScriptInfo>
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", true),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", true),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", true)
            };

            Assert.IsTrue(_updateScriptsProvider.CheckAllLastUpdateScriptsAreUnsafe(scriptsToExecute));
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CheckAllLastUpdateScriptsAreUnsafe_LastScriptIsUnsafe_ReturnTrue()
        {
            List<UpdateScriptInfo> scriptsToExecute = new List<UpdateScriptInfo>
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", true)
            };

            Assert.IsTrue(_updateScriptsProvider.CheckAllLastUpdateScriptsAreUnsafe(scriptsToExecute));
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CheckAllLastUpdateScriptsAreUnsafe_FirstAndLastScriptsAreUnsafe_ReturnFalse()
        {
            List<UpdateScriptInfo> scriptsToExecute = new List<UpdateScriptInfo>
            {
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Description", true),
                new UpdateScriptInfo("_2017-01-01_01_01_02", "Description", false),
                new UpdateScriptInfo("_2017-01-01_01_01_03", "Description", true)
            };

            Assert.IsFalse(_updateScriptsProvider.CheckAllLastUpdateScriptsAreUnsafe(scriptsToExecute));
        }
    }
}