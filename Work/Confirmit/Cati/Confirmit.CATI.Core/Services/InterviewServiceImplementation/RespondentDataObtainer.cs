using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using System;
using System.Data;
using System.Linq;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SurveyDataService;

namespace Confirmit.CATI.Core.Services.InterviewServiceImplementation
{
    /// <summary>
    /// Contains methods to obtain sample (respondent) data from survey database.
    /// </summary>
    public class RespondentDataObtainer : IRespondentObtainer, IRespondentBatchObtainer
    {
        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly IRemoteDataCopier _remoteDataCopier;
        private readonly IConnectionStrings _connectionStrings;
        private readonly ICompanyInfo _companyInfo;

        public RespondentDataObtainer(
            ISurveyConnectionStringProvider surveyConnectionStringProvider,
            IRemoteDataCopier remoteDataCopier,
            IConnectionStrings connectionStrings,
            ICompanyInfo companyInfo)
        {
            _surveyConnectionStringProvider = surveyConnectionStringProvider;
            _remoteDataCopier = remoteDataCopier;
            _connectionStrings = connectionStrings;
            _companyInfo = companyInfo;
        }

        public RespondentRecord[] GetRespondentBatchPartition(BvSurveyEntity survey, int batchId, int startRangeOfInterviewId, int partitionSize, bool isSampleUpdate)
        {
            string whereClause;

            if (isSampleUpdate)
            {
                whereClause = String.Format("r.BatchID <> r.UpdateBatchID AND r.UpdateBatchID = {0} AND respId >= {1}", batchId, startRangeOfInterviewId);
            }
            else
            {
                whereClause = String.Format("r.BatchID = {0} AND respId >= {1}", batchId, startRangeOfInterviewId);
            }

            return GetRespondents(survey, whereClause, null, partitionSize, isSampleUpdate);
        }

        public RespondentRecord[] GetRespondentsForSynchronization(BvSurveyEntity survey, int partitionSize)
        {
            var whereClause = $"i.SurveySID = '{survey.SID}'";
            return GetLeftRespondents(survey, whereClause, null, null, partitionSize);
        }

        public RespondentRecord[] GetLeftRespondents(BvSurveyEntity survey, string whereClause, int? timeout, int? topCount, int partitionSize)
        {
            var companyId = _companyInfo.CompanyId;
            var companyConnectionString = _connectionStrings.GetConnectionStringForSpecificCompany(companyId);
            var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(survey.SID);
            var surveyDbConnectionString = surveyConnectionInfo.ConnectionString;

            var where = string.IsNullOrEmpty(whereClause) ? string.Empty : "WHERE " + whereClause;

            var copyDataQuery = $@"SELECT  
                                    i.ID AS RespondentId
                                    FROM BvInterview AS i {where}";

            string tempTableName = "#tempRespId";

            using (var surveyConnectionScope = new RemoteConnectionProvider(surveyDbConnectionString))
            {
                _remoteDataCopier.CopyDataToNewTable(companyConnectionString, surveyConnectionScope, tempTableName, copyDataQuery);
                var whereResp = $"r.respid NOT IN (SELECT RespondentId FROM [{tempTableName}])";
                return GetRespondents(survey, whereResp, null, partitionSize, false, surveyConnectionScope);
            }
        }

        public RespondentRecord GetRespondent(BvSurveyEntity survey, int respId)
        {
            var whereClause = String.Format("r.respId = {0}", respId);

            return GetRespondents(survey, whereClause, null, null, false).First();
        }

        /// <summary>
        /// Determines whether the specified column exists in the specified table of the database.
        /// TODO: This function should NOT be in that class at all, it is too generic
        /// </summary>
        /// <param name="surveyConnectionInfo">Connection info to survey database</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="columnName">Name of the column</param>
        /// <returns>
        /// <c>true</c> if the specified column exists; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsColumnExists(SurveyConnectionInfo surveyConnectionInfo, string tableName, string columnName)
        {
            string sql = $@"SELECT COUNT(*) FROM INFORMATION_SCHEMA.Columns 
                            WHERE [TABLE_SCHEMA] = '{surveyConnectionInfo.SchemaName.Trim('[', ']')}' and [TABLE_NAME] = '{tableName}' and [COLUMN_NAME] = '{columnName}'";

            int columnsCount = new DatabaseEngine(surveyConnectionInfo.ConnectionString).ExecuteScalarInNewConnection<int>(sql, CommandType.Text);

            return columnsCount == 1;
        }
        public static string generateSqlField(bool isFieldExists, string fieldName, string type, string defaultValue)
        {
            return isFieldExists 
                ? $@"
                ISNULL(TRY_CAST({fieldName} AS {type}), CAST({defaultValue} AS {type}))"
                : $@"
                CAST({defaultValue} AS {type})";
        }
        private RespondentRecord[] GetRespondents(BvSurveyEntity survey, string whereClause, int? timeout, int? topCount, bool isSampleUpdate, RemoteConnectionProvider remoteConnectionScope = null)
        {
            var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(survey.SID);
            var surveyDbConnectionString = surveyConnectionInfo.ConnectionString;

            bool isCatiCallTimeColumnExists = IsColumnExists(surveyConnectionInfo, "respondent", "CatiCallTime");
            bool isCatiCallPriorityColumnExists = IsColumnExists(surveyConnectionInfo, "respondent", "CatiCallPriority");
            bool isCatiShiftTypeColumnExists = IsColumnExists(surveyConnectionInfo, "respondent", "CatiShiftType");
            bool isCatiCallStateColumnExists = IsColumnExists(surveyConnectionInfo, "respondent", "CatiCallState");
            bool isCatiCallExpirationTimeColumnExists = IsColumnExists(surveyConnectionInfo, "respondent", "CatiCallExpirationTime");
            bool isCatiExtendedStatusColumnExists = IsColumnExists(surveyConnectionInfo, "respondent", "CatiExtendedStatus");
            bool isDialModeColumnExists = IsColumnExists(surveyConnectionInfo, "respondent", "DialMode");
            bool isCatiAssignmentsExists = IsColumnExists(surveyConnectionInfo, "respondent", "CatiAssignments");
            bool isDialTypeExists = IsColumnExists(surveyConnectionInfo, "respondent", "DialType");


            string telephoneInBlackListQuery = survey.IsTelephoneBlacklistSupported
                ? @"CASE WHEN EXISTS( 
                            SELECT 1 FROM [dbo].BvFnBlacklist_IsTelephoneNumberFiltered( [dbo].RemoveNonNumericCharacters(data.RespondentPhone) ) WHERE IsFiltered > 0 
                         )
                         THEN 1 ELSE 0 END"
                : "0";

            string cellIdSubQuery = "0";
            if (!string.IsNullOrEmpty(survey.ClusteredQuotaName))
            {
                Bv_ClusterQuotaService_GetCellIdQueryAdapter.ExecuteNonQuery(survey.SID, survey.ClusteredQuotaName, "repl", out cellIdSubQuery);
            }

            var catiCallTime = generateSqlField(isCatiCallTimeColumnExists, "r.CatiCallTime", "VARCHAR(120)", "''");
            var catiCallPriority = generateSqlField(isCatiCallPriorityColumnExists, "r.CatiCallPriority", "NVARCHAR(256)", "''");
            var catiShiftType = generateSqlField(isCatiShiftTypeColumnExists, "r.CatiShiftType", "NVARCHAR(256)", "''");
            var catiCallState = generateSqlField(isCatiCallStateColumnExists, "r.CatiCallState", "NVARCHAR(256)", "''");
            var catiCallExpirationTime = generateSqlField(isCatiCallExpirationTimeColumnExists, "r.CatiCallExpirationTime", "VARCHAR(120)", "''");
            var catiExtendedStatus = generateSqlField(isCatiExtendedStatusColumnExists, "r.CatiExtendedStatus", "NVARCHAR(MAX)", "''");
            var dialMode = generateSqlField(isDialModeColumnExists, "CASE WHEN r.DialMode IN(2, 5) THEN r.DialMode ELSE 0 END", "INT", "0");
            var resourceIds = generateSqlField(isCatiAssignmentsExists, "r.CatiAssignments", "NVARCHAR(256)", "NULL");
            var dialType = generateSqlField(isDialTypeExists, "CASE WHEN r.DialType IN (1,2) THEN r.DialType ELSE 0 END", "TINYINT", "0");


            var where = string.IsNullOrEmpty(whereClause) ? string.Empty : "WHERE " + whereClause;
            var topClause = topCount == null ? string.Empty : $"TOP({topCount})";
            var orderClause = topCount == null ? string.Empty : "ORDER BY r.respid";

            var copyDataQuery = $@"SELECT {topClause} r.sid AS Sid,
                                    r.respid AS InterviewId,
                                    ISNULL( r.RespondentName, '') AS RespondentName,
                                    ISNULL( r.TelephoneNumber, '') AS RespondentPhone,
                                    r.LastInterviewStart AS LastCallTime, 
                                    ISNULL(r.TotalDuration, 0) AS TotalDuration, 
                                    ISNULL(r.ExtensionNumber, '') AS ExtensionNumber, 
                                    ISNULL(r.CallAttemptCount, 0) AS DialAttempts, 
                                    ISNULL(r.TimeZoneId, 0) AS TimeZoneId, 
                                    CAST(ISNULL(r.LastChannelId, 0) AS TINYINT) AS LastChannelId,
                                    ISNULL(r.CatiInterviewerID, 0) AS Resource,
                                    {catiCallTime} AS CatiCallTime,
                                    {catiCallPriority} AS CatiCallPriority,
                                    {catiShiftType} AS CatiShiftType,
                                    {catiCallState} AS CatiCallState,
                                    {catiCallExpirationTime} AS CatiCallExpirationTime,
                                    {catiExtendedStatus} AS CatiExtendedStatus,
                                    {dialMode} AS DialMode,
                                    {resourceIds} AS ResourceIds,
                                    {dialType} AS DialType
                                FROM <Schema>.respondent AS r {where} {orderClause}";

            DataTable dataTable;
            using (var connectionScope = new ConnectionScope())
            {
                string tableName = "#RemoteData";
                var createTempTableQuery = $@"CREATE TABLE [{tableName}]
                    (
                        Sid                 VARCHAR(64),
                        InterviewId         INT,
                        RespondentName      NVARCHAR(256),
                        RespondentPhone     NVARCHAR(256),
                        LastCallTime        DATETIME,
                        TotalDuration       INT,
                        ExtensionNumber     NVARCHAR(256),
                        DialAttempts        INT,
                        TimeZoneId          INT,
                        LastChannelId       TINYINT,
                        Resource            INT,
                        CatiCallTime        NVARCHAR(256),
                        CatiCallPriority    NVARCHAR(256),
                        CatiShiftType       NVARCHAR(256),
                        CatiCallState       NVARCHAR(256),
                        CatiCallExpirationTime  NVARCHAR(256),
                        CatiExtendedStatus  NVARCHAR(256),
                        DialMode            INT,
                        ResourceIds         NVARCHAR(256),
                        DialType            TINYINT
                    )";

                var databaseEngine = new DatabaseEngine();
                databaseEngine.ExecuteNonQuery(createTempTableQuery);

                using (var surveyConnectionScope = remoteConnectionScope ?? new RemoteConnectionProvider(surveyDbConnectionString))
                {
                    _remoteDataCopier.CopyDataToExistTable(surveyConnectionScope, connectionScope, tableName,
                        copyDataQuery, surveyConnectionInfo.SchemaName);
                }

                var transientState = isSampleUpdate ? "i.TransientState" : "NULL";
                var join = isSampleUpdate ? "INNER JOIN BvInterview i ON data.InterviewId = i.Id AND i.SurveySID = " + survey.SID : "";

                var isClosedSQL = $@"
                CAST( 
                    CASE 
                        when 
                            (SELECT DISTINCT 1 from 
                            BvInterviewQuotaCell AS icell INNER JOIN
                            BvSurveyQuotaCell AS qcell
                            ON qcell.SurveyID = {survey.SID} AND qcell.QuotaID = icell.QuotaID  and qcell.CellID = icell.CellID AND qcell.IsOpen = 0
                            WHERE Icell.SurveyID = {survey.SID} AND icell.InterviewId = data.InterviewId)
                        IS NOT NULL
                        then 1 
                        else 0
                    end
                AS BIT)";

                var sql = $@"SELECT data.*, {isClosedSQL} AS IsClosedCell,
                            {cellIdSubQuery} AS ClusteredCellId,
                            {telephoneInBlackListQuery} as IsTelephoneInBlackList,
                            {transientState} AS TransientState
                        FROM [{tableName}] data
                        {join}
                        LEFT JOIN {survey.DestinationTableName} repl ON data.InterviewId = repl.respid
                        ORDER BY data.InterviewId";

                dataTable = databaseEngine.ExecuteDataTable<DataTable>(sql, CommandType.Text, timeout ?? Constants.DefaultDatabaseCommandTimeout);
            }

            return (from entity in dataTable.Select()
                    select new RespondentRecord
                    {
                        Sid = entity["Sid"] as string,
                        InterviewId = (int)entity["InterviewId"],
                        RespondentName = entity["RespondentName"] as string,
                        RespondentPhone = entity["RespondentPhone"] as string,
                        LastCallTime = entity["LastCallTime"] as DateTime?,
                        TotalDuration = (int)entity["TotalDuration"],
                        ExtensionNumber = entity["ExtensionNumber"] as string,
                        DialAttempts = (int)entity["DialAttempts"],
                        TimeZoneId = (int)entity["TimeZoneId"],
                        LastChannelId = (byte)entity["LastChannelId"],
                        Resource = (int)entity["Resource"],
                        CatiCallTime = entity["CatiCallTime"] as string,
                        CatiCallPriority = entity["CatiCallPriority"] as string,
                        CatiShiftType = entity["CatiShiftType"] as string,
                        CatiCallState = entity["CatiCallState"] as string,
                        CatiCallExpirationTime = entity["CatiCallExpirationTime"] as string,
                        CatiExtendedStatus = entity["CatiExtendedStatus"] as string,
                        IsClosedCell = (bool)entity["IsClosedCell"],
                        IsTelephoneInBlackList = Convert.ToBoolean(entity["IsTelephoneInBlackList"]),
                        DialMode = (int)entity["DialMode"],
                        ClusteredCellId = (int)entity["ClusteredCellId"],
                        ResourceIds = entity["ResourceIds"] as string,
                        DialTypeId = entity["DialType"] == DBNull.Value ? (byte)0 : (byte)entity["DialType"],
                        TransientState = entity["TransientState"] == DBNull.Value ? 0 : (int)entity["TransientState"]
                    }).ToArray();
        }
    }
}
