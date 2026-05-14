using Confirmit.Configuration;
using Confirmit.Databases;
using Confirmit.DataServices.RDataAccess;
using Microsoft.Data.SqlClient;

namespace LoadTestUpdateActiveQuestionSpCaller;

public class SqlConnectionProvider
{
    public SqlConnection GetConnection()
    {
        var configuration = ConfigurationProvider.GetConfiguration();
        ConfigSettings.CatiSqlServerName = configuration.CatiSqlServerName;
        var companyId = configuration.CatiCompanyId;
        
        if (companyId == 0)
        {
            throw new ArgumentException("Company id should not be zero.");
        }

        return new SqlConnection(DbLib.GetCatiConnectInfo(companyId).GetConnectString());
    }
}