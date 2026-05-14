using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces.Database;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.Database
{
    public class DatabaseNameService : IDatabaseNameService
    {
        private readonly IConnectionStrings _connectionStrings;

        public DatabaseNameService(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public string GetLinkToConfirmlogDatabase()
        {
            var dbName = new SqlConnectionStringBuilder(_connectionStrings.ConfirmlogConnectionString).InitialCatalog;
            
            return string.Format("[{0}]", dbName);
        }
    }
}
