using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Reports.Interfaces;
using Confirmit.CATI.Core.Timezones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Reports
{
    public class InterviewerSessionsReportQuery : IInterviewerSessionsReportQuery
    {
        private readonly IRemoteDataCopier _remoteDataCopier;

        public InterviewerSessionsReportQuery(IRemoteDataCopier remoteDataCopier)
        {
            _remoteDataCopier = remoteDataCopier;
        }

        public List<InterviewerSessionsReportEntity> Execute(InterviewerSessionsReportParams parameters, out int totalCount)
        {
            var confirmlogConnectionString = BackendInstance.Current.ConfirmlogConnectionString;
            List<InterviewerSessionsReportEntity> list;

            var searchCondition = GetSearchCondition(parameters);
            var personIds = ReportManager.ConvertArrayToStringParameter(parameters.Persons);

            var catiPersonTableName = "#CatiPerson";
            GenerateBaseQueryToCatiInterviewerSessionHistoryTable(parameters, catiPersonTableName, personIds, searchCondition, out var remoteQuery, out var countQuery);

            string catiPersonSessionHistoryTableName = "#CatiPersonSessionHistory";
            using (var connectionScope = new ConnectionScope())
            {
                int loginTotalCount = 0;
                int catiPersonSessionHistoryCount = 0;
                if (parameters.EventType != 0)
                {
                    using (var confirmitConnectionProvider = new RemoteConnectionProvider(confirmlogConnectionString))
                    using (var sqlCommand = new SqlCommand(countQuery, confirmitConnectionProvider.Connection))
                    {
                        var copyBvPersonQuery = $"SELECT SID, Name FROM BvPerson WHERE SID IN({personIds})";

                        _remoteDataCopier.CopyDataToNewTable(connectionScope, confirmitConnectionProvider, catiPersonTableName, copyBvPersonQuery);

                        loginTotalCount = (int)sqlCommand.ExecuteScalar();

                        _remoteDataCopier.CopyDataToNewTable(confirmitConnectionProvider, connectionScope, catiPersonSessionHistoryTableName, remoteQuery);
                    }

                    var databaseEngine = new DatabaseEngine();
                    catiPersonSessionHistoryCount = databaseEngine.ExecuteScalar<int>($"SELECT COUNT(*) FROM {catiPersonSessionHistoryTableName}");
                }

                string query = GenerateQueryToCatiDatabase(personIds, parameters.EventType, catiPersonSessionHistoryTableName);

                var reader = BvSpGetObjectsPageAdapter.ExecuteReader(
                    parameters.PagingArgs.PageIndex,
                    parameters.PagingArgs.PageSize,
                    parameters.PagingArgs.SortField,
                    parameters.PagingArgs.SortOrderAsc,
                    query,
                    parameters.PagingArgs.SortField,
                    searchCondition,
                    null,
                    out totalCount);

                list = ReadList(reader);

                list.ForEach(x =>
                {
                    x.StartTime = TimezoneManager.ConvertToTzLocalTime(parameters.TimezoneId, x.StartTime.GetValueOrDefault());
                    if (x.FinishTime != null)
                    {
                        x.FinishTime = TimezoneManager.ConvertToTzLocalTime(parameters.TimezoneId, x.FinishTime.GetValueOrDefault());
                    }
                });

                if (loginTotalCount > 0)
                {
                    totalCount = totalCount + loginTotalCount - catiPersonSessionHistoryCount;
                }
            }

            return list;
        }

        private string GetSearchCondition(InterviewerSessionsReportParams parameters)
        {
            string searchCondition = SearchManager.GetSqlCondition(parameters.PagingArgs.SearchParameters, parameters.TimezoneId);
            if (string.IsNullOrEmpty(searchCondition))
            {
                searchCondition = "1=1";
            }

            return searchCondition;
        }

        private void GenerateBaseQueryToCatiInterviewerSessionHistoryTable(
           InterviewerSessionsReportParams parameters, string catiPersonTableName, string personIds, string searchCondition, out string remoteQuery, out string countQuery)
        {
            string personIdsToKeep = null;
            if (!string.IsNullOrEmpty(personIds))
            {
                personIdsToKeep = $" WHERE SID NOT IN({personIds})";
            }

            var orderDirection = parameters.PagingArgs.SortOrderAsc ? " ASC " : " DESC ";
            var orderClause = " ORDER BY " + parameters.PagingArgs.SortField + orderDirection;

            var top = parameters.PagingArgs.PageIndex * parameters.PagingArgs.PageSize;

            remoteQuery = $@" SELECT 
                    Name AS PersonName,
	                LoginTime AS StartTime, 
	                LogoutTime AS FinishTime, 
	                DATEDIFF(SECOND, LoginTime, LogoutTime) AS Duration,
                    1 as Event
                FROM CatiInterviewerSessionHistory
                INNER JOIN {catiPersonTableName} person ON person.SID = CatiInterviewerSessionHistory.InterviewerId
	            WHERE CatiInterviewerSessionHistory.CallCenterId = {parameters.CallCenterId}
                    AND CatiInterviewerSessionHistory.CompanyId = {parameters.CompanyId}";

            countQuery = $"SELECT COUNT(*) FROM ({remoteQuery}) t WHERE {searchCondition} ";
            remoteQuery = $"SELECT TOP({top}) * FROM ({remoteQuery}) t WHERE {searchCondition}{orderClause}";
        }

        private string GenerateQueryToCatiDatabase(string personIds, int eventType, string catiPersonSessionHistoryTableName)
        {
            var loginQuery = $"SELECT PersonName, StartTime, FinishTime, Duration, Event, NULL as Note FROM {catiPersonSessionHistoryTableName}";

            if (personIds == null)
                personIds = "";

            var breakQuery = $@"SELECT BvPerson.Name as PersonName,
	           StartTime,
		       DATEADD(second, Duration, StartTime) as FinishTime,
		       Duration,
		       0 as Event,
		       bt.Name as Note
	        FROM BvTimeBreaksHistory
	        INNER JOIN dbo.utilSplitNumbers('{personIds}', ',') s1 ON s1.Item = InterviewerId
	        INNER JOIN BvPerson ON SID = InterviewerId
	        LEFT JOIN bvBreakType bt on bt.Id = BvTimeBreaksHistory.BreakTypeId";

            switch (eventType)
            {
                case -1:
                    return breakQuery + " UNION ALL " + loginQuery;
                case 0:
                    return breakQuery;
                case 1:
                    return loginQuery;
                default:
                    return "";
            }
        }

        private List<InterviewerSessionsReportEntity> ReadList([NotNull] IDataReader rd)
        {
            var interviewerSessionsReportEntityList = new List<InterviewerSessionsReportEntity>();

            int personName = rd.GetOrdinal("PersonName");
            int startTime = rd.GetOrdinal("StartTime");
            int finishTime = rd.GetOrdinal("FinishTime");
            int duration = rd.GetOrdinal("Duration");
            int @event = rd.GetOrdinal("Event");
            int note = rd.GetOrdinal("Note");

            while (true)
            {
                bool isRead = rd.Read();

                if (isRead == false)
                    break;

                var entity = new InterviewerSessionsReportEntity();

                if (!rd.IsDBNull(personName))
                    entity.PersonName = rd.GetString(personName);

                if (!rd.IsDBNull(startTime))
                    entity.StartTime = rd.GetDateTime(startTime);

                if (!rd.IsDBNull(finishTime))
                    entity.FinishTime = rd.GetDateTime(finishTime);

                if (!rd.IsDBNull(duration))
                    entity.Duration = rd.GetInt32(duration);

                if (!rd.IsDBNull(@event))
                    entity.Event = rd.GetInt32(@event);

                if (!rd.IsDBNull(note))
                    entity.Note = rd.GetString(note);

                interviewerSessionsReportEntityList.Add(entity);
            }

            return interviewerSessionsReportEntityList;
        }

    }
}
