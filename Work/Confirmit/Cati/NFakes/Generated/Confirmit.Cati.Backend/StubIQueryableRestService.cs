using System;
using System.Net.Http;
using System.Web.OData.Query;
using Confirmit.CATI.Backend.WebApiServices;
using System.Linq;
using System.Collections.Generic;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubIQueryableRestService : IQueryableRestService 
    {
        private IQueryableRestService _inner;

        public StubIQueryableRestService()
        {
            _inner = null;
        }

        public IQueryableRestService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        HttpResponseMessage IQueryableRestService.GetList<T>(HttpRequestMessage request, ODataQueryOptions<T> options, IQueryable query, IDatabaseContext databaseContext)
        {


            return default(HttpResponseMessage);
        }

        IEnumerable<T> IQueryableRestService.GetCollection<T>(HttpRequestMessage request, ODataQueryOptions<T> options, IQueryable query, IDatabaseContext databaseContext)
        {


            return default(IEnumerable<T>);
        }

    }
}