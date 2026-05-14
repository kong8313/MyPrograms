using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public class ServerConnectionFactory
    {
        public static ServerConnection Create(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return new ServerConnection(builder.DataSource, builder.UserID, builder.Password)
            {
                DatabaseName = builder.InitialCatalog
            };
        }
    }
}