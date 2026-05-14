using System;
using System.Windows.Forms;
using BootstrapperLibrary.Interfaces;
using BootstrapperLibrary.Properties;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary
{
    public partial class SelectActionForm : Form, ISelectActionForm
    {
        private CommandLineParseResult _commandLineParseResult;
        private IInstalledProductSearcher _installedProductSearcher;
        private IMsiParametersStringCreator _msiParametersStringCreator;
        private ILogger _logger;
        private IDialogService _dialogService;

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public SelectActionForm()
        {
            InitializeComponent();
        }

        public CommandLineParseResult ShowForm(
            ILogger logger,
            Version currentVersion,
            IInstalledProductSearcher installedProductSearcher,
            IObjectFactory objectFactory,
            IMsiParametersStringCreator msiParametersStringCreator)
        {
            _installedProductSearcher = installedProductSearcher;
            _msiParametersStringCreator = msiParametersStringCreator;
            _logger = logger;
            _dialogService = objectFactory.CreateDialogservice();

            if (!_installedProductSearcher.IsProductAlreadyInstalled)
            {
                return new CommandLineParseResult { Action = InstallationAction.Install };
            }

            _commandLineParseResult = null;

            labelTitle.Text = Resources.SelectActionFormTitle;
            labelProductName.Text = Text = string.Format("{0} setup", installedProductSearcher.CurrentProductName);
            radioButtonRemove.Text = string.Format(Resources.RemoveProduct, installedProductSearcher.ProductName);
            radioButtonUpdate.Text = string.Format(Resources.UpdateProduct, installedProductSearcher.ProductName);
            buttonNext.Text = Resources.Next;
            buttonCancel.Text = Resources.Cancel;
            SetCheckBoxWarning(currentVersion);

            ShowDialog();

            return _commandLineParseResult;
        }

        private void SetCheckBoxWarning(Version currentVersion)
        {
            if (currentVersion == _installedProductSearcher.InstalledVersion)
            {
                checkBoxWarning.Text = string.Format(Resources.WarningVersionsAreTheSame);
            }
            else if (currentVersion < _installedProductSearcher.InstalledVersion)
            {
                checkBoxWarning.Text = string.Format(Resources.WarningPreviousVersionIsBiggerThanTheCurentOne);
            }
            else
            {
                checkBoxWarning.Visible = false;
            }
        }

        private void ButtonNextClick(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonUpdate.Checked)
                {
                    _commandLineParseResult = new CommandLineParseResult
                    {
                        Action = InstallationAction.Update,
                        MsiPropertiesForUnattendedInstallation = _msiParametersStringCreator.CreateInstallationParametersString(
                            new ReadingInstallationParameters(_installedProductSearcher.InstallLocation))
                    };
                }
                else
                {
                    _commandLineParseResult = new CommandLineParseResult { Action = InstallationAction.Uninstall };
                }

                Close();
            }
            catch (Exception ex)
            {
                _commandLineParseResult = null;
                _logger.WriteLog(ex.ToString());
                _dialogService.Show(ex.Message, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RadioButtonRemoveCheckedChanged(object sender, EventArgs e)
        {
            checkBoxWarning.Enabled = radioButtonUpdate.Checked;

            buttonNext.Enabled = true;

            if (checkBoxWarning.Enabled && checkBoxWarning.Visible)
            {
                buttonNext.Enabled = checkBoxWarning.Checked;
            }
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void CheckBoxWarningCheckedChanged(object sender, EventArgs e)
        {
            buttonNext.Enabled = checkBoxWarning.Checked;
        }

        /// <summary>
        /// Press Next buttong by Enter and Cancel by Esc
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter && buttonNext.Enabled)
            {
                ButtonNextClick(null, null);
                return true;
            }

            if (keyData == Keys.Escape)
            {
                ButtonCancelClick(null, null);
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        private void SelectActionFormShown(object sender, EventArgs e)
        {
            if (checkBoxWarning.Visible && checkBoxWarning.Enabled)
            {
                buttonNext.Enabled = false;
            }
        }
    }
}
