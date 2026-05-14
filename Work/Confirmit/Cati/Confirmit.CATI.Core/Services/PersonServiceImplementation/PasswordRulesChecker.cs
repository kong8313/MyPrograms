using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation
{
    public class PasswordRulesChecker : IPasswordRulesChecker
    {
        public void CheckNewPasswordSatisfiesRules(string oldPassword, string newPassword, IInterviewerPasswordSettings interviewerPasswordSettings)
        {
            if (!interviewerPasswordSettings.IsResetToSamePasswordEnabled)
            {
                CheckOldAndNewPasswordsDiffer(oldPassword, newPassword);
            }

            if (interviewerPasswordSettings.IsMinimumPasswordLengthEnforced)
            {
                CheckPasswordLengthIsOk(newPassword, interviewerPasswordSettings.MinimumPasswordLength);
            }

            if (interviewerPasswordSettings.IsComplexPasswordEnforced)
            {
                CheckPasswordCompexity(newPassword);
            }
        }

        private void CheckOldAndNewPasswordsDiffer(string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) && string.IsNullOrEmpty(newPassword))
            {
                throw new TheSamePasswordException(Strings.ChangeToSamePasswordIsForbidden);
            }

            if (!string.IsNullOrEmpty(oldPassword) && oldPassword.Equals(newPassword))
            {
                throw new TheSamePasswordException(Strings.ChangeToSamePasswordIsForbidden);
            }
        }

        private void CheckPasswordLengthIsOk(string password, int minimumPasswordLength)
        {
            if (string.IsNullOrEmpty(password) ||
                password.Length < minimumPasswordLength)
            {
                throw new TooShortPasswordException(Strings.PasswordTooShort, minimumPasswordLength);
            }
        }

        private void CheckPasswordCompexity(string password)
        {
            //Note: if we are about to have some changable complexity rules 
            //then we maybe will create some kind of CompexityCheckerProvider
            
            if (string.IsNullOrEmpty(password))
            {
                throw new PasswordDoesNotSatisfyRulesException(Strings.PasswordDoesNotSatisfyRules);
            }

            var capitalsCount = 0;
            var nonAlphanumericCount = 0;

            foreach (var ch in password)
            {
                if (char.IsUpper(ch))
                {
                    capitalsCount++;
                }

                if (!(char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch)))
                {
                    nonAlphanumericCount++;
                }
            }

            if ((capitalsCount < 1) || (nonAlphanumericCount < 1))
            {
                throw new PasswordDoesNotSatisfyRulesException(Strings.PasswordDoesNotSatisfyRules);
            }
        }
    }
}
