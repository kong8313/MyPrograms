using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    /// Interface to work with interviewer groups
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// Get interviewer groups
        /// </summary>
        /// <param name="odataQuery">The string with an OData query</param>
        /// <returns>List of Group objects</returns>
        Task<List<Group>> GetAsync(string odataQuery);

        /// <summary>
        /// Get interviewer groups
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <returns>The Group object</returns>
        Task<Group> GetAsync(int groupId);

        /// <summary>
        /// Create a new interviewer group
        /// </summary>
        /// <param name="group">The instance of the group object</param>
        /// <returns>Unique identifier of the interviewer group</returns>
        Task<int> Create(Group group);

        /// <summary>
        /// Update an existing interviewer group
        /// </summary>
        /// <param name="group">The instance of the group object</param>
        /// <returns>Unique identifier of the interviewer group</returns>
        Task<int> Update(Group group);

        /// <summary>
        /// Delete an existing interviewer group
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <returns></returns>
        Task Delete(int groupId);

        /// <summary>
        /// Get all interviewers belonging to the specified group
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>List of Interviewer objects</returns>
        Task<List<Interviewer>> GetInterviewersAsync(int groupId, int callCenterId);

        /// <summary>
        /// Get all assignment information related to the specified group
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns>List of SurveyAssignment objects</returns>
        Task<List<SurveyAssignment>> GetAssignments(int groupId, int callCenterId);

        /// <summary>
        /// Assign the specified group to the specified survey
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        Task AssignOnSurvey(int groupId, string surveyId, int callCenterId);

        /// <summary>
        /// Unassign the specified group from the specified survey
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        Task DeAssignFromSurvey(int groupId, string surveyId, int callCenterId);

        /// <summary>
        /// Assign the specified group to the specified call
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="interviewId">Unique identifier of the interview</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        Task AssignOnCall(int groupId, string surveyId, int interviewId, int callCenterId);

        /// <summary>
        /// Unassign the specified group from all calls on the specified survey
        /// </summary>
        /// <param name="groupId">Unique identifier of the interviewer group</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="callCenterId">Unique identifier of the call center</param>
        /// <returns></returns>
        Task DeAssignFromCalls(int groupId, string surveyId, int callCenterId);
    }
}