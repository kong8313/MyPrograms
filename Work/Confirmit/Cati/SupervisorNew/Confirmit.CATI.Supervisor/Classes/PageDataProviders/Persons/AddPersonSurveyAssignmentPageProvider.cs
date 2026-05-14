using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.PageDataProviders.Persons
{
    internal class AddPersonSurveyAssignmentPageProvider : IPersonSurveyAssignmentPageProvider
    {
        #region ISurveyPersonAssignmentViewProvider Members

        public string GetPageHint()
        {
            return Strings.NotAssignedSurveysHint;
        }

        public List<SurveyInfoItem> GetSurveysListForAssignment(int interviewerId, string supervisorName, bool isGroup)
        {
            return ServiceLocator.Resolve<IAssignmentManager>().GetNotAssignedSurveysList(interviewerId, supervisorName, isGroup);
        }

        public void PerformAssignment(IEnumerable<int> interviewerOrGroupId, bool isGroup, List<int> selectedSurveysIDs, string supervisorName)
        {
            AssignmentWithEventLoggingPerformer.AssignSurveysToResources(isGroup, interviewerOrGroupId, selectedSurveysIDs);
        }

        public string GetSaveConfirmation()
        {
            return String.Empty;
        }

        #endregion
    }
}