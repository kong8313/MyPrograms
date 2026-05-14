using System;
using System.Threading;
using Confirmit.CATI.Core.Services.WaitingService;

namespace Confirmit.CATI.Core.Services.WaitingService.Fakes
{
    public class StubIWaitingService : IWaitingService 
    {
        private IWaitingService _inner;

        public StubIWaitingService()
        {
            _inner = null;
        }

        public IWaitingService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool WaitManualResetEventInt32Delegate(ManualResetEvent stopEvent, int period);
        public WaitManualResetEventInt32Delegate WaitManualResetEventInt32;

        bool IWaitingService.Wait(ManualResetEvent stopEvent, int period)
        {


            if (WaitManualResetEventInt32 != null)
            {
                return WaitManualResetEventInt32(stopEvent, period);
            } else if (_inner != null)
            {
                return ((IWaitingService)_inner).Wait(stopEvent, period);
            }

            return default(bool);
        }

        public delegate bool WaitManualResetEventTimeSpanDelegate(ManualResetEvent stopEvent, TimeSpan period);
        public WaitManualResetEventTimeSpanDelegate WaitManualResetEventTimeSpan;

        bool IWaitingService.Wait(ManualResetEvent stopEvent, TimeSpan period)
        {


            if (WaitManualResetEventTimeSpan != null)
            {
                return WaitManualResetEventTimeSpan(stopEvent, period);
            } else if (_inner != null)
            {
                return ((IWaitingService)_inner).Wait(stopEvent, period);
            }

            return default(bool);
        }

    }
}