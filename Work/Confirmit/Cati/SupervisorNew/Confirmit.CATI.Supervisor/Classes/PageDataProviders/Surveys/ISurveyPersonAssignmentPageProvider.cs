using System.Collections.Generic;

using Confirmit.CATI.Supervisor.Core.Persons;

namespace Confirmit.CATI.Supervisor.Classes.PageDataProviders.Surveys
{
    internal interface ISurveyPersonAssignmentPageProvider
    {
        string GetPageHint();

        List<PersonAndGroupInfoItem> GetInterviewersListForAssignment(int surveyId);

        void PerformAssignment( int surveyId, List<int> selectedInterviewersIDs);

        string GetSaveConfirmation();
    }
}