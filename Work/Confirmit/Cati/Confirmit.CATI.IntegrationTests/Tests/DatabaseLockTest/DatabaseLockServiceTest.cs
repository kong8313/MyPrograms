using System;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseLockTest
{
    [TestClass]
    public class DatabaseLockServiceTest : BaseMockedIntegrationTest
    {
        private const int LockTimeout = 4000;
        private readonly Random _random = new Random();

        [TestMethod, Owner(@"FIRM\alm")]
        public void BvAppLockRecordShouldNotBeDeletedByDefault()
        {
            var appLockService = new DatabaseAppLockService(new ProcessAndEnvironmentInfo());

            var expectedResourceName = "resource_" + _random.Next(100, 99999);
            var expectedOwnerName = "owner_" + _random.Next(100, 99999);

            using (var dbLock = DatabaseLockService.CreateLock(
                expectedResourceName,
                expectedOwnerName,
                LockTimeout))
            {
                if (!dbLock.TryEnterLock())
                {
                    Assert.Fail("TryEnterLock is failed");
                }

                var bvAppLocksEntity = appLockService.WhoLocked(expectedResourceName);

                Assert.AreEqual(expectedResourceName, bvAppLocksEntity.ResourceName,
                    "ResourceName is not as expected");

                Assert.AreEqual(expectedOwnerName, bvAppLocksEntity.ResourceOwner,
                    "ResourceOwner is not as expected");
            }

            // The record in the BvAppLock table should still exists after the dbLock is disposed

            var bvAppLocksEntity2 = appLockService.WhoLocked(expectedResourceName);

            Assert.AreEqual(expectedResourceName, bvAppLocksEntity2.ResourceName,
                "ResourceName is not as expected");

            Assert.AreEqual(expectedOwnerName, bvAppLocksEntity2.ResourceOwner,
                "ResourceOwner is not as expected");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void BvAppLockRecordShouldBeDeletedIfTheOptionIsSpecified()
        {
            var appLockService = new DatabaseAppLockService(new ProcessAndEnvironmentInfo());

            var expectedResourceName = "resource_" + _random.Next(100, 99999);
            var expectedOwnerName = "owner_" + _random.Next(100, 99999);

            using (var dbLock = DatabaseLockService.CreateLock(
                expectedResourceName,
                expectedOwnerName,
                LockTimeout,
                true))
            {
                if (!dbLock.TryEnterLock())
                {
                    Assert.Fail("TryEnterLock is failed");
                }

                var bvAppLocksEntity = appLockService.WhoLocked(expectedResourceName);

                Assert.AreEqual(expectedResourceName, bvAppLocksEntity.ResourceName,
                    "ResourceName is not as expected");

                Assert.AreEqual(expectedOwnerName, bvAppLocksEntity.ResourceOwner,
                    "ResourceOwner is not as expected");
            }

            // The record in the BvAppLock table should not exists after the dbLock is disposed

            var bvAppLocksEntity2 = appLockService.WhoLocked(expectedResourceName);

            Assert.IsNull(bvAppLocksEntity2,
                "bvAppLocksEntity2 is expected to be [null]");
        }
    }
}