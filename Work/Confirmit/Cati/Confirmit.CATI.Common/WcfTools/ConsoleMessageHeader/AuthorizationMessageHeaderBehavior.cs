using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader
{
    /// <summary>
    /// When applied to client endpoint - sends Login and AuthenticationKey or Login and Password 
    /// depending  on the called constructor in the WCF message header with all requests.
    /// Use methods GetIncomingMessageLogin and GetIncomingMessageKey
    /// </summary>
    public class AuthorizationMessageHeaderBehavior : IEndpointBehavior, IClientMessageInspector
    {
        /// <summary>
        /// Login name supplied in the constructor. Added to headers for the Console/ConsoleState services for all outgoing messages.
        /// </summary>
        private readonly string _login;

        /// <summary>
        /// Authentication key supplied in the constructor. Added to headers for the ConsoleState service for all outgoing messages.
        /// </summary>
        private readonly Guid _authenticationKey;

        /// <summary>
        /// Password supplied in the constructir. Added to the headers for the Console service for all outgoing messages..
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationMessageHeaderBehavior"/> class
        /// with the specified login name and authentication key. Must be used for the ConsoleState service.
        /// </summary>
        /// <param name="login">The interviewer login name.</param>
        /// <param name="authenticationKey">The authentication key.</param>
        public AuthorizationMessageHeaderBehavior(string login, Guid authenticationKey)
        {
            _login = login;
            _authenticationKey = authenticationKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationMessageHeaderBehavior"/> class
        /// with the specified login name and authentication key. Must be used for the Console service.
        /// </summary>
        /// <param name="login">The interviewer login name.</param>
        /// <param name="password">The interviewer password.</param>
        public AuthorizationMessageHeaderBehavior(string login, string password)
        {
            _login = login;
            _password = password;
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Registers <see cref="AuthorizationMessageHeaderBehavior"/> as a message inspector.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }

        /// <summary>
        /// Adds login and authentication key or login and password to the message header before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var loginHeader = MessageHeader.CreateHeader(
                AuthorizationMessageHeaderConstants.LoginHeaderName,
                AuthorizationMessageHeaderConstants.Namespace,
                _login);

            request.Headers.Add(loginHeader);

            if (string.IsNullOrEmpty(_password))
            {
                var keyHeader = MessageHeader.CreateHeader(
                    AuthorizationMessageHeaderConstants.KeyHeaderName,
                    AuthorizationMessageHeaderConstants.Namespace,
                    _authenticationKey);

                request.Headers.Add(keyHeader);

            }
            else
            {
                var passwordHeader = MessageHeader.CreateHeader(
                    AuthorizationMessageHeaderConstants.PasswordHeaderName,
                    AuthorizationMessageHeaderConstants.Namespace,
                    _password);

                request.Headers.Add(passwordHeader);
            }

            return null;
        }

        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
    }
}