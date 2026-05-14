using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Services.CompanyService
{
    public class CompanyInformationService : ICompanyInformationService
    {
        private const int CatiCallCentersAddonId = 49;
        private readonly IConnectionStrings _connectionStrings;

        public CompanyInformationService(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        /// <summary>
        /// The method is used during BackendInstance.Current initialisation, so, 
        /// it cannot access BackendInstance.Current.ConfirmlogConnectionString.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public string GetCompanyNameFromCompanyId(int companyId)
        {
            using (var connection = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT [Name] FROM [dbo].[company] WHERE [companyid] = @CompanyId";
                command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int) { Value = companyId });
                command.CommandType = CommandType.Text;

                return command.ExecuteScalar()  as string;
            }
        }

        [NotNull]
        public string GetCompanyAliasFromCompanyId(int companyId)
        {
            using (var connection = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT [CatiCompanyIdentifier] [companyid] FROM [dbo].[company] WHERE [companyid] = @CompanyId";
                command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int) { Value = companyId });
                command.CommandType = CommandType.Text;

                return command.ExecuteScalar() as string;
            }
        }

        public int GetCompanyIdFromAlias(string companyAlias)
        {
            using (var connection = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT [companyid] FROM [dbo].[company] WHERE CatiCompanyIdentifier = @companyAlias";
                command.Parameters.Add(new SqlParameter("@companyAlias", SqlDbType.NVarChar) { Value = companyAlias });
                command.CommandType = CommandType.Text;

                var companyId = command.ExecuteScalar();

                if (companyId == null)
                {
                    return 0;
                }

                return (int) companyId;
            }
        }

        public bool HasCompanyCallCentersAddon(int companyId)
        {
            using (var connection = new SqlConnection(_connectionStrings.ConfirmConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1 FROM [dbo].[addon_customer] WHERE [addon_companyid] = @CompanyId AND [addon_id] = @AddonId";
                command.Parameters.Add(new SqlParameter("@CompanyId", SqlDbType.Int) { Value = companyId });
                command.Parameters.Add(new SqlParameter("@AddonId", SqlDbType.Int) { Value = CatiCallCentersAddonId });
                command.CommandType = CommandType.Text;

                var result = command.ExecuteScalar();
                return result != null;
            }
        }

        public int GetMaxIvrAgentsForCurrentCompany()
        {
            using (var connection = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT [MaxIvrAgents] FROM [dbo].[company] WHERE companyid = @companyid";
                command.Parameters.Add(new SqlParameter("@companyid", SqlDbType.Int) { Value = BackendInstance.Current.CompanyId });
                command.CommandType = CommandType.Text;

                var maxIvrAgents = command.ExecuteScalar();

                if (maxIvrAgents == null)
                {
                    return 0;
                }

                return (int)maxIvrAgents;
            }
        }
        
        public void SetCatiSqlServerId(int companyId, int? sqlServerId)
        {
            var sql = "UPDATE company SET CatiDatabaseServerId = @SqlServerId WHERE companyid = @CompanyId";
            using (var conn = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddRange(new SqlParameter[] {
                        new SqlParameter("@CompanyId", companyId),
                        new SqlParameter("@SqlServerId", sqlServerId ?? (object)DBNull.Value)
                    });

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<int> GetChildCompanyIds(int parentCompanyId)
        {
            var ids = new List<int>();
            var sql = "SELECT [companyid] FROM [company] WHERE [ParentCatiCompanyId] = @CompanyId";
            using (var conn = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddRange(new[] { new SqlParameter("@CompanyId", parentCompanyId) });

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var id = (int)reader["companyid"];
                        ids.Add(id);
                    }
                }
            }

            return ids;
        }
    }
}