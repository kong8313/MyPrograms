using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Validators;
using Confirmit.CATI.Core.Validators.Interfaces;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.Persons;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Resources;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core;
using System;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public partial class ActivateCalls : BaseActionForm
    {
        private readonly IMultipleAssignmentValidator _multipleAssignmentValidator;
        private readonly ISystemSettings _systemSettings;

        private int Priority => wnePriority.ValueInt;

        private int ShiftType => ddlShiftType.SelectedShiftTypeID;

        private int? ExtendedStatus => ddlExtendedStatus.SelectedExtendedStatusID;

        private DateTime TimeToCall => cbxSetToNow.Checked ? CallHelper.FusionDateNow : dteTimeToCall.DateTimeValue;

        private bool EnableDisabledCalls => cbEnableDisabledCalls.Checked;

        public ActivateCalls()
        {
            _multipleAssignmentValidator = ServiceLocator.Resolve<IMultipleAssignmentValidator>();
            _systemSettings = ServiceLocator.Resolve<ISystemSettings>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            personsAndGroupsList.AllowMultiSelection = _systemSettings.MultipleAssignments.Enabled;
            personsAndGroupsList.Data = PersonManager.GetAllPersonsAndGroups(false);

            var height = 128;
            if (SurveyManager.GetDialingMode(SurveyID) == DialingMode.Predictive)
            {
                noPersonOrGroupHint.Text += " " + Strings.OnlyInterviewersWithSurveySelectionModeCanWorkOnPredictiveSurvey;
                height  += 12;
            }

            if (_systemSettings.MultipleAssignments.Enabled)
            {
                noPersonOrGroupHint.Text += Environment.NewLine + Strings.MultipleAssignments_PossibilityOfMultipleAssignments;
                height += 24;
            }

            updatePanel.Attributes.CssStyle["top"] = height + "px";
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            /* Use LoadComplete because filling of shiftType dropdown 
               requires SurveyId which is available on PageLoad stage */

            if (IsPostBack == false)
            {
                ddlShiftType.SelectedShiftTypeID = (int)CallShiftType.AnyValid;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterClientScripts();
        }

        protected void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                MultipleAssignmentValidationResult multipleAssignmentValidationResult =
                    _multipleAssignmentValidator.ValidateMultipleAssignment(personsAndGroupsList.SelectedKeys);

                if (multipleAssignmentValidationResult == MultipleAssignmentValidationResult.ContainsMultiplePersons)
                {
                    NotifyUser(Strings.ActivateCalls_MultiplePersonsWarning);
                    return;
                }

                if (multipleAssignmentValidationResult == MultipleAssignmentValidationResult.GroupsAssignmentContainsPersons)
                {
                    NotifyUser(Strings.ActivateCalls_GroupsContainsPersonsWarning);
                    return;
                }

                LegacySupervisorMetrics.OnCallManagementAction("Activate");
                var operationEntity = CallManager.ActivateCalls(
                        SurveyID,
                        Priority,
                        CallState,
                        personsAndGroupsList.SelectedInterviewersIDs.ToArray(),
                        ShiftType,
                        ExtendedStatus,
                        TimeToCall,
                        EnableDisabledCalls,
                        BatchParameters);

                Redirect(operationEntity);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void RegisterClientScripts()
        {
            cbxSetToNow.Attributes.Add("onclick", dteTimeToCall.ClientControllerName + ".setEnabled(!this.checked)");
        }
    }
}
