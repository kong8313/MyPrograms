using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.Misc;
using System.Web.OData;
using System.Web.OData.Query;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class InterviewerSessionHistoryController : ODataController
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IQueryableRestService _queryableRestService;

        public InterviewerSessionHistoryController(
            IDatabaseContextFactory databaseContextFactory,
            IQueryableRestService queryableRestService)
        {
            _databaseContextFactory = databaseContextFactory;
            _queryableRestService = queryableRestService;
        }

        /// <summary>
        /// Get history of interviewer sessions using OData filter
        /// </summary>
        /// <param name="options">OData query object</param>
        /// <returns>List of InterviewerSessionHistory objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<InterviewerSessionHistory>))]
        public HttpResponseMessage Get(ODataQueryOptions<InterviewerSessionHistory> options)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            HttpResponseMessage response;

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConfirmlogConnectionString))
            {
                var query =
                    from entity
                        in context.InterviewerSessionHistory
                    where entity.CompanyId == BackendInstance.Current.CompanyId
                    select entity;

                response = _queryableRestService.GetList(Request, options, query, context);
            }

            return response;
        }
    }
}