using Confirmit.CATI.Backend.Threads;
using Confirmit.CATI.Backend.WcfServices;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Backend.ProcessInitializers
{
    internal class ProcessInitializerFactory : IProcessInitializerFactory
    {
        public IProcessInitializer CreateProcessInitializer(int companyId)
        {
            if (IsRunningDefaultInstance(companyId))
            {
                return CreateDefaultProcessInitializer();
            }

            return CreateCatiProcessInitializer(companyId);
        }

        private static IProcessInitializer CreateDefaultProcessInitializer()
        {
            IPeriodicalThreadsManager periodicalThreadsManager = new PeriodicalThreadsManager();

            IWcfServicesManager wcfServicesManager = new WcfServicesManager();


            var setupSettings = ServiceLocator.Resolve<ISetupSettings>();

            var defaultProcessInitializer = new DefaultProcessInitializer(
                periodicalThreadsManager,
                wcfServicesManager,
                setupSettings);

            return defaultProcessInitializer;
        }

        private IProcessInitializer CreateCatiProcessInitializer(int companyId)
        {
            IPeriodicalThreadsManager periodicalThreadsManager = new PeriodicalThreadsManager();

            IWcfServicesManager wcfServicesManager = new WcfServicesManager();

            var campaignInitializer = ServiceLocator.Resolve<IDialerCampaignInitializer>();

            var setupSettings = ServiceLocator.Resolve<ISetupSettings>();

            var catiProcessInitializer = new CatiProcessInitializer(
                companyId,
                periodicalThreadsManager,
                wcfServicesManager,
                campaignInitializer,
                setupSettings);

            return catiProcessInitializer;
        }

        private bool IsRunningDefaultInstance(int companyId)
        {
            return companyId == 0;
        }
    }
}
