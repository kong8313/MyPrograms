using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Confirmit.CATI.Common.WcfTools.MessageInterceptor
{
    /// <summary>
    /// ChannelFactory that performs message Interception
    /// </summary>
    /// <remarks>
    /// Implementation based on the MSDN sample Custom Message Interceptor:
    /// http://msdn.microsoft.com/en-us/library/ms751495.aspx
    /// </remarks>
    internal class InterceptingChannelFactory<TChannel> : ChannelFactoryBase<TChannel>
    {
        private readonly ChannelMessageInterceptor interceptor;
        private readonly IChannelFactory<TChannel> innerChannelFactory;

        public InterceptingChannelFactory(ChannelMessageInterceptor interceptor, BindingContext context)
        {
            this.interceptor = interceptor;
            this.innerChannelFactory = context.BuildInnerChannelFactory<TChannel>();
            if (this.innerChannelFactory == null)
            {
                throw new InvalidOperationException("InterceptingChannelFactory requires an inner IChannelFactory.");
            }
        }

        public ChannelMessageInterceptor Interceptor
        {
            get { return this.interceptor; }
        }

        public override T GetProperty<T>()
        {
            return base.GetProperty<T>() ?? this.innerChannelFactory.GetProperty<T>();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.innerChannelFactory.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.innerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            this.innerChannelFactory.EndOpen(result);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            this.innerChannelFactory.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            var timeoutHelper = new TimeoutHelper(timeout);
            base.OnClose(timeoutHelper.RemainingTime());
            this.innerChannelFactory.Close(timeoutHelper.RemainingTime());
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedAsyncResult(timeout, callback, state, base.OnBeginClose, base.OnEndClose, innerChannelFactory.BeginClose, innerChannelFactory.EndClose);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override TChannel OnCreateChannel(EndpointAddress to, Uri via)
        {
            TChannel innerChannel = this.innerChannelFactory.CreateChannel(to, via);
            if (typeof(TChannel) == typeof(IOutputChannel))
            {
                return (TChannel)(object)new InterceptingOutputChannel(this, (IOutputChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new InterceptingRequestChannel(this, (IRequestChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (TChannel)(object)new InterceptingDuplexChannel(this, Interceptor, (IDuplexChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return (TChannel)(object)new InterceptingOutputSessionChannel(this, (IOutputSessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return (TChannel)(object)new InterceptingRequestSessionChannel(this,
                    (IRequestSessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new InterceptingDuplexSessionChannel(this, Interceptor, (IDuplexSessionChannel)innerChannel);
            }

            throw new InvalidOperationException();
        }

        internal class InterceptingOutputChannel : InterceptingChannelBase<IOutputChannel>, IOutputChannel
        {
            public InterceptingOutputChannel(InterceptingChannelFactory<TChannel> factory, IOutputChannel innerChannel)
                : base(factory, factory.Interceptor, innerChannel)
            {
            }

            public EndpointAddress RemoteAddress
            {
                get
                {
                    return this.InnerChannel.RemoteAddress;
                }
            }

            public Uri Via
            {
                get
                {
                    return this.InnerChannel.Via;
                }
            }

            public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
            {
                return BeginSend(message, DefaultSendTimeout, callback, state);
            }

            public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                this.OnSend(ref message);
                return new SendAsyncResult(this, message, timeout, callback, state);
            }

            public void EndSend(IAsyncResult result)
            {
                SendAsyncResult.End(result);
            }

            public void Send(Message message)
            {
                Send(message, DefaultSendTimeout);
            }

            public void Send(Message message, TimeSpan timeout)
            {
                this.OnSend(ref message);

                if (message != null)
                {
                    this.InnerChannel.Send(message, timeout);
                }
            }

            internal class SendAsyncResult : AsyncResult
            {
                private readonly IOutputChannel channel;
                private readonly AsyncCallback sendCallback = OnSend;
                
                public SendAsyncResult(IOutputChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state) 
                    : base(callback, state)
                {
                    if (message != null)
                    {
                        this.channel = channel;

                        IAsyncResult sendResult = channel.BeginSend(message, timeout, sendCallback, this);
                        if (!sendResult.CompletedSynchronously)
                        {
                            return;
                        }

                        CompleteSend(sendResult);
                    }

                    this.Complete(true);
               }

                void CompleteSend(IAsyncResult result)
                {
                    channel.EndSend(result);
                }

                static void OnSend(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                    {
                        return;
                    }

                    var thisPtr = (SendAsyncResult)result.AsyncState;
                    Exception completionException = null;

                    try
                    {
                        thisPtr.CompleteSend(result);
                    }
                    catch (Exception e)
                    {
                        completionException = e;
                    }

                    thisPtr.Complete(false, completionException);
                }

                public static void End(IAsyncResult result)
                {
                    End<SendAsyncResult>(result);
                }
            }
        }

        public class InterceptingRequestChannel : InterceptingChannelBase<IRequestChannel>, IRequestChannel
        {
            public InterceptingRequestChannel(InterceptingChannelFactory<TChannel> factory, IRequestChannel innerChannel)
                : base(factory, factory.Interceptor, innerChannel)
            {
            }

            public EndpointAddress RemoteAddress
            {
                get
                {
                    return this.InnerChannel.RemoteAddress;
                }
            }

            public Uri Via
            {
                get
                {
                    return this.InnerChannel.Via;
                }
            }

            public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
            {
                return BeginRequest(message, this.DefaultSendTimeout, callback, state);
            }

            public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                this.OnSend(ref message);
                return new RequestAsyncResult(this, message, timeout, callback, state);
            }

            public Message EndRequest(IAsyncResult result)
            {
                Message reply = RequestAsyncResult.End(result);
                this.OnReceive(ref reply);
                return reply;
            }

            public Message Request(Message message)
            {
                return Request(message, this.DefaultSendTimeout);
            }

            public Message Request(Message message, TimeSpan timeout)
            {
                this.OnSend(ref message);
                Message reply = null;
                if (message != null)
                {
                    reply = this.InnerChannel.Request(message);
                }

                this.OnReceive(ref reply);
                return reply;
            }

            internal class RequestAsyncResult : AsyncResult
            {
                private Message replyMessage;

                private readonly InterceptingRequestChannel channel;
                private readonly AsyncCallback requestCallback = OnRequest;

                public RequestAsyncResult(InterceptingRequestChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
                    : base(callback, state)
                {
                    if (message != null)
                    {
                        this.channel = channel;

                        IAsyncResult requestResult = channel.InnerChannel.BeginRequest(message, timeout, requestCallback, this);
                        if (!requestResult.CompletedSynchronously)
                        {
                            return;
                        }

                        CompleteRequest(requestResult);
                    }

                    this.Complete(true);
                }

                private void CompleteRequest(IAsyncResult result)
                {
                    replyMessage = channel.InnerChannel.EndRequest(result);
                }

                private static void OnRequest(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                    {
                        return;
                    }

                    RequestAsyncResult thisPtr = (RequestAsyncResult)result.AsyncState;
                    Exception completionException = null;

                    try
                    {
                        thisPtr.CompleteRequest(result);
                    }
                    catch (Exception e)
                    {
                        completionException = e;
                    }

                    thisPtr.Complete(false, completionException);
                }

                public static Message End(IAsyncResult result)
                {
                    var thisPtr = End<RequestAsyncResult>(result);
                    return thisPtr.replyMessage;
                }
            }

        }

        internal class InterceptingOutputSessionChannel : InterceptingOutputChannel, IOutputSessionChannel
        {
            private readonly IOutputSessionChannel innerSessionChannel;

            public InterceptingOutputSessionChannel(InterceptingChannelFactory<TChannel> factory, IOutputSessionChannel innerChannel)
                : base(factory, innerChannel)
            {
                this.innerSessionChannel = innerChannel;
            }

            public IOutputSession Session
            {
                get
                {
                    return this.innerSessionChannel.Session;
                }
            }
        }

        internal class InterceptingRequestSessionChannel : InterceptingRequestChannel, IRequestSessionChannel
        {
            private readonly IRequestSessionChannel innerSessionChannel;

            public InterceptingRequestSessionChannel(InterceptingChannelFactory<TChannel> factory, IRequestSessionChannel innerChannel)
                : base(factory, innerChannel)
            {
                this.innerSessionChannel = innerChannel;
            }

            public IOutputSession Session
            {
                get
                {
                    return this.innerSessionChannel.Session;
                }
            }
        }
    }
}
