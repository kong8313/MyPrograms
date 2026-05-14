using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Confirmit.CATI.Backend.WcfServices.Tools
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MetricsBehaviour : Attribute, IServiceBehavior, IDispatchMessageInspector
    {
        public bool TrackMethodsSeparately { get; set; }
        public string ExcludeMethodsPrefix {get; set; }
        
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var dispatcher = (ChannelDispatcher)channelDispatcherBase;
                foreach (var ep in dispatcher.Endpoints)
                {
                    ep.DispatchRuntime.MessageInspectors.Add(this);
                }
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var action = request?.Headers?.Action;

            // Action usually contains the method name (SOAP action)
            // Example: http://tempuri.org/IMyService/MyMethod
            string methodName = TrackMethodsSeparately ? GetMethodNameFromAction(action) : string.Empty;

            if (!string.IsNullOrEmpty(ExcludeMethodsPrefix) &&
                methodName.StartsWith(ExcludeMethodsPrefix, StringComparison.OrdinalIgnoreCase))
                return null;

            string serviceName = instanceContext.Host.Description.ServiceType.Name;
            
            return Core.CustomMetrics.OnWcfRequest(
                serviceName,
                methodName);
        }
        
        private string GetMethodNameFromAction(string action)
        {
            if (!string.IsNullOrEmpty(action))
            {
                // Assuming action follows this pattern: http://namespace/Interface/Method
                Uri actionUri = new Uri(action);
                return actionUri.Segments.LastOrDefault();
            }
            return string.Empty;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (correlationState != null && correlationState is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}