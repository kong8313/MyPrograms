using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.ActivityLogging.Authoring
{
    public class SystemActivity : ISystemActivity
    {
        private readonly IConnectionStrings _connectionStrings;

        public SystemActivity(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public void AddSystemActivity(SystemActivityLogItem log)
        {
            using (var connection = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                connection.Open();

                const string sql = @"
INSERT INTO [activity] ([activitytypeid], [applicationid], [performed], [projectid], [test], [userid], [companyid], [description])
VALUES (@ActivityTypeId, 1, getdate(), @ProjectId, 0, @UserId, @CompanyId, @Description)";

                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@ActivityTypeId", (int)log.ActivityType);
                    cmd.Parameters.AddWithValue("@CompanyId", log.CompanyId);
                    cmd.Parameters.Add("@Description", SqlDbType.VarChar, 8000).Value = log.Description;
                    cmd.Parameters.Add("@ProjectId", SqlDbType.VarChar, 50).Value = log.ProjectId;
                    cmd.Parameters.Add("@UserId", SqlDbType.VarChar, 64).Value = log.UserName;
                    
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}