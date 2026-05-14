using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.REST.SDK.Model;

namespace Confirmit.CATI.REST.SDK.Interfaces
{
    /// <summary>
    /// Interface to work with interviewers
    /// </summary>
    public interface IInterviewerService
    {
        /// <summary>
        /// Get interviewer using OData filter
        /// </summary>
        /// <param name="odataQuery">The string with an OData query</param>
        /// <returns>List of Interviewer objects</returns>
        Task<List<Interviewer>> GetAsync(string odataQuery);

        /// <summary>
        /// Get interviewer by unique identifier of the interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns>The Interviewer objects</returns>
        Task<Interviewer> GetAsync(int interviewerId);

        /// <summary>
        /// Create a new interviewer
        /// </summary>
        /// <param name="interviewerProperties">The instance of the InterviewerProperties object</param>
        /// <returns>Unique identifier of the new interviewer</returns>
        Task<int> Create(InterviewerProperties interviewerProperties);

        /// <summary>
        /// Update an existing interviewer
        /// </summary>
        /// <param name="interviewerProperties">The instance of the InterviewerProperties object</param>
        /// <returns>Unique identifier of the interviewer</returns>
        Task<int> Update(InterviewerProperties interviewerProperties);

        /// <summary>
        /// Delete an existing interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns></returns>
        Task Delete(int interviewerId);

        /// <summary>
        /// Get list of groups which contains the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns>List of Group objects</returns>
        Task<List<Group>> GetGroupsAsync(int interviewerId);

        /// <summary>
        /// Get all assignment information related to the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns>List of SurveyAssignment objects</returns>
        Task<List<SurveyAssignment>> GetAssignments(int interviewerId);

        /// <summary>
        /// Assign the specified interviewer to the specified survey
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        Task AssignOnSurvey(int interviewerId, string surveyId);

        /// <summary>
        /// Unassign the specified interviewer from the specified survey
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        Task DeAssignFromSurvey(int interviewerId, string surveyId);

        /// <summary>
        /// Assign the specified interviewer to the specified call
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <param name="interviewId">Unique identifier of the interview</param>
        /// <returns></returns>
        Task AssignOnCall(int interviewerId, string surveyId, int interviewId);

        /// <summary>
        /// Unassign the specified interviewer from all calls on the specified survey
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <param name="surveyId">Unique identifier of the survey (pXXXXXXXX)</param>
        /// <returns></returns>
        Task DeAssignFromCalls(int interviewerId, string surveyId);

        /// <summary>
        /// Remove all assignments from the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns></returns>
        Task CleanAssignments(int interviewerId);

        /// <summary>
        /// Lock the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns></returns>
        Task Lock(int interviewerId);

        /// <summary>
        /// Unlock the specified interviewer
        /// </summary>
        /// <param name="interviewerId">Unique identifier of the interviewer</param>
        /// <returns></returns>
        Task Unlock(int interviewerId);
    }
}