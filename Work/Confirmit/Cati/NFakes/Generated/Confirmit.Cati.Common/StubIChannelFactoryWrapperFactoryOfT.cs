using System;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Common.WcfTools.Fakes
{
    public class StubIChannelFactoryWrapperFactory<T> : IChannelFactoryWrapperFactory<T>  where T : class 
    {
        private IChannelFactoryWrapperFactory<T> _inner;

        public StubIChannelFactoryWrapperFactory()
        {
            _inner = null;
        }

        public IChannelFactoryWrapperFactory<T> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IChannelFactoryWrapper<T> CreateIChannelFactoryWrapperConfigurationILoggerDelegate(IChannelFactoryWrapperConfiguration configuration, ILogger catiCommonILoggerToCodiILogger);
        public CreateIChannelFactoryWrapperConfigurationILoggerDelegate CreateIChannelFactoryWrapperConfigurationILogger;

        IChannelFactoryWrapper<T> IChannelFactoryWrapperFactory<T>.Create(IChannelFactoryWrapperConfiguration configuration, ILogger catiCommonILoggerToCodiILogger)
        {


            if (CreateIChannelFactoryWrapperConfigurationILogger != null)
            {
                return CreateIChannelFactoryWrapperConfigurationILogger(configuration, catiCommonILoggerToCodiILogger);
            } else if (_inner != null)
            {
                return ((IChannelFactoryWrapperFactory<T>)_inner).Create(configuration, catiCommonILoggerToCodiILogger);
            }

            return default(IChannelFactoryWrapper<T>);
        }

    }
}