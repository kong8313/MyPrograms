using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public class SqlObjectCreator
    {
        private readonly IntegrationTestingFramework _framework;

        public SqlObjectCreator(IntegrationTestingFramework framework)
        {
            _framework = framework;
        }

        /// <summary>
        /// Create survey database for tests with needed empty tables
        /// Note: Don't forget to add a new table to CleanTablesInSurveyDatabase method
        /// </summary>
        /// <param name="testSurveyDatabaseName"></param>
        public void CreateTestSurveyDatabase(string testSurveyDatabaseName)
        {
            var connstr = _framework.GetConfirmitSqlServerConnectionString(testSurveyDatabaseName);
            var master = new SqlConnectionStringBuilder(connstr) { InitialCatalog = "master" }.ToString();
            var databaseTools = new DatabaseTools(master);

            if (databaseTools.IsDatabaseExists(testSurveyDatabaseName))
            {
                databaseTools.DropDatabase(testSurveyDatabaseName);
            }

            databaseTools.CreateEmptyDatabase(testSurveyDatabaseName);

            var databaseEngine = new DatabaseEngine(connstr);

            // General tables
            string sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='loop_hierarchy')
            CREATE TABLE [dbo].[loop_hierarchy](
	            [fieldname] [varchar](50) NOT NULL,
	            [nesting_level] [int] NOT NULL,
	            [nesting_fieldname] [varchar](50) NOT NULL,
                CONSTRAINT [PK_loop_hierarchy] PRIMARY KEY NONCLUSTERED 
            (
	            [fieldname] ASC,
	            [nesting_level] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='modeswitch')
            CREATE TABLE [dbo].[modeswitch](
	            [modeswitchid] [int] IDENTITY(1,1) NOT NULL,
	            [performed] [datetime] NOT NULL,
	            [mode] [varchar](50) NOT NULL,
                CONSTRAINT [PK_modeswitch] PRIMARY KEY CLUSTERED 
            (
	            [modeswitchid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"CREATE TABLE [dbo].[respondent](
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
	            [batchId] [int] NULL,
                [UpdateBatchId] [int] NULL,
	            [email] [nvarchar](255) NULL,
	            [PhoneNumber] [nvarchar](255) NULL,
	            [DialType] [int] NULL,
	            [CatiCallTime] [nvarchar](255) NULL,
                [CatiCallExpirationTime] [nvarchar](255) NULL,
	            [CatiExtendedStatus] [nvarchar](255) NULL,
                [CatiInterviewerID] [int] NULL,
                [username] [nvarchar](255) NULL,
                [OptOut] [smallint] NULL,
                [OptOutDate]  [datetime] NULL,
                [CatiCallPriority] [nvarchar](255) NULL,
                [CatiShiftType] [nvarchar](255) NULL,
                [CatiCallState] [nvarchar](255) NULL,
                CONSTRAINT [pk_respondent] PRIMARY KEY CLUSTERED 
            (
	            [respid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"INSERT INTO [dbo].[respondent] (sid) values (1);
                    DELETE FROM [dbo].[respondent];
                    DBCC CHECKIDENT ('[dbo].[respondent]', RESEED, 0);";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='responsetable_map')
            CREATE TABLE [dbo].[responsetable_map](
	            [tableid] [int] NOT NULL,
	            [variable_flag] [int] NOT NULL,
	            [rootnode_fieldname] [varchar](50) NOT NULL,
	            [view_flag] [int] NULL,
	            [TableType] [int] NULL,
                CONSTRAINT [PK_responsetable_map] PRIMARY KEY NONCLUSTERED 
            (
	            [tableid] ASC,
	            [variable_flag] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='survey')
            CREATE TABLE [dbo].[survey](
	            [name] [nvarchar](50) NULL,
	            [descr] [text] NULL,
	            [companyname] [varchar](255) NULL,
	            [logofile] [varchar](100) NULL,
	            [status] [varchar](50) NULL,
	            [last_compile_dtm] [datetime] NULL,
	            [max_varcalc_responseid] [int] NULL,
	            [barred] [int] NULL,
	            [default_lang_id] [int] NULL,
	            [last_cleared] [datetime] NULL,
	            [QuestionnaireVersion] [int] NULL,
	            [last_touched_respondent] [datetime] NULL,
	            [bitstream_indexes] [varchar](256) NULL,
	            [UseStateBag] [varchar](1) NULL
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='survey_lang')
            CREATE TABLE [dbo].[survey_lang](
	            [lang] [int] NOT NULL,
	            [introduction] [text] NULL,
	            [completion] [text] NULL,
	            [title] [nvarchar](255) NULL,
	            [url_help_text] [nvarchar](255) NULL,
	            [url_help_link] [varchar](2083) NULL,
	            [url_end_text] [nvarchar](255) NULL,
	            [url_end_link] [varchar](2083) NULL,
	            [survey_closed_subject] [nvarchar](255) NULL,
	            [survey_closed_body] [ntext] NULL,
                CONSTRAINT [pk_survey_lang] PRIMARY KEY CLUSTERED 
            (
	            [lang] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='SurveyVersions')
            CREATE TABLE [dbo].[SurveyVersions](
	            [VersionID] [int] NOT NULL,
	            [Schema] [xml] NOT NULL,
	            [Date] [datetime] NOT NULL,
                CONSTRAINT [PK_SurveyVersions] PRIMARY KEY CLUSTERED 
            (
	            [VersionID] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='version')
            CREATE TABLE [dbo].[version](
	            [versionid] [int] NOT NULL,
	            [versiondate] [datetime] NULL,
	            [description] [varchar](255) NULL,
                CONSTRAINT [pk_version] PRIMARY KEY CLUSTERED 
            (
	            [versionid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            // Specific tables
            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='field')
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
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='field_lang')
            CREATE TABLE [dbo].[field_lang](
	            [parentid] [int] NOT NULL,
	            [lang] [int] NOT NULL,
	            [text] [nvarchar](2000) NULL,
	            [shorttext] [nvarchar](255) NULL,
                CONSTRAINT [pk_field_lang] PRIMARY KEY CLUSTERED 
            (
	            [parentid] ASC,
	            [lang] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='form')
            CREATE TABLE [dbo].[form](
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
	            [created] [datetime] NULL,
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
	            [dataWriteAccessLevel] [int] NULL,
	            [isBoolean] [int] NULL,
	            [characteristic] [int] NULL,
	            [question_category] [varchar](64) NULL,
	            [hasloopreference] [bit] NULL,
	            [flag_recoding] [bit] NULL,
	            [recoding_expression] [nvarchar](max) NULL,
	            [recoding_level] [int] NULL,
	            [flag_panelvarVisible] [bit] NULL,
                CONSTRAINT [pk_form] PRIMARY KEY CLUSTERED 
            (
	            [formid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='form_lang')
            CREATE TABLE [dbo].[form_lang](
	            [parentid] [int] NOT NULL,
	            [lang] [int] NOT NULL,
	            [name] [nvarchar](255) NULL,
	            [text] [ntext] NULL,
	            [short] [nvarchar](255) NULL,
	            [comment] [ntext] NULL,
                CONSTRAINT [pk_form_lang] PRIMARY KEY CLUSTERED 
            (
	            [parentid] ASC,
	            [lang] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='lookup')
            CREATE TABLE [dbo].[lookup](
	            [lookupid] [int] NOT NULL,
	            [parentid] [int] NULL,
	            [parent_lookupid] [int] NULL,
	            [in] [varchar](50) NULL,
	            [weight] [varchar](50) NULL,
	            [group] [varchar](50) NULL,
	            [keeppos] [int] NULL,
	            [other] [int] NULL,
	            [punchtype] [varchar](50) NULL,
	            [size] [varchar](50) NULL,
	            [bgcolor] [varchar](50) NULL,
	            [listsource] [nvarchar](255) NULL,
	            [loopreference] [bit] NULL,
	            [recoding_expression] [nvarchar](max) NULL,
                CONSTRAINT [pk_lookup] PRIMARY KEY CLUSTERED 
            (
	            [lookupid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='lookup_lang')
            CREATE TABLE [dbo].[lookup_lang](
	            [parentid] [int] NOT NULL,
	            [lang] [int] NOT NULL,
	            [text] [nvarchar](2000) NULL,
                CONSTRAINT [pk_lookup_lang] PRIMARY KEY CLUSTERED 
            (
	            [parentid] ASC,
	            [lang] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='routing')
            CREATE TABLE [dbo].[routing](
	            [routingid] [int] NOT NULL,
	            [type] [varchar](50) NULL,
	            [formid] [int] NULL,
	            [statement] [nvarchar](max) NULL,
	            [flag_performdelete] [int] NULL,
	            [loopiter] [varchar](32) NULL,
                CONSTRAINT [pk_routing] PRIMARY KEY CLUSTERED 
            (
	            [routingid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='field')
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
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='StateRecord')
            CREATE TABLE [dbo].[StateRecord](
	            [questionid] [varchar](50) NOT NULL,
	            [state] [int] NOT NULL,
                CONSTRAINT [PK_qid_state] PRIMARY KEY CLUSTERED 
            (
	            [questionid] ASC,
	            [state] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='response0')
            CREATE TABLE [dbo].[response0](
	            [responseid] [int] NOT NULL,
	            [respid] [int] NULL,
	            [q1] [int] NULL,
                [q2] [int] NULL,
	            [key] [int] NULL,
                CONSTRAINT [pk_response0] PRIMARY KEY CLUSTERED 
            (
	            [responseid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='response1')
            CREATE TABLE [dbo].[response1](
	            [responseid] [int] NOT NULL,
	            [respid] [int] NULL,
	            [q3] [int] NULL,
	            [q4] [int] NULL,
                CONSTRAINT [PK_response1] PRIMARY KEY CLUSTERED 
            (
	            [responseid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            // Required empty tables
            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='quota_field')
            CREATE TABLE [dbo].[quota_field](
	            [quotaid] [int] NOT NULL,
	            [fieldname] [varchar](50) NOT NULL,
                CONSTRAINT [PK_quota_field] PRIMARY KEY NONCLUSTERED 
            (
	            [quotaid] ASC,
	            [fieldname] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='quotas')
            CREATE TABLE [dbo].[quotas](
	            [quotaid] [int] IDENTITY(1,1) NOT NULL,
	            [poetid] [int] NOT NULL,
	            [quotaname] [varchar](16) NOT NULL,
	            [tablename] [varchar](16) NOT NULL,
	            [email] [varchar](128) NULL,
	            [iscati] [bit] NOT NULL,
                CONSTRAINT [PK_quotas] PRIMARY KEY NONCLUSTERED 
            (
	            [quotaid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            sql = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='response_control')
            CREATE TABLE [dbo].[response_control](
	            [responseid] [int] IDENTITY(1,1) NOT NULL,
	            [respid] [int] NULL,
	            [interviewerid] [varchar](50) NULL,
	            [companyid] [int] NULL,
	            [language] [int] NULL,
	            [iterationid] [int] NULL,
	            [search_start] [datetime] NULL,
	            [interview_start] [datetime] NULL,
	            [interview_end] [datetime] NULL,
	            [search] [int] NULL,
	            [status] [varchar](20) NULL,
	            [state] [int] NULL,
	            [time] [int] NULL,
	            [quotas_updated_flag] [int] NULL,
	            [rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	            [lastcomplete] [datetime] NULL,
	            [loopstate] [varchar](2000) NULL,
	            [callblockstate] [varchar](2000) NULL,
	            [start_page_context] [varchar](2005) NULL,
	            [last_touched] [datetime] NULL,
	            [first_question_on_last_page_displayed] [varchar](50) NULL,
	            [security_key] [varchar](255) NULL,
	            [security_key_expire] [datetime] NULL,
	            [surveypackage_version] [int] NULL,
	            [__channels__] [int] NULL,
	            [its] [int] NULL,
	            [skipstate] [varchar](2000) NULL,
                CONSTRAINT [pk_response_control] PRIMARY KEY CLUSTERED 
            (
	            [responseid] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            EnableChangeTracking(databaseEngine);
        }

        private void EnableChangeTracking(DatabaseEngine databaseEngine)
        {
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(
                @"  ALTER DATABASE " + databaseEngine.DatabaseName +
                @"  set change_tracking=on
                    (
                       change_retention = 1 days ,
                       auto_cleanup = on
                    )",
                CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(
                @" alter table dbo.respondent
                   enable change_tracking
                   WITH (TRACK_COLUMNS_UPDATED = ON)",
            CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(
                @" alter table dbo.response0
                   enable change_tracking
                   WITH (TRACK_COLUMNS_UPDATED = ON)",
                CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(
                @" alter table dbo.response1
                   enable change_tracking
                   WITH (TRACK_COLUMNS_UPDATED = ON)",
                CommandType.Text, Settings.Default.DefaultConnectionTimeout);
        }

        public void CleanTablesInSurveyDatabase(string testSurveyDatabaseName)
        {
            var connstr = _framework.GetConfirmitSqlServerConnectionString(testSurveyDatabaseName);
            var databaseEngine = new DatabaseEngine(connstr);

            string sql =
                @"DELETE FROM [dbo].[loop_hierarchy];
                DELETE FROM [dbo].[modeswitch];
                DELETE FROM [dbo].[respondent];
                DELETE FROM [dbo].[responsetable_map];
                DELETE FROM [dbo].[survey];
                DELETE FROM [dbo].[survey_lang];
                DELETE FROM [dbo].[SurveyVersions];
                DELETE FROM [dbo].[version];
                DELETE FROM [dbo].[field];
                DELETE FROM [dbo].[field_lang];
                DELETE FROM [dbo].[form];
                DELETE FROM [dbo].[form_lang];
                DELETE FROM [dbo].[lookup];
                DELETE FROM [dbo].[lookup_lang];
                DELETE FROM [dbo].[routing];
                DELETE FROM [dbo].[field];
                DELETE FROM [dbo].[StateRecord];
                DELETE FROM [dbo].[response0];
                DELETE FROM [dbo].[response1];
                DELETE FROM [dbo].[quota_field];
                DELETE FROM [dbo].[quotas];
                DELETE FROM [dbo].[response_control];
                DBCC CHECKIDENT ('[dbo].[respondent]', RESEED, 0);
                ";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
        }

        /// <summary>
        /// Create confirmlog database for tests
        /// NOTE: Don't forget to add a cleaning of new tables to ClearConfirmlogDatabase method
        /// </summary>
        public void CreateConfirmlogDatabase(string databaseName)
        {
            var connstr = _framework.GetConfirmitSqlServerConnectionString(databaseName);
            var master = new SqlConnectionStringBuilder(connstr) { InitialCatalog = "master" }.ToString();
            var databaseTools = new DatabaseTools(master);

            if (databaseTools.IsDatabaseExists(databaseName))
            {
                return;
            }

            databaseTools.CreateEmptyDatabase(databaseName);

            var databaseEngine = new DatabaseEngine(connstr);

            const string
                sql1 = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='CatiManagementActivity')
            CREATE TABLE [dbo].[CatiManagementActivity]
            (
                [Id] [int] IDENTITY(1,1) NOT NULL,
                [EventTypeId] [int] NOT NULL,
                [EventTypeName] [nvarchar](255) NULL,
                [ServerName] [varchar](50) NOT NULL,
                [CompanyId] [int] NOT NULL,
                [UserId] [varchar](50) NULL,
                [StartTime] [datetime] NOT NULL,
                [FinishTime] [datetime] NOT NULL,
                [Duration] [int] NOT NULL,
                [ObjectId] [int] NULL,
                [ObjectName] [nvarchar](255) NULL,
                [Details] [xml] NULL
            )";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql1, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            const string sql2 =
                    @"
                IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='CatiInterviewerActivity')
                CREATE TABLE [dbo].[CatiInterviewerActivity](
	                [ID] [int] IDENTITY(1,1) NOT NULL,
	                [EventTypeId] [int] NOT NULL,
	                [EventTypeName] [varchar](64) NOT NULL,
	                [ServerName] [varchar](50) NOT NULL,
	                [CompanyId] [int] NOT NULL,
	                [SurveyId] [int] NULL,
	                [SurveyName] [varchar](255) NULL,
	                [InterviewerSid] [int] NOT NULL,
	                [StartTime] [datetime] NOT NULL,
	                [FinishTime] [datetime] NOT NULL,
	                [Duration] [int] NOT NULL,
	                [PhoneNumber] [varchar](255) NULL,
	                [Details] [xml] NULL,
	                [InterviewId] [int] NULL
                    CONSTRAINT [PK_CatiInterviewerActivity_ID] PRIMARY KEY NONCLUSTERED 
                (
	                [ID] ASC
                ))
            ";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql2, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            const string sql3 =
                @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='CatiEventLog')
            CREATE TABLE [dbo].[CatiEventLog](
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [EventTypeId] [int] NOT NULL,
                    [EventTypeName] [nvarchar](255) NOT NULL,
                    [ServerName] [nvarchar](50) NOT NULL,
                    [CompanyId] [int] NOT NULL,
                    [EventTime] [datetime] NOT NULL,
                    [Text] [nvarchar](MAX) NULL,
                CONSTRAINT [PK_CatiEnevtLog_ID] PRIMARY KEY NONCLUSTERED 
                (
                    [Id] ASC
                ) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql3, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            const string sql4 =
                @"CREATE TABLE [dbo].[company](
	            [companyid] [int] NOT NULL,
	            [Name] [varchar](255) NULL,
	            [CatiCompanyIdentifier] [varchar](255) NULL)";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql4, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            const string sql5 =
                @"CREATE TABLE [dbo].[CatiInterviewerSessionHistory](
	            [SessionId] [int] IDENTITY(1,1) NOT NULL,
	            [CompanyId] [int] NOT NULL,
	            [CallCenterId] [int] NOT NULL,
	            [InterviewerId] [int] NOT NULL,
	            [LoginTime] [datetime] NOT NULL,
	            [LogoutTime] [datetime] NULL,
                CONSTRAINT [PK_CatiInterviewerSessionHistory] PRIMARY KEY CLUSTERED 
                (
	            [SessionId] ASC
                ) ON [PRIMARY]
            ) ON [PRIMARY]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql5, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            const string sql6 =
                @"CREATE TABLE [dbo].[activity](
	                [activityid] [bigint] IDENTITY(3000000000,1) NOT NULL,
	                [activitytypeid] [int] NULL,
	                [applicationid] [int] NULL,
	                [performed] [datetime] NOT NULL,
	                [projectid] [varchar](50) NULL,
	                [test] [tinyint] NOT NULL,
	                [userid] [varchar](254) NULL,
	                [companyid] [int] NULL,
	                [ipaddress] [varchar](50) NULL,
	                [script] [varchar](255) NULL,
	                [description] [varchar](8000) NULL,
	                [outputcount] [int] NULL,
	                [elapsed] [int] NULL,
	                [querystring] [varchar](8000) NULL,
	                [referer] [varchar](8000) NULL,
	                [enduser_user_id] [int] NULL,
	                [custom1] [varchar](8000) NULL,
	                [custom2] [varchar](8000) NULL,
	                [custom3] [varchar](8000) NULL,
	                [custom4] [int] NULL,
	                [externalid] [varchar](128) NULL,
                 CONSTRAINT [PK_activity2] PRIMARY KEY NONCLUSTERED 
                (
	                [activityid] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                ) ON [PRIMARY];
                
                ALTER TABLE [dbo].[activity] ADD  CONSTRAINT [DF_activity2_activitydate]  DEFAULT (getdate()) FOR [performed];
                
                ALTER TABLE [dbo].[activity] ADD  CONSTRAINT [DF_activity2_test]  DEFAULT ((0)) FOR [test]";

            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(sql6, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
        }

        public void CreateFusionLinkedServerIfNeeded()
        {
            var connstr = _framework.GetConfirmitSqlServerConnectionString("master");
            var databaseEngine = new DatabaseEngine(connstr);

            bool isFusionLinkedServerExist = databaseEngine.ExecuteScalarWithSpecificTimeOut<string>(
                "SELECT '1' FROM sys.servers WHERE name = 'FUSIONLINKEDSERVER'", 
                CommandType.Text, 
                Settings.Default.DefaultConnectionTimeout) == "1";

            if (isFusionLinkedServerExist)
            {
                return;
            }

            string query = string.Format(@"
                EXEC master.dbo.sp_addlinkedserver 
	                @server = N'FUSIONLINKEDSERVER', 
	                @srvproduct=N'FUSIONLINKEDSERVER', 
	                @provider=N'SQLNCLI11',
                    @datasrc=N'{0}'", IntegrationTestingFramework.GetCatiSqlServerInstanceName());
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            query = @"
                EXEC master.dbo.sp_addlinkedsrvlogin 
	                @rmtsrvname=N'FUSIONLINKEDSERVER',
	                @useself=N'False',
	                @locallogin=NULL,
	                @rmtuser=N'sa',
	                @rmtpassword='firm'";
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            query = "EXEC master.dbo.sp_serveroption @server=N'FUSIONLINKEDSERVER', @optname=N'rpc', @optvalue=N'true'";
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            query = "EXEC master.dbo.sp_serveroption @server=N'FUSIONLINKEDSERVER', @optname=N'rpc out', @optvalue=N'true'";
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
        }

        public void CreateTestConfirmitDeployUserIfNeeded()
        {
            var connstr = _framework.GetCatiSqlServerConnectionString("master");
            var databaseEngine = new DatabaseEngine(connstr);

            bool isTestConfirmitDeployExist = databaseEngine.ExecuteScalarWithSpecificTimeOut<string>(
                "SELECT '1' FROM sys.server_principals WHERE name = 'TestConfirmitDeploy'",
                CommandType.Text,
                Settings.Default.DefaultConnectionTimeout) == "1";

            if (isTestConfirmitDeployExist)
            {
                return;
            }

            string query = @"
                CREATE LOGIN [TestConfirmitDeploy] WITH PASSWORD=N'TestConfirmitDeploy', 
                    DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF";
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            query = "ALTER SERVER ROLE [securityadmin] ADD MEMBER [TestConfirmitDeploy]";
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            query = "ALTER SERVER ROLE [setupadmin] ADD MEMBER [TestConfirmitDeploy]";
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            query = "ALTER SERVER ROLE [processadmin] ADD MEMBER [TestConfirmitDeploy]";
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);

            query = "ALTER SERVER ROLE [dbcreator] ADD MEMBER [TestConfirmitDeploy]";
            databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
        }
    }
}
