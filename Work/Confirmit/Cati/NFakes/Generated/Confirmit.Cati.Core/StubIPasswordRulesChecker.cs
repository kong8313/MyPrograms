using System;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation.Fakes
{
    public class StubIPasswordRulesChecker : IPasswordRulesChecker 
    {
        private IPasswordRulesChecker _inner;

        public StubIPasswordRulesChecker()
        {
            _inner = null;
        }

        public IPasswordRulesChecker Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CheckNewPasswordSatisfiesRulesStringStringIInterviewerPasswordSettingsDelegate(string oldPassword, string newPassword, IInterviewerPasswordSettings interviewerPasswordSettings);
        public CheckNewPasswordSatisfiesRulesStringStringIInterviewerPasswordSettingsDelegate CheckNewPasswordSatisfiesRulesStringStringIInterviewerPasswordSettings;

        void IPasswordRulesChecker.CheckNewPasswordSatisfiesRules(string oldPassword, string newPassword, IInterviewerPasswordSettings interviewerPasswordSettings)
        {

            if (CheckNewPasswordSatisfiesRulesStringStringIInterviewerPasswordSettings != null)
            {
                CheckNewPasswordSatisfiesRulesStringStringIInterviewerPasswordSettings(oldPassword, newPassword, interviewerPasswordSettings);
            } else if (_inner != null)
            {
                ((IPasswordRulesChecker)_inner).CheckNewPasswordSatisfiesRules(oldPassword, newPassword, interviewerPasswordSettings);
            }
        }

    }
}