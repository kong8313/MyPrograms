using System;
using System.Data;
using System.Data.SqlClient;

namespace BuildServerRegistrator
{
    public class SqlRegistrator
    {
        private const string ExPropPrefixName = "###TEMPORALLY CREATED SERVER###";

        public void Register(ConfigParameters serverConfigurations, ConfirmQueryExecutor confirmQueryExecutor)
        {
            var serverName = Environment.MachineName;

            string query = "SELECT COUNT(*) FROM [CfgServer] WHERE [ServerName] = @serverName";
            bool needToRegister = confirmQueryExecutor.ExecuteScalar<int>(query, new SqlParameter("@serverName", serverName)) == 0;

            if (!needToRegister)
            {
                return;
            }

            var exPropName = $"{ExPropPrefixName} - {serverName}";
            query = $"EXEC sp_addextendedproperty @exPropName, @serverName";
            confirmQueryExecutor.ExecuteNonQuery(query, new SqlParameter("@serverName", serverName), new SqlParameter("@exPropName", exPropName));

            query = @"
                DECLARE @serverId INT, @roleId INT
                SET @roleId = 1

                IF NOT EXISTS (SELECT null FROM [CfgServer] WHERE [ServerName] = @serverName)
                  INSERT INTO [CfgServer] VALUES (@serverName)
 
                SELECT @serverId = (SELECT [ServerId] FROM [CfgServer] WHERE [ServerName] = @serverName)

                IF NOT EXISTS (SELECT null FROM [CfgServerRole] WHERE [ServerId] = @serverId AND [RoleId] = @roleId)
                  INSERT INTO [CfgServerRole] VALUES (@serverId, @roleId, 1)

                IF EXISTS (SELECT null FROM [CfgServerRole] WHERE [ServerId] = @serverId AND [RoleId] = @roleId AND [Enabled] = 0)
                  UPDATE [CfgServerRole] SET [Enabled] = 1 WHERE [ServerId] = @serverId AND [RoleId] = @roleId";

            confirmQueryExecutor.ExecuteNonQuery(query, new SqlParameter("@serverName", serverName));

            foreach (var cfgConfigValue in serverConfigurations.CfgServerConfigValues)
            {
                query = @"
                    DECLARE @serverId INT
                    SELECT @serverId = (SELECT [ServerId] FROM [CfgServer] WHERE [ServerName] = @serverName)

                    IF NOT EXISTS (SELECT null FROM [CfgServerConfig] WHERE [ServerId] = @serverId AND [ConfigId] = @configId)
                        INSERT INTO [CfgServerConfig] VALUES(@configId, @serverId, @configValue)";

                confirmQueryExecutor.ExecuteNonQuery(
                    query,
                    new SqlParameter("@serverName", serverName),
                    new SqlParameter("@configId", cfgConfigValue.ConfigId),
                    new SqlParameter("@configValue", cfgConfigValue.EncryptedConfigValue));
            }
        }

        public void Unregister(ConfigParameters serverConfigurations, ConfirmQueryExecutor confirmQueryExecutor)
        {
            string query = $@"
                SELECT [name], [value] FROM fn_listextendedproperty(NULL, NULL, NULL, NULL, NULL, NULL, NULL) 
                WHERE [name] like @exPropPrefixName";
            var dataTable = confirmQueryExecutor.ExecuteDataTable<DataTable>(query, new SqlParameter("@exPropPrefixName", ExPropPrefixName + "%"));

            foreach (DataRow row in dataTable.Rows)
            {
                var exPropName = row["name"].ToString();
                var serverName = row["value"].ToString();

                query = $@"
                    DELETE FROM [CfgServer] WHERE [ServerName] = @serverName;
                    EXEC sp_dropextendedproperty @exPropName";
                confirmQueryExecutor.ExecuteNonQuery(query, new SqlParameter("@serverName", serverName), new SqlParameter("@exPropName", exPropName));
            }
        }
    }
}
