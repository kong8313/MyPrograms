using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Handmade.Cache;

namespace Confirmit.CATI.Core.SystemSettings
{
    public class SystemSettingRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<ISideBySideManager, SideBySideManager>()
                .Register<IStartedServicesRepository, StartedServicesRepository>()
                .Register<ISystemSettings, SystemSettings>()
                .Register<ISystemSettingRepository, SystemSettingRepository>()
                .RegisterSingleton<ISystemSettingCache, SystemSettingCache>();
        }
    }
}
