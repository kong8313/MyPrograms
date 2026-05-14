using System;
using System.Diagnostics;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications.Fakes;
using DialerCommon.Logging.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerCommon.UnitTests
{
    [TestClass]
    public class DialerEventNotificationsSenderTest
    {
        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SendEventNotification_FirstAttemptSucceeded_EventSentOnce()
        {
            var count = 0;

            var stubIDialerEvent = new StubIDialerEvent
            {
                SendEventNotificationDialerEventsServiceClient = x => { count++; }
            };

            var target = new DialerEventNotificationsSender(new StubICommonLogger(), 1, 1);
            target.SendEventNotificationThreadProc(stubIDialerEvent, Stopwatch.StartNew());

            Assert.AreEqual(1, count, "SendEventNotificationSynchronously should be invoked once");
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SendEventNotification_SingleException_EventSentInTheSecondAttempt()
        {
            var count = 0;

            var stubIDialerEvent = new StubIDialerEvent
            {
                SendEventNotificationDialerEventsServiceClient = x =>
                {
                    if (count++ == 0)
                    {
                        // The very first attempt throws an exception
                        throw new Exception("Test exception");
                    }
                }
            };

            var target = new DialerEventNotificationsSender(new StubICommonLogger(), 1, 1);
            target.SendEventNotificationThreadProc(stubIDialerEvent, Stopwatch.StartNew());

            Assert.AreEqual(2, count, "SendEventNotificationSynchronously should be invoked twice");
        }

        [TestMethod, Owner(@"FIRM\KirillV")]
        public void SendEventNotification_AllAttemptsFailed_AttemptsCountLimited()
        {
            var count = 0;

            var stubIDialerEvent = new StubIDialerEvent
            {
                SendEventNotificationDialerEventsServiceClient = client =>
                {
                    count++;

                    throw new Exception();
                }
            };

            var target = new DialerEventNotificationsSender(new StubICommonLogger(), 1, 1);
            target.SendEventNotificationThreadProc(stubIDialerEvent, Stopwatch.StartNew());

            Assert.AreEqual(DialerEventNotificationsSender.MaxRetryCount, count,
                string.Format("SendEventNotificationSynchronously should be invoked MaxRetryCount({0}) times", DialerEventNotificationsSender.MaxRetryCount));
        }
    }
}