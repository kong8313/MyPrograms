using System;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubIRestApiMonitorHandler : IRestApiMonitorHandler 
    {
        private IRestApiMonitorHandler _inner;

        public StubIRestApiMonitorHandler()
        {
            _inner = null;
        }

        public IRestApiMonitorHandler Inner
        {
            set {_inner = value;} get {return _inner;}
        }

    }
}