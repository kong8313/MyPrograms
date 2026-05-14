using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Core.DAL.Handmade.Adapter.Query
{
    public class CallHistoryDataAdapter
    {
        public IEnumerable<CallHistoryDataEntity> GetForSurvey(int surveyId, DateTime? startTime,
    DateTime? endTime, string[] replicatedVariables, int maxRows)
        {
            var joinConditionForReplication = string.Empty;
            var replicatedColumnsForQuery = string.Empty;

            if (replicatedVariables != null && replicatedVariables.Any())
            {
                var replicatedColumns = ReplicationColumnsRepository.GetBySurveyId(surveyId);
                replicatedColumns = replicatedColumns.Where(x => replicatedVariables.Contains(x.ColumnName, StringComparer.OrdinalIgnoreCase)).ToList();
                var columnNames = replicatedColumns.Select(x => x.ColumnName).ToArray();
                if (columnNames.Any())
                {
                    joinConditionForReplication =
                        string.Format("LEFT JOIN [BvReplicatedData_{0}] [rep] ON [h].[InterviewId] = [rep].[respid]",
                            surveyId);
                    replicatedColumnsForQuery = string.Format(",{0}", string.Join(",", columnNames));
                }
            }

            var query = String.Format(@"
declare @SurveySID int =  {0}

SELECT TOP ({1})
    [h].[ID] AS [Id],
    [h].[FiredTime] AS [FiredTime],
    [s].[Name] AS [ProjectID],
	[s].[Description] AS [Name],
    [h].[InterviewId] AS [InterviewID],
    [h].[PersonSID] AS [InterviewerID],
	(CASE WHEN [p].[SID] IS NOT NULL THEN [p].[Name]
		WHEN [h].[PersonSID] = 0 THEN 'Dialer'
		ELSE NULL END) [InterviewerName],
    [h].[TelephoneNumber] AS [TelephoneNumber], 
    [h].[ITS] AS [ExtendedStatus],
    [h].[Duration] AS [Duration],
    [h].[WaitingTime] AS [WaitingTime],
	[vcc].[Name] AS [CallCenterName],
    [vcc].[ID] AS [CallCenterId]
	{2}

FROM      [BvHistory] [h] 
INNER JOIN [BvSurvey]  [s] ON [h].[SurveyId] = [s].[SID] AND [s].[SID] = @SurveySID
LEFT JOIN [BvCallCenter] [vcc] ON [h].[CallCenterID] = [vcc].ID
LEFT JOIN BvPerson [p] ON [p].SID = [h].[PersonSID]
{3}
WHERE 
        [h].[RoleID] = 2 /*CATI*/ 
    AND [h].[FiredTime] BETWEEN @StartDate AND @EndDate AND
        [h].[InterviewID] IS NOT NULL
          
ORDER BY 
    [h].[SurveyId], [h].[FiredTime]",
                    surveyId,
                    maxRows,
                    replicatedColumnsForQuery,
                    joinConditionForReplication);
            var parameters = new[]
            {
                new SqlParameter("@StartDate", startTime ?? new DateTime(1753, 1, 1, 0, 0, 0)),
                new SqlParameter("@EndDate", endTime ?? new DateTime(9999, 12, 31, 23, 59, 59))
            };

            var result = new DatabaseEngine().ExecuteDataTable<DataTable>(query, CommandType.Text, parameters).Select();

            var dataEntities = result.Select(x => GetCallHistoryDataEntityFromDataRow(x, replicatedVariables)).ToList();

            return dataEntities.ToList();
        }

        private CallHistoryDataEntity GetCallHistoryDataEntityFromDataRow(DataRow x, string[] ids)
        {
            var item = new CallHistoryDataEntity();

            if (x.IsNull("Id") == false)
                item.Id = (int)x["Id"];
            if (x.IsNull("Duration") == false)
                item.Duration = (int)x["Duration"];
            if (x.IsNull("ExtendedStatus") == false)
                item.ExtendedStatus = (short)x["ExtendedStatus"];
            if (x.IsNull("FiredTime") == false)
                item.FiredTime = (DateTime)x["FiredTime"];
            if (x.IsNull("InterviewID") == false)
                item.InterviewID = (int)x["InterviewID"];
            if (x.IsNull("InterviewerID") == false)
                item.InterviewerID = (int)x["InterviewerID"];
            if (x.IsNull("InterviewerName") == false)
                item.InterviewerName = x["InterviewerName"].ToString();
            if (x.IsNull("Name") == false)
                item.Name = x["Name"].ToString();
            if (x.IsNull("ProjectID") == false)
                item.ProjectID = x["ProjectID"].ToString();
            if (x.IsNull("TelephoneNumber") == false)
                item.TelephoneNumber = x["TelephoneNumber"].ToString();
            if (x.IsNull("WaitingTime") == false)
                item.WaitingTime = (int)x["WaitingTime"];
            if (x.IsNull("CallCenterName") == false)
                item.CallCenterName = x["CallCenterName"].ToString(); ;
            if (x.IsNull("CallCenterId") == false)
                item.CallCenterId = (int)x["CallCenterId"];

            item.ReplicatedVariables = new List<string>();

            if (ids == null || ids.Any() == false)
            {
                return item;
            }

            foreach (var id in ids)
            {
                item.ReplicatedVariables.Add(x.Table.Columns.Contains(id) ? x[id].ToString() : string.Empty);
            }

            return item;
        }
    }
}
