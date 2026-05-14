using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.AsynchronousTrigger.Triggers.CacheTriggers.CustomCacheTriggers.SystemSettingTriggers;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.IntegrationTests.Tests.IpAndDnsFiltering.Tools
{
    public class IpFilteringEngine
    {
        public void UpdateAccessAllowedIpAddresses(string newValue)
        {
            BvSystemSettingsAdapter.UpdateByCondition(new BvSystemSettingsEntity
            {
                SystemName = "Server.AccessAllowedIPAddresses",
                Value = newValue,
                Group = "System",
                Type = 1,
                Hidden = false
            }, "SystemName = 'Server.AccessAllowedIPAddresses'");

            var bvSystemSettingTrigger = new BvSystemSettingTrigger();
            bvSystemSettingTrigger.OnTableChanged(new TriggerMessage());
        } 
    }
}