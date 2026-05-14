using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation.Fakes
{
    public class StubIPersonAuthorizer : IPersonAuthorizer 
    {
        private IPersonAuthorizer _inner;

        public StubIPersonAuthorizer()
        {
            _inner = null;
        }

        public IPersonAuthorizer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool AuthorizeBvPersonEntityStringDelegate(BvPersonEntity person, string password);
        public AuthorizeBvPersonEntityStringDelegate AuthorizeBvPersonEntityString;

        bool IPersonAuthorizer.Authorize(BvPersonEntity person, string password)
        {


            if (AuthorizeBvPersonEntityString != null)
            {
                return AuthorizeBvPersonEntityString(person, password);
            } else if (_inner != null)
            {
                return ((IPersonAuthorizer)_inner).Authorize(person, password);
            }

            return default(bool);
        }

        public delegate bool IsPasswordExpiredBvPersonEntityIInterviewerPasswordSettingsDelegate(BvPersonEntity person, IInterviewerPasswordSettings interviewerPasswordSettings);
        public IsPasswordExpiredBvPersonEntityIInterviewerPasswordSettingsDelegate IsPasswordExpiredBvPersonEntityIInterviewerPasswordSettings;

        bool IPersonAuthorizer.IsPasswordExpired(BvPersonEntity person, IInterviewerPasswordSettings interviewerPasswordSettings)
        {


            if (IsPasswordExpiredBvPersonEntityIInterviewerPasswordSettings != null)
            {
                return IsPasswordExpiredBvPersonEntityIInterviewerPasswordSettings(person, interviewerPasswordSettings);
            } else if (_inner != null)
            {
                return ((IPersonAuthorizer)_inner).IsPasswordExpired(person, interviewerPasswordSettings);
            }

            return default(bool);
        }

    }
}