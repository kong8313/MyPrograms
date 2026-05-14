using System;
using System.Threading;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.ActivityLogging
{
    public class TestManagementEventWithoutDetails : ManagementActivityEvent<NoManagementParameters>
    {
        public TestManagementEventWithoutDetails() : base(ManagementEventCategory.System, ManagementEvent.Schedule)
        {
        }
    }

    public class TestManagementParameters : ManagementActivityEventDetails
    {
    }

    public class TestManagementEventWithDetails : ManagementActivityEvent<TestManagementParameters>
    {
        public TestManagementEventWithDetails() : base(ManagementEventCategory.System, ManagementEvent.Schedule)
        {
        }
    }

    [TestClass]
    public class ManagementActivityEventDetailsLoggingTests
    {
        private const int AdditionalLoggingTimeout = 5;

        [TestInitialize]
        public void TestInitialize()
        {
            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;

            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
            BackendInstance.Current = null;
        }

        [TestMethod, Owner(@"firm\egors")]
        public void ManagementActivityEventDetailsToXmlReturnsNotNullIfTimingsAreAvailable()
        {
            var evt = new TestManagementEventWithoutDetails();
            Thread.Sleep(TimeSpan.FromMilliseconds(BaseEventDetails.DefaultMinimumTimingToIgnoreInMs + AdditionalLoggingTimeout));
            evt.AddTiming("Foo");
            Assert.IsNotNull(evt.DetailsToXml());
        }

        [TestMethod, Owner(@"firm\egors")]
        public void ManagementActivityEventDetailsToXmlReturnsNotNullIfMessagesAreAvailable()
        {
            var evt = new TestManagementEventWithoutDetails();
            evt.Details.AddMessage("Foo");
            Assert.IsNotNull(evt.DetailsToXml());
        }

        [TestMethod, Owner(@"firm\egors")]
        public void ManagementActivityEventDetailsToXmlReturnsNotNullIfDetailsObjectIsNotNoManagementParameters()
        {
            var evt = new TestManagementEventWithDetails();
            Assert.IsNotNull(evt.DetailsToXml());
        }

        [TestMethod, Owner(@"firm\egors")]
        public void ManagementActivityEventDetailsToXmlReturnsNullIfThereIsNothingToLog()
        {
            var evt = new TestManagementEventWithoutDetails();
            Assert.IsNull(evt.DetailsToXml());
        }
    }
}