using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation
{
    public interface IPasswordRulesChecker
    {
        void CheckNewPasswordSatisfiesRules(string oldPassword, string newPassword, IInterviewerPasswordSettings interviewerPasswordSettings);
    }
}
