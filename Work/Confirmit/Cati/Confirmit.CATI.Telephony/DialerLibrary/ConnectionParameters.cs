using System.Xml;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class ConnectionParameters
    {
        public string DialerServiceAddress { get; private set; }
        public string DialerServiceEndpoint { get; private set; }
        public string AuthorizationKeyForOutgoingRequests { get; private set; }

        /// <summary>
        /// sets the dialer connection parameters from xml
        /// </summary>
        /// <param name="connectionParametersXml">
        /// <example>
        ///     <DialerConnectionParameters>
        ///        <ServiceAddress>http://DialerWsHost/DialerService</ServiceAddress>
        ///        <ServiceEndpoint>DialerServiceEndpoint</ServiceEndpoint>
        ///        <AuthorizationKeyForOutgoingRequests>Gfkr31ZZ7jyuUDoM+OQ0cHvaz88fqJoy9zoxdoRJjr7FVRYjWYtVX/C/afumpTX8erM0d5cQZPEtQ9khe/sbOUW8lSyswcJLkqzXkCbKy5mFMxJhTMhEcgE286I=</AuthorizationKeyForOutgoingRequests>
        ///    </DialerConnectionParameters>
        /// </example>
        /// </param>
        public ConnectionParameters(string connectionParametersXml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(connectionParametersXml);
            var connParamsNode = xmlDocument.SelectSingleNode("DialerConnectionParameters");
            DialerServiceAddress = ServiceLocator.Resolve<ISideBySideManager>().AddSideBySideNameToIISServiceUrl(
                connParamsNode.SelectSingleNode("ServiceAddress").InnerText);
            DialerServiceEndpoint = connParamsNode.SelectSingleNode("ServiceEndpoint").InnerText;
            AuthorizationKeyForOutgoingRequests = connParamsNode.SelectSingleNode("AuthorizationKeyForOutgoingRequests").InnerText;
        }
    }
}