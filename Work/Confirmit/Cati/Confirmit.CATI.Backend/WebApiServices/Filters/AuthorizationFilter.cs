using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Confirmit.CATI.Backend.WebApiServices.Authorization;

namespace Confirmit.CATI.Backend.WebApiServices.Filters
{
    public class AuthorizationFilter : AuthorizationFilterAttribute 
    {             
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.RequestUri.GetLeftPart(UriPartial.Path).EndsWith("/healthz/ready") ||
                actionContext.Request.RequestUri.GetLeftPart(UriPartial.Path).EndsWith("/healthz/live"))
            {
                return;
            }

            var authorizer = actionContext.Request.Resolve<IAuthorizer>();

            authorizer.Authorize();
        }
    }
}
