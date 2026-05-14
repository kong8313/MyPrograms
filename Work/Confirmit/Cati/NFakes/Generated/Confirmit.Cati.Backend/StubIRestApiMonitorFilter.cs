using System;

namespace Confirmit.CATI.Backend.WebApiServices.Filters.Fakes
{
    public class StubIRestApiMonitorFilter : IRestApiMonitorFilter 
    {
        private IRestApiMonitorFilter _inner;

        public StubIRestApiMonitorFilter()
        {
            _inner = null;
        }

        public IRestApiMonitorFilter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

    }
}