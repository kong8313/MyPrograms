using System;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Console.LightweightTelephony;

namespace CustomDialerDemo
{
    public class CustomDialerDemo : ICustomDialer
    {        
        public event EventHandler<CallStatusChangedEventArgs> CallStatusChanged;

        public void Dial(string phoneNumber)
        {
            Trace.TraceInformation("Dial: " + phoneNumber);            
            ThreadPool.QueueUserWorkItem(DialingProcess);
        }

        private void DialingProcess(object state)
        {
            RandomSleep();

            CustomCallOutcome callOutcome = RandomOutcome();

            Trace.TraceInformation("Call outcome: " + callOutcome);

            RaiseCallStatusChangedEvent(callOutcome);
        }

        private void RandomSleep()
        {
            const int minSleepTime = 1000; // 1000ms = 1 second
            const int maxSleepTime = 5000; // 5000ms = 5 seconds

            var sleepTime = new Random().Next(minSleepTime, maxSleepTime);
            Thread.Sleep(sleepTime);
        }

        private CustomCallOutcome RandomOutcome()
        {
            var allowedOutcomes = new[] { CustomCallOutcome.Connected, CustomCallOutcome.Busy, CustomCallOutcome.NoReply };

            var randomOutcomeIdx = new Random().Next(0, allowedOutcomes.Length);

            return allowedOutcomes[randomOutcomeIdx];
        }

        private void RaiseCallStatusChangedEvent(CustomCallOutcome callOutcome)
        {
            if (CallStatusChanged != null)
            {
                CallStatusChanged(this, new CallStatusChangedEventArgs { CustomCallStatus = callOutcome });
            }
        }

        public void HangUp()
        {
            Trace.TraceInformation("HangUp");
        }
    }
}