using System.Globalization;

using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.DAL.Framework;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Data.Builders;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    public static class ConfirmitTools
    {
        public static DatabaseEngine GetConfirmitSurveyDbOnTest(out string projectId)
        {
            return GetConfirmitSurveyDb(false, out projectId);
        }

        public static DatabaseEngine GetConfirmitSurveyDbOnClass(out string projectId)
        {
            return GetConfirmitSurveyDb(true, out projectId);
        }

        private static DatabaseEngine GetConfirmitSurveyDb(bool onClass, out string projectId)
        {
            var framework = IntegrationTestingFramework.Instance;
            projectId = BackendTools.GenerateSurveyName();
            var cfSurveyDbName = "survey_" + projectId;

            DatabaseEngine confirmitSurveyDb = onClass
                ? framework.CreateDatabaseOnClass(cfSurveyDbName)
                : framework.CreateDatabaseOnTest(cfSurveyDbName);

            CreateRespondentTable(confirmitSurveyDb);
            CreateResponseTable(confirmitSurveyDb, Enumerable.Range(1, 20).Select(x => "q" + x), "response1");
            CreateQuotaTables(confirmitSurveyDb);

            return confirmitSurveyDb;
        }

        public static void CreateRespondentTable(DatabaseEngine confirmitSurveyDb, IEnumerable<FormData> additionalColumns = null)
        {
            new SurveyDatabaseBuilder(confirmitSurveyDb).CreateRespondentTable(additionalColumns);
        }

        public static void CreateResponseTable(DatabaseEngine confirmitSurveyDb, IEnumerable<string> columnsName, string responseTableName)
        {
            new SurveyDatabaseBuilder(confirmitSurveyDb).CreateResponseTable(responseTableName, columnsName.Select(x => new FormData() { Name = x }));
        }

        /// <summary>
        /// All records are created with auto incremented respid from passed startRespId.
        /// all significant columns have value which based on respid
        /// </summary>
        /// <param name="db"></param>
        /// <param name="batchId"></param>
        /// <param name="startRespId">respid of first record</param>
        /// <param name="count">records count</param>
        /// <param name="timeZones">according timezones for sample records</param>
        /// <returns></returns>
        public static List<RespondentRecord> FillRespondentTable(DatabaseEngine db, int batchId, int startRespId, int count, IEnumerable<int> timeZones)
        {
            timeZones = timeZones ?? Enumerable.Repeat(1, count);

            var result = new List<RespondentRecord>();

            for (int i = startRespId; i < startRespId + count; ++i)
            {
                var timeZoneId = timeZones.ElementAtOrDefault(i - startRespId);
                result.Add(new RespondentRecord
                {
                    Sid = i.ToString(CultureInfo.InvariantCulture),
                    InterviewId = i,
                    RespondentName = "resp" + i,
                    RespondentPhone = i.ToString(CultureInfo.InvariantCulture),
                    LastCallTime = null,
                    TotalDuration = i,
                    ExtensionNumber = i.ToString(CultureInfo.InvariantCulture),
                    DialAttempts = i,
                    TimeZoneId = timeZoneId,
                    LastChannelId = (byte)i,
                    Resource = 0,
                });
            }

            FillRespondentTable(db, result, batchId);

            return result;
        }
        /// <summary>
        /// Processes a collection of data lines representing respondents, adds an auto-incrementing `sid` identifier 
        /// for each record, and inserts the data into the database using the provided `DatabaseEngine`.
        /// The first line in `lines` is treated as the header, which is used to create the column names.
        /// </summary>
        /// <param name="db">The database engine used to execute the data insertion.</param>
        /// <param name="batchId">The batch identifier for the current set of records, used for grouping records in the database.</param>
        /// <param name="startRespId">The starting respondent identifier (sid) for the first record, which increments for each subsequent record.</param>
        /// <param name="lines">The lines of data, with the first line as the header defining column names and the remaining lines as data records.</param>
        /// <returns>A tuple containing the processed column names as `IEnumerable<string>` and values as `IEnumerable<IEnumerable<string>>`.</returns>
        public static (IEnumerable<string>, IEnumerable<IEnumerable<string>>) FillRespondentTable(DatabaseEngine db, int batchId, int startRespId, IEnumerable<string> lines)
        {
            int id = startRespId;

            IEnumerable<string> columns = lines.First().Split('\t')
                .Prepend("sid");

            IEnumerable<IEnumerable<string>> values = lines.Skip(1)
                .Select(line => new List<string>
                {
            id++.ToString(CultureInfo.InvariantCulture), // Sid
                }
                .Concat(line.Split('\t')));

            FillRespondentTableWithRespondentData(db, columns, values, batchId);

            return (columns, values);
        }

        static public void FillRespondentTable(DatabaseEngine db, IEnumerable<RespondentRecord> respondents, int batchId)
        {
            const string query = "INSERT INTO respondent" +
                "(sid, RespondentName, TelephoneNumber, LastInterviewStart, TotalDuration, " +
                "ExtensionNumber, CallAttemptCount, TimeZoneId, LastChannelId, CatiInterviewerID, BatchID) VALUES" +
                "('{0}', '{1}', '{2}', null, {3}," +
                "'{4}', {5}, {6}, {7}%128, {8}, {9})";

            foreach (var respondent in respondents)
            {
                db.ExecuteNonQueryWithSpecificTimeOut(String.Format(query,
                    respondent.Sid,
                    respondent.RespondentName,
                    respondent.RespondentPhone,
                    respondent.TotalDuration,
                    respondent.ExtensionNumber,
                    respondent.DialAttempts,
                    respondent.TimeZoneId,
                    respondent.LastChannelId,
                    respondent.Resource,
                    batchId), CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            }
        }

        static public void ClearCatiInterviewerIdColumnInRespondentTable(DatabaseEngine db)
        {
            const string query = "UPDATE [respondent] SET [CatiInterviewerId] = 0";

            db.ExecuteNonQuery(query, CommandType.Text);
        }

        static public void FillRespondentTableWithRespondentIdsColumn(DatabaseEngine db, IEnumerable<RespondentRecord> respondents, int batchId)
        {
            const string query = "INSERT INTO respondent" +
                "(sid, RespondentName, TelephoneNumber, LastInterviewStart, TotalDuration, " +
                "ExtensionNumber, CallAttemptCount, TimeZoneId, LastChannelId, CatiInterviewerID, BatchID, CatiAssignments) VALUES" +
                "('{0}', '{1}', '{2}', null, {3}," +
                "'{4}', {5}, {6}, {7}%128, {8}, {9},'{10}')";

            foreach (var respondent in respondents)
            {
                db.ExecuteNonQueryWithSpecificTimeOut(String.Format(query,
                    respondent.Sid,
                    respondent.RespondentName,
                    respondent.RespondentPhone,
                    respondent.TotalDuration,
                    respondent.ExtensionNumber,
                    respondent.DialAttempts,
                    respondent.TimeZoneId,
                    respondent.LastChannelId,
                    respondent.Resource,
                    batchId,
                    respondent.ResourceIds
                    ), CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            }
        }

        static public void FillRespondentTableWithRespondentData(DatabaseEngine db, IEnumerable<string> columns, IEnumerable<IEnumerable<string>> values, int batchId)
        {
            string columnsString = string.Join(", ", columns);

            foreach (var row in values)
            {
                var respondentData = row.ToArray();
                if (respondentData.Length < columns.Count())
                {
                    respondentData = respondentData.Concat(Enumerable.Repeat(string.Empty, columns.Count() - respondentData.Length)).ToArray();
                }

                string valuePlaceholders = string.Join(", ", Enumerable.Range(0, respondentData.Length).Select(i => "'{" + i + "}'"));

                string query = $"INSERT INTO respondent ({columnsString}) VALUES ({valuePlaceholders})";

                var rowQuery = string.Format(query, respondentData);

                db.ExecuteNonQueryWithSpecificTimeOut(
                    rowQuery,
                    CommandType.Text,
                    Settings.Default.DefaultConnectionTimeout);
            }
        }

        public static void FillResponseTable(DatabaseEngine db, string responseTableName, IEnumerable<string> columnNames, int recordsCount, params IEnumerable<string>[] values)
        {
            string query = "INSERT INTO " + responseTableName +
                "(responseid, respid, " + string.Join(",", columnNames) + ") VALUES" +
                "({0}, {0}, " + string.Join(",", columnNames.Select((x, i) => "{" + (i + 1).ToString() + "}")) + ")";

            for (int i = 0; i < recordsCount; ++i)
            {
                var variables = new[] { (i + 1).ToString() }.Concat(values.Select(x => x.ElementAtOrDefault(i) ?? "null")).ToArray();
                db.ExecuteNonQueryWithSpecificTimeOut(String.Format(query, variables), CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            }
        }

        /// <summary>
        /// Quotas functionality requires availability some 
        /// tables in confirmit survey database. We are creating
        /// these tables here in order to avoid errors.
        /// </summary>
        public static void CreateQuotaTables(DatabaseEngine dbEngine)
        {
            // We use plain SQL instead of SMO to improve performance.
            const string sqlQuotas = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='quotas')
                    CREATE TABLE [dbo].[quotas]([quotaid] [int], [quotaname] [nvarchar](max), [tablename] [nvarchar](max), [iscati] [int] NOT NULL CONSTRAINT DF_quotas_iscati DEFAULT(0))";

            const string sqlQuotaField = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='quota_field')
                    CREATE TABLE [dbo].[quota_field]([quotaid] [int],[fieldname] [nvarchar](max))";

            const string sqlResponseControl = @"IF NOT EXISTS (SELECT TOP 1 NULL FROM [information_schema].[tables] WHERE [table_name]='response_control')
                    CREATE TABLE [dbo].[response_control]([ITS] [int],[respid] [int])";

            dbEngine.ExecuteNonQueryWithSpecificTimeOut(sqlQuotas, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            dbEngine.ExecuteNonQueryWithSpecificTimeOut(sqlQuotaField, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
            dbEngine.ExecuteNonQueryWithSpecificTimeOut(sqlResponseControl, CommandType.Text, Settings.Default.DefaultConnectionTimeout);
        }
    }
}
