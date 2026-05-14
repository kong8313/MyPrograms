using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader
{
    /// <summary>
    /// When applied to client endpoint - sends cookies 
    /// specified in the constructor in the HTTP header with all requests.
    /// </summary>
    public class CookieMessagingBehavior : IEndpointBehavior, IClientMessageInspector
    {
        private readonly CookieCollection _cookieCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieMessagingBehavior"/> class
        /// with the specified <see cref="CookieCollection"/>.
        /// </summary>
        /// <param name="cookieCollection">The cookie collection to send with service requests.</param>
        public CookieMessagingBehavior(CookieCollection cookieCollection)
        {
            _cookieCollection = cookieCollection;
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Registers <see cref="CookieMessagingBehavior"/> as a message inspector.
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
        /// Adds cookies to the HTTP header before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // The HTTP request object is made available in the outgoing message only
            // when the Visual Studio Debugger is attached to the running process
            if (!request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                request.Properties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());
            }

            var httpRequest = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

            foreach (var cookie in _cookieCollection)
            {
                httpRequest.Headers.Add(HttpRequestHeader.Cookie, cookie.ToString());
            }

            return null;
        }

        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
    }
}