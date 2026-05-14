using System.Collections.Generic;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class SystemSettingsBuilder : BaseObjectBuilder<Dictionary<string, object>>
    {
        public SystemSettingsBuilder(TestDataContext context, Dictionary<string, object> data, DataGenerator dataGenerator) 
            : base(context, data, dataGenerator)
        {
        }

        public override void Create()
        {
            var cache = ServiceLocator.Resolve<ISystemSettingCache>();
            foreach (var setting in Data)
            {
                cache.Set(setting.Key, setting.Value);
            }
        }
    }
}