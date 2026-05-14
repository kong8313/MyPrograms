using System;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerCommon.UnitTests
{
    [TestClass]
    public class DialerEventNotificationsSendersTest
    {
        //[TestInitialize]
        //public void TestInitialize()
        //{
        //}

        //[TestCleanup]
        //public void TestCleanup()
        //{
        //}

        [TestMethod, Owner(@"FIRM\alm")]
        [ExpectedException(typeof(ArgumentException))]
        public void CompanyIdIsZero_GetSender_ExceptionIsThrown()
        {
            var target = new DialerEventNotificationSenders(null);
            target.GetSender(0 /* companyId */, 0 /* dialerId */); // ArgumentException is expected here.
        }
    }
}