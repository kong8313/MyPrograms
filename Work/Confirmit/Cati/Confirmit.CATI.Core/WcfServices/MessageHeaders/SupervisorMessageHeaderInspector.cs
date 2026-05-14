using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace Confirmit.CATI.Core.WcfServices.MessageHeaders
{
    /// <summary>
    /// Adds the user name as a custom header to the WCF message.
    /// </summary>
    public class SupervisorMessageHeaderInspector : IClientMessageInspector 
    {
        /// <summary>
        /// The namespace to create message header with.
        /// </summary>
        private const string Namespace = "http://confirmit.com/2009/09/SupervisorMessageHeaderBehavior";

        /// <summary>
        /// The name of a custom header.
        /// </summary>
        private const string Name = "Username";

        /// <summary>
        /// Adds the user name as a custom header to the WCF message.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        /// <returns>
        /// The object that is returned as the <paramref name="correlationState "/>argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)"/> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid"/> to ensure that no two <paramref name="correlationState"/> objects are the same.
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            string currentContextUserName = HttpContext.Current.User.Identity.Name;

            var userNameHeader = MessageHeader.CreateHeader(Name, Namespace, currentContextUserName);

            request.Headers.Add(userNameHeader);

            return null;
        }

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        /// <summary>
        /// Gets the supervisor name from the custom header of the current incoming message.
        /// </summary>
        /// <returns>The supervisor name if found, empty string otherwise.</returns>
        public static string GetIncomingMessageSupervisor()
        {
            var result = string.Empty;
            if (OperationContext.Current != null)
            {
                var headers = OperationContext.Current.IncomingMessageHeaders;
                int headerIndex = headers.FindHeader(Name, Namespace);
                if (headerIndex >= 0)
                {
                    result = headers.GetHeader<string>(headerIndex);
                }
            }

            return result;
        }
    }
}