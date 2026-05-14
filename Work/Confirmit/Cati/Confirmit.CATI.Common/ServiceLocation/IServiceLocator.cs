namespace Confirmit.CATI.Common.ServiceLocation
{
    public interface IServiceLocator
    {
        IServiceResolver CreateChildContainer();
    }
}