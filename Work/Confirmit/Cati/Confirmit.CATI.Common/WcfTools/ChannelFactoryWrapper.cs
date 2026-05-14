using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Common.WcfTools
{
    /// <summary>
    /// Caching strategy.
    /// </summary>
    public enum CachingStrategy
    {
        /// <summary>
        /// Cache and reuse both ChannelFactory and channels. Recommended for HTTPS connection to keep existing connections open.
        /// </summary>
        FactoryAndChannels,

        /// <summary>
        /// Cache and reuse ChannelFactory only. Recommended for HTTP connection.
        /// </summary>
        Factory,

        /// <summary>
        /// New channel and ChannelFactory will be created for each call.
        /// </summary>
        None
    }

    /// <summary>
    /// Wrapper for <see cref="ChannelFactory"/> and channels. 
    /// Provides caching of channels and factories, error handling, logging, 
    /// ability to adjust web service URL with company ID, ability to set login and password, etc.
    /// </summary>
    /// <typeparam name="T">The type of channel produced by the channel factory wrapper.</typeparam>
    public class ChannelFactoryWrapper<T> : IChannelFactoryWrapper<T> where T : class
    {
        private const string HttpsString = "https";
        private readonly object _channelLock = new object();
        private readonly object _factoryLock = new object();
        private readonly IChannelFactoryWrapperConfiguration _configuration;
        private readonly WcfExecutor wcfExecutor;
        private readonly ILogger logger;
        private T channel;
        private ChannelFactory<T> factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Confirmit.CATI.Common.WcfTools.ChannelFactoryWrapper{T}"/> class.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        /// <param name="logger">The logger to use.</param>
        public ChannelFactoryWrapper(IChannelFactoryWrapperConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            this.logger = logger;
            wcfExecutor = new WcfExecutor(configuration.LogAllCalls, configuration.LogExceptions, logger);
        }

        public Uri GetFactoryUri()
        {
            if (factory == null)
            {
                return null;
            }

            return factory.Endpoint.Address.Uri;
        }

        private ChannelFactory<T> GetChannelFactory()
        {
            if (_configuration.CachingStrategy == CachingStrategy.None)
            {
                return CreateNewChannelFactory();
            }

            if (factory == null)
            {
                lock (_factoryLock)
                {
                    if (factory == null)
                    {
                        factory = CreateNewChannelFactory();
                        logger.Log(string.Format("{0} channel factory initialized.", typeof(T).Name), TraceEventType.Information);
                    }
                }
            }

            return factory;
        }

        private ChannelFactory<T> CreateNewChannelFactory()
        {
            EventDetailsScope.Current.AddTiming("Begin ChannelFactoryWrapper.CreateNewChannelFactory");
            var channelFactory = new ChannelFactory<T>(_configuration.EndpointConfigurationName);

            channelFactory.Endpoint.Address = 
                new EndpointAddress(_configuration.AdjustEndpointUri(channelFactory.Endpoint.Address.ToString()));

            if (_configuration.UseLogicalAddressReplacementForHttps())
            {
                ChangeLogicalAddressForHttps(channelFactory);
            }

            _configuration.InitializeClientCredentials(channelFactory.Credentials);

            var behaviors = _configuration.EndpointBehaviors;

            if (behaviors != null)
            {
                foreach (var behavior in behaviors)
                {
                    channelFactory.Endpoint.Behaviors.Add(behavior);
                }
            }
            
            if (_configuration.KeepAliveEnabled)
            {
                var binding = channelFactory.Endpoint.Binding;
                if (binding != null)
                {
                    var customBinding = new CustomBinding(binding);
                    var httpTransportBindingElement = customBinding.Elements.Find<HttpTransportBindingElement>();
                    if (httpTransportBindingElement != null)
                    {
                        httpTransportBindingElement.KeepAliveEnabled = true;
                    }
                    
                    var httpsTransportBindingElement = customBinding.Elements.Find<HttpsTransportBindingElement>();
                    if (httpsTransportBindingElement != null)
                    {
                        httpsTransportBindingElement.KeepAliveEnabled = true;
                    }
                    
                    channelFactory.Endpoint.Binding = customBinding;
                }
            }

            channelFactory.Open();

            EventDetailsScope.Current.AddTiming("End ChannelFactoryWrapper.CreateNewChannelFactory");

            return channelFactory;
        }

        private void ChangeLogicalAddressForHttps(ChannelFactory<T> channelFactory)
        {
            bool isSecure = channelFactory.Endpoint.Address.Uri.Scheme.Equals(HttpsString, StringComparison.InvariantCultureIgnoreCase);

            if (isSecure)
            {
                // endpoint.Address property actually contains Logical address.
                // ListenUri is real address endpoint is listening on.
                // So, we should change Address to the something that is not
                // depend on port number or URI scheme. To have the same Logical address
                // in case when SSL Accelerator installed or not.
                // In the same time we should leave ListenUri without changes.
                // 
                // PS:
                //     See http://msdn.microsoft.com/en-us/magazine/cc163412.aspx
                var listeningUri = channelFactory.Endpoint.Address.Uri;
                var logicalUri = new Uri("urn://" + listeningUri.Host + listeningUri.PathAndQuery);

                channelFactory.Endpoint.Address = new EndpointAddress(logicalUri);
                channelFactory.Endpoint.Behaviors.Add(new ClientViaBehavior(listeningUri));
            }
        }

        /// <summary>
        /// Gets the WCF client proxy. Either cached or newly constructed - depending on caching option in constructor.
        /// </summary>
        /// <returns>WCF client proxy.</returns>
        public T GetChannel()
        {
            T result;

            if (_configuration.CachingStrategy == CachingStrategy.FactoryAndChannels)
            {
                if (channel == null)
                {
                    lock (_channelLock)
                    {
                        if (channel == null)
                        {
                            EventDetailsScope.Current.AddTiming("Start creating WCF channel");

                            channel = GetChannelFactory().CreateChannel();
                            var clientChannel = (IClientChannel)channel;
                            clientChannel.Faulted += OnChannelFaulted;
                            clientChannel.Closed += OnChannelClosed;
                            //clientChannel.Open();

                            logger.Log(string.Format("{0} client proxy initialized.", typeof(T).Name), TraceEventType.Information);

                            EventDetailsScope.Current.AddTiming("WCF channel was created");
                        }
                    }
                }

                result = channel;
            }
            else
            {
                EventDetailsScope.Current.AddTiming("Start creating WCF channel");

                result = GetChannelFactory().CreateChannel();

                EventDetailsScope.Current.AddTiming("WCF channel was created");

                // When we do not cache channels (create new channel for each request) - it is explicitly opened in WcfExecutor.
            }

            return result;
        }

        private void OnChannelClosed(object sender, EventArgs e)
        {
            logger.Log(string.Format("{0} client proxy closed.", typeof(T).Name), TraceEventType.Information);

            // Channel in closed state can not be used for communication. 
            // So we release it here and it will be reinitialized on the next request.
            channel = null;
        }

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            logger.Log(string.Format("{0} client proxy faulted.", typeof(T).Name), TraceEventType.Warning);
            var clientChannel = channel as IClientChannel;

            // Channel in faulted state can not be used for communication. 
            // Release channel here and it will be reinitialized on the next request.
            if (clientChannel != null)
            {
                clientChannel.Abort();
            }

            channel = null;
        }

        /// <summary>
        /// Releases inner channel factory and closes all active connections of all channels created by this factory.
        /// </summary>
        public void Release()
        {
            wcfExecutor.ReleaseCommunicationObject(factory);
            factory = null;
        }

        /// <summary>
        /// Executes the specified action over the channel stored in this class.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="methodName"></param>
        public void Execute(Action<T> action, [CallerMemberName]string methodName="")
        {
            switch (_configuration.CachingStrategy)
            {
                case CachingStrategy.FactoryAndChannels:
                    wcfExecutor.Execute(GetChannel, action, methodName);
                    break;
                case CachingStrategy.Factory:
                case CachingStrategy.None:
                    wcfExecutor.ExecuteAndRelease(GetChannel, action, methodName);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Executes the specified function over the channel stored in this class.
        /// </summary>
        /// <param name="function">The function to execute.</param>
        /// <param name="methodName"></param>
        public TResult Execute<TResult>(Func<T, TResult> function, [CallerMemberName]string methodName="")
        {
            switch (_configuration.CachingStrategy)
            {
                case CachingStrategy.FactoryAndChannels:
                    return wcfExecutor.Execute(GetChannel, function, methodName);
                case CachingStrategy.Factory:
                case CachingStrategy.None:
                    return wcfExecutor.ExecuteAndRelease(GetChannel, function, methodName);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
