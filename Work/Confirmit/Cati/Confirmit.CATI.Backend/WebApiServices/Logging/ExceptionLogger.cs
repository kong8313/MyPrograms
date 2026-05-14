using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Filters;
using Confirmit.CATI.Backend.WebApiServices.ExceptionsHandling;

namespace Confirmit.CATI.Backend.WebApiServices.Logging
{
    public class ExceptionLogger : IExceptionLogger
    {
        public void LogException(HttpActionExecutedContext context)
        {
            LogException(context.Request, context.Exception);
        }

        public void LogException(HttpRequestMessage request, Exception exception)
        {
            const string errorTextFormat = "{0}\r\n\r\nException: {1} \"{2}\"\r\n\r\n{3}";

            string errorText = string.Format(
                errorTextFormat,
                ""/*_requestInfo.GetRequestInfo(request)*/,
                exception.GetType(),
                exception.Message,
                exception);

            Trace.TraceError(errorText);
        }
    }
}
