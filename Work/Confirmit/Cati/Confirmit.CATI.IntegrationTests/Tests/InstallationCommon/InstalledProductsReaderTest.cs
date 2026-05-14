using System.Diagnostics;
using Confirmit.CATI.Installation.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.InstallationCommon
{
    [TestClass]
    public class InstalledProductsReaderTest
    {
        private InstalledProductsReader _installedProductsReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _installedProductsReader = new InstalledProductsReader();
        }

        

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetInstalledProducts_GetInstalledProductsFromCurrentServer_MethodWorksAndReturnsNonZeroProducts()
        {
            var installedProducts = _installedProductsReader.GetInstalledProducts();
            Trace.TraceInformation("Count of installed products: {0}", installedProducts.Count);

            Assert.IsTrue(installedProducts.Count > 0);
        } 
    }
}