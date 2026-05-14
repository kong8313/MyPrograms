using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Misc;
using Telerik.Reporting;
using System.Data.SqlTypes;
using Confirmit.CATI.Core.SystemSettings;


namespace Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport
{
    public class InterviewerProductivityReportDataProvider
    {
        private readonly StandardColumnsProvider _standardColumnsProvider;
        private readonly IConnectionStrings _connectionStrings;
        private readonly ICompanyInfo _companyInfo;
        private readonly ISystemSettings _systemSettings;

        public InterviewerProductivityReportDataProvider()
        {
            _standardColumnsProvider = new StandardColumnsProvider();
            _companyInfo = ServiceLocator.Resolve<ICompanyInfo>();
            _systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            _connectionStrings = ServiceLocator.Resolve<IConnectionStrings>();
        }

        private DataTable GenerateTableWithItses(InterviewerProductivityReportTemplate template)
        {
            var columnsWithStatuses = template.Columns.Where(x => x is ProductivityReportTemplateColumnWithStatuses)
                .Cast<ProductivityReportTemplateColumnWithStatuses>()
                .ToList();

            var defaultStateGroupId = StateGroupRepository.GetDefault().ID;
            var states = StateRepository.GetAll(defaultStateGroupId);

            DataTable itsTable = new DataTable("ItsList");
            itsTable.Columns.Add("StateId");
            int columnWithStatusesIndex = 0;
            for (int i = 0; i < columnsWithStatuses.Count; i++)
            {
                if (_standardColumnsProvider.IsStandardColumn(columnsWithStatuses[i].StandardColumnName))
                {
                    itsTable.Columns.Add(columnsWithStatuses[i].StandardColumnName);
                }
                else
                {
                    itsTable.Columns.Add("its" + columnWithStatusesIndex);
                    columnWithStatusesIndex++;
                }
            }

            foreach (var state in states)
            {
                var stateID = state.StateID;

                DataRow row = itsTable.NewRow();
                row[0] = stateID;
                for (int i = 0; i < columnsWithStatuses.Count; i++)
                {
                    var columnsWithStatus = columnsWithStatuses[i];

                    bool isContains = columnsWithStatus.ExtendedStatuses.Contains(stateID);
                    if (columnsWithStatus.IsIncludeStatuses)
                        row[i + 1] = isContains ? 1 : 0;
                    else
                        row[i + 1] = isContains ? 0 : 1;
                }

                itsTable.Rows.Add(row);
            }

            return itsTable;
        }

        private string GenerateCreateTableQueryPart(DataTable itsTable)
        {
            var columnDefinition = StringService.Join(",", "{0} int", itsTable.Columns.Cast<System.Data.DataColumn>().Select(x => x.ColumnName));
            var createTableSb = new StringBuilder($"create table #ItsList ({columnDefinition})");

            string columnsByComma = StringService.Join(",", "{0}", itsTable.Columns.Cast<System.Data.DataColumn>().Select(x => x.ColumnName));
            createTableSb.Append($"insert into #ItsList ({columnsByComma}) values ");

            foreach (DataRow row in itsTable.Rows)
            {
                createTableSb.Append($"({string.Join(",", row.ItemArray)}),");
            }

            return createTableSb.ToString().TrimEnd(',');
        }

        private string GenerateSelectQueryPart(DataTable itsTable)
        {
            var selectString = StringService.Join(
                ",",
                "ISNULL(SUM(itl.{0}), 0) as {0}",
                itsTable.Columns.Cast<System.Data.DataColumn>().Where(x => x.ColumnName != "StateId").Select(x => x.ColumnName));

            return "  ," + selectString;
        }

        public DataTable GetData(InterviewerProductivityReportTemplate template, ReportParameterCollection reportParameters, out bool hasRecords)
        {
            var dbEngine = new DatabaseEngine();

            DataTable itsTable = GenerateTableWithItses(template);
            string createItsTableSqlPart = GenerateCreateTableQueryPart(itsTable);
            string selectItsSqlPath = GenerateSelectQueryPart(itsTable);

            string sql = $@"
 DECLARE @DiallerName NVARCHAR(20)
 SET  @DiallerName = N'Dialer';

 CREATE TABLE #Persons(
     PersonSid int primary key, 
     Name NVARCHAR (255), 
     FullName NVARCHAR (255), 
     Attribute1 NVARCHAR (50), 
     Attribute2 NVARCHAR (50), 
     Attribute3 NVARCHAR (50), 
     Attribute4 NVARCHAR (50), 
     Attribute5 NVARCHAR (50), 
     DurationPaid int, 
     DurationUnpaid int)
 INSERT INTO #Persons
 SELECT p.SID AS PersonSid,
        p.Name AS Name,
        p.FullName AS FullName,
        p.Attribute1,
        p.Attribute2,
        p.Attribute3,
        p.Attribute4,
        p.Attribute5,
		NULL,
		NULL
 FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
 INNER JOIN BvPerson p ON p.SID = s.Item
 UNION 
 SELECT p.Sid AS PersonSid,
        p.Name AS Name,
        p.FullName AS FullName,
        p.Attribute1,
        p.Attribute2,
        p.Attribute3,
        p.Attribute4,
        p.Attribute5,
		NULL,
		NULL
 FROM BvPerson p
 WHERE @PersonSids IS NULL AND (p.CallCenterID = @CallCenterId OR @CallCenterId IS NULL)
 UNION
 SELECT DialerSid AS PersonSid,
        @DiallerName AS Name,
        @DiallerName AS FullName,
        NULL,
        NULL,
        NULL,
        NULL,
        NULL,
		NULL,
		NULL
 FROM (SELECT 0 AS DialerSid) dailerSids
 WHERE @UseDialer = 1

 create table #SurveyIdsList(SurveyId  int primary key)
 insert into #SurveyIdsList
 SELECT Item AS SurveyId 
 FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',')

 IF ( @CalcAllBreakHistory = 1 )
	INSERT INTO #SurveyIdsList VALUES(0)

DECLARE @diff TIME

IF CAST(@StartShiftTime AS TIME) > CAST(@EndShiftTime AS TIME)
	select @diff = CAST('00:00:00' - (@StartShiftTime-@EndShiftTime) AS TIME )
else
	select @diff = CAST(@EndShiftTime-@StartShiftTime AS TIME)


 ;WITH TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(ISNULL(
	   	    	CASE WHEN CAST( '00:00:00' - (DATEADD ( SECOND, Duration, StartTime ) - @EndShiftTime)  AS TIME )  < @diff
				THEN
					Duration 
				ELSE
				     DATEDIFF(SECOND,  CAST(StartTime AS TIME), CAST ( @EndShiftTime AS TIME) )
			END, Duration)
	), 0) Duration, 
	InterviewerId,
	ISNULL(bt.IsPaid, 1) as IsPaid
    FROM BvTimeBreaksHistory h
	LEFT JOIN #SurveyIdsList s
	ON h.SurveyId = s.SurveyId
	LEFT JOIN BvBreakType bt on bt.Id = h.BreakTypeId
    WHERE StartTime BETWEEN @StartDateTime AND @EndDateTime AND ( s.SurveyId IS NOT NULL )
	      AND (@StartShiftTime IS NULL OR CAST( StartTime - @StartShiftTime  AS TIME ) <= @diff)
    GROUP BY InterviewerId, IsPaid
 ),
 AggregatedBreaksHistory AS (
	SELECT tbh.InterviewerId,
	DurationPaid = sum(CASE WHEN tbh.IsPaid = 1 THEN tbh.Duration ELSE 0 END),
	DurationUnpaid = sum(CASE WHEN tbh.IsPaid = 0 THEN tbh.Duration ELSE 0 END)
	FROM TimeBreaksHistory tbh
	GROUP BY tbh.InterviewerId
 )
 update #persons
 SET DurationPaid = AggregatedBreaksHistory.DurationPaid,
 DurationUnpaid = AggregatedBreaksHistory.DurationUnpaid
 FROM AggregatedBreaksHistory
 WHERE #persons.PersonSid = AggregatedBreaksHistory.InterviewerId

 CREATE TABLE #respids 
( 
	surveyid INT,
	respid int,
	PRIMARY KEY CLUSTERED 
	(
		[SurveyId] ASC,
		[respid] ASC
	)
)

IF (@SurveyDataFilter IS NOT NULL)
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = N'INSERT INTO #respids SELECT ' +  @SurveySids + ', respid from [dbo].[BvReplicatedData_' + @SurveySids + '] AS CFInterview WHERE ' + @SurveyDataFilter 
	EXEC (@sql)
END

{createItsTableSqlPart}

;WITH FilteredHistory AS 
(
	SELECT 
	Duration,
	ConfirmitDuration,
	WaitingTime,
	OpenEndReviewDuration,
	IIF(@IncludeOpenEndReviewTimeInInterviewDuration = 0, ISNULL(OpenEndReviewDuration, 0), 0) as OpenEndReviewDurationForLogonTime,
    PreviewTime,
    ConnectedTime,
    WrapTime,
    PersonSid,
    FiredTime,
	RoleID,
	ITS,
	SurveyId,
	InterviewId
	FROM BvHistory
	WHERE (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
)
 SELECT
  p.PersonSid AS PersonId,
  p.Name AS PersonName,
  p.FullName AS DisplayName,
  p.Attribute1,
  p.Attribute2,
  p.Attribute3,
  p.Attribute4,
  p.Attribute5,
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(p.DurationPaid, 0) +
  ISNULL(p.DurationUnpaid, 0)) + ISNULL(SUM(h.OpenEndReviewDurationForLogonTime), 0) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(p.DurationPaid, 0) AS OnBreakTimePaid,
  ISNULL(p.DurationUnpaid, 0) AS OnBreakTimeUnpaid,
  ISNULL(AVG(CASE WHEN itl.Completes > 0 THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration,
  ISNULL(SUM(h.OpenEndReviewDuration), 0) AS OpenEndReviewDuration,
  ISNULL(SUM(h.PreviewTime), 0) AS PreviewDuration,
  ISNULL(SUM(h.WrapTime), 0) AS WrapDuration,
  ISNULL(SUM(h.ConnectedTime), 0) AS ConnectedDuration,
  ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) AS InterviewDuration
{selectItsSqlPath}
 FROM #Persons p
 LEFT JOIN FilteredHistory h ON p.PersonSid = h.PersonSid AND
        h.FiredTime >= @StartDateTime AND
        h.FiredTime <= @EndDateTime AND
        h.RoleID = 2 AND --we should not calced calls which were added during sample addition
        h.SurveyId IN (SELECT sil.SurveyId FROM #SurveyIdsList sil)
 LEFT JOIN #ItsList itl ON itl.StateId = h.ITS
 LEFT JOIN #respids i on i.respid = h.InterviewId AND i.surveyid = h.SurveyId
 WHERE i.respid IS NOT NULL OR @SurveyDataFilter IS NULL

 GROUP BY p.PersonSid, p.Name, p.FullName, p.Attribute1, p.Attribute2, p.Attribute3, p.Attribute4, p.Attribute5, p.DurationPaid, p.DurationUnpaid
 HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0 OR p.PersonSid = 0)
";
            var startDate = reportParameters["DbStartDate"].Value ?? SqlDateTime.MinValue;
            var endDate = reportParameters["DbEndDate"].Value ?? SqlDateTime.MaxValue;
            var sqlParameters = new SqlParameter[] {
                CreateSqlParameter("SurveySids", SqlDbType.NVarChar, reportParameters["DbSurveyIds"].Value),
                CreateSqlParameter("PersonSids", SqlDbType.NVarChar, reportParameters["DbPersonIds"].Value),
                CreateSqlParameter("UseDialer", SqlDbType.Bit, reportParameters["DbShowDialerAttempts"].Value),
                CreateSqlParameter("HideEmpty", SqlDbType.Bit, reportParameters["DbHideEmpty"].Value),
                CreateSqlParameter("CalcAllBreakHistory", SqlDbType.Bit, reportParameters["DbCalcAllBreakHistory"].Value),
                CreateSqlParameter("StartDateTime", SqlDbType.DateTime, startDate),
                CreateSqlParameter("EndDateTime", SqlDbType.DateTime, endDate),
                CreateSqlParameter("SurveyDataFilter", SqlDbType.NVarChar, reportParameters["DbSurveyDataFilter"].Value),
                CreateSqlParameter("StartShiftTime", SqlDbType.DateTime, reportParameters["DbStartShiftTime"].Value),
                CreateSqlParameter("EndShiftTime", SqlDbType.DateTime, reportParameters["DbEndShiftTime"].Value),
                CreateSqlParameter("IncludeOpenEndReviewTimeInInterviewDuration", SqlDbType.Bit, _systemSettings.Console.IncludeOpenEndReviewTimeInInterviewDuration),
                CreateSqlParameter("CallCenterId", SqlDbType.Int, reportParameters["DbCallCenterId"].Value)
            };

            var dataTable = dbEngine.ExecuteDataTable<DataTable>(sql, CommandType.Text, sqlParameters);

            if ((string)reportParameters["SurveyNames"].Value == "All")
            {
                var ids = dataTable.Select().Select(x => (int)x["PersonId"]).ToArray();
                var logonData = GetLogOnData(startDate, endDate, ids);
                dataTable.PrimaryKey = new System.Data.DataColumn[] { dataTable.Columns["PersonId"] };
                logonData.PrimaryKey = new System.Data.DataColumn[] { logonData.Columns["PersonId"] };
                dataTable.Merge(logonData, false);

                dataTable.PrimaryKey = null;
                dataTable.Columns["PersonId"].Unique = false;

            }

            hasRecords = dataTable.Rows.Count > 0;
            return dataTable;
        }

        private DataTable GetLogOnData(object startDate, object endDate, int[] personIds)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("CompanyId", _companyInfo.CompanyId),
                CreateSqlParameter("StartDateTime", SqlDbType.DateTime, startDate),
                CreateSqlParameter("EndDateTime", SqlDbType.DateTime, endDate),
                new SqlParameter("PersonIds", String.Join(",", personIds))
            };

            var sql = $@"
                SELECT 
                    [InterviewerId] AS PersonId,
                    SUM(
                        DATEDIFF(
                            SECOND, 
                            IIF([LoginTime] >= @StartDateTime, [LoginTime], @StartDateTime), 
                            IIF(
                                ISNULL([LogoutTime], GETUTCDATE()) <= @EndDateTime, 
                                ISNULL([LogoutTime], GETUTCDATE()), 
                                @EndDateTime
                            )
                        )
                    ) AS LogOnTime
                FROM CatiInterviewerSessionHistory 
                INNER JOIN STRING_SPLIT(@personIds, ',') ON value = [InterviewerId]
                WHERE 
                    (CompanyId = @CompanyId AND
                    LoginTime <= @EndDateTime AND 
                    ISNULL([LogoutTime], GETUTCDATE()) >= @StartDateTime) AND
                    ([LogoutTime] IS NOT NULL OR DATEDIFF(HOUR, LoginTime, GETUTCDATE()) < 12)
                    
                GROUP BY [InterviewerId]";

            using (var conn = new SqlConnection(_connectionStrings.ConfirmlogConnectionString))
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddRange(parameters.ToArray());

                    var dataTable = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dataTable);

                    return dataTable;
                }
            }
        }

        private string GetStatusesForColumn(InterviewerProductivityReportTemplate template, string columnName)
        {
            var column = (ProductivityReportTemplateColumnWithStatuses)template.Columns.FirstOrDefault(x => x.StandardColumnName == columnName);

            if (column == null)
            {
                return string.Empty;
            }

            return string.Join(",", column.ExtendedStatuses);
        }

        private SqlParameter CreateSqlParameter(string name, SqlDbType dbType, object value)
        {
            var sqlParameter = new SqlParameter(name, dbType);
            sqlParameter.Value = value ?? DBNull.Value;
            return sqlParameter;
        }
    }
}