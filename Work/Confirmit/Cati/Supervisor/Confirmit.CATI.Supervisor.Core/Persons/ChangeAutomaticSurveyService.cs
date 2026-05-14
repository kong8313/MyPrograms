using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.Assignment;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
    public class ChangeAutomaticSurveyService : IChangeAutomaticSurveyService
    {
        private readonly IAssignmentManager _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
        private readonly IInterviewerApiClient _interviewerApiClient = ServiceLocator.Resolve<IInterviewerApiClient>();
        private readonly ICompanyInfo _companyInfo = ServiceLocator.Resolve<ICompanyInfo>();

        public bool ChangeSeamless(int personId, int surveyId)
        {
            var person = PersonRepository.GetById(personId);

            if (!IsSurveyAssignmentTaskChoice(person))
            {
                return false;
            }
            
            var isAssigned = _assignmentManager.IsPersonOrGroupAssigned(surveyId, personId);
            
            BvSpPerson_SetAutomaticSurveyEntity result;
            using (var transaction = new DatabaseTransactionScope(new DatabaseTransactionOptions("ChangeAutomaticSurvey", DeadlockPriority.Supervisor)))
            {
                if (!isAssigned)
                {
                    AssignmentWithEventLoggingPerformer.AssignSurveysToResource(false, personId, new List<int> { surveyId });
                }
                
                result = PersonService.SetAutomaticSurveySeamless(personId, surveyId);
                
                transaction.Commit();
            }

            if (result != null)
            {
                _interviewerApiClient.NotifyAutomaticSurveyChanged(_companyInfo.CompanyId, personId, surveyId);
            }
            // Stored procedure returns information from BvTasks if person is logged in and survey is being switched.

            return (result != null);
        }

        private bool IsSurveyAssignmentTaskChoice(BvPersonEntity person)
        {
            var isExplicitSurveyAssignmentTaskChoice = ((AgentTaskChoiceMode)person.ManualSelection == AgentTaskChoiceMode.CampaignAssignment);

            return (isExplicitSurveyAssignmentTaskChoice || IsManuallySelectedSurveyAssignmentTaskChoice(person));
        }

        private bool IsManuallySelectedSurveyAssignmentTaskChoice(BvPersonEntity person)
        {
            if ((AgentTaskChoiceMode)person.ManualSelection != AgentTaskChoiceMode.Choice)
            {
                return false;
            }

            if (person.AllowedChoices == null)
            {
                return false;
            }

            var isSurveyAssignment = (TaskChoicePermissions)person.AllowedChoices & TaskChoicePermissions.SurveyAssignment;

            return (isSurveyAssignment == TaskChoicePermissions.SurveyAssignment);
        }
    }
}
