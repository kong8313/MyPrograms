using System.Diagnostics;
using Confirmit.CATI.IntegrationTests.Tests.InstallationTests.Tools;
using Confirmit.Test.Common.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.InstallationTests
{
    [TestClass]
    public class CheckSettingsTest
    {
        private StringGetter _stringGetter;

        [TestInitialize]
        public void TestInitialize()
        {
            _stringGetter = new StringGetter(TestContext);
        }
        
        public TestContext TestContext { get; set; }

        [TestMethod, Owner(@"FIRM\GrigoryK"), CannotWorkInParallel]
        public void AuthorizationValidation_CompareAuthorizationKeysFromDialerConfigFileAndFromInstallation_TheyAreTheSame()
        {
            Trace.TraceInformation("START TEST {0}", TestContext.TestName);
            Trace.TraceInformation("TestDir={0}", TestContext.TestDir);

            string[] authorizationKeysFromConfig = _stringGetter.GetAutorizationKeysFromConfigFile();            

            string authorizationKeyFromProduct = _stringGetter.GetAutorizationKeyFromProductFile();
            Trace.TraceInformation("authorizationKeyFromProduct={0}", authorizationKeyFromProduct);

            foreach (var authorizationKeyFromConfig in authorizationKeysFromConfig)
            {
                Assert.AreEqual(authorizationKeyFromConfig, authorizationKeyFromProduct, @"One of authorization keys in 'assemblies\Backend\DialerConfig.xml' and on authorization key from 'Confirmit.CATI.Setup\Confirmit.CATI.GenericDialerWebService\Product.wxs' are different");
            }
            
        }
    }
}
