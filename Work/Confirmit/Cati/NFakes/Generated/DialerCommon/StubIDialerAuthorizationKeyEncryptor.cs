using System;
using DialerCommon;

namespace DialerCommon.Fakes
{
    public class StubIDialerAuthorizationKeyEncryptor : IDialerAuthorizationKeyEncryptor 
    {
        private IDialerAuthorizationKeyEncryptor _inner;

        public StubIDialerAuthorizationKeyEncryptor()
        {
            _inner = null;
        }

        public IDialerAuthorizationKeyEncryptor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string EncryptStringStringDelegate(string text);
        public EncryptStringStringDelegate EncryptStringString;

        string IDialerAuthorizationKeyEncryptor.EncryptString(string text)
        {


            if (EncryptStringString != null)
            {
                return EncryptStringString(text);
            } else if (_inner != null)
            {
                return ((IDialerAuthorizationKeyEncryptor)_inner).EncryptString(text);
            }

            return default(string);
        }

        public delegate string DecryptStringStringDelegate(string cipherText);
        public DecryptStringStringDelegate DecryptStringString;

        string IDialerAuthorizationKeyEncryptor.DecryptString(string cipherText)
        {


            if (DecryptStringString != null)
            {
                return DecryptStringString(cipherText);
            } else if (_inner != null)
            {
                return ((IDialerAuthorizationKeyEncryptor)_inner).DecryptString(cipherText);
            }

            return default(string);
        }

        public delegate void ClearDelegate();
        public ClearDelegate Clear;

        void IDialerAuthorizationKeyEncryptor.Clear()
        {

            if (Clear != null)
            {
                Clear();
            } else if (_inner != null)
            {
                ((IDialerAuthorizationKeyEncryptor)_inner).Clear();
            }
        }

    }
}