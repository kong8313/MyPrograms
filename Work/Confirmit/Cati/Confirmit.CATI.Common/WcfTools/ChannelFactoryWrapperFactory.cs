namespace Confirmit.CATI.Common.WcfTools
{
    public class ChannelFactoryWrapperFactory<T> : IChannelFactoryWrapperFactory<T> where T : class
    {
        public IChannelFactoryWrapper<T> Create(
            IChannelFactoryWrapperConfiguration configuration,
            ILogger catiCommonILoggerToCodiILogger)
        {
            return new ChannelFactoryWrapper<T>(configuration, catiCommonILoggerToCodiILogger);
        }
    }
}