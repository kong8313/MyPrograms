namespace Confirmit.CATI.Core.Misc
{
    public interface IConnectionStrings
    {
        string MasterConnectionString { get; }
        string ConfirmlogConnectionString { get; }
        string ConfirmConnectionString { get; }
        string DefaultInstanceConnectionString { get; }
        string GetConnectionStringForSpecificCompany(int companyId);
        string GetMasterConnectionStringForSpecificServer(int serverId);
        string GetMasterConnectionStringForSpecificCompanyServer(int companyId);
    }
}