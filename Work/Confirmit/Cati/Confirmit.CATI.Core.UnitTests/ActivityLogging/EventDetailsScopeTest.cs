using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.ActivityLogging
{
    [TestClass]
    public class EventDetailsScopeTest
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

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddTiming_UsingInsideLogScope_TimingIsLogged()
        {
            const string message = "Timing which is logged inside of using of EventDetailsScope";

            EventDetailsScope.Current.AddTiming("Timing which is logged before using of EventDetailsScope");

            var evt = new ScheduleEvent();
            using( new EventDetailsScope(evt.Details))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(BaseEventDetails.DefaultMinimumTimingToIgnoreInMs + AdditionalLoggingTimeout));
                EventDetailsScope.Current.AddTiming(message);
            }

            EventDetailsScope.Current.AddTiming("Timing which is logged after using of EventDetailsScope");

            Assert.AreEqual(1, evt.Details.Timings.Count, "Different count of messages." );
            CollectionAssert.AreEqual(new[] { message }, evt.Details.Timings.Select(TrimTimout).ToArray());
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddTiming_UsingInsideChildLogScope_TimingsAreLoggedCorrectly()
        {
            const string beforeMessage = "Timing which is logged before using of child EventDetailsScope";
            const string childMessage = "Timing which is logged inside of using of child EventDetailsScope";
            const string afterMessage = "Timing which is logged after using of child EventDetailsScope";

            var parentEvt = new ScheduleEvent();
            var childEvt = new ScheduleEvent(); 
            
            using (new EventDetailsScope(parentEvt.Details))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(BaseEventDetails.DefaultMinimumTimingToIgnoreInMs + AdditionalLoggingTimeout));
                EventDetailsScope.Current.AddTiming(beforeMessage);

                using (new EventDetailsScope(childEvt.Details))
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(BaseEventDetails.DefaultMinimumTimingToIgnoreInMs + AdditionalLoggingTimeout));
                    EventDetailsScope.Current.AddTiming(childMessage);
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(BaseEventDetails.DefaultMinimumTimingToIgnoreInMs + AdditionalLoggingTimeout));
                EventDetailsScope.Current.AddTiming(afterMessage);

            }

            CollectionAssert.AreEqual(new[] { childMessage }, childEvt.Details.Timings.Select(TrimTimout).ToArray());
            CollectionAssert.AreEqual(new[] { beforeMessage, afterMessage }, parentEvt.Details.Timings.Select(TrimTimout).ToArray());
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddMessage_UsingInsideLogScope_TimingIsLogged()
        {
            const string message = "Message which is logged inside of using of EventDetailsScope";

            EventDetailsScope.Current.AddMessage("Message which is logged before using of EventDetailsScope");

            var evt = new ScheduleEvent();
            using (new EventDetailsScope(evt.Details))
            {
                EventDetailsScope.Current.AddMessage(message);
            }

            EventDetailsScope.Current.AddMessage("Message which is logged after using of EventDetailsScope");

            CollectionAssert.AreEqual(new[] { message }, evt.Details.Messages.ToArray());
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddMessage_UsingInsideChildLogScope_TimingsAreLoggedCorrectly()
        {
            const string beforeMessage = "Message which is logged before using of child EventDetailsScope";
            const string childMessage = "Message which is logged inside of using of child EventDetailsScope";
            const string afterMessage = "Message which is logged after using of child EventDetailsScope";

            var parentEvt = new ScheduleEvent();
            var childEvt = new ScheduleEvent();

            using (new EventDetailsScope(parentEvt.Details))
            {
                EventDetailsScope.Current.AddMessage(beforeMessage);

                using (new EventDetailsScope(childEvt.Details))
                {
                    EventDetailsScope.Current.AddMessage(childMessage);
                }

                EventDetailsScope.Current.AddMessage(afterMessage);

            }

            CollectionAssert.AreEqual(new[] { childMessage }, childEvt.Details.Messages.ToArray());
            CollectionAssert.AreEqual(new[] { beforeMessage, afterMessage }, parentEvt.Details.Messages.ToArray());
        }

        public static string TrimTimout( string str)
        {
            const string patter = @"^(?<text>.*): \d+$";
            return Regex.Match(str, patter).Groups["text"].Value;
        }
    }
}
