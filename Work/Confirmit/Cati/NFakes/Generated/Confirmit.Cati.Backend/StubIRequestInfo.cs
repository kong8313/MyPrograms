using System;
using System.Net.Http;
using Confirmit.CATI.Backend.WebApiServices;
using Microsoft.Owin;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubIRequestInfo : IRequestInfo 
    {
        private IRequestInfo _inner;

        public StubIRequestInfo()
        {
            _inner = null;
        }

        public IRequestInfo Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IOwinRequest GetOwinRequestHttpRequestMessageDelegate(HttpRequestMessage request);
        public GetOwinRequestHttpRequestMessageDelegate GetOwinRequestHttpRequestMessage;

        IOwinRequest IRequestInfo.GetOwinRequest(HttpRequestMessage request)
        {


            if (GetOwinRequestHttpRequestMessage != null)
            {
                return GetOwinRequestHttpRequestMessage(request);
            } else if (_inner != null)
            {
                return ((IRequestInfo)_inner).GetOwinRequest(request);
            }

            return default(IOwinRequest);
        }

        public delegate string GetRequestInfoIOwinRequestDelegate(IOwinRequest request);
        public GetRequestInfoIOwinRequestDelegate GetRequestInfoIOwinRequest;

        string IRequestInfo.GetRequestInfo(IOwinRequest request)
        {


            if (GetRequestInfoIOwinRequest != null)
            {
                return GetRequestInfoIOwinRequest(request);
            } else if (_inner != null)
            {
                return ((IRequestInfo)_inner).GetRequestInfo(request);
            }

            return default(string);
        }

        public delegate bool IsKubeProbeOrMetricsRequestIOwinRequestDelegate(IOwinRequest request);
        public IsKubeProbeOrMetricsRequestIOwinRequestDelegate IsKubeProbeOrMetricsRequestIOwinRequest;

        bool IRequestInfo.IsKubeProbeOrMetricsRequest(IOwinRequest request)
        {


            if (IsKubeProbeOrMetricsRequestIOwinRequest != null)
            {
                return IsKubeProbeOrMetricsRequestIOwinRequest(request);
            } else if (_inner != null)
            {
                return ((IRequestInfo)_inner).IsKubeProbeOrMetricsRequest(request);
            }

            return default(bool);
        }

    }
}