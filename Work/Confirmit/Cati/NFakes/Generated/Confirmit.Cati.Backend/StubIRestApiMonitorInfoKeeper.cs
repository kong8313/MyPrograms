using System;
using Confirmit.CATI.Backend.WebApiServices.Logging;
using Confirmit.CATI.Backend.WebApiServices.Filters;

namespace Confirmit.CATI.Backend.WebApiServices.Filters.Fakes
{
    public class StubIRestApiMonitorInfoKeeper : IRestApiMonitorInfoKeeper 
    {
        private IRestApiMonitorInfoKeeper _inner;

        public StubIRestApiMonitorInfoKeeper()
        {
            _inner = null;
        }

        public IRestApiMonitorInfoKeeper Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void StoreRestApiMonitorInfoDelegate(RestApiMonitorInfo restApiMonitorInfo);
        public StoreRestApiMonitorInfoDelegate StoreRestApiMonitorInfo;

        void IRestApiMonitorInfoKeeper.Store(RestApiMonitorInfo restApiMonitorInfo)
        {

            if (StoreRestApiMonitorInfo != null)
            {
                StoreRestApiMonitorInfo(restApiMonitorInfo);
            } else if (_inner != null)
            {
                ((IRestApiMonitorInfoKeeper)_inner).Store(restApiMonitorInfo);
            }
        }

        public delegate RestApiMonitorInfo GetInfoDelegate();
        public GetInfoDelegate GetInfo;

        RestApiMonitorInfo IRestApiMonitorInfoKeeper.GetInfo()
        {


            if (GetInfo != null)
            {
                return GetInfo();
            } else if (_inner != null)
            {
                return ((IRestApiMonitorInfoKeeper)_inner).GetInfo();
            }

            return default(RestApiMonitorInfo);
        }

    }
}