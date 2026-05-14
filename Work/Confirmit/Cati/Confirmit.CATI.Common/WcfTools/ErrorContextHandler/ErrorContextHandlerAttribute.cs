using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Confirmit.CATI.Common.WcfTools.ErrorContextHandler
{
    public enum WebServiceType
    {
        Internal,
        External
    }

    /// <summary>
    /// A service behavior for applying the error context handler for a service via attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ErrorContextHandlerAttribute : Attribute, IServiceBehavior, IOperationBehavior
    {
        private readonly WebServiceType _webServiceType;

        public ErrorContextHandlerAttribute(WebServiceType webServiceType)
        {
            _webServiceType = webServiceType;
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
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
                    operation.Behaviors.Add(this);
                }

                break;
            }
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new ErrorHandlingInvoker(dispatchOperation.Invoker, _webServiceType);
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }
    }
}