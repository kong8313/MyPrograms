namespace Confirmit.CATI.Common.WcfTools.ErrorServiceMessageHeader
{
    public class LoginPasswordAuthenticationDataProvider : ILoginPasswordAuthenticationDataProvider
    {
        public LoginPasswordAuthenticationDataProvider(string login, string password, int companyId)
        {
            Login = login;
            Password = password;
            CompanyId = companyId;
        }

        public string Login 
        { 
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }

        public int CompanyId
        {
            get; 
            private set; 
        }
    }
}
