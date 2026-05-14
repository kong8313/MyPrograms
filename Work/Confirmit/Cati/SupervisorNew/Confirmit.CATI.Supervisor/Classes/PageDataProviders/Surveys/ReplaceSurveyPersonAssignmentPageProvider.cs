using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.PageDataProviders.Surveys
{
    internal class ReplaceSurveyPersonAssignmentPageProvider : ISurveyPersonAssignmentPageProvider
    {
        private IAssignmentWithEventLoggingPerformer _assignmentWithEventLoggingPerformer = ServiceLocator.Resolve<IAssignmentWithEventLoggingPerformer>();
        
        public string GetPageHint()
        {
            return Strings.ReplaceSurveyPersonAssignmentHint;
        }

        public List<PersonAndGroupInfoItem> GetInterviewersListForAssignment(int surveyId)
        {
            return PersonManager.GetAllPersonsAndGroups();
        }

        public void PerformAssignment(int surveyId, List<int> selectedInterviewersIDs)
        {
            _assignmentWithEventLoggingPerformer.ReplaceSurveyPersonAssignments(surveyId, selectedInterviewersIDs);
        }

        public string GetSaveConfirmation()
        {
            return String.Format("if (!confirm('{0}')) return false;", Strings.ReplaceAssignment_Confirmation);
        }
    }
}