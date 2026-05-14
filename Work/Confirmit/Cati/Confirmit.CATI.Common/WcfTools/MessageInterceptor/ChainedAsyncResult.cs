using System;

namespace Confirmit.CATI.Common.WcfTools.MessageInterceptor
{
    internal delegate IAsyncResult ChainedBeginHandler(TimeSpan timeout, AsyncCallback asyncCallback, object state);
    internal delegate void ChainedEndHandler(IAsyncResult result);

    /// <remarks>
    /// Implementation based on the MSDN sample Custom Message Interceptor:
    /// http://msdn.microsoft.com/en-us/library/ms751495.aspx
    /// </remarks>
    class ChainedAsyncResult : AsyncResult
    {
        private ChainedBeginHandler begin2;
        private ChainedEndHandler end1;
        private ChainedEndHandler end2;
        private readonly TimeoutHelper timeoutHelper;
        private static readonly AsyncCallback begin1Callback = Begin1Callback;
        private static readonly AsyncCallback begin2Callback = Begin2Callback;

        protected ChainedAsyncResult(TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.timeoutHelper = new TimeoutHelper(timeout);
        }

        public ChainedAsyncResult(TimeSpan timeout, AsyncCallback callback, object state, ChainedBeginHandler begin1, ChainedEndHandler end1, ChainedBeginHandler begin2, ChainedEndHandler end2)
            : base(callback, state)
        {
            this.timeoutHelper = new TimeoutHelper(timeout);
            Begin(begin1, end1, begin2, end2);
        }

        protected void Begin(ChainedBeginHandler begin1, ChainedEndHandler end1, ChainedBeginHandler begin2, ChainedEndHandler end2)
        {
            this.end1 = end1;
            this.begin2 = begin2;
            this.end2 = end2;

            IAsyncResult result = begin1(this.timeoutHelper.RemainingTime(), begin1Callback, this);
            if (!result.CompletedSynchronously)
            {
                return;
            }

            if (Begin1Completed(result))
            {
                this.Complete(true);
            }
        }

        private static void Begin1Callback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

            ChainedAsyncResult thisPtr = (ChainedAsyncResult)result.AsyncState;

            bool completeSelf = false;
            Exception completeException = null;

            try
            {
                completeSelf = thisPtr.Begin1Completed(result);
            }
            catch (Exception exception)
            {
                completeSelf = true;
                completeException = exception;
            }

            if (completeSelf)
            {
                thisPtr.Complete(false, completeException);
            }
        }

        private bool Begin1Completed(IAsyncResult result)
        {
            end1(result);

            result = begin2(this.timeoutHelper.RemainingTime(), begin2Callback, this);
            if (!result.CompletedSynchronously)
            {
                return false;
            }

            end2(result);
            return true;
        }

        private static void Begin2Callback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
                return;

            ChainedAsyncResult thisPtr = (ChainedAsyncResult)result.AsyncState;

            Exception completeException = null;

            try
            {
                thisPtr.end2(result);
            }
            catch (Exception exception)
            {
                completeException = exception;
            }

            thisPtr.Complete(false, completeException);
        }

        public static void End(IAsyncResult result)
        {
            End<ChainedAsyncResult>(result);
        }
    }
}
