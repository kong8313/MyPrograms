using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Assignment
{
    public interface IAssignmentWithEventLoggingPerformer
    {
        void ReplaceSurveyPersonAssignments(int surveyId, List<int> interviewerOrGroupIds);

        void ReplacePersonSurveyAssignments(bool isGroup, int interviewerOrGroupId, List<int> surveysIds, string supervisorName);

        void ReplacePersonSurveyAssignments(bool isGroup, IEnumerable<int> interviewerOrGroupIds, List<int> surveysIds, string supervisorName);
        
        void DeassignResourcesFromSurveyCalls(int surveyId, IEnumerable<int> interviewerOrGroupIds);

        int AssignResourcesToSurveyUsingSurveyAssignmentsDialog(int surveySid, IEnumerable<int> personSids);

        int DeassignResourcesFromSurveyUsingSurveyAssignmentsDialog(int surveySid, IEnumerable<int> personSids);
    }
}