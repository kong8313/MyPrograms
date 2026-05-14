using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.OData;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.Misc;
using System.Web.OData.Query;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class BreakHistoryController : ODataController
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IQueryableRestService _queryableRestService;

        public BreakHistoryController(
            IDatabaseContextFactory databaseContextFactory,
            IQueryableRestService queryableRestService)
        {
            _databaseContextFactory = databaseContextFactory;
            _queryableRestService = queryableRestService;
        }

        /// <summary>
        /// Get history of breaks using OData filter
        /// </summary>
        /// <param name="options">OData query object</param>
        /// <returns>List of BreakHistory objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<BreakHistory>))]
        public HttpResponseMessage Get(ODataQueryOptions<BreakHistory> options)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                return _queryableRestService.GetList(Request, options, context.BreakHistory, context);
            }
        }

        /// <summary>
        /// Get a specific break history entity by id
        /// </summary>
        /// <param name="key">Unique identifier of the break history entity</param>
        /// <returns>BreakHistory object</returns>
        [SwaggerResponse(200, "OK", typeof(BreakHistory))]
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
                    in context.BreakHistory
                    where entity.Id == key
                    select entity;

                var breakHistory = query.SingleOrDefault();

                response = breakHistory == null 
                    ? Request.CreateResponse(HttpStatusCode.NotFound) 
                    : Request.CreateResponse(HttpStatusCode.OK, breakHistory);
            }

            return response;
        }
    }
}

