using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Confirmit.CATI.Common.WcfTools.ErrorServiceMessageHeader
{
    public class ErrorServiceMessageHeaderBehavior : IEndpointBehavior, IClientMessageInspector
    {
        #region Fields

        private ILoginPasswordAuthenticationDataProvider _authenticationDataProvider;
        private const string Namespace = "http://confirmit.com/2010/05/25/ErrorServiceMessageHeaderBehavior";

        private const string LoginHeaderName = "Login";
        private const string PasswordHeaderName = "Password";
        private const string CompanyHeaderName = "Company";

        private readonly IMessageHeaderAccessor _messageHeaderAccessor;
        #endregion

        public ErrorServiceMessageHeaderBehavior(
            ILoginPasswordAuthenticationDataProvider authenticationDataProvider, IMessageHeaderAccessor messageHeaderAccessor)
        {
            _authenticationDataProvider = authenticationDataProvider;
            _messageHeaderAccessor = messageHeaderAccessor;
        }

        #region Implementation of IEndpointBehavior

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

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

        #endregion

        #region Implementation of IClientMessageInspector

        /// <summary>
        /// Adds login and authentication key to the message header before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var loginHeader = MessageHeader.CreateHeader(LoginHeaderName, Namespace, _authenticationDataProvider.Login);
            var passwordHeader = MessageHeader.CreateHeader(PasswordHeaderName, Namespace, _authenticationDataProvider.Password);
            var companyHeader = MessageHeader.CreateHeader(CompanyHeaderName, Namespace, _authenticationDataProvider.CompanyId);

            request.Headers.Add(loginHeader);
            request.Headers.Add(passwordHeader);
            request.Headers.Add(companyHeader);

            return null;
        }

        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        #endregion

        public void UpdateAuthenticationDataProvider(
            ILoginPasswordAuthenticationDataProvider authenticationDataProvider)
        {
            _authenticationDataProvider = authenticationDataProvider;
        }

        public string GetIncomingMessageLogin()
        {
            return _messageHeaderAccessor.GetValueFromHeader<string>(LoginHeaderName, Namespace) ?? string.Empty;
        }

        public string GetIncomingMessagePassword()
        {
            return _messageHeaderAccessor.GetValueFromHeader<string>(PasswordHeaderName, Namespace) ?? string.Empty;
        }
    }
}
