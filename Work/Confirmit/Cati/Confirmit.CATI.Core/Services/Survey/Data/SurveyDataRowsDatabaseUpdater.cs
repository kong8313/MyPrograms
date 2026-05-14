using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.Survey.Data
{
    class SurveyDataRowsDatabaseUpdater : ISurveyDataRowsDatabaseUpdater
    {
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;

        public SurveyDataRowsDatabaseUpdater(ISurveyDatabaseEngine surveyDatabaseEngine)
        {
            _surveyDatabaseEngine = surveyDatabaseEngine;
        }

        public bool Update(int surveyId, int interviewId, SurveyDataRowCache[] rows)
        {
            if (rows.Length == 0)
            {
                return true;
            }

            var parameters = new List<SqlParameter>();
            var query = new StringBuilder();
            var paramId = 0;

            query.AppendLine("DECLARE @NoError AS BIT = 1");
            query.AppendLine("BEGIN TRAN;");

            int rowId = 0;
            foreach (var row in rows)
            {
                query.AppendLine("IF @NoError = 1");
                query.AppendFormat("    UPDATE <Schema>.[{0}] SET", row.TableName);
                query.AppendLine();

                bool first = true;
                foreach (var column in row.ChangedColumns)
                {
                    var paramName = String.Format("@P{0}", paramId++);

                    var value = row.GetFieldValue(column) ?? DBNull.Value;

                    parameters.Add(new SqlParameter(paramName, value));

                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        query.Append(",");
                    }

                    query.AppendFormat("        [{0}] = {1}", column, paramName);
                    query.AppendLine();
                }

                query.Append("    WHERE respId = @respId");
                for (int i = 0; i < row.LoopPath.Length; i++)
                {
                    var paramName = String.Format("@l{0}_{1}", rowId, i);
                    query.Append(String.Format(" AND [{0}] = {1}", row.LoopPath[i], paramName));
                    parameters.Add(new SqlParameter(paramName, row.LoopQualifyer[i]));
                }

                query.AppendLine();
                query.AppendLine("    IF @@ROWCOUNT = 0");
                query.AppendLine("        SET @NoError = 0");
                rowId++;
            }

            query.AppendLine("IF @NoError = 1 COMMIT TRAN ELSE ROLLBACK TRAN");
            query.AppendLine("SELECT @NoError");

            parameters.Add(new SqlParameter("@respId", interviewId));

            var result = _surveyDatabaseEngine.ExecuteScalar<bool>(surveyId, query.ToString(), parameters.ToArray());

            EventDetailsScope.Current.AddTiming("SurveyDataRowsDatabaseUpdater.Update");

            return result;
        }

        public bool Process(int surveyId, int interviewId, SurveyDataRowCache[] rows)
        {
            if (rows.Length == 0)
            {
                return true;
            }

            var query = new StringBuilder();
            query.Append($@"
            DECLARE @NoError AS BIT = 1
            BEGIN TRAN;

            DECLARE @responseId INT;
            SET @responseId = -1;
            SELECT @responseId = responseId FROM <Schema>.[response_control] WHERE respId = @respId
            IF (@responseId = -1)
            BEGIN
                INSERT INTO <Schema>.[response_control](respid) VALUES(@respId);
                SET @responseId = SCOPE_IDENTITY();
            END");

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@respId", interviewId));

            int rowId = 0;
            foreach (var row in rows)
            {
                query.Append($@"
                IF @NoError = 1
                BEGIN
                    IF EXISTS(SELECT 1 FROM <Schema>.[{row.TableName}] WHERE {GetWhereCondition(row, rowId)})
                    BEGIN
                        {GetUpdateQueryString(row, rowId)}
                    END
                    ELSE
                    BEGIN
                        {GetInsertQueryString(row, rowId)}
                    END

                    IF @@ROWCOUNT = 0
                        SET @NoError = 0
                END");

                foreach (var column in row.ChangedColumns)
                {
                    var paramName = $"@P{rowId}_{column}";
                    var value = row.GetFieldValue(column) ?? DBNull.Value;
                    parameters.Add(new SqlParameter(paramName, value));
                }

                for (int i = 0; i < row.LoopPath.Length; i++)
                {
                    var paramName = $"@l{rowId}_{i}";
                    parameters.Add(new SqlParameter(paramName, row.LoopQualifyer[i]));
                }

                rowId++;
            }

            query.Append(@"
                IF @NoError = 1 
                    COMMIT TRAN ELSE ROLLBACK TRAN
                SELECT @NoError
            ");

            bool result;
            try
            {
                result = _surveyDatabaseEngine.ExecuteScalar<bool>(surveyId, query.ToString(), parameters.ToArray());
            }
            catch (SqlException e) when (e.Number == 2627 || e.Number == 2601)//try to run same query one more time in case of constraint violation exception 
            {                                                                 //this may happen when survey engine inserts records to database at the same time
                result = _surveyDatabaseEngine.ExecuteScalar<bool>(surveyId, query.ToString(), parameters.ToArray());
            }

            EventDetailsScope.Current.AddTiming("SurveyDataRowsDatabaseUpdater.Process");

            return result;
        }

        private string GetUpdateQueryString(SurveyDataRowCache row, int rowId)
        {
            var query = new StringBuilder();
            query.Append($@"UPDATE <Schema>.[{row.TableName}] SET ");

            bool first = true;
            foreach (var column in row.ChangedColumns)
            {
                var paramName = $"@P{rowId}_{column}";

                if (first)
                {
                    first = false;
                }
                else
                {
                    query.Append(",");
                }

                query.Append($" [{column}] = {paramName}");
            }

            query.Append($@"
                WHERE {GetWhereCondition(row, rowId)}");

            return query.ToString();
        }

        private string GetWhereCondition(SurveyDataRowCache row, int rowId)
        {
            var query = new StringBuilder();

            query.Append(@"respId = @respId");
            for (int i = 0; i < row.LoopPath.Length; i++)
            {
                var paramName = $"@l{rowId}_{i}";
                query.Append($" AND [{row.LoopPath[i]}] = {paramName}");
            }

            return query.ToString();
        }

        private string GetInsertQueryString(SurveyDataRowCache row, int rowId)
        {
            var query = new StringBuilder();

            var columns = row.ChangedColumns.ToList();
            if (row.LoopPath.Length > 0)
            {
                columns.AddRange(row.LoopPath);
            }

            query.Append($@"INSERT INTO <Schema>.[{row.TableName}](responseId, respid, {String.Join(", ", columns)}) VALUES(@responseId, @respId");
            foreach (var column in row.ChangedColumns)
            {
                var paramName = $"@P{rowId}_{column}";
                query.Append($", {paramName} ");
            }

            for (int i = 0; i < row.LoopPath.Length; i++)
            {
                var paramName = $"@l{rowId}_{i}";
                query.Append($", {paramName} ");
            }

            query.Append(")");
            return query.ToString();
        }
    }
}