using System;
using System.Diagnostics;
using BvCallHandlerLibrary;
using Confirmit.CATI.Core.Misc;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests
{
    /// <summary>
    ///This is a test class for BvCallHandlerRootTest and is intended
    ///to contain all BvCallHandlerRootTest Unit Tests
    ///</summary>
    [TestClass]
    public class BvCallHandlerRootTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// It's not a test in fact. It's used in order to get error texts for review or other purposes.
        ///</summary>
        [TestMethod, Owner(@"FIRM\alm")]
        [Ignore] // The test is for manual use only.
        public void TraceAllStartAudioMonitorErrors()
        {
            var backendInstance = new BackendInstance { CompanyId = 9 };
            BackendInstance.Current = backendInstance;
            //Isolate.WhenCalled(() => BackendInstance.Current.CompanyId).WillReturn(9);

            foreach (var errorCode in (DialerErrorCode[])Enum.GetValues(typeof(DialerErrorCode)))
            {
                try
                {
                    AudioMonitoring.ProcessStartAudioMonitorError(3, "Super1", 1, "+44987654321", errorCode);
                }
                catch (Exception ex)
                {
                    Trace.TraceInformation(ex.ToString());
                }
            }

            try
            {
                // And one value that does not defined in the DialerErrorCode enum
                const DialerErrorCode unknownValue = (DialerErrorCode)300;

                Assert.IsFalse(Enum.IsDefined(typeof(DialerErrorCode), unknownValue),
                    "DialerErrorCode should not be defined in the DialerErrorCode enum according to the test logic");

                AudioMonitoring.ProcessStartAudioMonitorError(3, "Super1", 1, "+44987654321", unknownValue);
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.ToString());
            }
        }
    }
}
