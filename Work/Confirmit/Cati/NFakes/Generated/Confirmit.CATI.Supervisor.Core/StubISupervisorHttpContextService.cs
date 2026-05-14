using System;
using Confirmit.CATI.Supervisor.Core.Security;
using System.Web;
using System.Collections.Specialized;
using System.Collections;

namespace Confirmit.CATI.Supervisor.Core.Security.Fakes
{
    public class StubISupervisorHttpContextService : ISupervisorHttpContextService 
    {
        private ISupervisorHttpContextService _inner;

        public StubISupervisorHttpContextService()
        {
            _inner = null;
        }

        public ISupervisorHttpContextService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetRemoteIpAddressDelegate();
        public GetRemoteIpAddressDelegate GetRemoteIpAddress;

        string ISupervisorHttpContextService.GetRemoteIpAddress()
        {


            if (GetRemoteIpAddress != null)
            {
                return GetRemoteIpAddress();
            } else if (_inner != null)
            {
                return ((ISupervisorHttpContextService)_inner).GetRemoteIpAddress();
            }

            return default(string);
        }

        public delegate HttpCookieCollection GetRequestCookiesDelegate();
        public GetRequestCookiesDelegate GetRequestCookies;

        HttpCookieCollection ISupervisorHttpContextService.GetRequestCookies()
        {


            if (GetRequestCookies != null)
            {
                return GetRequestCookies();
            } else if (_inner != null)
            {
                return ((ISupervisorHttpContextService)_inner).GetRequestCookies();
            }

            return default(HttpCookieCollection);
        }

        public delegate NameValueCollection GetRequestHeadersDelegate();
        public GetRequestHeadersDelegate GetRequestHeaders;

        NameValueCollection ISupervisorHttpContextService.GetRequestHeaders()
        {


            if (GetRequestHeaders != null)
            {
                return GetRequestHeaders();
            } else if (_inner != null)
            {
                return ((ISupervisorHttpContextService)_inner).GetRequestHeaders();
            }

            return default(NameValueCollection);
        }

        public delegate IDictionary GetContextItemsDelegate();
        public GetContextItemsDelegate GetContextItems;

        IDictionary ISupervisorHttpContextService.GetContextItems()
        {


            if (GetContextItems != null)
            {
                return GetContextItems();
            } else if (_inner != null)
            {
                return ((ISupervisorHttpContextService)_inner).GetContextItems();
            }

            return default(IDictionary);
        }

    }
}