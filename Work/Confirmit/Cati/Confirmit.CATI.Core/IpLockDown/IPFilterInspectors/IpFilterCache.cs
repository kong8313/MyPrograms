using System;

namespace Confirmit.CATI.Core.IpLockDown.IPFilterInspectors
{
    public class IpFilterCache : IIpFilterCache
    {
        private readonly object _lock = new object();

        private IpFilterCacheData InternalServices { get; set; }
        private IpFilterCacheData ExternalServices { get; set; }

        public IpFilterCacheData LoadInternalServicesIpCacheIfEmpty(Func<IpFilterCacheData> f)
        {
            var value = InternalServices;

            if (value != null)
            {
                return value;
            }

            lock (_lock)
            {
                if (InternalServices == null)
                {
                    InternalServices = f();
                }

                return InternalServices;
            }
        }

        public IpFilterCacheData LoadDialerWsServiceIpCacheIfEmpty(Func<IpFilterCacheData> f)
        {
            var value = ExternalServices;

            if (value != null)
            {
                return value;
            }

            lock (_lock)
            {
                if (ExternalServices == null)
                {
                    ExternalServices = f();
                }

                return ExternalServices;
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                InternalServices = null;
                ExternalServices = null;
            }
        }
    }
}