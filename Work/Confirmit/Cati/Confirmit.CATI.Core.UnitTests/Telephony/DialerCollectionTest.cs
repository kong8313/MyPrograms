using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.BvCallHandlerLibrary.Fakes;
using Confirmit.CATI.Core.Repositories.Interfaces.Fakes;
using Confirmit.CATI.Core.Telephony;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Telephony
{
    [TestClass]
    public class DialerCollectionTest
    {
//        [TestInitialize]
//        public void TestInitialiaze()
//        {
//        }
//
//        [TestCleanup]
//        public void TestCleanup()
//        {
//        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void NotInitialized_DialerIds_ExceptionIsThrown()
        {
            const string expectedMessage = "DialerCollection is not initialized.";

            var target = new DialerCollection(new StubIDialersRepository(), new StubIDialerInstanceFactory());

            try
            {
                var dummy = target.GetDialerIds(DialType.Landline);
                Assert.Fail("InternalErrorException was expected but is not thrown");
            }
            catch (InternalErrorException ex)
            {
                // InternalErrorException is expected
                Assert.AreEqual(expectedMessage, ex.Message, "Exception message is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void NotInitialized_GetDialers_ExceptionIsThrown()
        {
            const string expectedMessage = "DialerCollection is not initialized.";

            var target = new DialerCollection(new StubIDialersRepository(), new StubIDialerInstanceFactory());

            try
            {
                var dummy = target.GetDialers();
                Assert.Fail("InternalErrorException was expected but is not thrown");
            }
            catch (InternalErrorException ex)
            {
                // InternalErrorException is expected
                Assert.AreEqual(expectedMessage, ex.Message, "Exception message is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void NotInitialized_GetDialerById_ExceptionIsThrown()
        {
            const string expectedMessage = "DialerCollection is not initialized.";

            var target = new DialerCollection(new StubIDialersRepository(), new StubIDialerInstanceFactory());

            try
            {
                var dummy = target.GetDialerById(0);
                Assert.Fail("InternalErrorException was expected but is not thrown");
            }
            catch (InternalErrorException ex)
            {
                // InternalErrorException is expected
                Assert.AreEqual(expectedMessage, ex.Message, "Exception message is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void NotInitialized_FirstLoadedDialerApi_ExceptionIsThrown()
        {
            const string expectedMessage = "DialerCollection is not initialized.";

            var target = new DialerCollection(new StubIDialersRepository(), new StubIDialerInstanceFactory());

            try
            {
                var dummy = target.FirstLoadedDialerApi;
                Assert.Fail("InternalErrorException was expected but is not thrown");
            }
            catch (InternalErrorException ex)
            {
                // InternalErrorException is expected
                Assert.AreEqual(expectedMessage, ex.Message, "Exception message is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void NotInitialized_FirstInitializedDialer_ExceptionIsThrown()
        {
            const string expectedMessage = "DialerCollection is not initialized.";

            var target = new DialerCollection(new StubIDialersRepository(), new StubIDialerInstanceFactory());

            try
            {
                var dummy = target.GetFirstInitializedDialer(DialType.Landline);
                Assert.Fail("InternalErrorException was expected but is not thrown");
            }
            catch (InternalErrorException ex)
            {
                // InternalErrorException is expected
                Assert.AreEqual(expectedMessage, ex.Message, "Exception message is not as expected");
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void NotInitialized_IsDialerInitialized_ReturnsFalse()
        {
            var target = new DialerCollection(new StubIDialersRepository(), new StubIDialerInstanceFactory());

            var actual = target.IsDialerInitialized(0);
            Assert.AreEqual(false, actual, "IsDialerInitialized() result value is expected to be 'false'");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void NotInitialized_InitializedDialerExists_ReturnsFalse()
        {
            var target = new DialerCollection(new StubIDialersRepository(), new StubIDialerInstanceFactory());

            var actual = target.InitializedDialerExists(DialType.Landline);
            Assert.AreEqual(false, actual, "InitializedDialerExists() result value is expected to be 'false'");
        }
    }
}