namespace Confirmit.CATI.Core.Misc
{
    public interface IBackendInstanceFactory
    {
        BackendInstance Create(
            int companyId,
            HostType hostType);
    }
}