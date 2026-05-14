using System.Data;
using System.Linq;
using System.Data.SqlClient;
using Confirmit.CATI.Core.Misc;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.CleaningService
{
    public class SurveyCleaningConfirmitDataAccess : ISurveyCleaningConfirmitDataAccess
    {
        private readonly IConnectionStrings _connectionStrings;

        public SurveyCleaningConfirmitDataAccess(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public void SetCreators(List<CleaningServiceEmailInfo> emailInfo)
        {
            if (emailInfo.Count == 0)
            {
                return;
            }

            var projectNames = "'" + string.Join("', '", emailInfo.Select(x => x.Name).ToArray()) + "'";

            using (var connection = new SqlConnection(_connectionStrings.ConfirmConnectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $@"
SELECT [projectid], 
(CASE WHEN [firstname] is null and [lastname] is null THEN [username] 
      WHEN [lastname] is null THEN [firstname] 
      WHEN [firstname] is null THEN [lastname] 
      ELSE [firstname] + ' ' + [lastname] END) as fullname
FROM [dbo].[qProjectList]
WHERE [projectid] in ({projectNames})";

                command.CommandType = CommandType.Text;
                using (var reader = command.ExecuteReader())
                {
                    var dataTable = new DataTable();

                    dataTable.Load(reader);

                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        var cleaningServiceEmailInfo = emailInfo.SingleOrDefault(x => x.Name == dataRow["projectid"].ToString());
                        if (cleaningServiceEmailInfo != null)
                        {
                            cleaningServiceEmailInfo.Creator = dataRow["fullname"].ToString();
                        }
                    }
                }
            }
        }
    }
}