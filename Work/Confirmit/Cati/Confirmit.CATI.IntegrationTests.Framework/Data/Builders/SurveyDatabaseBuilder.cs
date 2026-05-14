using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.SqlServer.Management.Smo;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class SurveyDatabaseBuilder : ISurveyDatabaseBuilder
    {
        public DatabaseEngine Db { get; private set; }
        public FormData[] Forms { get; private set; }
        public string ProjectId { get; private set; }
        private Dictionary<string, FormData> _fieldToForm = new Dictionary<string, FormData>();

        public SurveyDatabaseBuilder(string connectionString, FormData[] forms = null, string projectId = null)
        {
            Init(new DatabaseEngine(connectionString), forms, projectId);
        }
       
        public SurveyDatabaseBuilder(DatabaseEngine db, FormData[] forms = null, string projectId = null)
        {
            Init(db, forms, projectId);
        }

        private void Init(DatabaseEngine db, FormData[] forms, string projectId)
        {
            Db = db;
            Forms = forms;
            ProjectId = projectId;
            if (forms != null)
                _fieldToForm = MapFieldsToForms(forms);
        }

        private static Dictionary<string, FormData> MapFieldsToForms(IEnumerable<FormData> forms)
        {
            return forms.SelectMany(x =>
            {
                if (x is MultiFormData)
                    return ((MultiFormData)x).Precodes.Select(y => new { Field = x.Name + "_" + y, Form = x }).ToArray();
                return new[] { new { Field = x.Name, Form = x } };
            }).ToDictionary(y => y.Field, z => z.Form);
        }

        public string ConnectionString { get { return Db.ConnectionString; } }
        
        private static readonly object _createDbObject = new object();

        public static SurveyDatabaseBuilder Create(string projectId, FormData[] forms)
        {
            var framework = IntegrationTestingFramework.Instance;
            var databaseName = "survey_" + projectId;

            var master = framework.GetConfirmitSqlServerConnectionString("master");
            var databaseTools = new DatabaseTools(master);
            
            lock (_createDbObject)
            {
	            if (databaseTools.IsDatabaseExists(databaseName))
	            {
		            databaseTools.DropDatabase(databaseName);
	            }
	            
	            databaseTools.CreateEmptyDatabase(databaseName);
            }

            return new SurveyDatabaseBuilder(framework.GetConfirmitSqlServerConnectionString(databaseName), forms, projectId);
        }

        public void CreateQuotaTables()
        {
            // We use plain SQL instead of SMO to improve performance.
            const string sqlQuotas = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='quotas')
                    CREATE TABLE [dbo].[quotas]([quotaid] [int], [quotaname] [nvarchar](max), [tablename] [nvarchar](max), [iscati] [int] NOT NULL CONSTRAINT DF_quotas_iscati DEFAULT(0), [is_optimistic] [bit] NOT NULL CONSTRAINT DF_quotas_is_optimistic DEFAULT(0))";

            const string sqlQuotaField = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='quota_field')
                    CREATE TABLE [dbo].[quota_field]([quotaid] [int],[fieldname] [nvarchar](max))";

            const string sqlResponseControl = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='response_control')
                    CREATE TABLE [dbo].[response_control]([ITS] [int],[respid] [int], [responseId] [int] IDENTITY(1, 1) NOT NULL)";

            Db.ExecuteNonQueryWithSpecificTimeOut(sqlQuotas, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            Db.ExecuteNonQueryWithSpecificTimeOut(sqlQuotaField, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            Db.ExecuteNonQueryWithSpecificTimeOut(sqlResponseControl, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
        }

        public void CreateRespondentTable(IEnumerable<FormData> forms = null)
        {
            var processedFormDatas = ProcessRespondentTableFormsData(forms);

            string sql = @"CREATE TABLE [dbo].[respondent](
	[respid] [int] IDENTITY(1,1) NOT NULL,
	[sid] [varchar](64) NULL,
	[last_handled] [datetime] NULL,
	[callback] [datetime] NULL,
	[interviewerid] [varchar](50) NULL,
	[tries] [int] NULL,
	[never_again] [varchar](50) NULL,
	[in_use] [smallint] NULL,
	[not_in_quota] [smallint] NULL,
	[iterationid] [int] NULL,
	[order] [float] NULL,
	[comment] [varchar](255) NULL,
	[sample_category] [varchar](20) NULL,
	[noOfEmailsSent] [int] NULL,
	[quotas_updated_flag] [int] NULL,
	[userid] [nvarchar](64) NULL,
	[LastUpdated] [datetime] NULL,
	[smtpstatus] [varchar](32) NULL,
	[smtpcode] [varchar](12) NULL,
	[smtpStatusDate] [datetime] NULL,
	[smtpTaskId] [int] NULL,
	[CapiInterviewerId] [int] NULL,
	[rowguid] [uniqueidentifier] ROWGUIDCOL  NULL,
	[TelephoneNumber] [nvarchar](255) NULL,
	[ExtensionNumber] [nvarchar](255) NULL,
	[LastInterviewStart] [datetime] NULL,
	[LastChannelId] [int] NULL,
	[TimeZoneId] [int] NULL,
	[RespondentName] [nvarchar](255) NULL,
	[DialStatus] [int] NULL,
	[DialMode] [int] NULL,
	[CallAttemptCount] [int] NULL,
	[TotalAttempts] [int] NULL,
	[TotalDuration] [int] NULL,
	[CatiInterviewerId] [int] NULL,
	[username] [nvarchar](255) NULL,
	[OptOut] [smallint] NULL,
	[OptOutDate] [datetime] NULL,
	[batchId] [int] NULL,
    [UpdateBatchId] [int] NULL,
	[email] [nvarchar](255) NULL,
    [CatiExtendedStatus] INT NULL,
    [CatiCallTime] [nvarchar](255) NULL,
    [CatiCallExpirationTime] [nvarchar](255) NULL,
    [DialType] [int] NULL,
    [CatiCallPriority] [nvarchar](255) NULL,
    [CatiShiftType] [nvarchar](255) NULL,
    [CatiCallState] [nvarchar](255) NULL"
    + (processedFormDatas != null && processedFormDatas.Any() ? ", " + StringService.Join(", ", x => String.Format("[{0}] {1}", x.Name, x.SqlType), processedFormDatas) : "") +
 @", CONSTRAINT [pk_respondent] PRIMARY KEY CLUSTERED 
(
	[respid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]";

            Db.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
        }

        private List<FormData> ProcessRespondentTableFormsData(IEnumerable<FormData> formDatas)
        {
            var exceptNames = new[] { "CallAttemptCount", "DialMode", "TimeZoneId","TelephoneNumber", "RespondentName", "ExtensionNumber", "DialType", };

            if (formDatas == null)
            {
                return null;
            }

            return (from data in formDatas
                        let valid = exceptNames.All(exceptName => !data.Name.Equals(exceptName, StringComparison.InvariantCultureIgnoreCase))
                        where valid
                    select data).ToList();
        }

        public void CreateResponseTable(string tableName, IEnumerable<FormData> forms)
        {
            var fieldToForm = MapFieldsToForms(forms);

            string sql = @"CREATE TABLE [dbo].[" + tableName + @"](
	[responseid] [int] NOT NULL, [respid] [int] NULL, " + StringService.Join(", ", (x) => String.Format("[{0}] {1}", x.Key, x.Value.SqlType), fieldToForm) + @",
 CONSTRAINT [pk_response1] PRIMARY KEY CLUSTERED 
(
	[responseid] ASC
))
select name, column_id from sys.columns WHERE object_id IN ( object_id('" + tableName + @"'))
";

            
            using (var reader = Db.ExecuteReaderInNewConnection(sql, CommandType.Text))
            {
                while (reader.Read())
                {
                    var name = (string)reader["name"];
                    var columnId = (int)reader["column_id"];
                    var form = forms.SingleOrDefault(x => x.Name == name);
                    if (form != null) form.ColumnId = columnId;
                }
            }
        }

        private const string QuotaTableFormat = "quota_{0}";

        public void CreateQuota(QuotaData quota, FormData[] formData)
        {
            //create quota
            string quotaTable = String.Format(QuotaTableFormat, quota.Id);
            Db.ExecuteNonQueryWithSpecificTimeOut("INSERT INTO quotas VALUES(@quotaid, @quotaname, @tablename, 1, @is_optimistic)",
                CommandType.Text,
                Settings.Default.DefaultConnectionTimeout,
                new SqlParameter("@quotaid", quota.Id),
                new SqlParameter("@quotaname", quota.Name),
                new SqlParameter("@tablename", quotaTable),
                new SqlParameter("@is_optimistic", quota.IsOptimistic));

            var forms = quota.Fields.Select(x => formData.Single(f => f.Name == x)).ToArray();

            foreach (var form in forms)
            {
                Db.ExecuteNonQueryWithSpecificTimeOut("INSERT INTO quota_field VALUES(@quotaid, @fieldname)",
                    CommandType.Text,
                    Settings.Default.DefaultConnectionTimeout,
                    new SqlParameter("@quotaid", quota.Id),
                    new SqlParameter("@fieldname", form.Name));
            }

            Db.CreateTable(quotaTable,
                new[]
                    {
                        new KeyValuePair<string, DataType>("quotaid", DataType.Int),
                        new KeyValuePair<string, DataType>("counter", DataType.Int),
                        new KeyValuePair<string, DataType>("limit", DataType.Int),
                        new KeyValuePair<string, DataType>("send_email_flag", DataType.VarChar(1)),
                        new KeyValuePair<string, DataType>("email_sent_flag", DataType.VarChar(1)),
                        new KeyValuePair<string, DataType>("live_counter", DataType.Int),
                        new KeyValuePair<string, DataType>("live_limit", DataType.Int),
                        new KeyValuePair<string, DataType>("disabled", DataType.Bit),
                        new KeyValuePair<string, DataType>("balancing_priority", DataType.Int)
                    }.Union(forms.Select(x => new KeyValuePair<string, DataType>(x.Name, DataType.NVarChar(32)))).ToArray());

            foreach (var cell in quota.Cells)
            {
                string query = "INSERT INTO " + quotaTable +
                                   " VALUES(@quotaid, @counter, @limit, 'Y', 'N', 0, @limit, @disabled, @priority";
                var parameters = new List<SqlParameter>
                    {
                        new SqlParameter("@quotaid", cell.Id),
                        new SqlParameter("@limit", cell.Limit),
                        new SqlParameter("@counter", cell.Counter),
						new SqlParameter("@disabled", cell.IsDisabled),
                        new SqlParameter("@priority", (int)cell.Priority)
                    };

                    foreach ( var valueItem in cell.Values.Split(','))
                    {
                        var parts = valueItem.Split('=');
                        var name = parts[0];
                        var value = parts[1] == "" ? null : parts[1];
                        query += ", @" + name;
                        parameters.Add(new SqlParameter("@" + name, value));
                    }

                    query += ")";

                    Db.ExecuteNonQuery(query,
                        CommandType.Text,
                        parameters.ToArray());
            }
        }

        /// <summary>
        /// Create only respondent in survey database
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="catiExtendedStatus"></param>
        /// <param name="interview"></param>
        /// <returns></returns>
        public int CreateRespondent(int batchId, string catiExtendedStatus, InterviewData interview)
        {
            const string query = "INSERT INTO respondent" +
                "(sid,            RespondentName,   TelephoneNumber,    LastInterviewStart, TotalDuration, " +
                "ExtensionNumber, CallAttemptCount, TimeZoneId,         LastChannelId,      CatiInterviewerID, " +
                "BatchID,         UpdateBatchId,    CatiExtendedStatus, interviewerid,      rowguid, " +
                "DialMode,        CatiCallTime,  CatiCallExpirationTime,    DialType,	" +
                "CatiCallPriority,	CatiShiftType,	CatiCallState ) VALUES" +
                "('{0}', '{1}', {2}, null, 0, " +
                "'{3}', {4}, {5}, {6}, 0, " +
                "{7}, {7}, {8}, {9}, '{10}', " + 
                "{11}, {12}, {13}, {14}," +
                "{15}, {16}, {17}) SELECT @@IDENTITY";

            return (int)Db.ExecuteScalar<decimal>(string.Format(query,
                string.IsNullOrEmpty(interview.Sid) ? "0" : interview.Sid,
                string.IsNullOrEmpty(interview.RespondentName) ? "respName" : interview.RespondentName,
                interview.TelephoneNumber == null ? "NULL" : String.Format( "'{0}'", interview.TelephoneNumber),

                string.IsNullOrEmpty(interview.ExtensionNumber) ? "0" : interview.ExtensionNumber,
                string.IsNullOrEmpty(interview.CallAttemptCount) ? "0" : interview.CallAttemptCount,
                string.IsNullOrEmpty(interview.TimeZoneId) ? "0" : interview.TimeZoneId,
                string.IsNullOrEmpty(interview.LastChannelId) ? "0" : interview.LastChannelId,

                batchId,
                string.IsNullOrEmpty(catiExtendedStatus) ? "NULL" : "'" + catiExtendedStatus + "'",
                string.IsNullOrEmpty(interview.InterviewerId) ? "NULL" : "'" + interview.InterviewerId + "'",
                Guid.NewGuid(),

                string.IsNullOrEmpty(interview.DialMode) ? "NULL" : interview.DialMode,
                string.IsNullOrEmpty(interview.CatiCallTime) ? "NULL" : "'" + interview.CatiCallTime + "'",
                string.IsNullOrEmpty(interview.CatiCallExpirationTime) ? "NULL" : "'" + interview.CatiCallExpirationTime + "'",
                (int)interview.DialType,
                
                string.IsNullOrEmpty(interview.CatiCallPriority) ? "NULL" : "'" + interview.CatiCallPriority + "'",
                string.IsNullOrEmpty(interview.CatiShiftType) ? "NULL" : "'" + interview.CatiShiftType + "'",
                string.IsNullOrEmpty(interview.CatiCallState) ? "NULL" : "'" + interview.CatiCallState + "'"), CommandType.Text);
        }

        public void DeleteRespondent(int respId)
        {
            const string query = "DELETE FROM respondent WHERE respid = {0}; ";

            Db.ExecuteNonQuery(string.Format(query,
                respId), CommandType.Text);
        }

        public void SetRespondentTableColumnValue(int[] respondentIds, string column, string value)
        {
            string query = $"UPDATE respondent SET {column} = {value} WHERE respId= {{0}}";

            foreach (var respId in respondentIds)
            {
                Db.ExecuteNonQueryWithSpecificTimeOut(String.Format(query, respId), CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            }
        }

        public void SetInterviewData(int respid, string data)
        {
            if (data == null)
                return;

            var tables = data.Split(',').Select(x =>
            {
                var parts = x.Split('=');
                var form = _fieldToForm[parts[0]];
                return new Tuple<FormData, string, string>(form, parts[0], parts[1]);
            }).GroupBy(f => f.Item1.TableName);

            foreach (var table in tables)
            {
                if (table.Key == "respondent")
                {
                    string query = String.Format("UPDATE respondent SET {0} WHERE respid = {1}",
                        StringService.Join(",", x => String.Format("[{0}]={1}", x.Item1.Name, ToSqlParam(x.Item3)), table),
                        respid);
                    Db.ExecuteNonQuery(query, CommandType.Text);
                }
                else
                {
                    string query = String.Format( ";merge {0} as target " +
                           "USING (SELECT {1})" +
                           "AS source (respid) ON target.respid=source.respid " +
                           "WHEN NOT MATCHED THEN " +
                           "   INSERT(responseid, respid, {2}) VALUES(source.respid, source.respid, {3}) " +
                           "WHEN MATCHED THEN " +
                           "   UPDATE SET {4};", 
                                table.Key,
                                respid,
                                StringService.Join(",", "[{0}]", table.Select(x=> x.Item2)),
                                StringService.Join(",", "{0}", table.Select(x => ToSqlParam(x.Item3))),
                                StringService.Join(",", x => String.Format("[{0}]={1}", x.Item2, ToSqlParam(x.Item3) ), table));
                    Db.ExecuteNonQuery(query, CommandType.Text);
                }
            }
        }

        public string GetInterviewData(int respid, string requestedColumns)
        {
            if (requestedColumns == null)
                return null;
            var result = string.Empty;

            var tables = requestedColumns.Split(',').Select(x =>
            {
                var form = _fieldToForm[x];
                return new Tuple<string, string>(form.TableName, x);
            }).GroupBy(f => f.Item1);

            foreach (var table in tables)
            {
                var requestedTableColumns = table.Select(x => x.Item2).ToArray();
                string query = string.Format("SELECT {0} FROM [{1}] WHERE respid = @respId", StringService.Join(",", "{0}", requestedTableColumns), table.Key);
                using (var reader = Db.ExecuteReaderInNewConnection(query, CommandType.Text, new SqlParameter("@respId", respid)))
                {
                    while (reader.Read())
                    {
                        result += string.Join(",", requestedTableColumns.Select(c => c + "=" + (reader[c] != DBNull.Value ? reader[c] : "NULL")));
                    }
                }
            }

            return result;
        }

        private string ToSqlParam(string value)
        {
            return String.IsNullOrEmpty(value) ? "NULL" : String.Format("'{0}'", value);
        }

        public void ClearQuotaTables()
        {
            Db.ExecuteScalarList<string>("DELETE FROM quota_field  DELETE FROM quotas OUTPUT deleted.tableName", CommandType.Text)
                .ForEach(x => Db.DropTable(x));
        }

        public void CloseCell(int quotaId, int cellId)
        {
            string quotaTable = String.Format(QuotaTableFormat, quotaId);

            Db.ExecuteNonQuery(String.Format(
                "update {0} set counter = limit where quotaid = {1}",
                quotaTable,
                cellId
                ), CommandType.Text);
        }

        public void CloseCellOptimistically(int quotaId, int cellId)
        {
            string quotaTable = String.Format(QuotaTableFormat, quotaId);

            Db.ExecuteNonQuery(String.Format(
                "update {0} set counter = limit - 1, live_counter = 1, live_limit = limit where quotaid = {1}",
                quotaTable,
                cellId
                ), CommandType.Text);
        }

        public void OpenCell(int quotaId, int cellId)
        {
            string quotaTable = String.Format(QuotaTableFormat, quotaId);

            Db.ExecuteNonQuery(String.Format(
                "update {0} set counter = 0 where quotaid = {1}",
                quotaTable,
                cellId
                ), CommandType.Text);
        }

        public void SetBatchId(int respId, int batchId, int updateBatchId)
        {
            Db.ExecuteNonQuery(String.Format(
                "update respondent set batchId = {0}, updateBatchId = {1} where respid = {2}",
                batchId,
                updateBatchId,
                respId
                ), CommandType.Text);
        }

        public int AddInterview(int batchId, string catiExtendedStatus, InterviewData interview)
        {
            int respId = CreateRespondent(batchId, catiExtendedStatus, interview);

            SetInterviewData(respId, interview.Data);

            return respId;
        }

        public int GetNewBatchId()
        {
	        var lastSampleRecord = BvSamplesAdapter.GetByCondition("1=1 ORDER BY BatchId DESC").FirstOrDefault();
	        return (lastSampleRecord != null ? lastSampleRecord.BatchID + 1 : 1);
        }

        public void CreateFormAndFieldTable(FormData[] forms)
        {
            const string sql = @"CREATE TABLE [dbo].[form](
	[formid] [int] NOT NULL,
	[type] [varchar](50) NULL,
	[poetid] [int] NULL,
	[formname] [varchar](50) NULL,
	[loopid] [int] NULL,
	[precodemask] [nvarchar](255) NULL,
	[scaleprecodemask] [nvarchar](255) NULL,
	[columnmask] [nvarchar](255) NULL,
	[validation_code] [nvarchar](max) NULL,
	[fieldwidth] [int] NULL,
	[rows] [int] NULL,
	[cols] [int] NULL,
	[listrows] [int] NULL,
	[listcols] [int] NULL,
	[precision] [int] NULL,
	[scale] [int] NULL,
	[lowerlimit] [float] NULL,
	[upperlimit] [float] NULL,
	[ext_lookup_id] [int] NULL,
	[ext_lookup_type] [int] NULL,
	[answerlist_order] [int] NULL,
	[scale_order] [int] NULL,
	[flag_dropdown] [int] NULL,
	[flag_randomize] [int] NULL,
	[flag_backgroundvar] [int] NULL,
	[flag_panelvar] [int] NULL,
	[flag_quotavar] [int] NULL,
	[flag_ordered] [int] NULL,
	[flag_hiddenvar] [int] NULL,
	[flag_notrequired] [int] NULL,
	[flag_open] [int] NULL,
	[flag_password] [int] NULL,
	[flag_numeric] [int] NULL,
	[flag_lowerinclusive] [int] NULL,
	[flag_upperinclusive] [int] NULL,
	[flag_autocheckother] [int] NULL,
	[flag_autosum] [int] NULL,
	[flag_rankedorder] [int] NULL,
	[flag_gridslider] [int] NULL,
	[answerimagemodus] [int] NULL,
	[answerimagedefault] [varchar](255) NULL,
	[answerimageselect] [varchar](255) NULL,
	[answerimageselected] [varchar](255) NULL,
	[answerimagewidth] [varchar](6) NULL,
	[answerimageheight] [varchar](6) NULL,
	[scrollcontrolmodus] [int] NULL,
	[scrollcontrolsize] [int] NULL,
	[barred] [int] NULL,
	[open_coding_fieldname] [varchar](50) NULL,
	[open_coding_user] [varchar](50) NULL,
	[created] [datetime] NULL DEFAULT (getdate()),
	[loopstate_tableid] [int] NULL,
	[flag_singleslider] [int] NULL,
	[slidebarcolor] [varchar](16) NULL,
	[startposition_num] [int] NULL,
	[generateprecodes_flg] [int] NULL,
	[has_weights] [int] NULL,
	[flag_custom] [int] NULL,
	[parent3dgrid] [varchar](50) NULL,
	[benchmark_type] [int] NULL,
	[flag_disabled] [int] NULL,
	[flag_isbitstreamvariable] [int] NULL,
	[flag_isdate] [int] NULL,
	[dataWriteAccessLevel] [int] NULL DEFAULT ((0)),
	[isBoolean] [int] NULL DEFAULT ((0)),
	[characteristic] [int] NULL DEFAULT ((0)),
	[question_category] [varchar](64) NULL,
	[hasloopreference] [bit] NULL,
 CONSTRAINT [pk_form] PRIMARY KEY CLUSTERED 
(
	[formid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE TABLE [dbo].[field](
	[fieldid] [int] NOT NULL,
	[parentid] [int] NULL,
	[fieldname] [varchar](50) NULL,
	[tableid] [int] NULL,
	[keeppos] [int] NULL,
	[other] [int] NULL,
	[sqltype] [int] NULL,
	[listsource] [nvarchar](255) NULL,
	[loopreferenceid] [int] NULL,
 CONSTRAINT [pk_field] PRIMARY KEY CLUSTERED 
(
	[fieldid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

CREATE TABLE [dbo].[loop_hierarchy](
	[fieldname] [varchar](50) NOT NULL,
	[nesting_level] [int] NOT NULL,
	[nesting_fieldname] [varchar](50) NOT NULL,
 CONSTRAINT [PK_loop_hierarchy] PRIMARY KEY NONCLUSTERED 
(
	[fieldname] ASC,
	[nesting_level] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

    INSERT INTO loop_hierarchy( fieldname, nesting_level, nesting_fieldname ) VALUES( 'responseid', 0, 'responseid' )
";

            Db.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            int formId = 0;
            foreach (var form in forms)
            {
                var match = new Regex(@"^response(?<Id>[0-9]+)$").Match(form.TableName);
                
                if (!match.Success)
                    continue;

                formId++;

                var tableId = match.Groups["Id"].Value;
                var query = @"INSERT INTO form(formid, formname) VALUES(@FormId, @FormName)";
                if (form is MultiFormData)
                {
                    var precodes = ((MultiFormData) form).Precodes;
                    foreach (var category in precodes)
                    {
                        query += string.Format("\r\nINSERT INTO field(fieldid, parentid, fieldname, tableid) VALUES({0}, @FormId, '{1}', @TableId)", formId + Array.IndexOf(precodes, category), form.Name + "_" + category);
                    }
                }
                else
                {
                    query += "\r\nINSERT INTO field(fieldid, parentid, fieldname, tableid) VALUES(@FormId, @FormId, @FormName, @TableId)";
                }

                Db.ExecuteNonQueryWithSpecificTimeOut(
                    query, 
                    CommandType.Text, 
                    Settings.Default.DefaultConnectionTimeout,
                    new SqlParameter("@FormId", formId), 
                    new SqlParameter("@FormName", form.Name), 
                    new SqlParameter("@TableId", tableId));
            }
        }

        public void EnableChangeTracking(TableInfo[] tableInfo)
        {
            BackendTools.EnableChangeTracking(Db, tableInfo);
        }
    }
}
