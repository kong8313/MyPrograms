using Confirmit.Configuration;
using Confirmit.Databases;
using Confirmit.DataServices.RDataAccess;

namespace Confirmit.CATI.Core.Misc
{
    public class DbLibProvider : IDbLibProvider
    {
        public DbLibProvider()
        {
            new ConfigurationLoader().LoadConfiguration();
        }

        public string CatiDefaultConnectionString => DbLib.GetCatiDefaultConnectInfo().GetConnectString();
        public string ConfirmConnectionString => DbLib.GetConfirmConnectInfo().GetConnectString();
        public string ConfirmlogConnectionString => DbLib.GetConfirmlogConnectInfo().GetConnectString();
        public string ConfirmAdminConnectionString(string projectId) => DbLib.GetConfirmAdminConnectInfo(projectId).GetConnectString();
        public string GetConnectionStringForSpecificCompany(int companyId) => DbLib.GetCatiConnectInfo(companyId).GetConnectString();
        public int GetRandomCatiSqlServerId() => DbLib.GetRandomCatiServerId();
        public string GetMasterConnectionStringForServer(int sqlServerId) => DbLib.GetCatiMasterConnectInfo(sqlServerId).GetConnectString();
    }
}