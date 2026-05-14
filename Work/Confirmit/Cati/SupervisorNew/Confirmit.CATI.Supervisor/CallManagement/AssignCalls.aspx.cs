using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Validators;
using Confirmit.CATI.Core.Validators.Interfaces;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Surveys;
using System;
using Confirmit.CATI.Core;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public partial class AssignCall : BaseActionForm
    {
        private readonly IMultipleAssignmentValidator _multipleAssigmentValidator;
        private readonly ISystemSettings _systemSettings;

        private string[] SelectedKeys
        {
            get { return personsAndGroupsList.SelectedKeys; }
        }

        public AssignCall()
        {
            _multipleAssigmentValidator = ServiceLocator.Resolve<IMultipleAssignmentValidator>();
            _systemSettings = ServiceLocator.Resolve<ISystemSettings>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            personsAndGroupsList.AllowMultiSelection = _systemSettings.MultipleAssignments.Enabled;
            personsAndGroupsList.Data = PersonManager.GetAllPersonsAndGroups(false);

            if (!_systemSettings.MultipleAssignments.Enabled)
            {
                personsAndGroupsList.HintText = Strings.MultipleAssignments_PossibilityOfMultipleAssignments;
            }
        }

        protected void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (SelectedKeys.Length == 0)
                {
                    NotifyUser(GetResString("SelectPersonOrGroup"));
                    return;
                }

                if (personsAndGroupsList.SelectedInterviewersIDs.Count == 0)
                {
                    NotifyUser(GetResString("SelectPersonOrGroup"));
                    return;
                }

                MultipleAssignmentValidationResult multipleAssgimentValidationResult =
                    _multipleAssigmentValidator.ValidateMultipleAssignment(personsAndGroupsList.SelectedKeys);

                if (multipleAssgimentValidationResult == MultipleAssignmentValidationResult.ContainsMultiplePersons)
                {
                    NotifyUser(GetResString("AssignCalls_MultiplePersonsWarning"));
                    return;
                }

                if (multipleAssgimentValidationResult == MultipleAssignmentValidationResult.GroupsAssignmentContainsPersons)
                {
                    NotifyUser(GetResString("AssignCalls_GroupsContainsPersonsWarning"));
                    return;
                }

                LegacySupervisorMetrics.OnCallManagementAction("Assign");
                var operationEntity = CallManager.AssignCalls(SurveyID, personsAndGroupsList.SelectedInterviewersIDs.ToArray(), BatchParameters);

                Redirect(operationEntity);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}
