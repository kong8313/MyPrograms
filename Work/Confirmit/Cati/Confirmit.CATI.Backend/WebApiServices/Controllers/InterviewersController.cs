using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Persons;
using System.Web.OData;
using System.Web.OData.Query;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class InterviewersController : ODataController
    {
        private readonly ICallCenterProvider _callCenterProvider;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IAssignmentManager _assignmentManager;
        private readonly IQueryableRestService _queryableRestService;
        private readonly ISupervisorInfoProvider _supervisorInfoProvider;
        private readonly IAssignmentService _assignmentService;
        private readonly IInterviewerApiClient _interviewerApiClient;
        private readonly ICompanyInfo _companyInfo;
        
        public InterviewersController(
            ICallCenterProvider callCenterProvider,
            ISurveyRepository surveyRepository,
            IPersonRepository personRepository,
            IDatabaseContextFactory databaseContextFactory,
            IAssignmentManager assignmentManager,
            IQueryableRestService queryableRestService,
            ISupervisorInfoProvider supervisorInfoProvider,
            IAssignmentService assignmentService,
            IInterviewerApiClient interviewerApiClient,
            ICompanyInfo companyInfo)
        {
            _callCenterProvider = callCenterProvider;
            _surveyRepository = surveyRepository;
            _personRepository = personRepository;
            _databaseContextFactory = databaseContextFactory;
            _assignmentManager = assignmentManager;
            _queryableRestService = queryableRestService;
            _supervisorInfoProvider = supervisorInfoProvider;
            _assignmentService = assignmentService;
            _interviewerApiClient = interviewerApiClient;
            _companyInfo = companyInfo;
        }

        /// <summary>
        /// Get interviewers list using OData filter
        /// </summary>
        /// <param name="options"></param>
        /// <returns>List of Interviewer objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<Interviewer>))]
        public HttpResponseMessage Get(ODataQueryOptions<Interviewer> options)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                return _queryableRestService.GetList(Request, options, context.Interviewer, context);
            }
        }

        /// <summary>
        /// Get a specific interviewer by id
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <returns>Interviewer object</returns>
        [SwaggerResponse(200, "OK", typeof(Interviewer))]
        public HttpResponseMessage Get(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            // Check if interviewer exists
            _personRepository.GetById(key);

            HttpResponseMessage response;

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                var query =
                    from entity
                    in context.Interviewer
                    where entity.InterviewerId == key
                    select entity;

                var interviewer = query.SingleOrDefault();

                response = Request.CreateResponse(HttpStatusCode.OK, interviewer);
            }

            return response;
        }

        /// <summary>
        /// Get list of groups which contains the specified interviewer
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <returns>List of Group objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<Group>))]
        public HttpResponseMessage GetGroups(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            // Check if interviewer exists
            _personRepository.GetById(key);

            HttpResponseMessage response;

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                var groups =
                    (from membership in context.Membership
                     join interviewergroup in context.InterviewerGroup on membership.ContainerSID equals interviewergroup.GroupId
                     where membership.ObjectSID == key
                     select interviewergroup).ToList<Group>();

                response = Request.CreateResponse(HttpStatusCode.OK, groups);
            }

            return response;
        }

        /// <summary>
        /// Get all assignment information related to the specified interviewer
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <returns>List of SurveyAssignment objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<SurveyAssignment>))]
        public HttpResponseMessage GetAssignments(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var supervisorName = _supervisorInfoProvider.GetInfo().Name;

            var interviewer = _personRepository.GetById(key);

            var assignments = _assignmentManager.GetPersonAssignments(key, supervisorName, interviewer.CallCenterID);

            var responseData = from assignment in assignments
                select
                    new SurveyAssignment
                    {
                        SurveyId = assignment.ProjectID,
                        AssignedCallsCount = assignment.AssignedCallsCount,
                        AssignmentType = (AssignmentType) assignment.AssignmentType
                    };

            var response = Request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

        /// <summary>
        /// Assign the specified interviewer to the specified survey
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>true if success</returns>
        [SwaggerResponse(200, "OK", typeof(bool))]
        [HttpGet]
        public HttpResponseMessage AssignOnSurvey(int key, string surveyId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var survey = _surveyRepository.GetByName(surveyId);

            var interviewer = _personRepository.GetById(key);
            BvSpPerson_SetAutomaticSurveyEntity result;
            using (var transaction = new DatabaseTransactionScope("AssignOnSurvey"))
            {
                AssignmentService.AssignResourceToSurvey(survey.SID, key, interviewer.CallCenterID);

                result = PersonService.SetAutomaticSurveySeamless(key, survey.SID);
                
                transaction.Commit();
            }

            if (result != null)
            {
                _interviewerApiClient.NotifyAutomaticSurveyChanged(_companyInfo.CompanyId, key, survey.SID);
            }
            
            var response = Request.CreateResponse(HttpStatusCode.OK, true);

            return response;
        }

        /// <summary>
        /// Unassign the specified interviewer from the specified survey
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns>true if success</returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(bool))]
        public HttpResponseMessage DeAssignFromSurvey(int key, string surveyId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var survey = _surveyRepository.GetByName(surveyId);

            var interviewer = _personRepository.GetById(key);

            using (var transaction = new DatabaseTransactionScope("DeassignFromSurvey"))
            {
                AssignmentService.DeassignResourceFromSurvey(survey.SID, key, interviewer.CallCenterID);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, true);

            return response;
        }

        /// <summary>
        /// Assign the specified interviewer to the specified call
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="interviewId">Unique identifier of the interview</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage AssignOnCall(int key, string surveyId, int interviewId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var survey = _surveyRepository.GetByName(surveyId);

            var interviewer = _personRepository.GetById(key);

            using (var transaction = new DatabaseTransactionScope("AssignrOnCall"))
            {
                AssignmentService.AssignResourceToInterview(survey.SID, interviewId, interviewer.SID, interviewer.CallCenterID);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        /// <summary>
        /// Unassign the specified interviewer from all calls on the specified survey
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage DeAssignFromCalls(int key, string surveyId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var survey = _surveyRepository.GetByName(surveyId);

            var interviewer = _personRepository.GetById(key);

            using (var transaction = new DatabaseTransactionScope("DeAssignFromCalls"))
            {
                _assignmentService.DeassignResourcesFromSurveyCalls(survey.SID, new[] { interviewer.SID }, interviewer.CallCenterID);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        /// <summary>
        /// Remove all assignments from the specified interviewer
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage CleanAssignments(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var interviewer = _personRepository.GetById(key);

            var supervisorName = _supervisorInfoProvider.GetInfo().Name;

            var callCenterId = _callCenterProvider.GetCurrentId();

            using (var transaction = new DatabaseTransactionScope("CleanAssignments"))
            {
                _assignmentService.ClearPersonAssignments(interviewer.SID, supervisorName, callCenterId);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        /// <summary>
        /// Lock interviewer
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage Lock(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var transaction = new DatabaseTransactionScope("Rest.Interviewer.Lock"))
            {
                PersonService.LockPerson(key, true);
                transaction.Commit();
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Unlock interviewer
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage Unlock(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var transaction = new DatabaseTransactionScope("Rest.Interviewer.Unlock"))
            {
                PersonService.UnlockPerson(key, true);
                transaction.Commit();
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
