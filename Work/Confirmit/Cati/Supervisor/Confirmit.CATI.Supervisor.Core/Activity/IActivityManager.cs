using System.Collections.Generic;
using Confirmit.CATI.Core.Tasks;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    public interface IActivityManager
    {
        /// <summary>
        /// Get interviewers performance data. 
        /// </summary>
        /// <param name="onlyLogged">If true the data will be selected only for logged users</param>
        /// <param name="filterBySurveys">If true the data will be splitted by surveys</param>
        /// <param name="activeSurveysOnly">If true the data will be selected only for active surveys</param>
        /// <param name="callCenterId">Call center</param>
        /// <param name="interviewersId">Selected interviewers for which you want to get performance</param>
        /// <param name="surveysId">Selected surveys for which you want to get performance</param>
        /// <returns>Information about interviewers performance</returns>
        List<InterviewerPerformanceInfo> GetInterviewerPerformanceData(bool onlyLogged, bool filterBySurveys, bool activeSurveysOnly, int callCenterId, int[] interviewersId = null, IEnumerable<int> surveysId = null);

        void TerminateTaskByPerson(int personId, string reason = null);

        List<StatusInfo> GetStatusBreakdown(int surveyId);

        /// <summary>
        /// Gets general survey-specific CATI activity data from Fusion.
        /// </summary>
        /// <param name="sortExpression">Database name of column the list is currently sorted on.</param>
        /// <param name="sortOrderAsc">Sorting direction (ascending or descending).</param>
        /// <param name="showOnlyActiveSurveys">If true only surveys that have people working on them appear.</param>
        /// <param name="surveys">The surveys.</param>
        /// <param name="its">Array of its (up to 5) to include in query</param>
        /// <returns>List of survey activity data objects.</returns>
        List<SurveyActivityInfo> GetSurveyActivityData(
            string sortExpression,
            bool sortOrderAsc,
            bool showOnlyActiveSurveys,
            IEnumerable<int> surveys,
            bool onlyCatiInterviews,
            params int[] its
        );

        List<TaskActivityInfo> GetTasksActivityData(
            string sortExpression,
            bool sortOrderAsc,
            bool alertsOnTop,
            IEnumerable<int> surveys,
            IEnumerable<int> interviewers,
            string superName);

        List<TaskActivityInfo> GetTasksActivityData(
           string sortExpression,
           bool sortOrderAsc,
           bool alertsOnTop,
           IEnumerable<int> surveys,
           IEnumerable<int> interviewers,
           string superName,
           bool allCalcenters);

        List<StatusAlertInfo> GetStatusAlertsList(bool includeDefault);
    }
}