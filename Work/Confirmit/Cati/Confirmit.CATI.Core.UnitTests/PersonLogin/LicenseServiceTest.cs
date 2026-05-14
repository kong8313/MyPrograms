using System;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.PersonLogin;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.PersonLogin
{
    [TestClass]
    public class LicenseServiceTest
    {
        [TestMethod, Owner(@"FIRM\alm")]
        public void CheckLicenseForIvrAgent_NoExceptionIsThrown()
        {
            var target = new LicenseService(null, null);

            target.CheckLicense(AgentType.IvrAgent);
        }

        //TODO: Finish test implementation
        [Ignore, TestMethod, Owner(@"FIRM\alm")]
        public void CheckLicenseForLiveAgent_LimitIsNotReached_NoExceptionIsThrown()
        {
            var target = new LicenseService(null, null);

            target.CheckLicense(AgentType.LiveAgent);
        }

        //TODO: Finish test implementation
        [Ignore, TestMethod, Owner(@"FIRM\alm")]
        public void CheckLicenseForLiveAgent_LimitIsReached_UserMessageExceptionIsThrown()
        {
            var target = new LicenseService(null, null);

            try
            {
                target.CheckLicense(AgentType.LiveAgent);
                Assert.Fail("UserMessageException was expected but is not thrown");
            }
            catch (UserMessageException)
            {
                // UserMessageException is expected

                //TODO: Check the exception text here
//                Assert.AreEqual(aa, ex.Message);
            }
        }
    }
}