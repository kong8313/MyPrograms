using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;
using System;
using System.Web.UI.WebControls;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using System.Linq;

namespace Confirmit.CATI.Supervisor.Resources.Controls.Settings
{
    public partial class InterviewerConsoleSettingsControl : SettingsControlBase
    {
        private readonly IMnTciTools _mnTciTools;
        private readonly IDialersRepository _dialersRepository;
        private ISupervisorServiceClient _supervisorServiceClient;

        public InterviewerConsoleSettingsControl()
        {
            _mnTciTools = ServiceLocator.Resolve<IMnTciTools>();
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
        }

        public override GeneralToolbar Toolbar
        {
            get { return toolbar; }
        }

        public override XpMenuItem SaveButton
        {
            get { return btnSaveProperties; }
        }

        public override Button DefaultButton
        {
            get { return btnDefault; }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var isRedialVisible = IsRedialVisible();

            rowEnableRedialToolbarButtonSettings.Visible = isRedialVisible;
            rowEnableRedialNewNumber.Visible = isRedialVisible;

            rowEnableInternalCallTransfer.Visible = IsInternalCallTransferVisible();
            rowEnableExternalCallTransfer.Visible = IsExternalCallTransferVisible();
            rowEnableSoftphoneIntegration.Visible = IsSoftphoneIntegrationVisible();
            rowEnablePerformanceMetricsLink.Visible = IsPerformanceMetricsConfigurationEnabled();
        }

        public override void FillSettings()
        {
            var consoleSettings = SystemSettings.Console;

            cbEnablePreviousPage.Checked = consoleSettings.EnablePreviousPageToolbarButton;
            cbEnableNextPage.Checked = consoleSettings.EnableNextPageToolbarButton;
            cbEnableAppointment.Checked = consoleSettings.EnableAppointmentToolbarButton;
            cbEnableRedo.Checked = consoleSettings.EnableRedoToolbarButton;
            cbEnableFastForward.Checked = consoleSettings.EnableFastForwardToolbarButton;
            cbEnableCheckSpelling.Checked = consoleSettings.EnableCheckSpellingToolbarButton;
            cbEnableLogoutAfterFinish.Checked = consoleSettings.EnableLogoutAfterFinishToolbarButton;
            cbEnableTerminate.Checked = consoleSettings.EnableTerminateToolbarButton;
            cbEnableTakeBreak.Checked = consoleSettings.EnableTakeBreakToolbarButton;
            cbEnableChangeTaskChoice.Checked = consoleSettings.EnableChangeTaskChoiceToolbarButton;
            cbEnableMessageForm.Checked = consoleSettings.EnableMessageFormToolbarButton;
            cbEnableAppointmensList.Checked = consoleSettings.EnableAppointmensListToolbarButton;
            cbEnableRefresh.Checked = consoleSettings.EnableRefreshToolbarButton;

            cbEnableRedial.Checked = consoleSettings.EnableRedialToolbarButton;
            cbEnableHangUp.Checked = consoleSettings.EnableHangUpToolbarButton;
            cbEnableInternalCallTransfer.Checked = consoleSettings.EnableInternalCallTransferToolbarButton;
            cbEnableExternalCallTransfer.Checked = consoleSettings.EnableExternalCallTransferToolbarButton;
            cbEnableSoftphoneIntegration.Checked = consoleSettings.EnableSoftphoneIntegration;

            cbEnableAppointmentTimeZoneAdjustment.Checked = consoleSettings.EnableAppointmentTimeZoneAdjustment;
            cdEnableRedialNewNumber.Checked = consoleSettings.EnableRedialNewNumberRedialDialogAbility;
            cdEnableAppointmentsOutsidePermittedShiftTimes.Checked = consoleSettings.EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes;
            cdEnableAbilityToCancelDial.Checked = consoleSettings.EnableAbilityToCancelDial;
            cdEnableLogoutFromErrorAndWaitingScreen.Checked = consoleSettings.EnableLogoutFromErrorAndWaitingScreen;
            cdEnableTwoWayMessaging.Checked = consoleSettings.EnableTwoWayMessaging;
            cdEnableAutomaticScrolling.Checked = consoleSettings.EnableAutomaticScrolling;
            cbEnableInterviewsRandomization.Checked = consoleSettings.EnableInterviewsRandomization;
            cbOrderInterviewsByPriority.Checked = consoleSettings.OrderInterviewsByPriority;
            cbManualCallsInsideShiftOnly.Checked = consoleSettings.ManualCallsInsideShiftOnly;
            cbManualDialTypeSelection.Checked = consoleSettings.ManualDialTypeSelection;
        }

        public override void SaveSettings()
        {
            var consoleSettings = SystemSettings.Console;

            using (var transactionScope = new DatabaseTransactionScope("SetConsoleSettings", DeadlockPriority.Supervisor))
            {
                consoleSettings.EnablePreviousPageToolbarButton = cbEnablePreviousPage.Checked;
                consoleSettings.EnableNextPageToolbarButton = cbEnableNextPage.Checked;
                consoleSettings.EnableAppointmentToolbarButton = cbEnableAppointment.Checked;
                consoleSettings.EnableRedoToolbarButton = cbEnableRedo.Checked;
                consoleSettings.EnableFastForwardToolbarButton = cbEnableFastForward.Checked;
                consoleSettings.EnableCheckSpellingToolbarButton = cbEnableCheckSpelling.Checked;
                consoleSettings.EnableLogoutAfterFinishToolbarButton = cbEnableLogoutAfterFinish.Checked;
                consoleSettings.EnableTerminateToolbarButton = cbEnableTerminate.Checked;
                consoleSettings.EnableTakeBreakToolbarButton = cbEnableTakeBreak.Checked;
                consoleSettings.EnableChangeTaskChoiceToolbarButton = cbEnableChangeTaskChoice.Checked;
                consoleSettings.EnableMessageFormToolbarButton = cbEnableMessageForm.Checked;
                consoleSettings.EnableAppointmensListToolbarButton = cbEnableAppointmensList.Checked;
                consoleSettings.EnableRefreshToolbarButton = cbEnableRefresh.Checked;

                consoleSettings.EnableRedialToolbarButton = cbEnableRedial.Checked;
                consoleSettings.EnableHangUpToolbarButton = cbEnableHangUp.Checked;
                consoleSettings.EnableInternalCallTransferToolbarButton = cbEnableInternalCallTransfer.Checked;
                consoleSettings.EnableExternalCallTransferToolbarButton = cbEnableExternalCallTransfer.Checked;
                consoleSettings.EnableSoftphoneIntegration = cbEnableSoftphoneIntegration.Checked;

                consoleSettings.EnableRedialNewNumberRedialDialogAbility = cdEnableRedialNewNumber.Checked;
                consoleSettings.EnableAppointmentTimeZoneAdjustment = cbEnableAppointmentTimeZoneAdjustment.Checked;
                consoleSettings.EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes = cdEnableAppointmentsOutsidePermittedShiftTimes.Checked;
                consoleSettings.EnableAbilityToCancelDial = cdEnableAbilityToCancelDial.Checked;
                consoleSettings.EnableLogoutFromErrorAndWaitingScreen = cdEnableLogoutFromErrorAndWaitingScreen.Checked;
                consoleSettings.EnableTwoWayMessaging = cdEnableTwoWayMessaging.Checked;
                consoleSettings.EnableAutomaticScrolling = cdEnableAutomaticScrolling.Checked;
                consoleSettings.EnableInterviewsRandomization = cbEnableInterviewsRandomization.Checked;
                consoleSettings.OrderInterviewsByPriority = cbOrderInterviewsByPriority.Checked;
                consoleSettings.ManualCallsInsideShiftOnly = cbManualCallsInsideShiftOnly.Checked;
                consoleSettings.ManualDialTypeSelection = cbManualDialTypeSelection.Checked;
                transactionScope.Commit();
            }

            if (NeedShowWarning(consoleSettings))
            {
                ShowClientMessage(Strings.SettingsAllCatiInterviwerButtonsAreDiabledWarning);
            }
        }

        private bool NeedShowWarning(IConsoleSettings consoleSettings)
        {
            bool totalValue = consoleSettings.EnablePreviousPageToolbarButton;
            totalValue |= consoleSettings.EnableNextPageToolbarButton;
            totalValue |= consoleSettings.EnableAppointmentToolbarButton;
            totalValue |= consoleSettings.EnableRedoToolbarButton;
            totalValue |= consoleSettings.EnableFastForwardToolbarButton;
            totalValue |= consoleSettings.EnableCheckSpellingToolbarButton;

            if (IsRedialVisible())
            {
                totalValue |= consoleSettings.EnableRedialToolbarButton;
                totalValue |= consoleSettings.EnableRedialNewNumberRedialDialogAbility;
            }

            if (IsInternalCallTransferVisible())
            {
                totalValue |= consoleSettings.EnableInternalCallTransferToolbarButton;
            }

            if (IsExternalCallTransferVisible())
            {
                totalValue |= consoleSettings.EnableExternalCallTransferToolbarButton;
            }

            totalValue |= consoleSettings.EnableHangUpToolbarButton;
            totalValue |= consoleSettings.EnableSoftphoneIntegration;
            totalValue |= consoleSettings.EnableLogoutAfterFinishToolbarButton;
            totalValue |= consoleSettings.EnableTerminateToolbarButton;
            totalValue |= consoleSettings.EnableTakeBreakToolbarButton;
            totalValue |= consoleSettings.EnableChangeTaskChoiceToolbarButton;
            totalValue |= consoleSettings.EnableMessageFormToolbarButton;
            totalValue |= consoleSettings.EnableAppointmensListToolbarButton;
            totalValue |= consoleSettings.EnableRefreshToolbarButton;

            totalValue |= consoleSettings.EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes;
            totalValue |= consoleSettings.EnableAbilityToCancelDial;

            return !totalValue;
        }

        private bool IsRedialVisible()
        {
            var doesCompanyUseTelephony = _mnTciTools.DoesCompanyUseTelephony();
            return doesCompanyUseTelephony && SystemSettings.Console.ShowRedialButtonSetting;
        }

        private bool IsInternalCallTransferVisible()
        {
            return SystemSettings.Toggle.EnableInternalTransfer;
        }

        private bool IsExternalCallTransferVisible()
        {
            return SystemSettings.Toggle.EnableExternalTransfer;
        }

        private bool IsSoftphoneIntegrationVisible()
        {
            var doesCompanyUseTelephony = _mnTciTools.DoesCompanyUseTelephony();
            if (doesCompanyUseTelephony)
            {
                var dialers = _dialersRepository.GetAll();
                foreach (var dialer in dialers)
                {
                    var features = _supervisorServiceClient.GetDialerSupportedFeatures(dialer.Id);
                    if (features.IsSoftphoneSingleSignOnSupported == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private bool IsPerformanceMetricsConfigurationEnabled()
        {
            return SystemSettings.Toggle.EnableInterviewerMetricsConfiguration;
        }
    }
}
