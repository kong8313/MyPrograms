namespace Confirmit.CATI.Common.WcfTools
{
    public interface IChannelFactoryWrapperFactory<T> where T : class
    {
        IChannelFactoryWrapper<T> Create(
            IChannelFactoryWrapperConfiguration configuration,
            ILogger catiCommonILoggerToCodiILogger);
    }
}