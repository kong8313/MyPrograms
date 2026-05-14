using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.IntegrationTests.Tests.InstallationCommon.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Web.Administration;

namespace Confirmit.CATI.IntegrationTests.Tests.InstallationCommon
{
    // This is a class with tests for 2 classes: IisEngine and IsAliveHtmlEngine
    // Tests for both classes use IIS, make, change and remove objects and can't work in parallel
    // To avoid of time groving these tests are situated in one class
    [TestClass]
    public class IisTests
    {
        private IIsAliveHtmEngine _isAliveHtmEngine;
        private const string TestPageName = "TestPage.htm";

        public const string DefaultSiteName = "Default Web Site";
        private IISEngine _iisEngine;
        private string _testFolderPath;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _isAliveHtmEngine = new IsAliveHtmEngine(new TraceLogger());

            var logger = new TraceLogger();
            _iisEngine = new IISEngine(logger);
            _testFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, Guid.NewGuid().ToString());
        }

        #region IisEngineTest
        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void DisableContentCaching_CreateVitualFolderWithDefaultCaching_CachingIsSetToImmediately()
        {
            string testVirtualDirectoryName = "/TestVirtualDirectory" + Guid.NewGuid();
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                iisEngineTestHelper.CreateVirtualFolder(null, testVirtualDirectoryName, _testFolderPath);

                _iisEngine.DisableContentCaching(DefaultSiteName, testVirtualDirectoryName);

                string cacheControlMode = iisEngineTestHelper.GetCacheControlMode(testVirtualDirectoryName);

                Assert.AreEqual("1", cacheControlMode, "DisableContentCaching method work incorrect. CacheControlMode parameter must be DisableCache (code 1) but it is different");
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void SetMaxAgeContentExpirationForSpecifiedFolders_CreateVitualFolderWithSomeSubfolder_CachingIsSetToCorrectTime()
        {
            string testVirtualDirectoryName = "/TestVirtualDirectory" + Guid.NewGuid();
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                iisEngineTestHelper.CreateVirtualFolder(null, testVirtualDirectoryName, _testFolderPath);

                _iisEngine.DisableContentCaching(DefaultSiteName, testVirtualDirectoryName);

                for (int i = 0; i < 3; i++)
                {
                    string directoryPath = Path.Combine(_testFolderPath, i.ToString());
                    iisEngineTestHelper.CreateFolder(directoryPath);
                }

                _iisEngine.SetMaxAgeContentExpirationForSpecifiedFolders(
                    DefaultSiteName,
                    testVirtualDirectoryName,
                    new[] { "0", "2" },
                    new TimeSpan(2, 0, 0, 0));


                string cacheControlMode = iisEngineTestHelper.GetCacheControlMode(testVirtualDirectoryName);

                Assert.AreEqual("1", cacheControlMode, "SetMaxAgeContentExpirationForSpecifiedFolders method work incorrect. CacheControlMode parameter must be disabled (code 1) for virtual directory but it is different");

                cacheControlMode = iisEngineTestHelper.GetCacheControlMode(testVirtualDirectoryName + "/0");
                string maxAge = iisEngineTestHelper.GetCacheControlMaxAge(testVirtualDirectoryName + "/0");
                Assert.AreEqual("2", cacheControlMode, "SetMaxAgeContentExpirationForSpecifiedFolders method work incorrect. CacheControlMode parameter must be UseMaxAge (code 2) for test directory 0 but it is different");
                Assert.AreEqual("2.00:00:00", maxAge, "SetMaxAgeContentExpirationForSpecifiedFolders method work incorrect. CacheControlMaxAge parameter must be 2 days for test directory 0 but it is different");

                cacheControlMode = iisEngineTestHelper.GetCacheControlMode(testVirtualDirectoryName + "/1");
                Assert.AreEqual("1", cacheControlMode, "SetMaxAgeContentExpirationForSpecifiedFolders method work incorrect. CacheControlMode parameter must be disabled (code 1) for test directory 1 but it is different");

                cacheControlMode = iisEngineTestHelper.GetCacheControlMode(testVirtualDirectoryName + "/2");
                maxAge = iisEngineTestHelper.GetCacheControlMaxAge(testVirtualDirectoryName + "/2");
                Assert.AreEqual("2", cacheControlMode, "SetMaxAgeContentExpirationForSpecifiedFolders method work incorrect. CacheControlMode parameter must be UseMaxAge (code 2) for test directory 2 but it is different");
                Assert.AreEqual("2.00:00:00", maxAge, "SetMaxAgeContentExpirationForSpecifiedFolders method work incorrect. CacheControlMaxAge parameter must be 2 days for test directory 0 but it is different");
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void GetAppPools_CorrectAppPoolsAreReturned()
        {
            var appPools = _iisEngine.GetAppPools();

            using (var sm = new ServerManager())
            {
                Assert.AreEqual(sm.ApplicationPools.Count, appPools.Count, "GetAppPools returns wrong count of application pools");

                foreach (var appPool in appPools)
                {
                    Assert.IsTrue(sm.ApplicationPools.Any(x => x.Name == appPool), "GetAppPools returns wrong application pools");
                }
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void GetWebSites_CorrectWebSitesAreReturned()
        {
            var webSites = _iisEngine.GetWebSites();

            using (var sm = new ServerManager())
            {
                Assert.AreEqual(sm.Sites.Count, webSites.Count, "GetWebSites returns wrong count of web sites");

                foreach (var webSite in webSites)
                {
                    Assert.IsTrue(sm.Sites.Any(x => x.Name == webSite), "GetWebSites returns wrong web sites");
                }
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void GetWebSiteId_CorrectWebSiteIdAreReturned()
        {
            var webSiteId = _iisEngine.GetWebSiteId(DefaultSiteName);

            using (var sm = new ServerManager())
            {
                Assert.AreEqual(sm.Sites[DefaultSiteName].Id.ToString(), webSiteId, "GetWebSiteId returns wrong id of web site");
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void CreateAndRemoveAlias_AliasIsCreatedAndRemovedSuccessfull()
        {
            string applicationName = "TestApplication" + Guid.NewGuid();
            string appPoolName;
            using (var sm = new ServerManager())
            {
                appPoolName = sm.ApplicationPools[0].Name;
            }

            _iisEngine.CreateAlias(DefaultSiteName, applicationName, appPoolName, @"c:\");
            using (var sm = new ServerManager())
            {
                Assert.IsTrue(sm.Sites[DefaultSiteName].Applications.Any(x => x.Path == "/" + applicationName), "CreateAlias does not create application");
            }

            _iisEngine.RemoveAlias(DefaultSiteName, applicationName);
            using (var sm = new ServerManager())
            {
                Assert.IsFalse(sm.Sites[DefaultSiteName].Applications.Any(x => x.Path == "/" + applicationName), "RemoveAlias does not remove application");
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void CreateAlias_AliasWasAlreadyExistsWithDifferentPath_AliasHasCorrectPath()
        {
            string applicationName = "TestApplication" + Guid.NewGuid();
            try
            {
                string appPoolName1, appPoolName2;
                using (var sm = new ServerManager())
                {
                    appPoolName1 = sm.ApplicationPools[0].Name;
                    appPoolName2 = sm.ApplicationPools[1].Name;
                }

                _iisEngine.CreateAlias(DefaultSiteName, applicationName, appPoolName1, @"c:\123\");
                _iisEngine.CreateAlias(DefaultSiteName, applicationName, appPoolName2, @"c:\");

                using (var sm = new ServerManager())
                {
                    var app = sm.Sites[DefaultSiteName].Applications.FirstOrDefault(x => x.Path == "/" + applicationName);

                    Assert.IsNotNull(app, "CreateAlias for existing application wotk incorrect");
                    Assert.AreEqual(@"c:\", app.VirtualDirectories[0].PhysicalPath, "CreateAlias does not change path for existing application");
                    Assert.AreEqual(appPoolName2, app.ApplicationPoolName, "CreateAlias does not change application pool for existing application");
                }
            }
            finally
            {
                _iisEngine.RemoveAlias(DefaultSiteName, applicationName);
            }
        }


        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void RemoveVirtualDirectory_CreateVirtDirAndRemoveIt_VirtDirIsRemovedSuccessfull()
        {
            string testVirtualDirectoryName = "/TestVirtualDirectory" + Guid.NewGuid();
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                iisEngineTestHelper.CreateVirtualFolder(null, testVirtualDirectoryName, _testFolderPath);

                _iisEngine.RemoveVirtualDirectory(DefaultSiteName, testVirtualDirectoryName);
                using (var sm = new ServerManager())
                {
                    Assert.IsFalse(sm.Sites[DefaultSiteName].Applications[0].VirtualDirectories.Any(x => x.Path == "/" + testVirtualDirectoryName), "RemoveVirtualDirectory does not remove virtual directory");
                }
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void CreateAppPoolAndRemoveEmptyAppPool_AppPoolIsCreatedAndRemovedSuccessfull()
        {
            var testAppPoolName = "TestAppPool" + Guid.NewGuid();

            try
            {
                _iisEngine.CreateAppPool(testAppPoolName);
                using (var sm = new ServerManager())
                {
                    Assert.IsTrue(sm.ApplicationPools.Any(x => x.Name == testAppPoolName), "CreateAppPool does not create application pool");
                }
            }
            finally
            {
                _iisEngine.RemoveEmptyAppPool(testAppPoolName);
                using (var sm = new ServerManager())
                {
                    Assert.IsFalse(sm.ApplicationPools.Any(x => x.Name == testAppPoolName), "RemoveEmptyAppPool does not remove empty application pool");
                }
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void RemoveEmptyAppPool_CreateAppPoolAndApplicationUsedThisPool_AppPoolCantBeRemovedUntilItISUsedForAplication()
        {
            var testAppPoolName = "TestAppPool" + Guid.NewGuid();
            var testApplicationName = "TestApplication" + Guid.NewGuid();

            _iisEngine.CreateAppPool(testAppPoolName);
            using (var sm = new ServerManager())
            {
                Assert.IsTrue(sm.ApplicationPools.Any(x => x.Name == testAppPoolName), "CreateAppPool does not create application pool");
            }

            _iisEngine.CreateAlias(DefaultSiteName, testApplicationName, testAppPoolName, @"c:\");

            _iisEngine.RemoveEmptyAppPool(testAppPoolName);
            using (var sm = new ServerManager())
            {
                Assert.IsTrue(sm.ApplicationPools.Any(x => x.Name == testAppPoolName), "RemoveEmptyAppPool remove not empty application pool");
            }

            _iisEngine.RemoveAlias(DefaultSiteName, testApplicationName);

            _iisEngine.RemoveEmptyAppPool(testAppPoolName);
            using (var sm = new ServerManager())
            {
                Assert.IsFalse(sm.ApplicationPools.Any(x => x.Name == testAppPoolName), "RemoveEmptyAppPool does not remove empty application pool");
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void SetRecyclingValueToZero_CreateTestAppPoolWithNonZeroRecycligTime_RecyclingTimeIsZero()
        {
            var testAppPoolName = "TestAppPool" + Guid.NewGuid();

            try
            {
                _iisEngine.CreateAppPool(testAppPoolName);
                using (var sm = new ServerManager())
                {
                    sm.ApplicationPools[testAppPoolName].Recycling.PeriodicRestart.Time = TimeSpan.FromDays(1);
                }

                _iisEngine.SetRecyclingValueToZero(testAppPoolName);

                using (var sm = new ServerManager())
                {
                    Assert.AreEqual(sm.ApplicationPools[testAppPoolName].Recycling.PeriodicRestart.Time, TimeSpan.Zero, "SetRecyclingValueToZero doesn't set recycling time to zero");
                }
            }
            finally
            {
                _iisEngine.RemoveEmptyAppPool(testAppPoolName);
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void SetOrphaningForAppPool_CreateTestAppPoolAndSetOrphaningOptionWithDifferentTypes_OrphaningOptionsAreCorrect()
        {
            var testAppPoolName = "TestAppPool" + Guid.NewGuid();
            const string cmdFilePath = @"c:\test.cmd";

            try
            {
                _iisEngine.CreateAppPool(testAppPoolName);

                _iisEngine.SetOrphaningForAppPool(DumpCreationOptions.CreateDump, testAppPoolName, cmdFilePath);

                using (var sm = new ServerManager())
                {
                    Assert.AreEqual(true, sm.ApplicationPools[testAppPoolName].Failure.OrphanWorkerProcess, "SetOrphaningForAppPool doesn't set correct OrphanWorkerProcess for DumpCreationOptions.CreateDump mode");
                    Assert.AreEqual(cmdFilePath, sm.ApplicationPools[testAppPoolName].Failure.OrphanActionExe, "SetOrphaningForAppPool doesn't set correct OrphanActionExe for DumpCreationOptions.CreateDump mode");
                    Assert.AreEqual("%1%", sm.ApplicationPools[testAppPoolName].Failure.OrphanActionParams, "SetOrphaningForAppPool doesn't set correct OrphanActionParams for DumpCreationOptions.CreateDump mode");
                }

                _iisEngine.SetOrphaningForAppPool(DumpCreationOptions.DoNotModifyCurrentOptions, testAppPoolName, cmdFilePath);

                using (var sm = new ServerManager())
                {
                    Assert.AreEqual(true, sm.ApplicationPools[testAppPoolName].Failure.OrphanWorkerProcess, "SetOrphaningForAppPool change OrphanWorkerProcess for DumpCreationOptions.DoNotModifyCurrentOptions mode");
                    Assert.AreEqual(cmdFilePath, sm.ApplicationPools[testAppPoolName].Failure.OrphanActionExe, "SetOrphaningForAppPool change OrphanActionExe for DumpCreationOptions.DoNotModifyCurrentOptions mode");
                    Assert.AreEqual("%1%", sm.ApplicationPools[testAppPoolName].Failure.OrphanActionParams, "SetOrphaningForAppPool change OrphanActionParams for DumpCreationOptions.DoNotModifyCurrentOptions mode");
                }

                _iisEngine.SetOrphaningForAppPool(DumpCreationOptions.DoNotCreateDump, testAppPoolName, cmdFilePath);

                using (var sm = new ServerManager())
                {
                    Assert.AreEqual(false, sm.ApplicationPools[testAppPoolName].Failure.OrphanWorkerProcess, "SetOrphaningForAppPool doesn't set correct OrphanWorkerProcess for DumpCreationOptions.DoNotCreateDump mode");
                    Assert.AreEqual("", sm.ApplicationPools[testAppPoolName].Failure.OrphanActionExe, "SetOrphaningForAppPool doesn't set correct OrphanActionExe for DumpCreationOptions.DoNotCreateDump mode");
                    Assert.AreEqual("", sm.ApplicationPools[testAppPoolName].Failure.OrphanActionParams, "SetOrphaningForAppPool doesn't set correct OrphanActionParams for DumpCreationOptions.DoNotCreateDump mode");
                }
            }
            finally
            {
                _iisEngine.RemoveEmptyAppPool(testAppPoolName);
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureVirtualDirectories_CreateOneVirtDir_VirtDirIsCreatedSuccessfull()
        {
            string virtualDirectoriesTree = "/TestVirtDirectory" + Guid.NewGuid();

            try
            {
                _iisEngine.ConfigureVirtualDirectories(DefaultSiteName, virtualDirectoriesTree, @"c:\");

                using (var sm = new ServerManager())
                {
                    Assert.IsTrue(sm.Sites[DefaultSiteName].Applications[0].VirtualDirectories.Any(x => x.Path == virtualDirectoriesTree), "ConfigureVirtualDirectories creates wrong virtual directories");
                }
            }
            finally
            {
                _iisEngine.RemoveVirtualDirectory(DefaultSiteName, virtualDirectoriesTree);
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureVirtualDirectories_CreateOneVirtDirAndOneSubVirtualDir_VirtDirsAreCreatedSuccessfull()
        {
            string virtualDirectory1 = "/Test1VirtDirectory" + Guid.NewGuid();
            string virtualDirectoriesTree = virtualDirectory1 + "/Test2VirtDirectory" + Guid.NewGuid();

            try
            {
                _iisEngine.ConfigureVirtualDirectories(DefaultSiteName, virtualDirectoriesTree, TestContext.TestDir);

                using (var sm = new ServerManager())
                {
                    Assert.IsTrue(sm.Sites[DefaultSiteName].Applications[0].VirtualDirectories.Any(x => x.Path == virtualDirectory1), "ConfigureVirtualDirectories creates wrong virtual directories");
                    Assert.IsTrue(sm.Sites[DefaultSiteName].Applications[0].VirtualDirectories.Any(x => x.Path == virtualDirectoriesTree), "ConfigureVirtualDirectories creates wrong virtual directories");
                }
            }
            finally
            {
                _iisEngine.RemoveVirtualDirectory(DefaultSiteName, virtualDirectoriesTree);
                _iisEngine.RemoveVirtualDirectory(DefaultSiteName, virtualDirectory1);
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void ConfigureVirtualDirectories_CreateOneVirtDirAndOneSubVirtualDir__CreateThemAgainWithDifferentPath_VirtDirsAreCreatedSuccessfull()
        {
            string virtualDirectory1 = "/Test1VirtDirectory" + Guid.NewGuid();
            string virtualDirectoriesTree = virtualDirectory1 + "/Test2VirtDirectory" + Guid.NewGuid();

            try
            {
                _iisEngine.ConfigureVirtualDirectories(DefaultSiteName, virtualDirectoriesTree, TestContext.TestDir);

                string newTestPath = Path.Combine(TestContext.TestDir, "Test");
                if (!Directory.Exists(newTestPath))
                {
                    Directory.CreateDirectory(newTestPath);
                }

                _iisEngine.ConfigureVirtualDirectories(DefaultSiteName, virtualDirectoriesTree, newTestPath);

                using (var sm = new ServerManager())
                {
                    Assert.IsTrue(sm.Sites[DefaultSiteName].Applications[0].VirtualDirectories.Any(x => x.Path == virtualDirectory1), "ConfigureVirtualDirectories creates wrong virtual directories");
                    Assert.IsTrue(sm.Sites[DefaultSiteName].Applications[0].VirtualDirectories.Any(x => x.Path == virtualDirectoriesTree), "ConfigureVirtualDirectories creates wrong virtual directories");
                    Assert.AreEqual(newTestPath, sm.Sites[DefaultSiteName].Applications[0].VirtualDirectories[virtualDirectoriesTree].PhysicalPath, "ConfigureVirtualDirectories creates wrong virtual directories");
                }
            }
            finally
            {
                _iisEngine.RemoveVirtualDirectory(DefaultSiteName, virtualDirectoriesTree);
                _iisEngine.RemoveVirtualDirectory(DefaultSiteName, virtualDirectory1);
            }
        }
        #endregion

        #region IsAliveHtmEngineTest        
        [TestMethod, Owner(@"FIRM\GrigoryK"), ExpectedException(typeof(ValidateException))]
        public void VerifyAccesToPageByUrl_PageDoesNotExists_ExceptionOccureds()
        {
            const string pageUrl = "http://localhost/NotExistedTestPage.htm";
            _isAliveHtmEngine.VerifyAccesToPageByUrl(pageUrl);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void VerifyAccesToPageByUrl_PageDoesNotExistsButWeIgnoreIt_NoExceptions()
        {
            const string pageUrl = "http://localhost/NotExistedTestPage.htm";
            var isAliveHtmEngine = new IsAliveHtmEngine(new TraceLogger(), true);
            isAliveHtmEngine.VerifyAccesToPageByUrl(pageUrl);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void VerifyAccesToPageByUrl_PageExists_NoExceptions()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testPagePhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                const string pageUrl = "http://localhost//" + TestPageName;
                _isAliveHtmEngine.VerifyAccesToPageByUrl(pageUrl);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), ExpectedException(typeof(FileNotFoundException))]
        public void BackupIsAliveHtmFile_PageDoesNotExists_ExceptionsOccurred()
        {
            var isAliveHtmEngine = new IsAliveHtmEngine(new TraceLogger());

            isAliveHtmEngine.BackupIsAliveHtmFile("/NotExistedTestPage.htm");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageDoesNotExistsButWeIgnoreIt_NoExceptions()
        {
            var isAliveHtmEngine = new IsAliveHtmEngine(new TraceLogger(), true);

            isAliveHtmEngine.BackupIsAliveHtmFile("/NotExistedTestPage.htm");

            isAliveHtmEngine.RestoreIsAliveHtmFile("/NotExistedTestPage.htm");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRestoreIsAliveHtmFile_PageHasNormalName_PageIsRenaimedBack()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                CheckAndRemoveTestPages(iisEngineTestHelper.RootPhysicalPath);

                // Create page with renaimed name
                string testPagePhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                // Run backup for page with normal name
                var isAliveHtmEngine = new IsAliveHtmEngine(new TraceLogger(), true);
                bool res = isAliveHtmEngine.BackupIsAliveHtmFile(testPagePhysicalPath);
                Assert.IsTrue(res);

                // Run restore and check that page with renaimed name still exists
                isAliveHtmEngine.RestoreIsAliveHtmFile(testPagePhysicalPath);
                _isAliveHtmEngine.VerifyAccesToPageByUrl("http://localhost/" + TestPageName);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRestoreIsAliveHtmFile_PageHasRestoredName_PageIsNotRenaimedBack()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                // Create page with renaimed name
                string renaimedTestPagePhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "_" + TestPageName);
                iisEngineTestHelper.CreatePage(renaimedTestPagePhysicalPath);

                // Run backup for page with normal name
                string testPagePhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, TestPageName);
                var isAliveHtmEngine = new IsAliveHtmEngine(new TraceLogger(), true);
                bool res = isAliveHtmEngine.BackupIsAliveHtmFile(testPagePhysicalPath);
                Assert.IsFalse(res);

                // Run restore and check that page with renaimed name still exists
                isAliveHtmEngine.RestoreIsAliveHtmFile(testPagePhysicalPath);
                _isAliveHtmEngine.VerifyAccesToPageByUrl("http://localhost/_" + TestPageName);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInRootFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                CheckAndRemoveTestPages(iisEngineTestHelper.RootPhysicalPath);

                string testPagePhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(string.Empty);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string uniqueTestFolderName = "FolderTest_" + Guid.NewGuid();
                string testFolderPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, uniqueTestFolderName);
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork("/" + uniqueTestFolderName);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInVirtualFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testVirtualFolderPath = "/VirtualFolderTest_" + Guid.NewGuid();
                string testVirtualFolderPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateVirtualFolder("/", testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testVirtualFolderPath);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInApplication_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testApplicationPath = "/ApplicationTest_" + Guid.NewGuid();
                string testApplicationPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testApplicationPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInFolderInApplication_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testApplicationPath = "/ApplicationTest_" + Guid.NewGuid();
                string testApplicationPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                string testFolderPhysicalPath = Path.Combine(testApplicationPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath + "/SubFolderTest");
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInVirtualFolderInApplication_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testApplicationPath = "/ApplicationTest_" + Guid.NewGuid();
                string testApplicationPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                const string testVirtualFolderPath = "/VirtualFolderTest";
                string testVirtualFolderPhysicalPath = Path.Combine(testApplicationPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateVirtualFolder(testApplicationPath, testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath + testVirtualFolderPath);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInApplicationInFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string uniqueTestFolderName = "FolderTest_" + Guid.NewGuid();
                string testFolderPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, uniqueTestFolderName);
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                string testApplicationPath = "/" + uniqueTestFolderName + "/ApplicationTest";
                string testApplicationPhysicalPath = Path.Combine(testFolderPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testApplicationPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInVirtualFolderInFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string uniqueTestFolderName = "FolderTest_" + Guid.NewGuid();
                string testFolderPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, uniqueTestFolderName);
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                string testVirtualFolderPath = "/" + uniqueTestFolderName + "/VirtualFolderTest";
                string testVirtualFolderPhysicalPath = Path.Combine(testFolderPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateVirtualFolder("/", testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testVirtualFolderPath);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInApplicationInVirtualFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testVirtualFolderPath = "/VirtualFolderTest_" + Guid.NewGuid();
                string testVirtualFolderPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateVirtualFolder("/", testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testApplicationPath = testVirtualFolderPath + "/ApplicationTest";
                string testApplicationPhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testApplicationPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInFolderInVirtualFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testVirtualFolderPath = "/VirtualFolderTest_" + Guid.NewGuid();
                string testVirtualFolderPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateVirtualFolder("/", testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testFolderPhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testVirtualFolderPath + "/SubFolderTest");
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInFolderInVirtualFolderInApplication_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testApplicationPath = "/ApplicationTest_" + Guid.NewGuid();
                string testApplicationPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                const string testVirtualFolderPath = "/VirtualFolderTest";
                string testVirtualFolderPhysicalPath = Path.Combine(testApplicationPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateVirtualFolder(testApplicationPath, testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testFolderPhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, "Sub2FolderTest");
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath + testVirtualFolderPath + "/Sub2FolderTest");
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInVirtualFolderInFolderInApplication_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testApplicationPath = "/ApplicationTest_" + Guid.NewGuid();
                string testApplicationPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                string testFolderPhysicalPath = Path.Combine(testApplicationPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                const string testVirtualFolderPath = "/SubFolderTest/VirtualFolderTest";
                string testVirtualFolderPhysicalPath = Path.Combine(testFolderPhysicalPath, "Sub2FolderTest");
                iisEngineTestHelper.CreateVirtualFolder(testApplicationPath, testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath + testVirtualFolderPath);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInFolderInApplicationInVirtualFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testVirtualFolderPath = "/VirtualFolderTest_" + Guid.NewGuid();
                string testVirtualFolderPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateVirtualFolder("/", testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testApplicationPath = testVirtualFolderPath + "/ApplicationTest";
                string testApplicationPhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                string testFolderPhysicalPath = Path.Combine(testApplicationPhysicalPath, "Sub2FolderTest");
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testFolderPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath + "/Sub2FolderTest");
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void BackupAndRenameIsAliveHtmFile_PageInApplicationInFolderInVirtualFolder_Success()
        {
            using (var iisEngineTestHelper = new IisEngineTestHelper())
            {
                string testVirtualFolderPath = "/VirtualFolderTest_" + Guid.NewGuid();
                string testVirtualFolderPhysicalPath = Path.Combine(iisEngineTestHelper.RootPhysicalPath, "FolderTest");
                iisEngineTestHelper.CreateVirtualFolder("/", testVirtualFolderPath, testVirtualFolderPhysicalPath);

                string testFolderPhysicalPath = Path.Combine(testVirtualFolderPhysicalPath, "SubFolderTest");
                iisEngineTestHelper.CreateFolder(testFolderPhysicalPath);

                string testApplicationPath = testVirtualFolderPath + "/SubFolderTest/ApplicationTest";
                string testApplicationPhysicalPath = Path.Combine(testFolderPhysicalPath, "Sub2FolderTest");
                iisEngineTestHelper.CreateApplication(testApplicationPath, testApplicationPhysicalPath);

                string testPagePhysicalPath = Path.Combine(testApplicationPhysicalPath, TestPageName);
                iisEngineTestHelper.CreatePage(testPagePhysicalPath);

                VerifyBackupAndRenamePageWork(testApplicationPath);
            }
        }

        private void CheckAndRemoveTestPages(string folderPath)
        {
            string testPagePhysicalPath = Path.Combine(folderPath, TestPageName);
            string testChangedPagePhysicalPath = Path.Combine(folderPath, "___" + TestPageName);
            if (File.Exists(testPagePhysicalPath))
            {
                File.Delete(testPagePhysicalPath);
            }

            if (File.Exists(testChangedPagePhysicalPath))
            {
                File.Delete(testChangedPagePhysicalPath);
            }
        }

        private void VerifyBackupAndRenamePageWork(string urlPathBeforePage)
        {
            string urlPath = urlPathBeforePage + "/" + TestPageName;
            string pageUrl = "http://localhost" + urlPathBeforePage + "/" + TestPageName;
            string pageRenamedUrl = "http://localhost" + urlPathBeforePage + "/___" + TestPageName;

            try
            {
                bool res = _isAliveHtmEngine.BackupIsAliveHtmFile(urlPath);
                Assert.IsTrue(res);
                VerifyAccesToPageByUrlSeveralTimes(pageRenamedUrl);
            }
            finally
            {
                _isAliveHtmEngine.RestoreIsAliveHtmFile(urlPath);
                VerifyAccesToPageByUrlSeveralTimes(pageUrl);
            }
        }

        private void VerifyAccesToPageByUrlSeveralTimes(string pageUrl)
        {
            int cnt = 0;
            while (true)
            {
                try
                {
                    _isAliveHtmEngine.VerifyAccesToPageByUrl(pageUrl);
                    return;
                }
                catch
                {
                    Trace.TraceInformation("cnt=" + cnt);
                    if (cnt > 20)
                    {
                        throw;
                    }

                    Thread.Sleep(100);
                    cnt++;
                }
            }
        }
        #endregion
    }
}
