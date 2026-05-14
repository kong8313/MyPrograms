﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Confirmit.CATI.Common.ServiceLocation;
using System.ServiceModel.Dispatcher;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Backend.WcfServices.Tools.IPFilter
{
    public class IpFilterBehavior : Attribute, IServiceBehavior
    {
        [DefaultValue(IpFilterMode.SystemSetting)]
        public IpFilterMode IpFilterMode { get; set; }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // in container environment k8s blocks access to internal APIs, so app-level handling is not nesessary
            if (BootstrapConfig.IsContainerEnvironment && IpFilterMode == IpFilterMode.SystemSetting)
                return;

            IDispatchMessageInspector inspector = CreateMessageInspector();

            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var dispatcher = (ChannelDispatcher)channelDispatcherBase;
                foreach (var ep in dispatcher.Endpoints)
                {
                    ep.DispatchRuntime.MessageInspectors.Add(inspector);
                }
            }
        }

        private IDispatchMessageInspector CreateMessageInspector()
        {
            if (IpFilterMode == IpFilterMode.SystemSetting)
            {
                return ServiceLocator.Resolve<IpAndDnsFilterInspector>();
            }

            return ServiceLocator.Resolve<DialerIpFilterInspector>();
        }
    }
}