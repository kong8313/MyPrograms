using System.Data.SqlClient;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.SystemTestFramework
{
    public class StubIDbLibProvider : IDbLibProvider
    {
        public string GetMasterConnectionStringForServer(int sqlServerId)
        {
            throw new System.NotImplementedException();
        }

        public string GetConfirmAdminConnectionStringForSpecificServer(int sqlServerId)
        {
            throw new System.NotImplementedException();
        }

        public string CatiDefaultConnectionString => Properties.Settings.Default.CatiConnectionString;

        public string ConfirmConnectionString => Properties.Settings.Default.ConfirmConnectionString;

        public string ConfirmlogConnectionString =>
            new SqlConnectionStringBuilder(Properties.Settings.Default.ConfirmConnectionString)
                {InitialCatalog = "confirmlog"}.ToString();

        public string ConfirmAdminConnectionString(string projectId)
        {
            throw new System.NotImplementedException();
        }

        public string GetConnectionStringForSpecificCompany(int companyId)
        {
            return CatiDefaultConnectionString.Replace("ConfirmitCATIV15", "ConfirmitCATIV15_" + companyId);
        }

        public int GetRandomCatiSqlServerId()
        {
            throw new System.NotImplementedException();
        }
    }
}