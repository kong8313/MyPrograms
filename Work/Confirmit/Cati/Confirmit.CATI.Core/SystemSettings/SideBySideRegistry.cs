using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.SystemSettings
{
    public class SideBySideRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.Register<ISideBySideManager, SideBySideManager>();
        }
    }
}
