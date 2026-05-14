using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class IvrStaticSettings : BaseForm
    {
        private readonly IIvrSettings _ivrSettings;
        private List<Label> _errorLabels;

        public IvrStaticSettings()
        {
            _ivrSettings = ServiceLocator.Resolve<IIvrSettings>();
            _errorLabels = new List<Label>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(GetType(), "disableAutofocus", "Common._disableAutoFocus = true;", true);
            Page.Form.DefaultFocus = TextBoxTermChar.ClientID;

            if (!IsPostBack)
            {
                ddlRecordType.Items.Clear();

                ddlRecordType.Items.Add("audio/x-wav");
                ddlRecordType.Items.Add("audio/basic");
                ddlRecordType.Items.Add("audio/x-alaw-basic");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            HideAllErrorLabels();

            if (!IsPostBack)
            {
                TextBoxTermChar.Text = _ivrSettings.TermChar;
                ddlRecordType.Text = _ivrSettings.RecordType;
                CheckBoxBeep.Checked = _ivrSettings.Beep;
                TextBoxMaxTime.Text = _ivrSettings.MaxTime.ToString();
                TextBoxTransferTimeout.Text = _ivrSettings.TransferTimeout.TotalSeconds.ToString();
                TextBoxFinalSilence.Text = _ivrSettings.FinalSilence.ToString();
                CheckBoxDtmfTerm.Checked = _ivrSettings.DtmfTerm;
            }
            else
            {
                ShowErrorLabels();
            }

            stateChecker.AddSaveButton(ButtonSave);
        }

        private void HideAllErrorLabels()
        {
            LabelTermCharErrorAsterisk.Visible =
            LabelMaxTimeErrorAsterisk.Visible =
            LabelFinalSilenceErrorAsterisk.Visible =
            LabelTransferTimeoutErrorAsterisk.Visible = false;
        }

        private void ShowErrorLabels()
        {
            foreach (var errorLabel in _errorLabels)
            {
                errorLabel.Visible = true;
            }
        }

        public void SaveIvrStaticSettings(object sender, EventArgs e)
        {
            try
            {
                ValidateSettings();

                if (_errorLabels.Count > 0)
                {
                    throw new UserMessageException(Strings.IvrStaticSettingsAreWrong);
                }

                _ivrSettings.TermChar = TextBoxTermChar.Text;
                _ivrSettings.RecordType = ddlRecordType.Text;
                _ivrSettings.Beep = CheckBoxBeep.Checked;
                _ivrSettings.MaxTime = Convert.ToInt32(TextBoxMaxTime.Text);
                _ivrSettings.FinalSilence = Convert.ToInt32(TextBoxFinalSilence.Text);
                _ivrSettings.TransferTimeout = TimeSpan.FromSeconds(Convert.ToInt32(TextBoxTransferTimeout.Text));
                _ivrSettings.DtmfTerm = CheckBoxDtmfTerm.Checked;

                stateChecker.MarkAsUnchanged();
            }
            catch (UserMessageException ex)
            {
                AddUserMessage(ex);
            }
        }

        private void ValidateSettings()
        {
            _errorLabels = new List<Label>();

            if (TextBoxTermChar.Text.Length != 1)
            {
                _errorLabels.Add(LabelTermCharErrorAsterisk);
            }

            if(!int.TryParse(TextBoxMaxTime.Text, out var temp) || temp < 0)
            {
                _errorLabels.Add(LabelMaxTimeErrorAsterisk);
            }

            if (!int.TryParse(TextBoxFinalSilence.Text, out temp) || temp < 0)
            {
                _errorLabels.Add(LabelFinalSilenceErrorAsterisk);
            }

            if (!int.TryParse(TextBoxTransferTimeout.Text, out temp) || temp < 0)
            {
                _errorLabels.Add(LabelTransferTimeoutErrorAsterisk);
            }
        }
    }
}