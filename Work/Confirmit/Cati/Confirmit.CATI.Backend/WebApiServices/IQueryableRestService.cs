using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.OData.Query;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public interface IQueryableRestService
    {
        HttpResponseMessage GetList<T>(
            HttpRequestMessage request, 
            ODataQueryOptions<T> options,
            IQueryable query, 
            IDatabaseContext databaseContext) where T : class;

        IEnumerable<T> GetCollection<T>(
            HttpRequestMessage request,
            ODataQueryOptions<T> options,
            IQueryable query,
            IDatabaseContext databaseContext) where T : class;
    }
}
