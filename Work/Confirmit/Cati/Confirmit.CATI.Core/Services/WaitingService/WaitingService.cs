using System;
using System.Threading;

namespace Confirmit.CATI.Core.Services.WaitingService
{
    public class WaitingService : IWaitingService
    {
        public bool Wait(ManualResetEvent stopEvent, int period)
        {
            return !stopEvent.WaitOne(period, false);
        }

        public bool Wait(ManualResetEvent stopEvent, TimeSpan period)
        {
            return !stopEvent.WaitOne(period, false);
        }
    }
}