namespace Confirmit.CATI.Backend.ProcessInitializers
{
    internal interface IProcessInitializerFactory
    {
        IProcessInitializer CreateProcessInitializer(int companyId);
    }
}