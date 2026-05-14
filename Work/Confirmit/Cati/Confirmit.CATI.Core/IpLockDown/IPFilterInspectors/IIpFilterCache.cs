using System;

namespace Confirmit.CATI.Core.IpLockDown.IPFilterInspectors
{
    public interface IIpFilterCache
    {
        IpFilterCacheData LoadInternalServicesIpCacheIfEmpty(Func<IpFilterCacheData> f);
        IpFilterCacheData LoadDialerWsServiceIpCacheIfEmpty(Func<IpFilterCacheData> f);
        void Reset();
    }
}