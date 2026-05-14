using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Misc
{
    public class ConnectionStrings : IConnectionStrings
    {
        private readonly IDbLibProvider _dbLibProvider;

        private string _defaultInstanceConnectionString;
        private string _masterConnectionString;
        private string _confirmConnectionString;
        private string _confirmlogConnectionString;

        public ConnectionStrings(IDbLibProvider dbLibProvider)
        {
            _dbLibProvider = dbLibProvider;
        }

        private string FormatConnectionStringToTheSpecificOrDefaultDatabase(string connectionString, string initialCatalog)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            if (!string.IsNullOrEmpty(initialCatalog))
            {
                connectionStringBuilder.InitialCatalog = initialCatalog;
            }

            return connectionStringBuilder.ConnectionString;
        }

        public string MasterConnectionString 
        {
            get
            {
                if (_masterConnectionString == null)
                {
                    _masterConnectionString = FormatConnectionStringToTheSpecificOrDefaultDatabase(_dbLibProvider.CatiDefaultConnectionString, "master");
                }

                return _masterConnectionString;
            }
        }

        public string ConfirmlogConnectionString 
        {
            get
            {
                if (_confirmlogConnectionString == null)
                { 
                    _confirmlogConnectionString = _dbLibProvider.ConfirmlogConnectionString;
                }

                return _confirmlogConnectionString;
            }
        }

        public string ConfirmConnectionString 
        {
            get
            {
                if (_confirmConnectionString == null)
                {
                    _confirmConnectionString = _dbLibProvider.ConfirmConnectionString;
                }

                return _confirmConnectionString;
            }
        }

        public string DefaultInstanceConnectionString 
        {
            get
            {
                if (_defaultInstanceConnectionString == null)
                {
                    _defaultInstanceConnectionString = FormatConnectionStringToTheSpecificOrDefaultDatabase(_dbLibProvider.CatiDefaultConnectionString, null);
                }

                return _defaultInstanceConnectionString;
            }
        }

        public string GetConnectionStringForSpecificCompany(int companyId)
        {
            if (companyId == 0)
            {
                return DefaultInstanceConnectionString;
            }
            return _dbLibProvider.GetConnectionStringForSpecificCompany(companyId);
        }

        public string GetMasterConnectionStringForSpecificServer(int serverId)
        {
            return _dbLibProvider.GetMasterConnectionStringForServer(serverId);
        }

        public string GetMasterConnectionStringForSpecificCompanyServer(int companyId)
        {
            var connectionString = GetConnectionStringForSpecificCompany(companyId);
            return FormatConnectionStringToTheSpecificOrDefaultDatabase(connectionString, "master");
        }
    }
}