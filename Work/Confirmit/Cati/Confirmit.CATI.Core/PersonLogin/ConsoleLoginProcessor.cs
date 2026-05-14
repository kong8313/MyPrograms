using System.Collections.Generic;
using System.Linq;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.PersonLogin
{
    public class ConsoleLoginProcessor : IConsoleLoginProcessor
    {
        private readonly ITelephony _telephony;
        private readonly IToggleSettings _toggleSettings;
        private readonly IConsoleSettings _consoleSettings;
        private readonly IDialerSettings _dialerSettings;
        private readonly IDialerAvailabilityManager _dialerAvailabilityManager;
        private readonly ILicenseService _licenseService;
        private readonly IPersonDeferredMonitoringRepository _personDeferredMonitoringRepository;
        private readonly IBreakTypeRepository _breakTypeRepository;


        public ConsoleLoginProcessor(
            ITelephony telephony,
            IToggleSettings toggleSettings,
            IConsoleSettings consoleSettings,
            IDialerSettings dialerSettings,
            IDialerAvailabilityManager dialerAvailabilityManager,
            ILicenseService licenseService,
            IPersonDeferredMonitoringRepository personDeferredMonitoringRepository,
            IBreakTypeRepository breakTypeRepository)
        {
            _telephony = telephony;
            _toggleSettings = toggleSettings;
            _consoleSettings = consoleSettings;
            _dialerSettings = dialerSettings;
            _dialerAvailabilityManager = dialerAvailabilityManager;
            _licenseService = licenseService;
            _personDeferredMonitoringRepository = personDeferredMonitoringRepository;
            _breakTypeRepository = breakTypeRepository;
        }

        public BvTasksEntity Login(
            [NotNull] BvPersonEntity person,
            [CanBeNull] BvTasksEntity task,
            [NotNull] StationInfo stationInfo,
            out bool isAlreadyLoggedIn)
        {
            isAlreadyLoggedIn = task != null && task.StatusLogout != (byte)LoginState.NOT_LOGGED_IN;

            if (isAlreadyLoggedIn)
            {
                // TODO: Should that check be removed as we do same check in the AuthorizeAndReturnCompanyId method?
                TaskService.CheckNotLoggedInFromAnotherStation(person, task, stationInfo.StationId);

                EventDetailsScope.Current.AddTiming("Login:CheckNotLoggedInFromAnotherStation");

                TaskService.GenerateAndUpdateAuthenticationKeyForTask(task);

                EventDetailsScope.Current.AddTiming("Login:GenerateAndUpdateAuthenticationKeyForTask");

                ClearDeferredRecordCallIdIfExist(task);

                EventDetailsScope.Current.AddTiming("Login:ClearDeferredRecordCallIdIfExist");
            }
            else
            {
                if (person.Type == (byte)AgentType.LiveAgent && !_toggleSettings.EnableDesktopConsoleLogin)
                {
                    throw new UserMessageException("Login to desktop console is prohibited", "AccessIsNotSupported");
                }

                _licenseService.CheckLicense((AgentType)person.Type);

                //TODO: Review this call. Most likely it should be moved from here and placed somewhere closer login to dialer
                ValidatePersonModeDialerCompatibility(person, stationInfo.DialerId);

                task = PersonService.LoginPerson(person.SID, stationInfo);

                EventDetailsScope.Current.AddTiming("Login:PersonService.LoginPerson");

                // If interviewer selecting mode is 'Manual' or 'SurveyAssignment' move task to 'Selecting' state
                if (person.AllowedChoices == null &&
                    person.ManualSelection != (int)AgentTaskChoiceMode.Automatic)
                {
                    TaskService.MoveTaskToState(task, InterviewState.SELECTING, DialingMode.Manual);

                    EventDetailsScope.Current.AddTiming("Login:MoveTaskToState");
                }
            }

            return task;
        }

        private void ValidatePersonModeDialerCompatibility([NotNull] BvPersonEntity person, int dialerId)
        {
            // TODO: Should that check be moved out of that class at all to the stage where we do log in to the dialer?
            if (_dialerAvailabilityManager.IsConnectedToDialer(person.DialType, dialerId) &&
                !_telephony.IsPersonModeSupported((AgentTaskChoiceMode)person.ManualSelection))
            {
                var exceptionDescription = string.Format(
                    "Dialer {0} does no work with interviewers in {1} mode.",
                    _dialerSettings.DialerType,
                    (AgentTaskChoiceMode)person.ManualSelection);

                throw new UserMessageException(exceptionDescription, "Error_PersonModeIsNotSupportedByDialer");
            }
        }

        public PersonInfo GetPersonInfo([NotNull] BvPersonEntity person, [NotNull] BvTasksEntity task, bool isAlreadyLoggedIn)
        {
            var personInfo = new PersonInfo
            {
                PersonId = person.SID,
                PersonMode = person.ManualSelection,
                TaskChoicePermissions = person.AllowedChoices,
                AuthenticationKey = task.AuthenticationKey.Value,
                EncryptionKey = task.EncryptionKey,
                EncryptionIV = task.EncryptionIV,
                // DialType is being taken from the task as it should be the same until Logout
                DialType = (DialType)task.DialTypeId,
                AlreadyLoggedIn = isAlreadyLoggedIn
            };

            // If person in survey assignment mode then we return Confirmit id (e.g. 'p0000000')
            // of person automatic survey (if exists)
            BvSurveyEntity survey;
            if (personInfo.PersonMode == (int)AgentTaskChoiceMode.CampaignAssignment &&
                (survey = PersonService.GetPersonAutomaticSurvey(person)) != null)
            {
                personInfo.AutoSurveyId = survey.Name;
            }

            return personInfo;
        }

        private void ClearDeferredRecordCallIdIfExist([NotNull] BvTasksEntity task)
        {
            if (task.CallID.HasValue == false || task.CallID == 0)
            {
                return;
            }

            // TODO: Re-implement as 1 SQL statement
            var deferredRecord = _personDeferredMonitoringRepository.GetByCallId(task.CallID.Value);
            if (deferredRecord != null)
            {
                BvPersonDeferredMonitoringAdapterEx.ClearCallId(deferredRecord.ID);
            }
        }

        // TODO: Whole function should probably be moved out of that class to the login to dialer handling class
        public DiallerInfo GetDialerInfo([NotNull] BvTasksEntity task, [NotNull] StationInfo stationInfo, bool isAlreadyLoggedIn)
        {
            var dialerInfo = new DiallerInfo
            {
                CurrentLoggedInToDialerState = (int)LoginState.NOT_LOGGED_IN,
            };

            if (isAlreadyLoggedIn)
            {
                // TODO: Seems like if condition is wrong, e.g. why we do not have break related statuses in if, or may be we do not need if at all
                if (BvCallHandlerRoot.IsLoggedInToDialer(task))
                {
                    dialerInfo.CurrentIsPredictive = !task.IsLoginRCToDialer;
                }

                dialerInfo.CurrentLoggedInToDialerState = task.LoggedInToDialerState;
            }

            dialerInfo.ConnectedToDialer = _dialerAvailabilityManager.IsConnectedToDialer(task.DialType, stationInfo.DialerId);

            if (!dialerInfo.ConnectedToDialer)
            {
                return dialerInfo;
            }

            dialerInfo.IsHangUpSupported = _telephony.IsHangUpSupported();
            dialerInfo.IsPauseOrResumePlaybackSupported = _telephony.IsPauseOrResumePlaybackSupported();
            dialerInfo.IsToggleVoiceSourceSupported = _telephony.IsToggleInterviewerListensToPlaybackOrRespondentSupported();
            dialerInfo.HasExtensionNumber = !_telephony.IsDynamicExtensionNumberAllowed(stationInfo.IsLocal);

            return dialerInfo;
        }

        public CatiConsolePropertiesContainer GetConsolePropertiesInfo()
        {
            var breakTypes = _breakTypeRepository.GetAll()
                .Select(x => new BreakType(x.Id, x.Name, x.Description, x.IsPaid)).ToList();

            return new CatiConsolePropertiesContainer
            {
                MessageInterval = _consoleSettings.KeepAliveInterval,
                ShowRedialButton = _consoleSettings.ShowRedialButtonSetting,
                ShowInternalCallTransferButton = _toggleSettings.EnableInternalTransfer,
                ShowExternalCallTransferButton = _toggleSettings.EnableExternalTransfer,
                EnablePreviousPageToolbarButton = _consoleSettings.EnablePreviousPageToolbarButton,
                EnableNextPageToolbarButton = _consoleSettings.EnableNextPageToolbarButton,
                EnableAppointmentToolbarButton = _consoleSettings.EnableAppointmentToolbarButton,
                EnableRedoToolbarButton = _consoleSettings.EnableRedoToolbarButton,
                EnableFastForwardToolbarButton = _consoleSettings.EnableFastForwardToolbarButton,
                EnableCheckSpellingToolbarButton = _consoleSettings.EnableCheckSpellingToolbarButton,
                EnableRedialToolbarButton = _consoleSettings.EnableRedialToolbarButton,
                EnableHangUpToolbarButton = _consoleSettings.EnableHangUpToolbarButton,
                EnableInternalCallTransferButton = _consoleSettings.EnableInternalCallTransferToolbarButton,
                EnableExternalCallTransferButton = _consoleSettings.EnableExternalCallTransferToolbarButton,
                EnableLogoutAfterFinishToolbarButton = _consoleSettings.EnableLogoutAfterFinishToolbarButton,
                EnableTerminateToolbarButton = _consoleSettings.EnableTerminateToolbarButton,
                EnableTakeBreakToolbarButton = _consoleSettings.EnableTakeBreakToolbarButton,
                EnableChangeTaskChoiceToolbarButton = _consoleSettings.EnableChangeTaskChoiceToolbarButton,
                EnableMessageFormToolbarButton = _consoleSettings.EnableMessageFormToolbarButton,
                EnableAppointmensListToolbarButton = _consoleSettings.EnableAppointmensListToolbarButton,
                EnableRefreshToolbarButton = _consoleSettings.EnableRefreshToolbarButton,
                EnableLogoutToolbarButton = _consoleSettings.EnableLogoutToolbarButton,
                EnableRedialNewNumberRedialDialogAbility = _consoleSettings.EnableRedialNewNumberRedialDialogAbility,
                EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes = _consoleSettings.EnableAbilityToCreateAppointmensOutsideOfThePermittedShiftTimes,
                EnableAbilityToCancelDial = _consoleSettings.EnableAbilityToCancelDial,
                EnablePersistentConnectionClosing = _consoleSettings.EnablePersistentConnectionClosing,
                KeepAliveCallsToSave = _consoleSettings.KeepAliveCallsToSave,
                NormalConnectionThresholdMs = _consoleSettings.NormalConnectionThresholdMs,
                GoodConnectionThresholdMs = _consoleSettings.GoodConnectionThresholdMs,
                NoCallsTimeout = _consoleSettings.NoCallsTimeout,
                ForceUpdateToNewVersion = _consoleSettings.ForceUpdateToNewVersion,
                BreakTypes = breakTypes
            };
        }
    }
}