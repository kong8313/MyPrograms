using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Constants;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Services
{
    /// <summary>
    /// Class to work with interviewer groups
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly IRestClient _restClient;

        /// <summary>
        /// Creates and initializes an instance of GroupService class
        /// </summary>
        /// <param name="restClient">The instance of the rest client object</param>
        public GroupService(IRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Get interviewer groups using OData filter
        /// </summary>
        /// <param name="odataQuery">The string with an OData query</param>
        /// <returns>List of Group objects</returns>
        public async Task<List<Group>> GetAsync(string odataQuery)
        {
            return await _restClient.GetAsyncMany<Group>(UrlConstants.GroupsUrlItem, odataQuery);
        }

        /// <summary>
        /// Get interviewer group by unique identifier of the group
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <returns>The Group object</returns>
        public async Task<Group> GetAsync(int groupId)
        {
            return await _restClient.GetAsyncSingle<Group>(UrlConstants.GroupsUrlItem, groupId);
        }

        /// <summary>
        /// Create a new interviewer group
        /// </summary>
        /// <param name="group">The instance of the group object</param>
        /// <returns>Unique identifier of the new interviewer group</returns>
        public async Task<int> Create(Group group)
        {
            return await _restClient.PostAsync(UrlConstants.GroupsUrlItem, group, group.GroupId);
        }

        /// <summary>
        /// Update an existing interviewer group
        /// </summary>
        /// <param name="group">The instance of the group object</param>
        /// <returns>Unique identifier of the interviewer group</returns>
        public async Task<int> Update(Group group)
        {
            return await _restClient.PutAsync(UrlConstants.GroupsUrlItem, group, group.GroupId);
        }

        /// <summary>
        /// Delete an existing interviewer group
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <returns></returns>
        public async Task Delete(int groupId)
        {
            await _restClient.DeleteAsync(UrlConstants.GroupsUrlItem, groupId);
        }

        /// <summary>
        /// Get all interviewers belonging to the specified group
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>List of Interviewer objects</returns>
        public async Task<List<Interviewer>> GetInterviewersAsync(int groupId, int callCenterId)
        {
            return await _restClient.InvokeActionForList<Interviewer>(
                UrlConstants.GroupsUrlItem,
                groupId,
                GroupActions.Namespace,
                GroupActions.GetInterviewers,
                $"callCenterId={callCenterId}");
        }

        /// <summary>
        /// Get all assignment information related to the specified group
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>List of SurveyAssignment objects</returns>
        public async Task<List<SurveyAssignment>> GetAssignments(int groupId, int callCenterId)
        {
            return await _restClient.InvokeActionForList<SurveyAssignment>(
                UrlConstants.GroupsUrlItem,
                groupId,
                GroupActions.Namespace,
                GroupActions.GetAssignments,
                $"callCenterId={callCenterId}");
        }

        /// <summary>
        /// Assign the specified group to the specified survey
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        public async Task AssignOnSurvey(int groupId, string surveyId, int callCenterId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.GroupsUrlItem,
                groupId,
                GroupActions.Namespace,
                GroupActions.AssignOnSurvey,
                $"surveyId='{surveyId}', callCenterId={callCenterId}");
        }

        /// <summary>
        /// Unassign the specified group from the specified survey
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        public async Task DeAssignFromSurvey(int groupId, string surveyId, int callCenterId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.GroupsUrlItem,
                groupId,
                GroupActions.Namespace,
                GroupActions.DeAssignFromSurvey,
                $"surveyId='{surveyId}', callCenterId={callCenterId}");
        }

        /// <summary>
        /// Assign the specified group to the specified call
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="interviewId">Unique identifier of the interview</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        public async Task AssignOnCall(int groupId, string surveyId, int interviewId, int callCenterId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.GroupsUrlItem,
                groupId,
                GroupActions.Namespace,
                GroupActions.AssignOnCall,
                $"surveyId='{surveyId}',interviewId={interviewId},callCenterId={callCenterId}");
        }

        /// <summary>
        /// Unassign the specified group from all calls on the specified survey
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        public async Task DeAssignFromCalls(int groupId, string surveyId, int callCenterId)
        {
            await _restClient.InvokeActionForVoid(
                UrlConstants.GroupsUrlItem,
                groupId,
                GroupActions.Namespace,
                GroupActions.DeAssignFromCalls,
                $"surveyId='{surveyId}',callCenterId={callCenterId}");
        }
    }
}