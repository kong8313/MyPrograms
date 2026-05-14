using Confirmit.CATI.Core.SystemSettings;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.ActivityLogging.SiteSettings
{
    [ManagementEventAttribute(ManagementEvent.UpdateSecuritySiteSettings)]
    public class UpdateSecuritySiteSettingsEvent : UpdateSiteSettingsEventBase
    {
        public UpdateSecuritySiteSettingsEvent()
            : base(ManagementEvent.UpdateSecuritySiteSettings)
        {
        }

        protected override Dictionary<string, object> GetSiteSettingsAsDictionary(ISystemSettings systemSettings)
        {
            return new Dictionary<string, object>
            {
                { "AccountLocking.Enabled", systemSettings.AccountLocking.Enabled },
                { "AccountLocking.MaxFailedLoginAttempts", systemSettings.AccountLocking.MaxFailedLoginAttempts },
                { "InterviewerPassword.IsExpirationEnabled", systemSettings.InterviewerPassword.IsExpirationEnabled },
                { "InterviewerPassword.ExpirationPeriodInDays", systemSettings.InterviewerPassword.ExpirationPeriodInDays },
                { "InterviewerPassword.IsResetToSamePasswordEnabled", systemSettings.InterviewerPassword.IsResetToSamePasswordEnabled },
                { "InterviewerPassword.IsMinimumPasswordLengthEnforced", systemSettings.InterviewerPassword.IsMinimumPasswordLengthEnforced },
                { "InterviewerPassword.MinimumPasswordLength", systemSettings.InterviewerPassword.MinimumPasswordLength },
                { "InterviewerPassword.IsComplexPasswordEnforced", systemSettings.InterviewerPassword.IsComplexPasswordEnforced },
                { "Security.AlwaysEncryptFiles", systemSettings.Security.AlwaysEncryptFiles },
                { "Security.UserForEncryption", systemSettings.Security.UserForEncryption },
            };
        }

    }
}
