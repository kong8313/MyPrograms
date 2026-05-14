using Confirmit.CATI.Core.Misc.ConfirmitClientKey;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.PersonLogin;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Services.WaitingService;

namespace Confirmit.CATI.Core.Misc
{
    public class MiscRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.RegisterSingleton<IProcessAndEnvironmentInfo, ProcessAndEnvironmentInfo>()
                              .RegisterSingleton<IConfirmitClientKeyProvider, BackendConfirmitClientKeyProvider>()
                              .Register<ISystemSettingRepository, SystemSettingRepository>()
                              .RegisterSingleton<IAsyncManager, AsyncManager>()
                              .Register<ITimeService, TimeService>()
                              .Register<ILicenseService, LicenseService>()
                              .Register<IWaitingService, WaitingService>()
                              .Register<IConfirmitEncryptionSettingProvider, ConfirmitEncryptionSettingProvider>()
                              .Register<IPgpEncryptionService, PgpEncryptionService>()
                              .RegisterSingleton<IHttpClientFactory, HttpClientFactory>();
        }
    }
}
