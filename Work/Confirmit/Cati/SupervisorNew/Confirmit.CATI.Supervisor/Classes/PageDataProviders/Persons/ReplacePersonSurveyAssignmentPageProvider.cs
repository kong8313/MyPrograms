using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.PageDataProviders.Persons
{
    internal class ReplacePersonSurveyAssignmentPageProvider : IPersonSurveyAssignmentPageProvider
    {
        private IAssignmentWithEventLoggingPerformer _assignmentWithEventLoggingPerformer = ServiceLocator.Resolve<IAssignmentWithEventLoggingPerformer>();
        
        public string GetPageHint()
        {
            return Strings.ReplacePersonSurveyAssignmentHint;
        }

        public List<SurveyInfoItem> GetSurveysListForAssignment(int interviewerId, string supervisorName, bool isGroup)
        {
            return SurveyManager.GetSurveys(supervisorName, String.Empty);
        }

        public void PerformAssignment(IEnumerable<int> interviewerOrGroupId, bool isGroup, List<int> selectedSurveysIDs, string supervisorName)
        {
            _assignmentWithEventLoggingPerformer.ReplacePersonSurveyAssignments(isGroup, interviewerOrGroupId, selectedSurveysIDs, supervisorName);
        }

        public string GetSaveConfirmation()
        {
            return String.Format("if (!confirm('{0}')) return false;", Strings.ReplaceAssignment_Confirmation);
        }
    }
}