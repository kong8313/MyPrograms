using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.IntegrationTests.Framework.Data.Mock;

namespace Confirmit.CATI.IntegrationTests.Framework.Data
{
    public class TestDataMocker
    {
        public void Mock(TestDataContext context)
        {
            ServiceLocator.Resolve<IServiceRegistrator>()
                .RegisterInstance((IAuthoringService)new MockAuthoringService(context));
        }
    }
}