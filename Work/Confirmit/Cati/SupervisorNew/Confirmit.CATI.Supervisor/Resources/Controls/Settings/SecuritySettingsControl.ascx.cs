using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.UsersApi;

namespace Confirmit.CATI.Supervisor.Resources.Controls.Settings
{
    public partial class SecuritySettingsControl : SettingsControlBase
    {
        private readonly IPersonPwdSetDateSetter _personPwdSetDateSetter;
        private readonly IPgpEncryptionService _pgpEncryptionService;
        private readonly IUsersApiService _usersApiService;

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

        public SecuritySettingsControl()
        {
            _personPwdSetDateSetter = ServiceLocator.Resolve<IPersonPwdSetDateSetter>();
            _pgpEncryptionService = ServiceLocator.Resolve<IPgpEncryptionService>();
            _usersApiService = ServiceLocator.Resolve<IUsersApiService>();
        }

        public override void FillSettings()
        {
            cbAccountLockingEnabled.Checked = SystemSettings.AccountLocking.Enabled;
            neNumberOfAttempts.ValueInt = SystemSettings.AccountLocking.MaxFailedLoginAttempts;

            FillEncryptionSetting();

            FillPasswordSettings();
        }

        private void FillEncryptionSetting()
        {
            var confirmitSettingEnabled = _pgpEncryptionService.IsConfirmitSettingEnabled();
            if (!confirmitSettingEnabled && SystemSettings.Security.AlwaysEncryptFiles)
            {
                _pgpEncryptionService.DisableCatiAlwaysEncryptFilesSetting();
            }

            cbAlwaysEncryptFiles.Enabled = confirmitSettingEnabled;
            cbAlwaysEncryptFiles.Checked = SystemSettings.Security.AlwaysEncryptFiles;

            UserForEncryptionTextBox.Text = SystemSettings.Security.UserForEncryption;
        }

        private void FillPasswordSettings()
        {
            var interviewerPasswordSettings = SystemSettings.InterviewerPassword;

            cbPasswordExpirationEnabled.Checked = interviewerPasswordSettings.IsExpirationEnabled;
            neExpireAfterNumber.ValueInt = interviewerPasswordSettings.ExpirationPeriodInDays;

            cbEnforceMinimumPasswordLengthEnabled.Checked = interviewerPasswordSettings.IsMinimumPasswordLengthEnforced;
            nePasswordLength.ValueInt = interviewerPasswordSettings.MinimumPasswordLength;

            cbEnforceComplexPasswordsEnabled.Checked = interviewerPasswordSettings.IsComplexPasswordEnforced;
            cbIsChangeAfterFirstLoginRequired.Checked = interviewerPasswordSettings.IsChangeAfterFirstLoginRequired;
        }

        public override void SaveSettings()
        {
            var interviewerPasswordSettings = SystemSettings.InterviewerPassword;
            var accountLocking = SystemSettings.AccountLocking;
            var securitySettings = SystemSettings.Security;

            using (var transactionScope = new DatabaseTransactionScope("SetPasswordExpirationSettings", DeadlockPriority.Supervisor))
            {
                if (!interviewerPasswordSettings.IsExpirationEnabled && cbPasswordExpirationEnabled.Checked)
                {
                    _personPwdSetDateSetter.SetPwdSetDateToAllPersons(DateTime.UtcNow);
                }

                accountLocking.Enabled = cbAccountLockingEnabled.Checked;
                accountLocking.MaxFailedLoginAttempts = neNumberOfAttempts.ValueInt;

                interviewerPasswordSettings.IsExpirationEnabled = cbPasswordExpirationEnabled.Checked;
                interviewerPasswordSettings.ExpirationPeriodInDays = neExpireAfterNumber.ValueInt;

                interviewerPasswordSettings.IsMinimumPasswordLengthEnforced = cbEnforceMinimumPasswordLengthEnabled.Checked;
                interviewerPasswordSettings.MinimumPasswordLength = nePasswordLength.ValueInt;

                interviewerPasswordSettings.IsComplexPasswordEnforced = cbEnforceComplexPasswordsEnabled.Checked;
                interviewerPasswordSettings.IsChangeAfterFirstLoginRequired = cbIsChangeAfterFirstLoginRequired.Checked;

                securitySettings.AlwaysEncryptFiles = cbAlwaysEncryptFiles.Checked;
                securitySettings.UserForEncryption = UserForEncryptionTextBox.Text;

                transactionScope.Commit();
            }
        }

        public override void Validate()
        {
            cvUserForEncryption.Enabled = cbAlwaysEncryptFiles.Checked;
        }

        protected void ValidateUserForEncryption(object source, ServerValidateEventArgs args)
        {
            try
            {
                if (string.IsNullOrEmpty(args.Value))
                {
                    args.IsValid = false;
                    cvUserForEncryption.Text = Strings.UserForEncryptionInvalidFormatMessage;
                    return;
                }

                var users = _usersApiService.GetUsersByName(args.Value).Where(x=> x.UserName.Equals(args.Value, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!users.Any())
                {
                    cvUserForEncryption.Text = Strings.UserForEncryptionNotValidUserMessage;
                    args.IsValid = false;
                    return;
                }

                var user = users.Single();
                if (string.IsNullOrEmpty(user.EncryptionKeyId))
                {
                    cvUserForEncryption.Text = Strings.UserForEncryptionNoPublicEncryptionKey;
                    args.IsValid = false;
                }
            }
            catch (Exception ex)
            {
                args.IsValid = false;
                Context.AddError(ex);
            }
        }

    }
}
