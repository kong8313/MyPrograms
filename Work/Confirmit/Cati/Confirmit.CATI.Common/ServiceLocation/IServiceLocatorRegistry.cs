namespace Confirmit.CATI.Common.ServiceLocation
{
    public interface IServiceLocatorRegistry
    {
        void RegisterTypes(IServiceRegistrator serviceRegistrator);
    }
}
