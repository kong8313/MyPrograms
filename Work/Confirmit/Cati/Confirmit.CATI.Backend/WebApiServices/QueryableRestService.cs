using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.OData.Query;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class QueryableRestService : IQueryableRestService
    {
        private readonly IWebApiSettings _settings;
        private readonly IRequestExecutionLog _requestLog;

        public QueryableRestService(
            IWebApiSettings settings,
            IRequestExecutionLog requestLog)
        {
            _settings = settings;
            _requestLog = requestLog;
        }
        
        public HttpResponseMessage GetList<T>(
            HttpRequestMessage request, 
            ODataQueryOptions<T> options, 
            IQueryable query, 
            IDatabaseContext context) where T : class
        {
            var responseContent = GetCollection(request, options, query, context);

            return request.CreateResponse(HttpStatusCode.OK, responseContent);
        }

        public IEnumerable<T> GetCollection<T>(
            HttpRequestMessage request,
            ODataQueryOptions<T> options,
            IQueryable query,
            IDatabaseContext context) where T : class
        {
            // Ensure the request URI uses HTTPS scheme before OData processes it
            // This is necessary because newer OData versions use RequestUri directly for nextLink generation
            if (request.RequestUri.Scheme == "http")
            {
                var uriBuilder = new UriBuilder(request.RequestUri)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = -1 // Use default port for HTTPS
                };
                request.RequestUri = uriBuilder.Uri;
            }

            var oDataSettings = new ODataQuerySettings
            {
                PageSize = _settings.PageSize
            };

            // actually executes the query
            var responseContent = (IEnumerable<T>)options.ApplyTo(
                query,
                oDataSettings);

            _requestLog.AddEntry(context.ExecutionLog);

            return responseContent;
        }
    }
}