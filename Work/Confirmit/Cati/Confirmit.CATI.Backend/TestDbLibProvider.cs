using Confirmit.CATI.Core.Misc;
using System;
using System.Data.SqlClient;

namespace Confirmit.CATI.Backend
{
    public class TestDbLibProvider : IDbLibProvider
    {
        public string CatiDefaultConnectionString => Environment.GetEnvironmentVariable(GeneralConstants.TestCatiConnectionString, EnvironmentVariableTarget.Machine);
        
        public string ConfirmConnectionString => Environment.GetEnvironmentVariable(GeneralConstants.TestConfirmConnectionString, EnvironmentVariableTarget.Machine);
        
        public string ConfirmlogConnectionString => Environment.GetEnvironmentVariable(GeneralConstants.TestConfirmlogConnectionString, EnvironmentVariableTarget.Machine);

        public string ConfirmAdminConnectionString(string projectId)
        {
            throw new NotImplementedException();
        }

        public string GetConnectionStringForSpecificCompany(int companyId)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(CatiDefaultConnectionString);
            connectionStringBuilder.InitialCatalog = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);

            return connectionStringBuilder.ConnectionString;
        }

        public int GetRandomCatiSqlServerId()
        {
            return 0;
        }

        public string GetMasterConnectionStringForServer(int sqlServerId)
        {
            return CatiDefaultConnectionString;
        }
    }
}
