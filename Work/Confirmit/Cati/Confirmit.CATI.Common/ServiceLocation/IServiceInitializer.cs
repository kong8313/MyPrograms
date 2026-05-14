namespace Confirmit.CATI.Common.ServiceLocation
{
    public interface IServiceInitializer
    {
        void Initialize();
        void Cleanup();
    }
}