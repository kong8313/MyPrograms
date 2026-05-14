using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.IpLockDown.Resolvers;
using Confirmit.CATI.Core.IpLockDown.Validation;

namespace Confirmit.CATI.Core.IpLockDown
{
    public class IpLockDownRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<IIpHostEntryResolver, DnsIpHostEntryResolver>()
                .Register<IIpAddressValidator, IpAddressValidator>()
                .Register<IBaseIpFilterInspector, BaseIpFilterInspector>()
                .RegisterSingleton<IIpFilterCache, IpFilterCache>()
                .RegisterSingleton<IpAndDnsFilterInspector, IpAndDnsFilterInspector>()
                .RegisterSingleton<DialerIpFilterInspector, DialerIpFilterInspector>();
        }
    }
}
