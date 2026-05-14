using System;
using Confirmit.CATI.Common.Contracts.ConsoleAuthorizationService;

namespace Confirmit.CATI.Common.Contracts.ConsoleAuthorizationService.Fakes
{
    public class StubIConsoleAuthorizationService : IConsoleAuthorizationService 
    {
        private IConsoleAuthorizationService _inner;

        public StubIConsoleAuthorizationService()
        {
            _inner = null;
        }

        public IConsoleAuthorizationService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int AuthorizeAndReturnCompanyIdStringStringStringStringDelegate(string interviewerName, string interviewerPassword, string catiCompanyAlias, string stationId);
        public AuthorizeAndReturnCompanyIdStringStringStringStringDelegate AuthorizeAndReturnCompanyIdStringStringStringString;

        int IConsoleAuthorizationService.AuthorizeAndReturnCompanyId(string interviewerName, string interviewerPassword, string catiCompanyAlias, string stationId)
        {


            if (AuthorizeAndReturnCompanyIdStringStringStringString != null)
            {
                return AuthorizeAndReturnCompanyIdStringStringStringString(interviewerName, interviewerPassword, catiCompanyAlias, stationId);
            } else if (_inner != null)
            {
                return ((IConsoleAuthorizationService)_inner).AuthorizeAndReturnCompanyId(interviewerName, interviewerPassword, catiCompanyAlias, stationId);
            }

            return default(int);
        }

        public delegate void ChangePersonPasswordStringStringStringStringStringDelegate(string interviewerName, string oldPassword, string newPassword, string catiCompanyAlias, string stationId);
        public ChangePersonPasswordStringStringStringStringStringDelegate ChangePersonPasswordStringStringStringStringString;

        void IConsoleAuthorizationService.ChangePersonPassword(string interviewerName, string oldPassword, string newPassword, string catiCompanyAlias, string stationId)
        {

            if (ChangePersonPasswordStringStringStringStringString != null)
            {
                ChangePersonPasswordStringStringStringStringString(interviewerName, oldPassword, newPassword, catiCompanyAlias, stationId);
            } else if (_inner != null)
            {
                ((IConsoleAuthorizationService)_inner).ChangePersonPassword(interviewerName, oldPassword, newPassword, catiCompanyAlias, stationId);
            }
        }

        public delegate bool IsLatestVersionStringDelegate(string version);
        public IsLatestVersionStringDelegate IsLatestVersionString;

        bool IConsoleAuthorizationService.IsLatestVersion(string version)
        {


            if (IsLatestVersionString != null)
            {
                return IsLatestVersionString(version);
            } else if (_inner != null)
            {
                return ((IConsoleAuthorizationService)_inner).IsLatestVersion(version);
            }

            return default(bool);
        }

    }
}