namespace Confirmit.CATI.Common.ServiceLocation
{
    public interface IServiceResolver
    {
        T Resolve<T>();
    }
}