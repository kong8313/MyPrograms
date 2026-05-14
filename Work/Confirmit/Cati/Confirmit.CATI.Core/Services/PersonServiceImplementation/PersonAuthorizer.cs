using System;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation
{
    public class PersonAuthorizer : IPersonAuthorizer
    {
        private readonly IPasswordHash _passwordHash;
        private readonly IPasswordSaver _passwordSaver;
        private readonly IEmailSettings _emailSettings;
        private readonly IAccountLockingSettings _accountLockingSettings;
        private readonly ISupervisorNotificationService _supervisorNotificationService;

        public PersonAuthorizer(
            IPasswordHash passwordHash,
            IPasswordSaver passwordSaver,
            IEmailSettings emailSettings,
            IAccountLockingSettings accountLockingSettings,
            ISupervisorNotificationService supervisorNotificationService)
        {
            _passwordHash = passwordHash;
            _passwordSaver = passwordSaver;
            _emailSettings = emailSettings;
            _accountLockingSettings = accountLockingSettings;
            _supervisorNotificationService = supervisorNotificationService;
        }

        /// <summary>
        /// Checks person password and increases FailedLoginAttempts count if password is incorrect.(if locking functionality is enabled)
        /// </summary>
        /// <param name="person">person entity</param>
        /// <param name="password">Password to check</param>
        /// <returns>True if password is correct; false if there is no such person or password is incorrect.</returns>
        public bool Authorize(BvPersonEntity person, string password)
        {
            if (person == null || person.IsLocked)
            {
                return false;
            }

            return _accountLockingSettings.Enabled
                ? CheckPasswordWithUpdateLoginAttemptStatus(person, password)
                : CheckPassword(person, password);
        }

        /// <summary>
        /// Checks if person password is expired
        /// </summary>
        /// <param name="person"></param>
        /// <param name="interviewerPasswordSettings"></param>
        /// <returns>True, if password is expired, false otherwise</returns>
        public bool IsPasswordExpired(BvPersonEntity person, IInterviewerPasswordSettings interviewerPasswordSettings)
        {
            if (!interviewerPasswordSettings.IsExpirationEnabled)
            {
                return false;
            }

            return (person.PwdSetDate.AddDays(interviewerPasswordSettings.ExpirationPeriodInDays).CompareTo(DateTime.UtcNow) < 0);
        }

        private bool CheckPassword(BvPersonEntity person, string password)
        {
            return CheckPasswordAndRehashOldOne(person, password);
        }

        private bool CheckPasswordWithUpdateLoginAttemptStatus(BvPersonEntity person, string password)
        {
            bool isPersonAuthorized = false;
            int failedLoginAttempts = PersonService.GetFailedLoginAttempts(person.SID);

            if (failedLoginAttempts >= _accountLockingSettings.MaxFailedLoginAttempts) // need to check here because MaxFailedLoginAttempts could be decreased
            {
                LockPerson(person);
            }
            else if (!person.IsLocked)
            {
                isPersonAuthorized = CheckPasswordAndRehashOldOne(person, password);

                if (isPersonAuthorized)
                {
                    ResetLoginAttemptStatus(person, failedLoginAttempts);
                }
                else
                {
                    UpdateLoginAttemptStatus(person, failedLoginAttempts + 1);
                }
            }

            return isPersonAuthorized;
        }

        private bool CheckPasswordAndRehashOldOne(BvPersonEntity person, string password)
        {
            if (_passwordHash.IsLegacyHash(person.PwdHashTxt) == false)
            {
                return _passwordHash.ValidateHash(password, person.PwdSaltTxt, person.PwdHashTxt);
            }

            if (_passwordHash.ValidateLegacyHash(person.SID, password, person.PwdSaltTxt, person.PwdHashTxt) == false)
            {
                return false;
            }

            _passwordSaver.Save(person.SID, password);
            return true;
        }

        private void ResetLoginAttemptStatus(BvPersonEntity person, int failedLoginAttempts)
        {

            if (failedLoginAttempts > 0)
            {
                var evt = new ResetFailedLoginAttemptsEvent();

                PersonService.SetFailedLoginAttempts(person.SID, 0);

                evt.Save(person.SID);
            }
        }

        private void UpdateLoginAttemptStatus(BvPersonEntity person, int failedLoginAttempts)
        {
            var evt = new IncrementFailedLoginAttemptsEvent();
            PersonService.SetFailedLoginAttempts(person.SID, failedLoginAttempts);

            evt.Save(person.SID, failedLoginAttempts);

            if (failedLoginAttempts >= _accountLockingSettings.MaxFailedLoginAttempts)
            {
                LockPerson(person);
            }
        }

        private void LockPerson(BvPersonEntity person)
        {
            var lockEvent = new InterviewerLockedEvent();
            PersonService.LockPerson(person.SID, true);
            lockEvent.Save(person.SID);

            _supervisorNotificationService.SendAccountLockedEmailNotification(
                _emailSettings.AdministratorEmailAddress,
                person.Name);
        }
    }
}
