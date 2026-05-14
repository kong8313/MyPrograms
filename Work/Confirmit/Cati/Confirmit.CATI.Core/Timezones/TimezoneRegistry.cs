using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.Timezones
{
    public class TimezoneRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.Register<ITimezoneConverter, TimezoneManager>();
            serviceRegistrator.Register<ITimezoneManager, TimezoneManager>();
        }
    }
}
