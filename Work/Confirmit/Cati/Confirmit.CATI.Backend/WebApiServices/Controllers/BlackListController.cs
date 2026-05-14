using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Backend.WebApiServices.Services;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class BlackListController : ODataController
    {
        private const int ImportListSize = 10000;

        private readonly ICallCenterProvider _callCenterProvider;
        private readonly ITelephoneBlacklistRepository _telephoneBlacklistRepository;
        private readonly ISupervisorNameProvider _supervisorNameProvider;
        private readonly IAsyncOperationQueue _asyncOperationQueue;

        private readonly IQueryableRestService _queryableRestService;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly ICompanyInfo _companyInfo;
        private readonly IConnectionStrings _connectionStrings;

        public BlackListController(
            ICallCenterProvider callCenterProvider,
            ITelephoneBlacklistRepository telephoneBlacklistRepository,
            ISupervisorNameProvider supervisorNameProvider,
            IAsyncOperationQueue asyncOperationQueue,
            IQueryableRestService queryableRestService,
            IDatabaseContextFactory databaseContextFactory,
            ICompanyInfo companyInfo,
            IConnectionStrings connectionStrings)
        {
            _callCenterProvider = callCenterProvider;
            _telephoneBlacklistRepository = telephoneBlacklistRepository;
            _supervisorNameProvider = supervisorNameProvider;
            _asyncOperationQueue = asyncOperationQueue;
            _queryableRestService = queryableRestService;
            _databaseContextFactory = databaseContextFactory;
            _companyInfo = companyInfo;
            _connectionStrings = connectionStrings;
        }

        /// <summary>
        /// Get telephone number from the blacklist by the unique identifier
        /// </summary>
        /// <param name="key">Unique identifier of the telephone number</param>
        /// <returns>Telephone blacklist item</returns>
        [SwaggerResponse(200, "OK", typeof(TelephoneBlacklistItem))]
        [SwaggerResponse(400, "BadRequest")]
        public HttpResponseMessage Get([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var context =
                _databaseContextFactory.CreateDatabaseContext(
                    _connectionStrings.GetConnectionStringForSpecificCompany(_companyInfo.CompanyId)))
            {
                var telephoneBlackListItem = context.TelephoneBlacklist.Find(key);
                return telephoneBlackListItem == null
                    ? Request.CreateResponse(HttpStatusCode.NotFound, "Telephone blacklist item is not found")
                    : Request.CreateResponse(HttpStatusCode.OK, telephoneBlackListItem);
            }
        }

        /// <summary>
        /// Get telephone numbers from the blacklist using OData filter
        /// </summary>
        /// <param name="options">OData query object</param>
        /// <returns>List of the telephone blacklist items</returns>
        [SwaggerResponse(200, "OK", typeof(List<TelephoneBlacklistItem>))]
        [SwaggerResponse(400, "BadRequest")]
        public HttpResponseMessage Get(ODataQueryOptions<TelephoneBlacklistItem> options)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var context =
                _databaseContextFactory.CreateDatabaseContext(
                    _connectionStrings.GetConnectionStringForSpecificCompany(_companyInfo.CompanyId)))
            {
                return _queryableRestService.GetList(Request, options, context.TelephoneBlacklist, context);
            }
        }

        /// <summary>
        /// Add telephone number to the blacklist
        /// </summary>
        /// <param name="newItem">Telephone number to be added to blacklist</param>
        /// <returns>Unique identifier of the new telephone blacklist item</returns>
        [SwaggerResponse(201, "Created", typeof(int))]
        [SwaggerResponse(400, "BadRequest")]
        public HttpResponseMessage Post(TelephoneBlacklistItem newItem)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(ModelState.Values));
            }

            if (!BlackListHelper.IsTelephoneBlackListItemValid(newItem))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Wrong telephone number or blacklist pattern type");
            }

            var entity = new BvTelephoneBlacklistEntity { 
                TelephoneNumber = newItem.TelephoneNumber,
                Comment = "Added from api",
                Type = (byte)newItem.Type };

            var id = _telephoneBlacklistRepository.Insert(entity);

            return Request.CreateResponse(HttpStatusCode.Created, id);
        }

        /// <summary>
        /// Add a list of the telephone numbers to the blacklist. The number of added elements cannot be more than 10000.
        /// </summary>
        /// <param name="parameters">OData parameters which contains list of the telephone numbers to be added to the blacklist</param>
        /// <returns></returns>
        [SwaggerResponse(201, "Created", typeof(void))]
        [SwaggerResponse(400, "BadRequest")]
        [HttpPost]
        public HttpResponseMessage Import(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (!(parameters["BlackListItems"] is TelephoneBlacklistItems blackListItems))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (blackListItems.Items.Count() > ImportListSize)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"The number of added elements cannot be more than {ImportListSize}");
            }

            var blackListEntities = blackListItems.Items.Select(BlackListHelper.GetBvTelephoneBlacklistEntity).Where(x => x != null)
                .ToList();

            if (!blackListEntities.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Incorrect list of the telephone blacklist items");
            }

            _telephoneBlacklistRepository.Import(blackListEntities);

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        /// <summary>
        /// Update telephone number in the blacklist
        /// </summary>
        /// <param name="key">Unique identifier of the telephone number in the blacklist to be updated</param>
        /// <param name="updateItem">New blacklist item data</param>
        /// <returns></returns>
        [SwaggerResponse(200, "OK", typeof(TelephoneBlacklistItem))]
        [SwaggerResponse(400, "BadRequest")]
        public HttpResponseMessage Put([FromODataUri]int key, TelephoneBlacklistItem updateItem)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (key != updateItem.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (!BlackListHelper.IsTelephoneBlackListItemValid(updateItem))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Wrong telephone number or blacklist pattern type");
            }

            _telephoneBlacklistRepository.Update(new BvTelephoneBlacklistEntity
            {
                Id = key,
                TelephoneNumber = updateItem.TelephoneNumber,
                Timestamp = DateTime.UtcNow,
                Type = (byte)updateItem.Type
            });

            return Request.CreateResponse(HttpStatusCode.OK, updateItem);
        }

        /// <summary>
        /// Delete telephone number from the blacklist
        /// </summary>
        /// <param name="key">Unique identifier of the telephone number in the blacklist to be deleted</param>
        /// <returns></returns>
        [SwaggerResponse(204, "NoContent", typeof(void))]
        [SwaggerResponse(400, "BadRequest")]
        [SwaggerResponse(404, "NotFound")]
        public HttpResponseMessage Delete([FromODataUri] int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var itemToDelete = _telephoneBlacklistRepository.GetById(key);

            if (itemToDelete == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Telephone blacklist item is not found");
            }

            _telephoneBlacklistRepository.Delete(new[] {key});

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
