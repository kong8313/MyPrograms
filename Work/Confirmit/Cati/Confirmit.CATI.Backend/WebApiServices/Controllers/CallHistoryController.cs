using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.Misc;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class CallHistoryController : ODataController
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IQueryableRestService _queryableRestService;

        public CallHistoryController(
            IDatabaseContextFactory databaseContextFactory,
            IQueryableRestService queryableRestService)
        {
            _databaseContextFactory = databaseContextFactory;
            _queryableRestService = queryableRestService;
        }

        /// <summary>
        /// Get history of calls using OData filter
        /// </summary>
        /// <param name="options">OData query object</param>
        /// <returns>List of CallHistory objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<CallHistory>))]
        public HttpResponseMessage Get(ODataQueryOptions<CallHistory> options)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                return _queryableRestService.GetList(Request, options, context.CallHistory, context);
            }
        }

        /// <summary>
        /// Get a specific call history entity by id
        /// </summary>
        /// <param name="key">Unique identifier of the call history entity</param>
        /// <returns>CallHistory object</returns>
        [SwaggerResponse(200, "OK", typeof(CallHistory))]
        public HttpResponseMessage Get(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            HttpResponseMessage response;

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                var query =
                    from entity
                    in context.CallHistory
                    where entity.Id == key
                    select entity;

                var callHistory = query.SingleOrDefault();

                response = callHistory == null 
                    ? Request.CreateResponse(HttpStatusCode.NotFound) 
                    : Request.CreateResponse(HttpStatusCode.OK, callHistory);
            }

            return response;
        }
    }
}
