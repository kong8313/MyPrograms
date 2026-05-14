using System;
using System.Diagnostics;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using DialerCommon;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.BvCallHandlerLibrary
{
    public class DialerInstanceFactory: IDialerInstanceFactory
    {
        public IDialerInstance Create(BvDialersEntity dialerEntity)
        {
            var dialer = ServiceLocator.Resolve<IDialerInstance>();
            dialer.DialerId = dialerEntity.Id;
            dialer.DialerName = dialerEntity.Name;
            dialer.IsDialerInitialized = dialerEntity.DialerOperationalStateNotification;
            dialer.DialerOperationalState = dialerEntity.DialerOperationalStateNotification;
            dialer.DialType = (DialType) dialerEntity.DialTypeId;
            dialer.TenantIdInt = dialerEntity.TenantId;
            dialer.TenantId = dialerEntity.TenantId.ToString();

            try
            {
                if (dialer.IsDialerInitialized)
                {
                    if (dialerEntity.Features != null)
                    {
                        dialer.SupportedFeatures = JsonConvert.DeserializeObject<DialerFeatures>(dialerEntity.Features);
                    }
                    
                    dialer.Create();
                }
            }
            catch (Exception ex)
            {
                dialer.IsDialerInitialized = false;
                Trace.TraceError(
                    "DialerInstanceFactory.Create: Dialer[{0}, {1}] initialization failed with exception: {2}",
                    dialer.DialerName,
                    dialer.DialerId,
                    ex);
            }
            
            return dialer;
        }
    }
}