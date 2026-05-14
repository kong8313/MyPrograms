using System;
using System.ServiceModel.Channels;

namespace Confirmit.CATI.Common.WcfTools.MessageInterceptor
{
    /// <summary>
    /// Base channel class that uses an ChannelMessageInterceptor
    /// </summary>
    /// <remarks>
    /// Implementation based on the MSDN sample Custom Message Interceptor:
    /// http://msdn.microsoft.com/en-us/library/ms751495.aspx
    /// </remarks>
    internal class InterceptingChannelBase<TChannel> : ChannelBase where TChannel : class, IChannel
    {
        private readonly ChannelMessageInterceptor interceptor;
        private readonly TChannel innerChannel;

        protected InterceptingChannelBase(
            ChannelManagerBase manager, ChannelMessageInterceptor interceptor, TChannel innerChannel)
            : base(manager)
        {
            if (innerChannel == null)
            {
                throw new ArgumentException("InterceptingChannelBase requires a non-null inner channel.", "innerChannel");
            }

            this.interceptor = interceptor;
            this.innerChannel = innerChannel;
        }

        protected TChannel InnerChannel
        {
            get
            {
                return this.innerChannel;
            }
        }

        public override T GetProperty<T>()
        {
            return base.GetProperty<T>() ?? this.InnerChannel.GetProperty<T>();
        }

        protected override void OnAbort()
        {
            this.innerChannel.Abort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginClose(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannel.BeginOpen(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            this.innerChannel.Close(timeout);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            this.innerChannel.EndClose(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.innerChannel.EndOpen(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.innerChannel.Open(timeout);
        }

        protected void OnReceive(ref Message message)
        {
            this.interceptor.OnReceive(ref message);
        }

        protected void OnSend(ref Message message)
        {
            this.interceptor.OnSend(ref message);
        }
    }
}