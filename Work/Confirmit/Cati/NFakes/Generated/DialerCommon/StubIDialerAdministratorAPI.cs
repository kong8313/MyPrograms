using System;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Telephony.Fakes
{
    public class StubIDialerAdministratorAPI
    {
        private IDialerAdministratorAPI _inner;

        public StubIDialerAdministratorAPI()
        {
            _inner = null;
        }

        public IDialerAdministratorAPI Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeStringStringDelegate(string connectionParametersXml, string configurationParametersXml);
        public InitializeStringStringDelegate InitializeStringString;

        public void Initialize(string connectionParametersXml, string configurationParametersXml)
        {

            if (InitializeStringString != null)
            {
                InitializeStringString(connectionParametersXml, configurationParametersXml);
            } else if (_inner != null)
            {
            }
        }

        public delegate void LoginDelegate();
        public LoginDelegate Login;

        public void Login()
        {

            if (Login != null)
            {
                Login();
            } else if (_inner != null)
            {
            }
        }

        public delegate int CreateCompanyInt32StringStringInt32ArrayOfStringStringStringStringRefDelegate(int catiInstanceId, string companyName, string licenseType, int licenseCount, string[] campaignCLIs, string nailedUpCLI, string nailedUpPrefix, ref string connectionParametersXml);
        public CreateCompanyInt32StringStringInt32ArrayOfStringStringStringStringRefDelegate CreateCompanyInt32StringStringInt32ArrayOfStringStringStringStringRef;

        public int CreateCompany(int catiInstanceId, string companyName, string licenseType, int licenseCount, string[] campaignCLIs, string nailedUpCLI, string nailedUpPrefix, ref string connectionParametersXml)
        {


            if (CreateCompanyInt32StringStringInt32ArrayOfStringStringStringStringRef != null)
            {
                return CreateCompanyInt32StringStringInt32ArrayOfStringStringStringStringRef(catiInstanceId, companyName, licenseType, licenseCount, campaignCLIs, nailedUpCLI, nailedUpPrefix, ref connectionParametersXml);
            } else if (_inner != null)
            {
                return ((IDialerAdministratorAPI)_inner).CreateCompany(catiInstanceId, companyName, licenseType, licenseCount, campaignCLIs, nailedUpCLI, nailedUpPrefix, ref connectionParametersXml);
            }

            return default(int);
        }

        public delegate void DeleteCompanyInt32Delegate(int companyId);
        public DeleteCompanyInt32Delegate DeleteCompanyInt32;

        public void DeleteCompany(int companyId)
        {

            if (DeleteCompanyInt32 != null)
            {
                DeleteCompanyInt32(companyId);
            } else if (_inner != null)
            {
            }
        }

        public delegate int CreateUserStringStringStringStringStringDelegate(string companyId, string userEmail, string userFullName, string userPassword, string userType);
        public CreateUserStringStringStringStringStringDelegate CreateUserStringStringStringStringString;

        public int CreateUser(string companyId, string userEmail, string userFullName, string userPassword, string userType)
        {


            if (CreateUserStringStringStringStringString != null)
            {
                return CreateUserStringStringStringStringString(companyId, userEmail, userFullName, userPassword, userType);
            } else if (_inner != null)
            {
                return ((IDialerAdministratorAPI)_inner).CreateUser(companyId, userEmail, userFullName, userPassword, userType);
            }

            return default(int);
        }

    }
}