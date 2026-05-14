using System;
using Confirmit.CATI.Supervisor.Classes;
using System.Web;

namespace Confirmit.CATI.Supervisor.Classes.Fakes
{
    public class StubIBaseForm : IBaseForm 
    {
        private IBaseForm _inner;

        public StubIBaseForm()
        {
            _inner = null;
        }

        public IBaseForm Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private HttpRequest _Request;
        public Func<HttpRequest> RequestGet;
        public Action<HttpRequest> RequestSetHttpRequest;

        HttpRequest IBaseForm.Request
        {
            get
            {
                if (RequestGet != null)
                {
                    return RequestGet();
                } else if (_inner != null)
                {
                    return ((IBaseForm)_inner).Request;
                }

                if (RequestSetHttpRequest == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Request;
                }

                return default(HttpRequest);
            }

        }

    }
}