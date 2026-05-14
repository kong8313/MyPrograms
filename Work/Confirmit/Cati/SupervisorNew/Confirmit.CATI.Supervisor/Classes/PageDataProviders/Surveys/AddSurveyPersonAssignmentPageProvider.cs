using System;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.PageDataProviders.Surveys
{
    internal class AddSurveyPersonAssignmentPageProvider : ISurveyPersonAssignmentPageProvider
    {
        #region ISurveyPersonAssignmentViewProvider Members

        public string GetPageHint()
        {
            return Strings.NotAssignedInterviewersAndGroupsHint;
        }

        public List<PersonAndGroupInfoItem> GetInterviewersListForAssignment(int surveyId)
        {
            return PersonManager.GetAllNotAssignedPersonsAndGroups(surveyId);
        }

        public void PerformAssignment(int surveyId, List<int> selectedInterviewersIDs)
        {
            AssignmentWithEventLoggingPerformer.AssignResourcesToSurvey(surveyId, selectedInterviewersIDs);
        }

        public string GetSaveConfirmation()
        {
            return String.Empty;
        }

        #endregion
    }
}