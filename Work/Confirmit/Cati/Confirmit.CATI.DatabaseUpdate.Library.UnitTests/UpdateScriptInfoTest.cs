using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.DatabaseUpdateLibrary.UnitTests
{
    [TestClass]
    public class UpdateScriptInfoTest
    {
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void UpdateScriptInfo_MakeInfoAboutNormalScript_ObjectHasCorrectParameters()
        {
            const string scriptName = "_2017-01-01_01_01_01";
            const string scriptDescription = "Script description";
            var updateScriptInfo = new UpdateScriptInfo(scriptName, scriptDescription, false);

            Assert.IsFalse(updateScriptInfo.HasSqlScriptUnsafeType);
            Assert.AreEqual(scriptName, updateScriptInfo.Name);
            Assert.AreEqual(scriptDescription, updateScriptInfo.Description);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void UpdateScriptInfo_MakeInfoAboutUnsafeScript_ObjectHasCorrectParameters()
        {
            const string scriptName = "_2017-01-01_01_01_01";
            const string scriptDescription = "Script description";
            var updateScriptInfo = new UpdateScriptInfo(scriptName, scriptDescription, true);

            Assert.IsTrue(updateScriptInfo.HasSqlScriptUnsafeType);
            Assert.AreEqual(scriptName, updateScriptInfo.Name);
            Assert.AreEqual(scriptDescription, updateScriptInfo.Description);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void UpdateScriptInfo_MakeInfoAboutScriptWithAllParameters_ObjectHasCorrectParameters()
        {
            const string scriptName = "_2017-01-01_01_01_01";
            const string extension = "sql";
            const string scriptDescription = "Script description";
            const bool isUnsafe = true;
            DateTime time = DateTime.Now;
            const int duration = 1;
            const string scriptText = "scritp text";
            const string scriptOutput = "script output";
            const bool isAppliedDuringDBCreation = true;
            const string dbUpateUtilityVersion = "1.2.3.4";
            const string activeUser = "userName";

            var updateScriptInfo = new UpdateScriptInfo(
                scriptName, extension, scriptDescription, isUnsafe, time, duration, scriptText, scriptOutput, isAppliedDuringDBCreation, dbUpateUtilityVersion, activeUser);

            Assert.AreEqual(scriptName, updateScriptInfo.Name);
            Assert.AreEqual(extension, updateScriptInfo.Extension);
            Assert.AreEqual(scriptDescription, updateScriptInfo.Description);
            Assert.AreEqual(isUnsafe, updateScriptInfo.HasSqlScriptUnsafeType);
            Assert.AreEqual(time, updateScriptInfo.ScriptAppliedDate);
            Assert.AreEqual(duration, updateScriptInfo.Duration);
            Assert.AreEqual(scriptText, updateScriptInfo.ScriptText);
            Assert.AreEqual(scriptOutput, updateScriptInfo.ScriptOutput);
            Assert.AreEqual(isAppliedDuringDBCreation, updateScriptInfo.IsAppliedDuringDBCreation);
            Assert.AreEqual(dbUpateUtilityVersion, updateScriptInfo.DbUpateUtilityVersion);
            Assert.AreEqual(activeUser, updateScriptInfo.ActiveUser);
            Assert.AreEqual(-1, updateScriptInfo.ScriptNumber);
            Assert.AreEqual(string.Empty, updateScriptInfo.BranchName);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void UpdateScriptInfo_MakeInfoAboutScriptWithAllParametersInOldFormat_ObjectHasCorrectParameters()
        {
            const string scriptName = "_18_01_Main_02";
            const string extension = "sql";
            const string scriptDescription = "Script description";
            const bool isUnsafe = false;
            DateTime time = DateTime.Now;
            const int duration = 1;
            const string scriptText = "scritp text";
            const string scriptOutput = "script output";
            const bool isAppliedDuringDBCreation = true;
            const string dbUpateUtilityVersion = "1.2.3.4";
            const string activeUser = "userName";

            var updateScriptInfo = new UpdateScriptInfo(
                scriptName, extension, scriptDescription, isUnsafe, time, duration, scriptText, scriptOutput, isAppliedDuringDBCreation, dbUpateUtilityVersion, activeUser);

            Assert.AreEqual(scriptName, updateScriptInfo.Name);
            Assert.AreEqual(extension, updateScriptInfo.Extension);
            Assert.AreEqual(scriptDescription, updateScriptInfo.Description);
            Assert.AreEqual(isUnsafe, updateScriptInfo.HasSqlScriptUnsafeType);
            Assert.AreEqual(time, updateScriptInfo.ScriptAppliedDate);
            Assert.AreEqual(duration, updateScriptInfo.Duration);
            Assert.AreEqual(scriptText, updateScriptInfo.ScriptText);
            Assert.AreEqual(scriptOutput, updateScriptInfo.ScriptOutput);
            Assert.AreEqual(isAppliedDuringDBCreation, updateScriptInfo.IsAppliedDuringDBCreation);
            Assert.AreEqual(dbUpateUtilityVersion, updateScriptInfo.DbUpateUtilityVersion);
            Assert.AreEqual(activeUser, updateScriptInfo.ActiveUser);
            Assert.AreEqual(18, updateScriptInfo.Major);
            Assert.AreEqual(1, updateScriptInfo.Minor);
            Assert.AreEqual("Main", updateScriptInfo.BranchName);
            Assert.AreEqual(2, updateScriptInfo.ScriptNumber);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void UpdateScriptInfo_UseLineFromResourcesFile_NormalTypeOldFormat_ObjectHasCorrectParameters()
        {
            const string line = "18.00\\18.00.Main.00.sql First script description";

            var updateScriptInfo = new UpdateScriptInfo(line);

            Assert.AreEqual("_18_00_Main_00", updateScriptInfo.Name);
            Assert.AreEqual("sql", updateScriptInfo.Extension);
            Assert.AreEqual("First script description", updateScriptInfo.Description);
            Assert.IsFalse(updateScriptInfo.HasSqlScriptUnsafeType);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void UpdateScriptInfo_UseLineFromResourcesFile_NormalTypeWithPsExtensionInName_ObjectHasCorrectParameters()
        {
            const string line = "18.00\\18.00.Main.00.ps1 First script description";

            var updateScriptInfo = new UpdateScriptInfo(line);

            Assert.AreEqual("_18_00_Main_00", updateScriptInfo.Name);
            Assert.AreEqual("ps1", updateScriptInfo.Extension);
            Assert.AreEqual("First script description", updateScriptInfo.Description);
            Assert.IsFalse(updateScriptInfo.HasSqlScriptUnsafeType);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void UpdateScriptInfo_UseLineFromResourcesFile_NormalType_ObjectHasCorrectParameters()
        {
            const string line = "19.00\\2014-07-02_09.21.45.sql Second script description";

            var updateScriptInfo = new UpdateScriptInfo(line);

            Assert.AreEqual("_2014-07-02_09_21_45", updateScriptInfo.Name);
            Assert.AreEqual("sql", updateScriptInfo.Extension);
            Assert.AreEqual("Second script description", updateScriptInfo.Description);
            Assert.IsFalse(updateScriptInfo.HasSqlScriptUnsafeType);
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void UpdateScriptInfo_UseLineFromResourcesFile_UnsafeType_ObjectHasCorrectParameters()
        {
            const string line = "U22.00\\2017-07-04_15.18.18.sql Forth script description";

            var updateScriptInfo = new UpdateScriptInfo(line);

            Assert.AreEqual("_2017-07-04_15_18_18", updateScriptInfo.Name);
            Assert.AreEqual("sql", updateScriptInfo.Extension);
            Assert.AreEqual("Forth script description", updateScriptInfo.Description);
            Assert.IsTrue(updateScriptInfo.HasSqlScriptUnsafeType);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void UpdateScriptInfo_ExceptOneUpdateScriptInfoListFromAnother_UpdateScriptInfoComparerWorksCorrect()
        {
            UpdateScriptInfo[] updateScriptInfos = new UpdateScriptInfo[2];
            updateScriptInfos[0] = new UpdateScriptInfo("_2017-01-01_01_01_01", "Desc 1", false);
            updateScriptInfos[1] = new UpdateScriptInfo("_2017-01-01_01_01_02", "Desc 2", false);
            
            UpdateScriptInfo[] appliedUpdateScriptInfos = new UpdateScriptInfo[1];
            appliedUpdateScriptInfos[0] = new UpdateScriptInfo("_2017-01-01_01_01_01", "Desc 1", false);

            var scriptsToExecute = updateScriptInfos.Except(appliedUpdateScriptInfos).ToArray();
            Assert.AreEqual(1, scriptsToExecute.Length);
            Assert.AreEqual("_2017-01-01_01_01_02", scriptsToExecute[0].Name);
        }
    }
}