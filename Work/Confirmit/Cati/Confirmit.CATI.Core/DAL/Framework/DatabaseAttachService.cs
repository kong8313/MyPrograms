using Confirmit.CATI.Core.Misc;
using Confirmit.DataServices.RDataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public class DatabaseAttachService : IDatabaseAttachService
    {
        private readonly IDbLibProvider _dbLibProvider;
        public DatabaseAttachService(IDbLibProvider dbLibProvider)
        {
            _dbLibProvider = dbLibProvider;
        }

        public bool IsSurveyDatabaseAttached(string projectId)
        {
            var databaseName = "survey_" + projectId;
            using (var connection = new SqlConnection(_dbLibProvider.ConfirmAdminConnectionString(projectId)))
            {
                var query = "SELECT Active FROM SurveyDatabases WHERE Name = @DbName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@DbName", databaseName));
                    connection.Open();
                    return (bool)command.ExecuteScalar();
                }
            }
        }

        public void AttachSurveyDatabase(string projectId)
        {
            var databaseName = "survey_" + projectId;

            using (var connection = new SqlConnection(_dbLibProvider.ConfirmAdminConnectionString(projectId)))
            {
                SqlConnectionUtils.ForceEnsureAttached(connection, databaseName);
            }
        }

        public void DetachSurveyDatabase(string projectId)
        {
            var databaseName = "survey_" + projectId;
            using (var connection = new SqlConnection(_dbLibProvider.ConfirmAdminConnectionString(projectId)))
            {
                using (var command = new SqlCommand("usp_DeactivateSurveyDatabase", connection))
                {
                    command.Parameters.Add(new SqlParameter("@SurveyDatabaseName", databaseName));
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
