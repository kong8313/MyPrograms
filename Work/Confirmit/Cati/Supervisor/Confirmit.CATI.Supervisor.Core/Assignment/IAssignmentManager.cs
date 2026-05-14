using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Backend.Assignment;
using Confirmit.CATI.Supervisor.Core.Surveys;

namespace Confirmit.CATI.Supervisor.Core.Assignment
{
    public interface IAssignmentManager
    {
        /// <summary>
        /// Gets the list of CATI interviewers and groups assigned to specific survey.
        /// List includes information both for explisit and implicit assignments.
        /// </summary>
        /// <param name="surveySID">The survey SID to retive data for.</param>
        /// <returns>The list of interviewers and groups assigned to specific survey.</returns>
        List<SurveyAssignmentInfoItem> GetAssignedInterviewersAndGroupsList(int surveySID);
        List<SurveyAssignmentInfoItem> GetAssignedInterviewersAndGroupsList(int surveySID, int callCenterId);

        /// <summary>
        /// Returns list of surveys assigned to specific interviewer or group.
        /// </summary>
        /// <param name="sid">Person or group SID to get surveys assigned to.</param>
        /// <param name="supervisorName">Supervisor user name.</param>
        /// <returns>List of assignments.</returns>
        List<PersonAssignmentInfoItem> GetAssignedSurveyList(int sid, string supervisorName);
        
        /// <summary>
        /// Gets the list of surveys not assigned to specific interviewer or group.
        /// </summary>
        /// <param name="sid">The sid of interviewer or group to retive data for.</param>
        /// <param name="supervisorName">Name of current supervisor.</param>
        /// <param name="isGroup">true, if we pass group identifier as sid; otherwise false.</param>
        /// <returns>List of surveys not assigned to specific interviewer or group.</returns>
        List<SurveyInfoItem> GetNotAssignedSurveysList(int sid, string supervisorName, bool isGroup);

        /// <summary>
        /// Remove surveys from survey list which are assigned to specific interviewer or group.
        /// </summary>
        /// <param name="surveysList">List of survey to cleanup</param>
        /// <param name="sid">The sid of interviewer or group to retive data for.</param>
        /// <param name="supervisorName">Name of current supervisor.</param>
        /// <param name="isGroup">true, if we pass group identifier as sid; otherwise false.</param>
        /// <returns>List of surveys not assigned to specific interviewer or group.</returns>
        List<SurveyInfoItem> RemoveAssignedSurveysFromList(List<SurveyInfoItem> surveysList, int sid,
            string supervisorName, bool isGroup);

        /// <summary>
        /// Determines whether specific person assigned to the specified survey.
        /// </summary>
        /// <param name="surveySID">The survey SID to get data for.</param>
        /// <param name="personOrGroupSID">The person or group SID to get data for.</param>
        /// <returns>
        /// 	<c>true</c> if person assigned to the specified survey; otherwise, <c>false</c>.
        /// </returns>
        bool IsPersonOrGroupAssigned(int surveySID, int personOrGroupSID);

        void ClearSurveyAssignments(int surveyId, int callCenterId);

        List<PersonAssignmentInfoItemWithGroupName> GetPersonAssignments(int sid, string supervisorName, int callCenterId);
    }
}