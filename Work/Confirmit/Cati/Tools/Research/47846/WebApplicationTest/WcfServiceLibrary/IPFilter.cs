using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

using System.Diagnostics;
using System.Configuration;

namespace WcfServiceLibrary
{

    public class IPFilterBehavior : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var inspector = new IPFilterInspector();

            foreach (ChannelDispatcher chanDisp in serviceHostBase.ChannelDispatchers)
            {
                foreach (var ep in chanDisp.Endpoints)
                {
                    ep.DispatchRuntime.MessageInspectors.Add(inspector);
                }
            }
        }
    }
    public class IPFilterInspector : IDispatchMessageInspector
    {
        /// <summary>
        /// Get IPAddresses from a string
        /// </summary>
        /// <param name="ipAddressesString">String with ip addresses parted by semicolon</param>
        /// <returns></returns>
        private static IPAddress[] GetIPAddresses(string ipAddressesString)
        {
            ipAddressesString = ipAddressesString.Trim();

            string[] ipAddresesArray = ipAddressesString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            var ipAddresses = new IPAddress[ipAddresesArray.Length];
            for (int i = 0; i < ipAddresesArray.Length; i++)
            {
                var address2Parse = ipAddresesArray[i].Trim();

                if (address2Parse.Length == 0)
                {
                    Trace.TraceWarning("Empty entry in the AccessAllowedIPAddresses setting found.");
                    continue;
                }

                try
                {
                    ipAddresses[i] = IPAddress.Parse(address2Parse);
                }
                catch(Exception e)
                {
                    Trace.TraceError(
                        string.Format(
                            "Cannot parse IP address {0}. Fix configuration setting AccessAllowedIPAddresses and restart system. Exception {1}",
                            address2Parse,
                            e));
                }
            }

            return ipAddresses;
        }


        /// <summary>
        /// Verify, that IP address is valid
        /// </summary>
        /// <param name="address">IP address</param>
        /// <returns></returns>
        private static bool VerifyAddress(IPAddress address)
        {
            IPAddress[] validAddresses = GetIPAddresses(ConfigurationManager.AppSettings["AccessAllowedIPAddresses"]);

            foreach (IPAddress ipAddress in validAddresses)
            {
                if ( ipAddress.Equals( address ) )
                {
                    return true;
                }
            }
            return false;
        }


        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            // RemoteEndpointMessageProperty new in 3.5 allows us to get the remote endpoint address.
            var remoteEndpoint = request.Properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;

            // The address is a string so we have to parse to get as a number
            if (remoteEndpoint != null)
            {
                IPAddress address = IPAddress.Parse(remoteEndpoint.Address);
                if (IPAddress.IsLoopback(address))
                {
                    address = IPAddress.Parse("127.0.0.1");
                }

                // If ip address is denied clear the request mesage so service method does not get execute
                if (!VerifyAddress(address))
                {
                    var accessDeniedMessage = string.Format(
                        "WCF Service REQUEST DENIED because IP {0} not in allowed list.",
                        address);

                    Trace.TraceError(accessDeniedMessage);

                    var responseProperty = new HttpResponseMessageProperty
                    {
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                    OperationContext.Current.OutgoingMessageProperties["httpResponse"] = responseProperty;
                    //seems to be not needed.
                   //throw new UnauthorizedAccessException(accessDeniedMessage);
                }
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }
    }

    public class IPFilteringBehaviorElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new IPFilterBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                string s = typeof(IPFilterBehavior).AssemblyQualifiedName;
                return typeof(IPFilterBehavior);
            }
        }
    }    
}
