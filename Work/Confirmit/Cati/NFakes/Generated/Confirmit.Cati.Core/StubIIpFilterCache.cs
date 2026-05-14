using System;
using Confirmit.CATI.Core.IpLockDown;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;

namespace Confirmit.CATI.Core.IpLockDown.IPFilterInspectors.Fakes
{
    public class StubIIpFilterCache : IIpFilterCache 
    {
        private IIpFilterCache _inner;

        public StubIIpFilterCache()
        {
            _inner = null;
        }

        public IIpFilterCache Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IpFilterCacheData LoadInternalServicesIpCacheIfEmptyFuncOfIpFilterCacheDataDelegate(Func<IpFilterCacheData> f);
        public LoadInternalServicesIpCacheIfEmptyFuncOfIpFilterCacheDataDelegate LoadInternalServicesIpCacheIfEmptyFuncOfIpFilterCacheData;

        IpFilterCacheData IIpFilterCache.LoadInternalServicesIpCacheIfEmpty(Func<IpFilterCacheData> f)
        {


            if (LoadInternalServicesIpCacheIfEmptyFuncOfIpFilterCacheData != null)
            {
                return LoadInternalServicesIpCacheIfEmptyFuncOfIpFilterCacheData(f);
            } else if (_inner != null)
            {
                return ((IIpFilterCache)_inner).LoadInternalServicesIpCacheIfEmpty(f);
            }

            return default(IpFilterCacheData);
        }

        public delegate IpFilterCacheData LoadDialerWsServiceIpCacheIfEmptyFuncOfIpFilterCacheDataDelegate(Func<IpFilterCacheData> f);
        public LoadDialerWsServiceIpCacheIfEmptyFuncOfIpFilterCacheDataDelegate LoadDialerWsServiceIpCacheIfEmptyFuncOfIpFilterCacheData;

        IpFilterCacheData IIpFilterCache.LoadDialerWsServiceIpCacheIfEmpty(Func<IpFilterCacheData> f)
        {


            if (LoadDialerWsServiceIpCacheIfEmptyFuncOfIpFilterCacheData != null)
            {
                return LoadDialerWsServiceIpCacheIfEmptyFuncOfIpFilterCacheData(f);
            } else if (_inner != null)
            {
                return ((IIpFilterCache)_inner).LoadDialerWsServiceIpCacheIfEmpty(f);
            }

            return default(IpFilterCacheData);
        }

        public delegate void ResetDelegate();
        public ResetDelegate Reset;

        void IIpFilterCache.Reset()
        {

            if (Reset != null)
            {
                Reset();
            } else if (_inner != null)
            {
                ((IIpFilterCache)_inner).Reset();
            }
        }

    }
}