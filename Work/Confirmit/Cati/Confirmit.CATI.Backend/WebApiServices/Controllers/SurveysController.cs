using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Core.Assignment;
using System.Web.OData;
using System.Web.OData.Query;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class SurveysController : ODataController
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IAssignmentManager _assignmentManager;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISurveyStateService _surveyStateService;
        private readonly IScheduleRepository _scheduleService;
        private readonly IQueryableRestService _queryableRestService;

        public SurveysController(
            IDatabaseContextFactory databaseContextFactory,
            IAssignmentManager assignmentManager,
            ISurveyRepository surveyRepository,
            ISurveyStateService surveyStateService,
            IScheduleRepository scheduleService,
            IQueryableRestService queryableRestService)
        {
            _databaseContextFactory = databaseContextFactory;
            _assignmentManager = assignmentManager;
            _surveyRepository = surveyRepository;
            _surveyStateService = surveyStateService;
            _scheduleService = scheduleService;
            _queryableRestService = queryableRestService;
        }

        private string GetSurveyIdFromKey(string key)
        {
            return key.Trim('\'');
        }

        /// <summary>
        /// Get surveys using OData filter
        /// </summary>
        /// <param name="options"></param>
        /// <returns>List of Survey objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<Survey>))]
        public HttpResponseMessage Get(ODataQueryOptions<Survey> options)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                return _queryableRestService.GetList(Request, options, context.Survey, context);
            }
        }

        /// <summary>
        /// Get survey by survey ID
        /// </summary>
        /// <param name="key">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>Survey object</returns>
        [SwaggerResponse(200, "OK", typeof(Survey))]
        public HttpResponseMessage Get(string key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            HttpResponseMessage response;

            var surveyId = GetSurveyIdFromKey(key);

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                var query =
                    from entity
                        in context.Survey
                    where entity.SurveyId == surveyId
                    select entity;

                var survey = query.SingleOrDefault();

                if (survey == null)
                {
                    throw new SurveyNotFoundException(surveyId);
                }

                response = Request.CreateResponse(HttpStatusCode.OK, survey);

            }

            return response;
        }

        /// <summary>
        /// Get all assignment information related to the specified survey
        /// </summary>
        /// <param name="key">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId"></param>
        /// <returns>List of ResourceAssignment objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<ResourceAssignment>))]
        public HttpResponseMessage GetAssignments(string key, int callCenterId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var surveyId = GetSurveyIdFromKey(key);

            var survey = _surveyRepository.GetByName(surveyId);

            var interviewerAssignments = _assignmentManager.GetAssignedInterviewersAndGroupsList(survey.SID, callCenterId);

            var responseData = from interviewerAssignment in interviewerAssignments
                                select
                                    new ResourceAssignment
                                    {
                                        ResourceId = interviewerAssignment.SID,
                                        Name = interviewerAssignment.Name,
                                        AssignedCallsCount = interviewerAssignment.AssignedCallsCount,
                                        IsGroup = interviewerAssignment.IsGroup
                                    };

            var response = Request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

        /// <summary>
        /// Open the survey
        /// </summary>
        /// <param name="key">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>true - if success</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(bool))]
        public HttpResponseMessage Open(string key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var surveyId = GetSurveyIdFromKey(key);

            var survey = _surveyRepository.GetByName(surveyId);

            _surveyStateService.Open(survey.SID);

            var response = Request.CreateResponse(HttpStatusCode.OK, true);

            return response;
        }

        /// <summary>
        /// Close the survey
        /// </summary>
        /// <param name="key">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>true - if success</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(bool))]
        public HttpResponseMessage Close(string key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, false);
            }

            var surveyId = GetSurveyIdFromKey(key);

            var survey = _surveyRepository.GetByName(surveyId);

            _surveyStateService.CloseSurvey(survey.SID);

            var response = Request.CreateResponse(HttpStatusCode.OK, true);

            return response;
        }

        /// <summary>
        /// Shutdown the survey
        /// </summary>
        /// <param name="key">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>true - if success</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(bool))]
        public HttpResponseMessage Shutdown(string key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var surveyId = GetSurveyIdFromKey(key);

            var survey = _surveyRepository.GetByName(surveyId);

            _surveyStateService.ShutdownSurvey(survey.SID);

            var response = Request.CreateResponse(HttpStatusCode.OK, true);

            return response;
        }

        /// <summary>
        /// Remove all assignments related to the specified survey
        /// </summary>
        /// <param name="key">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>true - if success</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(bool))]
        public HttpResponseMessage CleanAssignments(string key, int callCenterId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var surveyId = GetSurveyIdFromKey(key);

            using (var transactionScope = new DatabaseTransactionScope("Api.Survey.CleanAssignments", DeadlockPriority.Supervisor))
            {
                var survey = _surveyRepository.GetByName(surveyId);

                _assignmentManager.ClearSurveyAssignments(survey.SID, callCenterId);

                transactionScope.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, true);

            return response;
        }

        /// <summary>
        /// Get basic properties of the survey
        /// </summary>
        /// <param name="key">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>SurveyBasicProperties object</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(SurveyBasicProperties))]
        public HttpResponseMessage GetBasicProperties(string key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var surveyId = GetSurveyIdFromKey(key);

            var survey = _surveyRepository.GetByName(surveyId);

            var properties = new SurveyBasicProperties
            {
                SurveyId = surveyId,

                CallDeliveryMode = survey.IsRandomCallDeliveryEnabled
                    ? CallDeliveryMode.Random
                    : CallDeliveryMode.InOrder,
                
                // TODO: Throw exception from the repository if schedule is not found
                Scheduling = _scheduleService.GetById(survey.ScheduleID).Name,

                // TODO: Throw exception from the repository if state group is not found
                ExtendedStatusGroup = StateGroupRepository.GetById(survey.StateGroupID).Name,

                CallGroups = (SurveySchedulingMode)survey.SurveySchedulingMode == SurveySchedulingMode.CallGroup
            };

            var responseData = properties;

            var response = Request.CreateResponse(HttpStatusCode.OK, responseData);
            
            return response;
        }

        /// <summary>
        /// Update basic properties of the survey
        /// </summary>
        /// <param name="key">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="properties">Json serialized string of SurveyBasicProperties object</param>
        /// <returns>true - if success</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(bool))]
        public HttpResponseMessage PutBasicProperties(string key, string properties)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var surveyId = GetSurveyIdFromKey(key);

            var basicProperties = JsonConvert.DeserializeObject<SurveyBasicProperties>(properties);

            using (var transactionScope = new DatabaseTransactionScope("Api.Survey.PutBasicProperties", DeadlockPriority.Supervisor))
            {
                var survey = _surveyRepository.GetByName(surveyId);

                survey.ScheduleID = _scheduleService.GetByName(basicProperties.Scheduling).ScheduleID;

                survey.StateGroupID = StateGroupRepository.GetByName(basicProperties.ExtendedStatusGroup).ID;

                survey.SurveySchedulingMode =
                    (short) (basicProperties.CallGroups ? SurveySchedulingMode.CallGroup : SurveySchedulingMode.Normal);

                _surveyRepository.Update(survey);

                SurveyService.SetCallDeliveryMode(survey.SID, basicProperties.CallDeliveryMode);

                transactionScope.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, true);

            return response;
        }
    }
}