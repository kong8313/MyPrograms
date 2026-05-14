using System.Collections.Generic;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces.Fakes;
using Confirmit.CATI.DatabaseUpdateLibrary.UnitTests.FakeClasses;
using Confirmit.CATI.Installation.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.DatabaseUpdateLibrary.UnitTests
{
    [TestClass]
    public class ValidatorTest
    {
        private IValidator _validator;
        private IResources _resources;
        private IConfiguration _configuration;
        private StubIDatabaseWorker _databaseWorker;
        private List<string> _databasesForUpgrade;

        [TestInitialize]
        public void TestInitialize()
        {
            _resources = new FakeResources();
            _configuration = new FakeConfiguration();
            _databaseWorker = new StubIDatabaseWorker
            {
                IsDatabaseExistsString = databaseName =>
                {
                    if (databaseName.EndsWith("NotExist"))
                    {
                        return false;
                    }

                    return true;
                },

                GetUserAccessString = databaseName =>
                {
                    if (databaseName.EndsWith("Single"))
                    {
                        return DatabaseUserAccess.Single;
                    }

                    if (databaseName.EndsWith("Restricted"))
                    {
                        return DatabaseUserAccess.Restricted;
                    }

                    return DatabaseUserAccess.Multiple;
                }
            };
            _validator = new Validator(_resources, _databaseWorker, _configuration);

            _databasesForUpgrade = new List<string> { "ConfirmitCATIV15", "ConfirmitCATIV15_1", "ConfirmitCATIV15_100" };
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CheckUpdateScriptsTestTest_RealResources_NoExcecption()
        {
            IResources resources = new Resources();
            _validator = new Validator(resources, _databaseWorker, _configuration);
            _validator.CheckUpdateScripts();
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), ExpectedException(typeof(ValidateException))]
        public void CheckUpdateScriptsTestTest_NoCATIInDescriptionForUpdateScriptN95_ExcecptionHasOccured()
        {
            for (int i = 3; i < 96; i++)
            {
                ((FakeResources)_resources).FakeUpdateScriptInfos.Add(
                    new UpdateScriptInfo("_17_00_Test_0" + i, "decription for update script N" + i, false){ScriptText = "select _17_00_Test_0" + i });
            }
            
            _validator.CheckUpdateScripts();
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CheckUpdateScriptsTest_NormalUpdateScripts_NoExcecption()
        {
            _validator.CheckUpdateScripts();
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK"), ExpectedException(typeof(ValidateException))]
        public void CheckUpdateScriptsTest_MissingDescription_ExcecptionHasOccured()
        {
            ((FakeResources)_resources).FakeUpdateScriptInfos.Add(new UpdateScriptInfo("_17_00_Test_03", "", false));
            _validator.CheckUpdateScripts();
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CheckDatabasesTest_NormalParameters_NoExcecption()
        {
            _validator.CheckDatabases(_databasesForUpgrade.ToArray());
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), ExpectedException(typeof(ValidateException))]
        public void CheckDatabasesTest_NoDatabaseForUpgrade_ExcecptionHasOccured()
        {
            _databasesForUpgrade.Clear();
            _validator.CheckDatabases(_databasesForUpgrade.ToArray());
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), ExpectedException(typeof(ValidateException))]
        public void CheckDatabasesTest_NotExistedDefaultDatabase_ExcecptionHasOccured()
        {
            ((FakeConfiguration)_configuration).DefaultDatabaseName = "ConfirmitCATIV15_NotExist";
            _validator.CheckDatabases(_databasesForUpgrade.ToArray());
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), ExpectedException(typeof(ValidateException))]
        public void CheckDatabasesTest_NotExistedDatabaseForUpgrade_ExcecptionHasOccured()
        {
            _databasesForUpgrade.Add("ConfirmitCATIV15_NotExist");
            _validator.CheckDatabases(_databasesForUpgrade.ToArray());
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK"), ExpectedException(typeof(ValidateException))]
        public void CheckDatabasesTest_OneDatabaseForUpgradeHasSingleDatabaseUserAccess_ExcecptionHasOccured()
        {            
            _databasesForUpgrade.Add("ConfirmitCATIV15_Single");
            _validator.CheckDatabases(_databasesForUpgrade.ToArray());
        }
    }
}
