using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.Assignment;
using System.Web.OData;
using System.Web.OData.Query;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class GroupsController : ODataController
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IPersonGroupRepository _personGroupRepository;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly ISupervisorInfoProvider _supervisorInfoProvider;
        private readonly IAssignmentManager _assignmentManager;
        private readonly IQueryableRestService _queryableRestService;
        private readonly IAssignmentService _assignmentService;

        public GroupsController(
            ISurveyRepository surveyRepository,
            IPersonGroupRepository personGroupRepository,
            IDatabaseContextFactory databaseContextFactory,
            ISupervisorInfoProvider supervisorInfoProvider,
            IAssignmentManager assignmentManager,
            IQueryableRestService queryableRestService,
            IAssignmentService assignmentService) 
        {
            _surveyRepository = surveyRepository;
            _personGroupRepository = personGroupRepository;
            _databaseContextFactory = databaseContextFactory;
            _supervisorInfoProvider = supervisorInfoProvider;
            _assignmentManager = assignmentManager;
            _queryableRestService = queryableRestService;
            _assignmentService = assignmentService;
        }

        /// <summary>
        /// Get groups using OData filter
        /// </summary>
        /// <param name="options">OData query object</param>
        /// <returns>List of Group objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<Group>))]
        public HttpResponseMessage Get(ODataQueryOptions<Group> options)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                return _queryableRestService.GetList(Request, options, context.InterviewerGroup, context);
            }
        }

        /// <summary>
        /// Get a specific group by an identifier
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <returns>Group object</returns>
        [SwaggerResponse(200, "OK", typeof(Group))]
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
                    in context.InterviewerGroup
                    where entity.GroupId == key
                    select entity;

                var group = query.SingleOrDefault();

                if (group == null)
                {
                    response = Request.CreateResponse(HttpStatusCode.NotFound);
                }
                else
                {
                    var groupId = group.GroupId;

                    group.ParentGroups =
                        (from membership
                        in context.Membership
                         where membership.ObjectSID == groupId
                         select membership.ContainerSID).ToList();

                    response = Request.CreateResponse(HttpStatusCode.OK, group);
                }
            }

            return response;
        }

        /// <summary>
        /// Create a new interviewer group
        /// </summary>
        /// <param name="group">The instance of the group object</param>
        /// <returns>Unique identifier of the new interviewer group</returns>
        [SwaggerResponse(200, "OK", typeof(int))]
        public HttpResponseMessage Post(Group group)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var entity = new BvPersonGroupEntity();
            entity.Name = group.Name;
            entity.Description = group.Description;

            int groupId;
            using (var transaction = new DatabaseTransactionScope("Rest.Group.Post"))
            {
                groupId = _personGroupRepository.Insert(entity);
                PersonGroupService.SetParentGroups(groupId, group.ParentGroups.ToArray());

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, groupId);

            return response;
        }

        /// <summary>
        /// Update an existing interviewer group
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <param name="group">The instance of the group object</param>
        /// <returns>Unique identifier of the interviewer group</returns>
        [SwaggerResponse(200, "OK", typeof(int))]
        public HttpResponseMessage Put(int key, Group group)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var entity = new BvPersonGroupEntity
            {
                SID = group.GroupId, 
                Name = group.Name, 
                Description = group.Description
            };

            using (var transaction = new DatabaseTransactionScope("Rest.Group.Put"))
            {
                _personGroupRepository.Update(entity);
                PersonGroupService.SetParentGroups(group.GroupId, group.ParentGroups.ToArray());

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK, group.GroupId);

            return response;
        }

        /// <summary>
        /// Delete an existing interviewer group
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <returns></returns>
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage Delete(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var transaction = new DatabaseTransactionScope("Rest.Group.Delete"))
            {
                PersonGroupRepository.Delete(key);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        /// <summary>
        /// Get all interviewers belonging to the specified group
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>List of Interviewer objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<Interviewer>))]
        public HttpResponseMessage GetInterviewers(int key, int callCenterId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            HttpResponseMessage response;

            using (var context = _databaseContextFactory.CreateDatabaseContext(BackendInstance.Current.ConnectionString))
            {
                var interviewers = (
                    from membership in context.Membership
                    join interviewer in context.Interviewer 
                    on membership.ObjectSID equals interviewer.InterviewerId
                    where membership.ContainerSID == key && interviewer.CallCenterId == callCenterId
                    select interviewer).ToList();

                response = Request.CreateResponse(HttpStatusCode.OK, interviewers);
            }

            return response;
        }

        /// <summary>
        /// Get all assignment information related to the specified group
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>List of SurveyAssignment objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<SurveyAssignment>))]
        public HttpResponseMessage GetAssignments(int key, int callCenterId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var supervisorName = _supervisorInfoProvider.GetInfo().Name;

            var assignments = _assignmentManager.GetPersonAssignments(key, supervisorName, callCenterId);

            var responseData = from assignment in assignments
                               select
                                   new SurveyAssignment
                                   {
                                       SurveyId = assignment.ProjectID,
                                       AssignedCallsCount = assignment.AssignedCallsCount,
                                       AssignmentType = (AssignmentType)assignment.AssignmentType
                                   };

            var response = Request.CreateResponse(HttpStatusCode.OK, responseData);

            return response;
        }

        /// <summary>
        /// Assign the specified group to the specified survey
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage AssignOnSurvey(int key, string surveyId, int callCenterId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var survey = _surveyRepository.GetByName(surveyId);

            var group = _personGroupRepository.GetById(key);

            using (var transaction = new DatabaseTransactionScope("AssignOnSurvey"))
            {
                AssignmentService.AssignResourceToSurvey(survey.SID, key, callCenterId);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        /// <summary>
        /// Unassign the specified group from the specified survey
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage DeAssignFromSurvey(int key, string surveyId, int callCenterId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var survey = _surveyRepository.GetByName(surveyId);

            var group = _personGroupRepository.GetById(key);

            using (var transaction = new DatabaseTransactionScope("DeAssignFromSurvey"))
            {
                AssignmentService.DeassignResourceFromSurvey(survey.SID, key, callCenterId);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        /// <summary>
        /// Assign the specified group to the specified call
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="interviewId">Unique identifier of the interview</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage AssignOnCall(int key, string surveyId, int interviewId, int callCenterId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var survey = _surveyRepository.GetByName(surveyId);

            var group = _personGroupRepository.GetById(key);

            using (var transaction = new DatabaseTransactionScope("AssignrOnCall"))
            {
                AssignmentService.AssignResourceToInterview(survey.SID, interviewId, group.SID, callCenterId);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }

        /// <summary>
        /// Unassign the specified group from all calls on the specified survey
        /// </summary>
        /// <param name="key">Unique identifier of the group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage DeAssignFromCalls(int key, string surveyId, int callCenterId)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var survey = _surveyRepository.GetByName(surveyId);

            var group = _personGroupRepository.GetById(key);

            using (var transaction = new DatabaseTransactionScope("DeAssignFromCalls"))
            {
                _assignmentService.DeassignResourcesFromSurveyCalls(survey.SID, new[] { group.SID }, callCenterId);

                transaction.Commit();
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);

            return response;
        }
    }
}
