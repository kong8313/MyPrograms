using System;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader
{
    public class AuthorizationMessageHeaderReader : IAuthorizationMessageHeaderReader
    {
        private readonly IMessageHeaderAccessor _messageHeaderAccessor;

        public AuthorizationMessageHeaderReader()
            : this(ServiceLocator.Resolve<IMessageHeaderAccessor>())
        {

        }

        public AuthorizationMessageHeaderReader(IMessageHeaderAccessor messageHeaderAccessor)
        {
            _messageHeaderAccessor = messageHeaderAccessor;    
        }

        /// <summary>
        /// Gets the interviewer login name from the custom header of the current incoming message.
        /// </summary>
        /// <returns>The interviewer login name if found, empty string otherwise.</returns>
        public string GetIncomingMessageLogin()
        {
            return _messageHeaderAccessor.GetValueFromHeader<string>(
                       AuthorizationMessageHeaderConstants.LoginHeaderName,
                       AuthorizationMessageHeaderConstants.Namespace) ?? string.Empty;
        }

        /// <summary>
        /// Gets the authentication key from the custom header of the current incoming message.
        /// </summary>
        /// <returns>The authentication key if found, Guid.Empty otherwise.</returns>
        public Guid GetIncomingMessageKey()
        {
            return _messageHeaderAccessor.GetValueFromHeader<Guid>(
                        AuthorizationMessageHeaderConstants.KeyHeaderName,
                        AuthorizationMessageHeaderConstants.Namespace);
        }

        /// <summary>
        /// Gets the interviewer password from the custom header of the current incoming message.
        /// </summary>
        /// <returns>The interviewer password if found, empty string otherwise.</returns>
        public string GetIncomingMessagePassword()
        {
            return _messageHeaderAccessor.GetValueFromHeader<string>(
                       AuthorizationMessageHeaderConstants.PasswordHeaderName,
                       AuthorizationMessageHeaderConstants.Namespace) ?? string.Empty;
        }
    }
}