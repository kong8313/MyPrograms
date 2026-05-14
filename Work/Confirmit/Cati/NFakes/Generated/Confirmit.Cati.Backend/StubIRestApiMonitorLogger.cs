using System;
using Confirmit.CATI.Backend.WebApiServices.Logging;

namespace Confirmit.CATI.Backend.WebApiServices.Logging.Fakes
{
    public class StubIRestApiMonitorLogger : IRestApiMonitorLogger 
    {
        private IRestApiMonitorLogger _inner;

        public StubIRestApiMonitorLogger()
        {
            _inner = null;
        }

        public IRestApiMonitorLogger Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void LogRestApiMonitorInfoDelegate(RestApiMonitorInfo info);
        public LogRestApiMonitorInfoDelegate LogRestApiMonitorInfo;

        void IRestApiMonitorLogger.Log(RestApiMonitorInfo info)
        {

            if (LogRestApiMonitorInfo != null)
            {
                LogRestApiMonitorInfo(info);
            } else if (_inner != null)
            {
                ((IRestApiMonitorLogger)_inner).Log(info);
            }
        }

    }
}