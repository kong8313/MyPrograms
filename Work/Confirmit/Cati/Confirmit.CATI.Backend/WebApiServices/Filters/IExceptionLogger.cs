using System;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Confirmit.CATI.Backend.WebApiServices.ExceptionsHandling
{
    public interface IExceptionLogger
    {
        void LogException(HttpActionExecutedContext context);
        void LogException(HttpRequestMessage request, Exception exception);
    }
}