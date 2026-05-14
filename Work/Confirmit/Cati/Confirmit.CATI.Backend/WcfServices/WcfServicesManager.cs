using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;

using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Configuration;

namespace Confirmit.CATI.Backend.WcfServices
{
    internal class WcfServicesManager : IWcfServicesManager
    {
        private List<WcfServiceDescriptionAndHost> _serviceHostsAndDescriptions;

        /// <summary>
        /// Creates, initializes and publishes ServiceHost for all Wcf services provided.
        /// </summary>
        /// <param name="serviceDescriptions">
        /// The service descriptions to start.
        /// </param>
        public void Start(IEnumerable<IWcfServiceDescription> serviceDescriptions)
        {
            if (_serviceHostsAndDescriptions != null)
            {
                throw new InternalErrorException("WcfServicesManager.Start already called");
            }

            var sideBySideManager = ServiceLocator.Resolve<ISideBySideManager>();
            var settings = ServiceLocator.Resolve<ISystemSettings>();

            _serviceHostsAndDescriptions = new List<WcfServiceDescriptionAndHost>();

            foreach (var serviceDescription in serviceDescriptions)
            {
                var riskyUri = new Uri(sideBySideManager.AddSideBySideNameToBackendWCFServiceUrl(serviceDescription.Uri));

                var serviceHost = new ServiceHost(
                    serviceDescription.ServiceType,
                    riskyUri);

                if (!serviceDescription.IsExternal && !serviceDescription.IsInternalHttpOnly)
                {
                    if (settings.Setup.IsLoadBalancedEnvironment == "True")
                    {
                        var securedUri = new UriBuilder(riskyUri)
                        {
                            Port = 81
                        };

                        serviceHost.AddServiceEndpoint(
                            serviceDescription.ServiceType.GetInterfaces()[0],
                            new BasicHttpBinding("InternalService_BasicHttpBinding"),
                            securedUri.Uri);
                    }
                    else
                    {
                        var securedUri = new UriBuilder(riskyUri)
                        {
                            Port = -1,
                            Scheme = "https"
                        };

                        serviceHost.AddServiceEndpoint(
                            serviceDescription.ServiceType.GetInterfaces()[0],
                            new BasicHttpsBinding("InternalService_BasicHttpsBinding"),
                            securedUri.Uri);
                    }
                }

                var serviceDescriptionAndHost = new WcfServiceDescriptionAndHost { Description = serviceDescription, Host = serviceHost };

                if (serviceDescription.RequireSchemaIndependentEndpointAddress)
                {
                    AdjustServiceEndpointLogicalAddress(
                        serviceHost);
                }

                if ((serviceDescription.IsExternal && settings.Debug.PublishMetadataForExternalWCFServices) ||
                   (!serviceDescription.IsExternal && settings.Debug.PublishMetadataForInternalWCFServices))
                {
                    AddMetadataEndpointToServiceHost(serviceDescriptionAndHost);
                }

                foreach (var endpoint in serviceHost.Description.Endpoints)
                {
                    Trace.TraceInformation(
                        "Opening '{0}' host .\r\n Name: {1} Logical URI: {2}\r\nListening URI: {3}",
                        serviceDescription.ServiceName,
                        endpoint.Name,
                        endpoint.Address.Uri,
                        endpoint.ListenUri);
                }

                serviceHost.Open();

                _serviceHostsAndDescriptions.Add(serviceDescriptionAndHost);

                Trace.TraceInformation(
                    "{0} successfully opened.",
                    serviceDescription.ServiceName);
            }
        }

        /// <summary>
        /// Closes all started wcf services.
        /// </summary>
        public void Stop()
        {
            if (_serviceHostsAndDescriptions == null)
            {
                return;
            }

            foreach (var serviceHostAndDescription in _serviceHostsAndDescriptions)
            {
                if (serviceHostAndDescription.Host != null)
                {
                    try
                    {
                        Trace.TraceInformation(
                            "Closing {0} Host.\r\n  Logical URI: {1}\r\nListening URI: {2}",
                            serviceHostAndDescription.Description.ServiceName,
                            serviceHostAndDescription.Host.Description.Endpoints[0].Address.Uri,
                            serviceHostAndDescription.Host.Description.Endpoints[0].ListenUri);

                        serviceHostAndDescription.Host.Close();

                        Trace.TraceInformation(
                            "{0} successfully closed.",
                            serviceHostAndDescription.Description.ServiceName);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(
                            "Exception occured during closing host for {0}. Aborting service.\r\n\r\nException:\r\n{1}",
                            serviceHostAndDescription.Description.ServiceName,
                            ex);

                        serviceHostAndDescription.Host.Abort();
                    }
                }
            }

            _serviceHostsAndDescriptions = null;
        }

        /// <summary>
        /// endpoint.Address property actually contains Logical address.
        /// ListenUri is real address endpoint is listening on.
        /// So, we should change Address to the something that is not
        /// depend on port number or URI scheme. To have the same Logical address
        /// in case when SSL Accelerator installed or not.
        /// In the same time we should leave ListenUri without changes.
        /// PS:
        /// See http://msdn.microsoft.com/en-us/magazine/cc163412.aspx
        /// </summary>
        /// <param name="service">The service to adjust logical address.</param>
        private static void AdjustServiceEndpointLogicalAddress(
            ServiceHost service)
        {
            ServiceEndpoint endpoint = service.Description.Endpoints[0];
            var currentListenUri = endpoint.ListenUri;
            endpoint.Address = new EndpointAddress("urn://" + endpoint.Address.Uri.Host + endpoint.Address.Uri.PathAndQuery);

            if (ConfirmitConfiguration.SslAcceleratorMode && ConfirmitConfiguration.SslAcceleratorPort == 80)
            {
                var builder = new UriBuilder(currentListenUri) { Port = 80 };
                currentListenUri = builder.Uri;
            }

            endpoint.ListenUri = currentListenUri;
        }

        private void AddMetadataEndpointToServiceHost(
            WcfServiceDescriptionAndHost serviceRuntime)
        {
            var metadataUri = ServiceLocator.Resolve<ISideBySideManager>().AddSideBySideNameToBackendWCFServiceUrl(serviceRuntime.Description.Uri) + "/mex";

            // Always use HTTP to publish metadata
            metadataUri = metadataUri.Replace("https", "http");

            Trace.TraceInformation(
                "Publishing {0} metadata. Metadata uri {1}",
                serviceRuntime.Description.ServiceName,
                metadataUri);

            serviceRuntime.Host.AddServiceEndpoint(
                typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexHttpBinding(),
                metadataUri);

            Trace.TraceInformation(
                "{0} metadata published successfully. Metadata uri {1}",
                serviceRuntime.Description.ServiceName,
                metadataUri);
        }
    }
}
