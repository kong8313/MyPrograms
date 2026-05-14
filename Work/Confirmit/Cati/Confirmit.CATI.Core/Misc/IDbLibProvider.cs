using Confirmit.Configuration;

namespace Confirmit.CATI.Core.Misc
{
    public interface IDbLibProvider
    {
        string CatiDefaultConnectionString { get; }
        
        string ConfirmConnectionString { get; }
        
        string ConfirmlogConnectionString { get; }

        string ConfirmAdminConnectionString(string projectId);
        string GetConnectionStringForSpecificCompany(int companyId);
        int GetRandomCatiSqlServerId();
        string GetMasterConnectionStringForServer(int sqlServerId);
    }
}
