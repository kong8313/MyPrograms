using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.OData;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;
using Swashbuckle.Swagger.Annotations;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class InterviewerPropertiesController : ODataController
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IPersonService _personService;
        private readonly IPersonRepository _personRepository;
        private readonly IInterviewerPasswordSettingsGroup _interviewerPasswordSettings;

        public InterviewerPropertiesController(
            ISurveyRepository surveyRepository,
            IPersonService personService,
            IPersonRepository personRepository, 
            IInterviewerPasswordSettingsGroup interviewerPasswordSettings)
        {
            _surveyRepository = surveyRepository;
            _personService = personService;
            _personRepository = personRepository;
            _interviewerPasswordSettings = interviewerPasswordSettings;
        }

        private HttpResponseMessage CreateOrUpdate(InterviewerProperties interviewerProperties)
        {
            int taskChoicePermissionsFlag = 0;
            foreach (var taskChoicePermissions in interviewerProperties.AllowedTaskChoice)
            {
                taskChoicePermissionsFlag = taskChoicePermissionsFlag | (int)taskChoicePermissions;
            }

            int automaticSurveySid = 0;

            if (!string.IsNullOrEmpty(interviewerProperties.AutomaticSurveyId))
            {
                var survey = _surveyRepository.GetByName(interviewerProperties.AutomaticSurveyId);
                automaticSurveySid = survey.SID;
            }

            if (interviewerProperties.ParentGroups.Count == 0)
            {
                interviewerProperties.ParentGroups = new List<int> {PersonGroupService.RootGroupId};
            }

            var attributes = new[] 
            {
                interviewerProperties.Attribute1, 
                interviewerProperties.Attribute2, 
                interviewerProperties.Attribute3, 
                interviewerProperties.Attribute4, 
                interviewerProperties.Attribute5
            };
            
            var interviewerSid = _personService.CreateOrUpdatePerson(
                interviewerProperties.CallCenterId,
                interviewerProperties.InterviewerId,
                interviewerProperties.Name,
                interviewerProperties.Description,
                interviewerProperties.DisplayName,
                interviewerProperties.Password,
                (AgentTaskChoiceMode)interviewerProperties.Mode,
                (PersonAssignmentListMode)interviewerProperties.AssignmentsListMode,
                (TaskChoicePermissions?)taskChoicePermissionsFlag == 0
                    ? null
                    : (TaskChoicePermissions?)taskChoicePermissionsFlag,
                interviewerProperties.ParentGroups,
                automaticSurveySid,
                interviewerProperties.CallGroupId,
                interviewerProperties.Location,
                (DialType)interviewerProperties.DialType,
                AgentType.LiveAgent,
                true,
                _interviewerPasswordSettings.IsChangeAfterFirstLoginRequired,
                attributes);

            var response = Request.CreateResponse(HttpStatusCode.OK, interviewerSid);

            return response;
        }

        /// <summary>
        /// Create a new interviewer
        /// </summary>
        /// <param name="interviewerProperties">The instance of the InterviewerProperties object</param>
        /// <returns>Unique identifier of the new interviewer</returns>
        [SwaggerResponse(200, "OK", typeof(int))]
        public HttpResponseMessage Post(InterviewerProperties interviewerProperties)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (interviewerProperties.InterviewerId != 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var transaction = new DatabaseTransactionScope("Rest.Interviewer.Post"))
            {
                var response = CreateOrUpdate(interviewerProperties);

                transaction.Commit();

                return response;
            }
        }

        /// <summary>
        /// Update an existing interviewer
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <param name="interviewerProperties">The instance of the InterviewerProperties object</param>
        /// <returns>Unique identifier of the new interviewer</returns>
        [SwaggerResponse(200, "OK", typeof(int))]
        public HttpResponseMessage Put(int key, InterviewerProperties interviewerProperties)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (interviewerProperties.InterviewerId == 0 || key == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var transaction = new DatabaseTransactionScope("Rest.Interviewer.Put"))
            {
                var response = CreateOrUpdate(interviewerProperties);

                transaction.Commit();

                return response;
            }
        }

        /// <summary>
        /// Delete an existing interviewer
        /// </summary>
        /// <param name="key">Unique identifier of the interviewer</param>
        /// <returns></returns>
        [SwaggerResponse(200, "OK", typeof(void))]
        public HttpResponseMessage Delete(int key)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var transaction = new DatabaseTransactionScope("Rest.Interviewer.Delete"))
            {
                _personRepository.Delete(key);

                transaction.Commit();
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
