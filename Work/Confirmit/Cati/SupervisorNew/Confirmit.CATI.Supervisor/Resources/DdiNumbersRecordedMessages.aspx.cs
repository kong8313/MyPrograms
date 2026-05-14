using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources.Classes;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DdiNumbersRecordedMessages : BaseForm
    {
        private readonly IDialerSettings _dialerSettings;
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IDialersRepository _dialersRepository;
        private List<Label> _errorLabels;
        private ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor> _inboundAudioMessagesDictionary;

        [StoreInViewState]
        protected string TelephoneNumber;

        protected Dictionary<AudioMessageType, DdiNumbersControlsContainer> DdiNumbersControls;

        private List<string> _wrongConfiguredDialersList;
        private readonly List<KeyValuePair<string, int?>> _playBehaviorChoices;

        public DdiNumbersRecordedMessages()
        {
            _dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _errorLabels = new List<Label>();
            _inboundTelephoneNumberRepository = ServiceLocator.Resolve<IInboundTelephoneNumberRepository>();

            _playBehaviorChoices = new List<KeyValuePair<string, int?>>
            {
                new KeyValuePair<string, int?>(Strings.Default, null),
                new KeyValuePair<string, int?>(Strings.Off, -1),
                new KeyValuePair<string, int?>(Strings.PlayOnce, 0),
                new KeyValuePair<string, int?>(Strings.Looping, int.MaxValue)
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TelephoneNumber = Request["TelephoneNumber"];
                FillDropDownListsForRepeatCount();
            }

            ClientScript.RegisterStartupScript(GetType(), "disableAutofocus", "Common._disableAutoFocus = true;", true);
            Page.Form.DefaultFocus = TextBoxIncomingCallCompulsoryMessageUrl.ClientID;
        }

        private bool IsDefaultEditing()
        {
            return string.IsNullOrEmpty(TelephoneNumber);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HideAllErrorLabels();

            if (!IsPostBack)
            {
                if (IsDefaultEditing())
                {
                    Toolbar.LeftLabel = Strings.DdiNumbersDefaultRecordedMessagesTitle;
                    ParametersHint.Text = Strings.DdiNumbersDefaultRecordedMessagesHint;

                    HideDefaultColumns();
                }
                else
                {
                    Toolbar.LeftLabel = Strings.DdiNumbersTelephoneSpecificRecordedMessagesTitle + TelephoneNumber;
                    ParametersHint.Text = Strings.DdiNumbersTelephoneSpecificRecordedMessagesHint;
                }

                FillDdiNumbersControls();

                PutInboundAudioMessagesToControls(AudioMessageType.IncomingCallMandatory);
                PutInboundAudioMessagesToControls(AudioMessageType.IncomingCall);
                PutInboundAudioMessagesToControls(AudioMessageType.DropCallSystemFault);
                PutInboundAudioMessagesToControls(AudioMessageType.DropCallCampaignNotAvailable);
                PutInboundAudioMessagesToControls(AudioMessageType.DropCallInterviewNotFound);
                PutInboundAudioMessagesToControls(AudioMessageType.DropCallOutOfShift);
            }
            else
            {
                ShowErrorLabels();
            }

            stateChecker.AddSaveButton(ButtonSave);
        }

        private void PutInboundAudioMessagesToControls(AudioMessageType audioMessageType)
        {
            var urlTextBox = DdiNumbersControls[audioMessageType].UrlTextBox;
            var defaultUrlTextBox = DdiNumbersControls[audioMessageType].DefaultUrlTextBox;
            var repeatCountDropDownList = DdiNumbersControls[audioMessageType].PlayBehaviorDropDownList;
            var defaultRepeatCountTextBox = DdiNumbersControls[audioMessageType].DefaultRepeatCountTextBox;

            int? behaviorValue;
            if (IsDefaultEditing())
            {
                urlTextBox.Text = _dialerSettings.GetInboundAudioMessageSource(audioMessageType);
                behaviorValue = GetPlayBehaviorByRepeatCount(_dialerSettings.GetInboundAudioMessageRepeatCount(audioMessageType));
            }
            else
            {
                var telephoneNumberEntity = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(TelephoneNumber);

                urlTextBox.Text = telephoneNumberEntity.GetInboundAudioMessageSource(audioMessageType);
                behaviorValue = GetPlayBehaviorByRepeatCount(telephoneNumberEntity.GetInboundAudioMessageRepeatCount(audioMessageType));

                defaultUrlTextBox.Text = _dialerSettings.GetInboundAudioMessageSource(audioMessageType);
                defaultRepeatCountTextBox.Text =
                    _playBehaviorChoices.FirstKey(
                        GetPlayBehaviorByRepeatCount(_dialerSettings.GetInboundAudioMessageRepeatCount(audioMessageType)));
            }

            repeatCountDropDownList.Text = behaviorValue.ToNullableString();
        }

        //New supported repeat counts is null, 0 and int.MaxValue.
        //If old stored value between 1 and int.MaxValue it will be replaced by value 0 (PlayOnce).
        private int? GetPlayBehaviorByRepeatCount(int? repeatCount) =>
            repeatCount != null && repeatCount > 0 && repeatCount < int.MaxValue ? 0 : repeatCount;

        public void SaveDdiNumberSettings(object sender, EventArgs e)
        {
            try
            {
                FillDdiNumbersControls();

                _errorLabels = new List<Label>();
                _inboundAudioMessagesDictionary = new ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>();

                AddInboundAudioMessage(AudioMessageType.DropCallOutOfShift);
                AddInboundAudioMessage(AudioMessageType.DropCallInterviewNotFound);
                AddInboundAudioMessage(AudioMessageType.DropCallCampaignNotAvailable);
                AddInboundAudioMessage(AudioMessageType.DropCallSystemFault);
                AddInboundAudioMessage(AudioMessageType.IncomingCallMandatory);
                AddInboundAudioMessage(AudioMessageType.IncomingCall);

                if (_errorLabels.Count > 0)
                {
                    throw new UserMessageException(Strings.InvalidUrl);
                }

                _wrongConfiguredDialersList = new List<string>();

                if (IsDefaultEditing())
                {
                    _dialerSettings.InboundAudioMessagesDictionary = _inboundAudioMessagesDictionary;

                    var dialers = _dialersRepository.GetAll().Where(x => _supervisorServiceClient.IsDialerOperational(x.Id));

                    foreach (var dialer in dialers)
                    {
                        ConfigureDialersWithDdiNumbers(dialer.Id);
                    }
                }
                else
                {
                    var telephoneNumberEntity = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(TelephoneNumber);

                    //if there no audio messages then let AudioMessagesJson property to null
                    telephoneNumberEntity.InboundAudioMessagesDictionary = _inboundAudioMessagesDictionary.Count > 0
                        ? _inboundAudioMessagesDictionary
                        : null;

                    _inboundTelephoneNumberRepository.Update(telephoneNumberEntity);

                    var dialer = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(TelephoneNumber);
                    if (_supervisorServiceClient.IsDialerOperational(dialer.DialerId))
                    {
                        ConfigureDialersWithDdiNumbers(dialer.DialerId);
                    }
                }

                stateChecker.MarkAsUnchanged();

                if (_wrongConfiguredDialersList.Count > 0)
                {
                    Context.AddError(new UserMessageException(string.Format(
                        Strings.WarningDuringDdiNumberRecordedMessagesConfiguration,
                        string.Join(", ", _wrongConfiguredDialersList))));
                }
            }
            catch (UserMessageException ex)
            {
                AddUserMessage(ex);
            }
        }

        private void ConfigureDialersWithDdiNumbers(int dialerId)
        {
            try
            {
                _supervisorServiceClient.ConfigureInboundDdiNumbers(dialerId);
            }
            catch (Exception ex)
            {
                _wrongConfiguredDialersList.Add(_dialersRepository.GetById(dialerId).Name);
                System.Diagnostics.Trace.TraceWarning(ex.ToString());
            }
        }

        private void AddInboundAudioMessage(AudioMessageType audioMessageType)
        {
            var urlTextBox = DdiNumbersControls[audioMessageType].UrlTextBox;
            var playBehaviorDropDownList = DdiNumbersControls[audioMessageType].PlayBehaviorDropDownList;
            var errorLabel = DdiNumbersControls[audioMessageType].LabelAsterisk;

            //if urlTextBox is blank and playBehavior is default not adding audio message
            if (string.IsNullOrWhiteSpace(urlTextBox.Text) && playBehaviorDropDownList.SelectedItem.Text == Strings.Default)
                return;
            
            //if urlTextBox is blank then drop play behavior to Off
            if (string.IsNullOrWhiteSpace(urlTextBox.Text) && playBehaviorDropDownList.SelectedItem.Text != Strings.Default)
            {
                urlTextBox.Text = string.Empty;
                playBehaviorDropDownList.Text = _playBehaviorChoices.First(x => x.Key == Strings.Off).Value.ToString(); //set Off value
                return;
            }

            if (playBehaviorDropDownList.SelectedItem.Text != Strings.Default)
                VerifyUrl(urlTextBox, errorLabel);

            _inboundAudioMessagesDictionary[audioMessageType] = new AudioMessageDescriptor
            {
                Type = AudioSourceType.AudioUrl,
                Source = urlTextBox.Text,
                RepeatCount = playBehaviorDropDownList.SelectedValue.ToNullableInt()
            };
        }

        private void FillDropDownListsForRepeatCount()
        {
            foreach (var item in _playBehaviorChoices)
            {
                //default
                if (IsDefaultEditing() && item.Value == null) continue;

                var ddlVal = item.Value.ToNullableString();

                //Compulsory - default, off, 1
                if (item.Value == null || item.Value < 2)
                    ddlIncomingCallCompulsoryMessageUrlRepeatCount.Items.Add(new ListItem(item.Key, ddlVal));

                //Waiting and Other - default, off, 1, looping
                if (item.Value == null || item.Value < 2 || item.Value == int.MaxValue)
                {
                    ddlIncomingCallUrlRepeatCount.Items.Add(new ListItem(item.Key, ddlVal));
                    ddlDropCallInterviewNotFoundRepeatCount.Items.Add(new ListItem(item.Key, ddlVal));
                    ddlDropCallOutsideOfOperationHoursRepeatCount.Items.Add(new ListItem(item.Key, ddlVal));
                    ddlDropCampaignIsNotAvailableRepeatCount.Items.Add(new ListItem(item.Key, ddlVal));
                    ddlSystemFaultUrlRepeatCount.Items.Add(new ListItem(item.Key, ddlVal));
                }
            }
        }

        private void FillDdiNumbersControls()
        {
            DdiNumbersControls = new Dictionary<AudioMessageType, DdiNumbersControlsContainer>
            {
                { AudioMessageType.IncomingCallMandatory, new DdiNumbersControlsContainer(TextBoxIncomingCallCompulsoryMessageUrl, TextBoxDefaultIncomingCallCompulsoryMessageUrl, ddlIncomingCallCompulsoryMessageUrlRepeatCount, TextBoxDefaultRepeatIncomingCallCompulsoryMessageUrl, LabelIncomingCallCompulsoryMessageErrorAsterisk) },
                { AudioMessageType.IncomingCall, new DdiNumbersControlsContainer(TextBoxIncomingCallUrl, TextBoxDefaultIncomingCallUrl, ddlIncomingCallUrlRepeatCount, TextBoxDefaultRepeatIncomingCallUrl, LabelIncomingCallErrorAsterisk) },
                { AudioMessageType.DropCallSystemFault, new DdiNumbersControlsContainer(TextBoxSystemFaultUrl, TextBoxDefaultSystemFaultUrl, ddlSystemFaultUrlRepeatCount, TextBoxDefaultRepeatSystemFaultUrl, LabelSystemFaultAsterisk) },
                { AudioMessageType.DropCallCampaignNotAvailable, new DdiNumbersControlsContainer(TextBoxDropCampaignIsNotAvailableUrl, TextBoxDefaultDropCampaignIsNotAvailableUrl, ddlDropCampaignIsNotAvailableRepeatCount, TextBoxDefaultRepeatDropCampaignIsNotAvailableUrl, LabelDropCampaignIsNotAvailableAsterisk) },
                { AudioMessageType.DropCallInterviewNotFound, new DdiNumbersControlsContainer(TextBoxDropCallInterviewNotFoundUrl, TextBoxDefaultDropCallInterviewNotFoundUrl, ddlDropCallInterviewNotFoundRepeatCount, TextBoxDefaultRepeatDropCallInterviewNotFoundUrl, LabelDropCallInterviewNotFoundAsterisk) },
                { AudioMessageType.DropCallOutOfShift, new DdiNumbersControlsContainer(TextBoxDropCallOutsideOfOperationHoursUrl, TextBoxDefaultDropCallOutsideOfOperationHoursUrl, ddlDropCallOutsideOfOperationHoursRepeatCount, TextBoxDefaultRepeatDropCallOutsideOfOperationHoursUrl, LabelDropCallOutsideOfOperationHoursAsterisk) }
            };
        }

        private void HideDefaultColumns()
        {
            DefaultDropCallInterviewNotFoundUrl.Visible = false;
            DefaultDropCallOutsideOfOperationHoursUrl.Visible = false;
            DefaultDropCampaignIsNotAvailableUrl.Visible = false;
            DefaultIncomingCallCompulsoryMessageUrl.Visible = false;
            DefaultIncomingCallUrl.Visible = false;
            DefaultSystemFaultUrl.Visible = false;

            DefaultRepeatColumnTitle.Visible = false;
            DefaultRepeatDropCallInterviewNotFoundUrl.Visible = false;
            DefaultRepeatDropCallOutsideOfOperationHoursUrl.Visible = false;
            DefaultRepeatDropCampaignIsNotAvailableUrl.Visible = false;
            DefaultRepeatIncomingCallCompulsoryMessageUrl.Visible = false;
            DefaultRepeatIncomingCallUrl.Visible = false;
            DefaultRepeatSystemFaultUrl.Visible = false;
        }

        private void HideAllErrorLabels()
        {
            LabelIncomingCallCompulsoryMessageErrorAsterisk.Visible = false;
            LabelIncomingCallErrorAsterisk.Visible = false;
            LabelSystemFaultAsterisk.Visible = false;
            LabelDropCampaignIsNotAvailableAsterisk.Visible = false;
            LabelDropCallInterviewNotFoundAsterisk.Visible = false;
            LabelDropCallOutsideOfOperationHoursAsterisk.Visible = false;
        }

        private void ShowErrorLabels()
        {
            foreach (var errorLabel in _errorLabels)
            {
                errorLabel.Visible = true;
            }
        }

        private void VerifyUrl(TextBox textBox, Label errorLabel)
        {
            if (!Uri.TryCreate(textBox.Text, UriKind.Absolute, out _))
            {
                Page.Form.DefaultFocus = textBox.ClientID;
                _errorLabels.Add(errorLabel);
            }
        }
    }

    internal static class PlayBehaviorExtensions
    {

        public static string ToNullableString(this int? val)
        {
            return val == null ? string.Empty : val.ToString();
        }

        public static int? ToNullableInt(this string val)
        {
            return val == string.Empty ?
                (int?)null :
                Convert.ToInt32(val);
        }

        public static TKey FirstKey<TKey, TVal>(this List<KeyValuePair<TKey, TVal>> list, TVal value)
        {
            return list.First(x => x.Value.Equals(value)).Key;
        }

    }
}