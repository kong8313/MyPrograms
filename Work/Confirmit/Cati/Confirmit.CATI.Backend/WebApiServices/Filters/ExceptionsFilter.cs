using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Confirmit.CATI.Backend.WebApiServices.Authorization;
using Confirmit.CATI.Backend.WebApiServices.ExceptionsHandling;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Backend.WebApiServices.Filters
{
    // Logs UNHANDLED exceptions
    public class ExceptionsFilter : ExceptionFilterAttribute
    {
        private readonly IExceptionLogger _exceptionsLogger;

        private void ProcessException(HttpActionExecutedContext context, Exception e, HttpStatusCode status)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = status,
                ReasonPhrase = e.Message,
                Content = new StringContent(e.Message)
            };

            context.Response = response;
        }

        public ExceptionsFilter(IExceptionLogger exceptionsLogger)
        {
            _exceptionsLogger = exceptionsLogger;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            _exceptionsLogger.LogException(context);

            if (context.Exception is WebApiDisabledException)
            {
                ProcessException(context, context.Exception, HttpStatusCode.ServiceUnavailable);
            }
            else if (context.Exception is AuthenticateException)
            {
                ProcessException(context, context.Exception, HttpStatusCode.Forbidden);
            }
            else if (context.Exception is InterviewerNotFoundException)
            {
                ProcessException(context, context.Exception, HttpStatusCode.NotFound);
            }
            else if (context.Exception is SurveyNotFoundException)
            {
                ProcessException(context, context.Exception, HttpStatusCode.NotFound);
            }
            else if (context.Exception is InterviewerGroupNotFoundException)
            {
                ProcessException(context, context.Exception, HttpStatusCode.NotFound);
            }
            else if (context.Exception is UserMessageException)
            {
                ProcessException(context, context.Exception, HttpStatusCode.InternalServerError);
            }
            else
            {
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = "Internal Server Error",
                    Content = new StringContent("Internal Server Error")
                };

                context.Response = response;
            }
        }
    }
}
