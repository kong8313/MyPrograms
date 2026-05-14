using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.Services.Survey.Quota;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    public class ReplicatedDataRepository : IReplicatedDataRepository
    {
        private readonly IQuotaDatabaseReader _quotaDatabaseReader;

        public ReplicatedDataRepository(IQuotaDatabaseReader quotaDatabaseReader)
        {
            _quotaDatabaseReader = quotaDatabaseReader;
        }

        public IDataReader ExecuteReplicatedDataReader(int surveyId)
        {
            var tableName = ReplicationSchemaService.GetDestinationTableName(surveyId);
            var databaseEngine = new DatabaseEngine();

            var query = $"SELECT {GetFieldsToFetch(surveyId)} FROM {tableName}";

            return databaseEngine.ExecuteReaderInNewConnection(query, CommandType.Text);
        }

        public DataTable GetInterviewsData(int surveyId, List<int> interviewsIds)
        {
            var tableName = ReplicationSchemaService.GetDestinationTableName(surveyId);
            var databaseEngine = new DatabaseEngine();

            var query = $"SELECT {GetFieldsToFetch(surveyId)} FROM {tableName} WHERE EXISTS(SELECT 1 FROM @ids where Value = respid)";

            return databaseEngine.ExecuteDataTable<DataTable>(query, CommandType.Text,
                 new SqlParameter("@surveyId", surveyId),
                 BvIntArrayTypeAdapter.CreateSqlParameter("@ids", interviewsIds));
        }

        private string GetFieldsToFetch(int surveyId)
        {
            var fields = _quotaDatabaseReader.GetAllFields(surveyId).ToList();
            fields.Add("respid");
            return String.Join(", ", fields.Select(x => $"[{x}]"));
        }
        
        public IDictionary<string, string> GetReplicationValues(int surveyId, int interviewId)
        {
            var tableName = ReplicationSchemaService.GetDestinationTableName(surveyId);
            var query = $"SELECT * FROM {tableName} WHERE respid = @InterviewId";

            var databaseEngine = new DatabaseEngine();
            var dataTable = databaseEngine.ExecuteDataTable<DataTable>(query, CommandType.Text,
                new SqlParameter("@InterviewId", interviewId));

            var result = new Dictionary<string, string>();
            
            if (dataTable.Rows.Count == 0)
            {
                return result;
            }

            foreach (DataColumn column in dataTable.Columns)
            {
                var value = dataTable.Rows[0][column.ColumnName];
                if (value is DBNull)
                {
                    result[column.ColumnName] = null;
                }
                else
                {
                    result[column.ColumnName] = value.ToString();
                }
            }

            return result;
        }
    }
}
