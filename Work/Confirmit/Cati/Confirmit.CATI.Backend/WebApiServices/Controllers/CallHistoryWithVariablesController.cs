using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Web.OData;
using Swashbuckle.Swagger.Annotations;

namespace Confirmit.CATI.Backend.WebApiServices.Controllers
{
    public class CallHistoryWithVariablesController : ODataController
    {
        private readonly ICallHistoryDataProvider _dataProvider;
        private readonly ISurveyRepository _surveyRepository;

        public CallHistoryWithVariablesController(ICallHistoryDataProvider dataProvider, ISurveyRepository surveyRepository)
        {
            _dataProvider = dataProvider;
            _surveyRepository = surveyRepository;
        }

        /// <summary>
        /// Get call history including replicated variables
        /// </summary>
        /// <param name="surveyIds">List of unique identifiers for the surveys (pXXXXXXXX), separated by comma or semicolon. If not specified, the history for all surveys will be returned.</param>
        /// <param name="includeBreakTimes">Whether break times are to be included</param>
        /// <param name="includeLoginLogoutInfo">Whether login/logout information is to be included in the result</param>
        /// <param name="startTime">When specified, only events occurring after this time will be returned</param>
        /// <param name="endTime">When specified, only events that occurred before this time will be returned</param>
        /// <param name="variables">Survey field names separated by comma or semicolon (having 'Available as CATI filter' option enabled) to be included in the result</param>
        /// <returns>List of CallHistoryWithVariables objects</returns>
        [SwaggerResponse(200, "OK", typeof(List<CallHistoryWithVariables>))]
        public HttpResponseMessage GetCallHistoryWithVariables(
            string surveyIds = null, 
            bool includeBreakTimes = false, 
            bool includeLoginLogoutInfo = false, 
            DateTime? startTime = null,
            DateTime? endTime = null,
            string variables = null)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            string internalSurveyIds;

            try
            {
                internalSurveyIds = ConvertProjectIdsToSurveyIds(surveyIds);
            }
            catch (ArgumentException ex)
            {
                Trace.TraceWarning(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var variablesArr = new string[0];
            if (!string.IsNullOrEmpty(variables))
            {
                _dataProvider.IncludeReplicatedVariables = true;
                variablesArr = variables.Split(',', ';').Select(x => x.Trim()).ToArray();
            }

            var callHistoryList = _dataProvider.GetCallHistoryData(internalSurveyIds, startTime, endTime, variablesArr, includeBreakTimes, includeLoginLogoutInfo);

            int numberInOrder = 0;
            var result = callHistoryList.Select(x=> new CallHistoryWithVariables() 
            {
                NumberInOrder = numberInOrder++,
                Id = x.Id,
                CallCenterId = x.CallCenterId,
                Duration = x.Duration,
                ExtendedStatus = x.ExtendedStatus,
                InterviewerId = x.InterviewerID,
                InterviewerName = x.InterviewerName,
                InterviewId = x.InterviewID,
                SurveyId = x.ProjectID,
                SurveyName = x.Name,
                TelephoneNumber = x.TelephoneNumber,
                Time = ConvertToDateTimeOffset(x.FiredTime),
                WaitingTime = x.WaitingTime,
                Variables = CreateVariableList(variablesArr, x.ReplicatedVariables)
            }).ToList();

            var response = Request.CreateResponse(HttpStatusCode.OK, result);

            return response;
        }

        private DateTimeOffset? ConvertToDateTimeOffset(DateTime? firedTime)
        {
            if (firedTime == null)
                return null;

            return new DateTimeOffset(firedTime.Value, TimeSpan.Zero);
        }

        /// <summary>
        /// Convert a string with project sids delimited by comma or semicolon to a string with survey ids delimited by comma
        /// </summary>
        /// <param name="surveyIds">Project sids delimited by comma or semicolon</param>
        /// <returns></returns>
        private string ConvertProjectIdsToSurveyIds(string surveyIds)
        {
            if (string.IsNullOrEmpty(surveyIds))
            {
                return null;
            }

            string ids = string.Join(",", surveyIds.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(TryGetSidByProjectId));

            // Protect from projectSids with delimits only and with not existed projects
            if (ids == string.Empty)
            {
                throw new ArgumentException($"Wrong surveyId parameter ({surveyIds}) in GetCallHistoryWithVariables method in CallHistoryWithVariablesController class. Return empty list.");
            }

            return ids;
        }

        private int? TryGetSidByProjectId(string projectSid)
        {
            var survey = _surveyRepository.TryGetByProjectId(projectSid);
            if (survey == null || survey.State == (int)SurveyState.SoftDeleted)
            {
                return null;
            }

            return survey.SID;
        }

        private List<Variable> CreateVariableList(string[] variablesArr, List<string> replicatedVariables)
        {
            var result = new List<Variable>();

            if (replicatedVariables == null || variablesArr.Length != replicatedVariables.Count)
            {
                return result;
            }

            for (int i = 0; i < variablesArr.Length; i++)
            {
                if (i < replicatedVariables.Count && !string.IsNullOrEmpty(replicatedVariables[i]))
                {
                    result.Add(new Variable() { Name = variablesArr[i], Value = replicatedVariables[i] });
                }
            }

            return result;
        }
    }
}
