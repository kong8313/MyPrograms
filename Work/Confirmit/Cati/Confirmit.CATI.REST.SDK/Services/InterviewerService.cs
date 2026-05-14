using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Services
{
    /// <summary>
    /// Class to work with interviewers
    /// </summary>
    public class InterviewerService : IInterviewerService
    {
        private readonly IRestClient _restClient;

        /// <summary>
        /// Creates and initializes an instance of InterviewerService class
        /// </summary>
        /// <param name="restClient">The instance of the rest client object</param>
        public InterviewerService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Get interviewer using OData filter
        /// </summary>
        /// <param name="odataQuery">The string with an OData query</param>
        /// <returns>List of Interviewer objects</returns>
        public async Task<List<Interviewer>> GetAsync(string odataQuery)
        {
            return await _restClient.GetAsyncMany<Interviewer>(UrlConstants.InterviewerUrlItem, odataQuery);
        }

        /// <summary>
        /// Get interviewer by unique identifier of the interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns>The Interviewer objects</returns>
        public async Task<Interviewer> GetAsync(int interviewerId)
        {
            return await _restClient.GetAsyncSingle<Interviewer>(UrlConstants.InterviewerUrlItem, interviewerId);
        }

        /// <summary>
        /// Create a new interviewer
        /// </summary>
        /// <param name="interviewerProperties">The instance of the InterviewerProperties object</param>
        /// <returns>Unique identifier of the new interviewer</returns>
        public async Task<int> Create(InterviewerProperties interviewerProperties)
        {
            var interviewerId =  await _restClient.PostAsync(UrlConstants.InterviewerPropertiesUrlItem, interviewerProperties, interviewerProperties.InterviewerId);
            
            interviewerProperties.InterviewerId = interviewerId;

            return interviewerId;
        }

        /// <summary>
        /// Update an existing interviewer
        /// </summary>
        /// <param name="interviewerProperties">The instance of the InterviewerProperties object</param>
        /// <returns>Unique identifier of the interviewer</returns>
        public async Task<int> Update(InterviewerProperties interviewerProperties)
        {
            return await _restClient.PutAsync( UrlConstants.InterviewerPropertiesUrlItem, interviewerProperties, interviewerProperties.InterviewerId);
        }

        /// <summary>
        /// Delete an existing interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns></returns>
        public async Task Delete(int interviewerId)
        {
            await _restClient.DeleteAsync(UrlConstants.InterviewerPropertiesUrlItem, interviewerId);
        }

        /// <summary>
        /// Get list of groups which contains the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns>List of Group objects</returns>
        public async Task<List<Group>> GetGroupsAsync(int interviewerId)
        {
            return await _restClient.InvokeActionForList<Group>(
                UrlConstants.InterviewerUrlItem,
                interviewerId,
                InterviewerActions.Namespace,
                InterviewerActions.GetGroups);
        }

        /// <summary>
        /// Get all assignment information related to the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns>List of SurveyAssignment objects</returns>
        public async Task<List<SurveyAssignment>> GetAssignments(int interviewerId)
        {
            return await _restClient.InvokeActionForList<SurveyAssignment>(
                UrlConstants.InterviewerUrlItem,
                interviewerId,
                InterviewerActions.Namespace,
                InterviewerActions.GetAssignments);
        }

        /// <summary>
        /// Assign the specified interviewer to the specified survey
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        public async Task AssignOnSurvey(int interviewerId, string surveyId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.InterviewerUrlItem, 
                interviewerId, 
                InterviewerActions.Namespace, 
                InterviewerActions.AssignOnSurvey,
                $"surveyId='{surveyId}'");
        }

        /// <summary>
        /// Unassign the specified interviewer from the specified survey
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        public async Task DeAssignFromSurvey(int interviewerId, string surveyId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.InterviewerUrlItem,
                interviewerId,
                InterviewerActions.Namespace,
                InterviewerActions.DeAssignFromSurvey,
                $"surveyId='{surveyId}'");
        }

        /// <summary>
        /// Assign the specified interviewer to the specified call
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="interviewId">Unique identifier of the interview</param>
        /// <returns></returns>
        public async Task AssignOnCall(int interviewerId, string surveyId, int interviewId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.InterviewerUrlItem,
                interviewerId,
                InterviewerActions.Namespace,
                InterviewerActions.AssignOnCall,
                $"surveyId='{surveyId}',interviewId={interviewId}");
        }

        /// <summary>
        /// Unassign the specified interviewer from all calls on the specified survey
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        public async Task DeAssignFromCalls(int interviewerId, string surveyId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.InterviewerUrlItem,
                interviewerId,
                InterviewerActions.Namespace,
                InterviewerActions.DeAssignFromCalls,
                $"surveyId='{surveyId}'");
        }

        /// <summary>
        /// Remove all assignments from the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns></returns>
        public async Task CleanAssignments(int interviewerId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.InterviewerUrlItem,
                interviewerId,
                InterviewerActions.Namespace,
                InterviewerActions.CleanAssignments);
        }

        /// <summary>
        /// Lock the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns></returns>
        public async Task Lock(int interviewerId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.InterviewerUrlItem,
                interviewerId,
                InterviewerActions.Namespace,
                InterviewerActions.Lock);
        }

        /// <summary>
        /// Unlock the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns></returns>
        public async Task Unlock(int interviewerId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.InterviewerUrlItem,
                interviewerId,
                InterviewerActions.Namespace,
                InterviewerActions.Unlock);
        }
    }
}