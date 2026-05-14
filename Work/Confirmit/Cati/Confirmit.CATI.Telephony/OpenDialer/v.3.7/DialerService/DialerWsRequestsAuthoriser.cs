using System.Diagnostics;
using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;
using DialerCommon.DialerExceptions;

namespace DialerCommon
{
    public class DialerWsRequestsAuthoriser
    {
        private readonly bool _authorizationEnabled;
        private readonly string _authorizationKey;

        public DialerWsRequestsAuthoriser(string authorizationKey)
            : this(authorizationKey, true)
        {
        }

        public DialerWsRequestsAuthoriser(string authorizationKey, bool authorizationEnabled)
        {
            _authorizationEnabled = authorizationEnabled;
            _authorizationKey = authorizationKey;
        }

        /// <summary>
        /// Authorises an request.
        /// </summary>
        public void AuthoriseRequest()
        {
            if (!_authorizationEnabled)
            {
                return;
            }

            var authorizationKeyFromIncomingRequest = new AuthorizationMessageHeaderReader().GetIncomingMessagePassword();

            ValidateAuthorizationKeyIsNotNull(authorizationKeyFromIncomingRequest);

            Authorise(authorizationKeyFromIncomingRequest);
        }

        private void ValidateAuthorizationKeyIsNotNull(string authorizationKeyToValidate)
        {
            if (string.IsNullOrEmpty(authorizationKeyToValidate))
            {
                ThrowDialerWsInvalidCredentialsException(string.Format("Invalid authorization key specified: [{0}]",
                    (authorizationKeyToValidate == null) ? "null" : "empty"));
            }
        }

        private void Authorise(string authorizationKeyToValidate)
        {
            if (authorizationKeyToValidate != _authorizationKey)
            {
                var message = string.Format("Invalid authorization key specified: {0}", authorizationKeyToValidate);
                Trace.TraceWarning(message);
                ThrowDialerWsInvalidCredentialsException(message);
            }
        }

        private void ThrowDialerWsInvalidCredentialsException(string message)
        {
            throw new DialerWsInvalidCredentialsException(message);
        }

    }
}