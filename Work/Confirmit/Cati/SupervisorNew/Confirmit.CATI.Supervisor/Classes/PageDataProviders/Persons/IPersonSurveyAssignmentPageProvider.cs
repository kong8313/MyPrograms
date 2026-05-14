using System.Collections.Generic;

using Confirmit.CATI.Supervisor.Core.Surveys;

namespace Confirmit.CATI.Supervisor.Classes.PageDataProviders.Persons
{
    internal interface IPersonSurveyAssignmentPageProvider
    {
        string GetPageHint();

        List<SurveyInfoItem> GetSurveysListForAssignment(int interviewerId, string supervisorName, bool isGroup);

        void PerformAssignment(IEnumerable<int> interviewerOrGroupId, bool isGroup, List<int> selectedSurveysIDs, string supervisorName);

        string GetSaveConfirmation();
    }
}