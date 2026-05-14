using System;
using Confirmit.CATI.Common.WcfTools.ErrorServiceMessageHeader;

namespace Confirmit.CATI.Common.WcfTools.ErrorServiceMessageHeader.Fakes
{
    public class StubILoginPasswordAuthenticationDataProvider : ILoginPasswordAuthenticationDataProvider 
    {
        private ILoginPasswordAuthenticationDataProvider _inner;

        public StubILoginPasswordAuthenticationDataProvider()
        {
            _inner = null;
        }

        public ILoginPasswordAuthenticationDataProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _Login;
        public Func<string> LoginGet;
        public Action<string> LoginSetString;

        string ILoginPasswordAuthenticationDataProvider.Login
        {
            get
            {
                if (LoginGet != null)
                {
                    return LoginGet();
                } else if (_inner != null)
                {
                    return ((ILoginPasswordAuthenticationDataProvider)_inner).Login;
                }

                if (LoginSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Login;
                }

                return default(string);
            }

        }

        private string _Password;
        public Func<string> PasswordGet;
        public Action<string> PasswordSetString;

        string ILoginPasswordAuthenticationDataProvider.Password
        {
            get
            {
                if (PasswordGet != null)
                {
                    return PasswordGet();
                } else if (_inner != null)
                {
                    return ((ILoginPasswordAuthenticationDataProvider)_inner).Password;
                }

                if (PasswordSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Password;
                }

                return default(string);
            }

        }

        private int _CompanyId;
        public Func<int> CompanyIdGet;
        public Action<int> CompanyIdSetInt32;

        int ILoginPasswordAuthenticationDataProvider.CompanyId
        {
            get
            {
                if (CompanyIdGet != null)
                {
                    return CompanyIdGet();
                } else if (_inner != null)
                {
                    return ((ILoginPasswordAuthenticationDataProvider)_inner).CompanyId;
                }

                if (CompanyIdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CompanyId;
                }

                return default(int);
            }

        }

    }
}