using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Confirmit.CATI.Core.Misc;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class RootController : ApiController
    {
        /// <summary>
        /// Return all used API URLs
        /// </summary>
        /// <returns>List of url information</returns>
        [SwaggerResponse(200, "OK", typeof(List<Link>))]
        public HttpResponseMessage GetOperations()
        {
            var companyId = BackendInstance.Current.CompanyId;
            var resource = new List<Link>
            {
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "callhistory",
                    HRef = $"/catiapi/companies/{companyId}/callhistory"
                },
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "callhistorywithvariables",
                    HRef = $"/catiapi/companies/{companyId}/callhistorywithvariables"
                },
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "breakhistory",
                    HRef = $"/catiapi/companies/{companyId}/breakhistory"
                },
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "surveys",
                    HRef = $"/catiapi/companies/{companyId}/surveys"
                },
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "interviewers",
                    HRef = $"/catiapi/companies/{companyId}/interviewers"
                },
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "groups",
                    HRef = $"/catiapi/companies/{companyId}/groups"
                },
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "healthz/ready",
                    HRef = $"/catiapi/companies/{companyId}/healthz/ready"
                },
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "healthz/live",
                    HRef = $"/catiapi/companies/{companyId}/healthz/live"
                },
                new Link
                {
                    Method = LinkMethodNames.Get,
                    Rel = "blacklist",
                    HRef = $"/catiapi/companies/{companyId}/blacklist"
                },
            };

            var response = Request.CreateResponse(HttpStatusCode.OK, resource);

            return response;
        }
    }
}
