using System;
using Confirmit.CATI.Common.WcfTools;
using System.ServiceModel.Description;
using System.Collections.Generic;

namespace Confirmit.CATI.Common.WcfTools.Fakes
{
    public class StubIChannelFactoryWrapperConfiguration : IChannelFactoryWrapperConfiguration 
    {
        private IChannelFactoryWrapperConfiguration _inner;

        public StubIChannelFactoryWrapperConfiguration()
        {
            _inner = null;
        }

        public IChannelFactoryWrapperConfiguration Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string AdjustEndpointUriStringDelegate(string originalUrl);
        public AdjustEndpointUriStringDelegate AdjustEndpointUriString;

        string IChannelFactoryWrapperConfiguration.AdjustEndpointUri(string originalUrl)
        {


            if (AdjustEndpointUriString != null)
            {
                return AdjustEndpointUriString(originalUrl);
            } else if (_inner != null)
            {
                return ((IChannelFactoryWrapperConfiguration)_inner).AdjustEndpointUri(originalUrl);
            }

            return default(string);
        }

        public delegate bool UseLogicalAddressReplacementForHttpsDelegate();
        public UseLogicalAddressReplacementForHttpsDelegate UseLogicalAddressReplacementForHttps;

        bool IChannelFactoryWrapperConfiguration.UseLogicalAddressReplacementForHttps()
        {


            if (UseLogicalAddressReplacementForHttps != null)
            {
                return UseLogicalAddressReplacementForHttps();
            } else if (_inner != null)
            {
                return ((IChannelFactoryWrapperConfiguration)_inner).UseLogicalAddressReplacementForHttps();
            }

            return default(bool);
        }

        public delegate void InitializeClientCredentialsClientCredentialsDelegate(ClientCredentials credentials);
        public InitializeClientCredentialsClientCredentialsDelegate InitializeClientCredentialsClientCredentials;

        void IChannelFactoryWrapperConfiguration.InitializeClientCredentials(ClientCredentials credentials)
        {

            if (InitializeClientCredentialsClientCredentials != null)
            {
                InitializeClientCredentialsClientCredentials(credentials);
            } else if (_inner != null)
            {
                ((IChannelFactoryWrapperConfiguration)_inner).InitializeClientCredentials(credentials);
            }
        }

        private string _EndpointConfigurationName;
        public Func<string> EndpointConfigurationNameGet;
        public Action<string> EndpointConfigurationNameSetString;

        string IChannelFactoryWrapperConfiguration.EndpointConfigurationName
        {
            get
            {
                if (EndpointConfigurationNameGet != null)
                {
                    return EndpointConfigurationNameGet();
                } else if (_inner != null)
                {
                    return ((IChannelFactoryWrapperConfiguration)_inner).EndpointConfigurationName;
                }

                if (EndpointConfigurationNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EndpointConfigurationName;
                }

                return default(string);
            }

        }

        private IEnumerable<IEndpointBehavior> _EndpointBehaviors;
        public Func<IEnumerable<IEndpointBehavior>> EndpointBehaviorsGet;
        public Action<IEnumerable<IEndpointBehavior>> EndpointBehaviorsSetIEnumerableOfIEndpointBehavior;

        IEnumerable<IEndpointBehavior> IChannelFactoryWrapperConfiguration.EndpointBehaviors
        {
            get
            {
                if (EndpointBehaviorsGet != null)
                {
                    return EndpointBehaviorsGet();
                } else if (_inner != null)
                {
                    return ((IChannelFactoryWrapperConfiguration)_inner).EndpointBehaviors;
                }

                if (EndpointBehaviorsSetIEnumerableOfIEndpointBehavior == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EndpointBehaviors;
                }

                return default(IEnumerable<IEndpointBehavior>);
            }

        }

        private CachingStrategy _CachingStrategy;
        public Func<CachingStrategy> CachingStrategyGet;
        public Action<CachingStrategy> CachingStrategySetCachingStrategy;

        CachingStrategy IChannelFactoryWrapperConfiguration.CachingStrategy
        {
            get
            {
                if (CachingStrategyGet != null)
                {
                    return CachingStrategyGet();
                } else if (_inner != null)
                {
                    return ((IChannelFactoryWrapperConfiguration)_inner).CachingStrategy;
                }

                if (CachingStrategySetCachingStrategy == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CachingStrategy;
                }

                return default(CachingStrategy);
            }

        }

        private bool _LogAllCalls;
        public Func<bool> LogAllCallsGet;
        public Action<bool> LogAllCallsSetBoolean;

        bool IChannelFactoryWrapperConfiguration.LogAllCalls
        {
            get
            {
                if (LogAllCallsGet != null)
                {
                    return LogAllCallsGet();
                } else if (_inner != null)
                {
                    return ((IChannelFactoryWrapperConfiguration)_inner).LogAllCalls;
                }

                if (LogAllCallsSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LogAllCalls;
                }

                return default(bool);
            }

        }

        private bool _LogExceptions;
        public Func<bool> LogExceptionsGet;
        public Action<bool> LogExceptionsSetBoolean;

        bool IChannelFactoryWrapperConfiguration.LogExceptions
        {
            get
            {
                if (LogExceptionsGet != null)
                {
                    return LogExceptionsGet();
                } else if (_inner != null)
                {
                    return ((IChannelFactoryWrapperConfiguration)_inner).LogExceptions;
                }

                if (LogExceptionsSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LogExceptions;
                }

                return default(bool);
            }

        }

        private bool _KeepAliveEnabled;
        public Func<bool> KeepAliveEnabledGet;
        public Action<bool> KeepAliveEnabledSetBoolean;

        bool IChannelFactoryWrapperConfiguration.KeepAliveEnabled
        {
            get
            {
                if (KeepAliveEnabledGet != null)
                {
                    return KeepAliveEnabledGet();
                } else if (_inner != null)
                {
                    return ((IChannelFactoryWrapperConfiguration)_inner).KeepAliveEnabled;
                }

                if (KeepAliveEnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _KeepAliveEnabled;
                }

                return default(bool);
            }

        }

    }
}