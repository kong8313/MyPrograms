using System;
using System.Web.Http.Filters;
using Confirmit.CATI.Backend.WebApiServices.ExceptionsHandling;
using System.Net.Http;

namespace Confirmit.CATI.Backend.WebApiServices.ExceptionsHandling.Fakes
{
    public class StubIExceptionLogger : IExceptionLogger 
    {
        private IExceptionLogger _inner;

        public StubIExceptionLogger()
        {
            _inner = null;
        }

        public IExceptionLogger Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void LogExceptionHttpActionExecutedContextDelegate(HttpActionExecutedContext context);
        public LogExceptionHttpActionExecutedContextDelegate LogExceptionHttpActionExecutedContext;

        void IExceptionLogger.LogException(HttpActionExecutedContext context)
        {

            if (LogExceptionHttpActionExecutedContext != null)
            {
                LogExceptionHttpActionExecutedContext(context);
            } else if (_inner != null)
            {
                ((IExceptionLogger)_inner).LogException(context);
            }
        }

        public delegate void LogExceptionHttpRequestMessageExceptionDelegate(HttpRequestMessage request, Exception exception);
        public LogExceptionHttpRequestMessageExceptionDelegate LogExceptionHttpRequestMessageException;

        void IExceptionLogger.LogException(HttpRequestMessage request, Exception exception)
        {

            if (LogExceptionHttpRequestMessageException != null)
            {
                LogExceptionHttpRequestMessageException(request, exception);
            } else if (_inner != null)
            {
                ((IExceptionLogger)_inner).LogException(request, exception);
            }
        }

    }
}