using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Confirmit.CATI.Backend.WcfServices.Tools.Logging
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HeadersHandlerAttribute : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                if (endpoint.Contract.Name == "IMetadataExchange")
                {
                    continue;
                }

                foreach (var operation in endpoint.Contract.Operations)
                {
                    operation.Behaviors.Add(new HeadersHandlerOperationBehaviour());
                }

                break;
            }
        }
    }
}