using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.Security
{
    public class SecurityRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator.Register<IPasswordHash, PasswordHash>();
        }
    }
}
