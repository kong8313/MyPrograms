using System;
using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;

namespace Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader.Fakes
{
    public class StubIAuthorizationMessageHeaderReader : IAuthorizationMessageHeaderReader 
    {
        private IAuthorizationMessageHeaderReader _inner;

        public StubIAuthorizationMessageHeaderReader()
        {
            _inner = null;
        }

        public IAuthorizationMessageHeaderReader Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetIncomingMessageLoginDelegate();
        public GetIncomingMessageLoginDelegate GetIncomingMessageLogin;

        string IAuthorizationMessageHeaderReader.GetIncomingMessageLogin()
        {


            if (GetIncomingMessageLogin != null)
            {
                return GetIncomingMessageLogin();
            } else if (_inner != null)
            {
                return ((IAuthorizationMessageHeaderReader)_inner).GetIncomingMessageLogin();
            }

            return default(string);
        }

        public delegate Guid GetIncomingMessageKeyDelegate();
        public GetIncomingMessageKeyDelegate GetIncomingMessageKey;

        Guid IAuthorizationMessageHeaderReader.GetIncomingMessageKey()
        {


            if (GetIncomingMessageKey != null)
            {
                return GetIncomingMessageKey();
            } else if (_inner != null)
            {
                return ((IAuthorizationMessageHeaderReader)_inner).GetIncomingMessageKey();
            }

            return default(Guid);
        }

        public delegate string GetIncomingMessagePasswordDelegate();
        public GetIncomingMessagePasswordDelegate GetIncomingMessagePassword;

        string IAuthorizationMessageHeaderReader.GetIncomingMessagePassword()
        {


            if (GetIncomingMessagePassword != null)
            {
                return GetIncomingMessagePassword();
            } else if (_inner != null)
            {
                return ((IAuthorizationMessageHeaderReader)_inner).GetIncomingMessagePassword();
            }

            return default(string);
        }

    }
}