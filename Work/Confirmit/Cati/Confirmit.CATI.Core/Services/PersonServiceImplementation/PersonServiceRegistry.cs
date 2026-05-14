using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Core.Services.PersonServiceImplementation
{
    public class PersonServiceRegistry : IServiceLocatorRegistry
    {
        public void RegisterTypes(IServiceRegistrator serviceRegistrator)
        {
            serviceRegistrator
                .Register<IPersonAuthorizer, PersonAuthorizer>()
                .Register<IPasswordSaver, PersonService>()
                .Register<IPersonPwdSetDateSetter, PersonService>();
        }
    }
}
