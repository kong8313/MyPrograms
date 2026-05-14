using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.Survey.Data
{
    public class InterviewFormDataDatabaseSourceService : IInterviewFormDataDatabaseSourceService
    {
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;
        private readonly ISurveyDataRowsDatabaseUpdater _surveyDataRowsDatabaseUpdater;
        private readonly ISurveyDataRowsWebServiceUpdater _surveyDataRowsWebServiceUpdater;
        private readonly IToggleSettings _toggleSettings;

        private readonly Dictionary<string, SurveyDataRowCache> _tableName2Row;

        public InterviewFormDataDatabaseSourceService(
            ISurveyDatabaseEngine surveyDatabaseEngine,
            ISurveyDataRowsDatabaseUpdater surveyDataRowsDatabaseUpdater,
            ISurveyDataRowsWebServiceUpdater surveyDataRowsWebServiceUpdater,
            IToggleSettings toggleSettings)
        {
            _surveyDatabaseEngine = surveyDatabaseEngine;
            _surveyDataRowsDatabaseUpdater = surveyDataRowsDatabaseUpdater;
            _surveyDataRowsWebServiceUpdater = surveyDataRowsWebServiceUpdater;
            _toggleSettings = toggleSettings;

            _tableName2Row = new Dictionary<string, SurveyDataRowCache>(StringComparer.InvariantCultureIgnoreCase);
        }

        public int SurveyId { get; private set; }
        public int InterviewId { get; private set; }

        public void Initialize(int surveyId, int interviewId)
        {
            SurveyId = surveyId;
            InterviewId = interviewId;
        }

        public string GetFormValue(FormDescBase desc, string category, string[] loopQualifyer)
        {
            var field = desc.GetFormFieldByCategory(category);

            lock (_tableName2Row)
            {
                var cache = GetRowCache(field.TableName, desc.FormLevel, desc.LoopPath.Skip(1).ToArray(), loopQualifyer);

                var value = cache.GetFieldValue(field.FieldName);

                EventDetailsScope.Current.AddTiming("InterviewFormDataDatabaseSourceService.GetFormValue");

                return value == null ? null : value.ToString();
            }
        }

        public void SetFormValue(FormDescBase desc, string category, string[] loopQualifyer, string value)
        {
            var field = desc.GetFormFieldByCategory(category);

            lock (_tableName2Row)
            {
                var cache = GetRowCache(field.TableName, desc.FormLevel, desc.LoopPath.Skip(1).ToArray(), loopQualifyer);

                cache.SetFieldValue(desc.FormName, field.FieldName, value);
            }
        }

        public void Commit()
        {
            lock (_tableName2Row)
            {
                var rowsToProcess = _tableName2Row.Values.Where(row => row.IsChanged).ToArray();

                var rowsToUpdate = rowsToProcess.Where(row => row.IsExists).ToArray();
                var rowsToCreate = rowsToProcess.Where(row => !row.IsExists).ToArray();

                var rowsToProcessByWebService = new List<SurveyDataRowCache>();

                if (_toggleSettings.DirectlyInsertResponses)
                {
                    if (!_surveyDataRowsDatabaseUpdater.Process(SurveyId, InterviewId, rowsToProcess))
                    {
                        rowsToProcessByWebService.AddRange(rowsToProcess);
                        Trace.TraceError("Interview with SID='{0}' and IID='{1}' is not saved through direct survey database access. So all interview data will be saved through WebService.",
                            SurveyId, InterviewId);
                    }
                }
                else
                {
                    rowsToProcessByWebService.AddRange(rowsToCreate);
                    if (!_surveyDataRowsDatabaseUpdater.Update(SurveyId, InterviewId, rowsToUpdate))
                    {
                        rowsToProcessByWebService.AddRange(rowsToUpdate);
                        Trace.TraceError("Interview with SID='{0}' and IID='{1}' is not saved through database updater. So all interview data will be saved through WebService.",
                            SurveyId, InterviewId);
                    }
                }
                
                _surveyDataRowsWebServiceUpdater.Update(SurveyId, InterviewId, rowsToProcessByWebService.ToArray());

                EventDetailsScope.Current.AddTiming("InterviewFormDataDatabaseSourceService.Commit");
            }
        }
        
        public string GetDiff()
        {
            return string.Join(Environment.NewLine,
                _tableName2Row.Select(x => ObjectDiffBuilder.GetDiff(x.Value)));
        }

        private SurveyDataRowCache GetRowCache(string tableName, string loopLevel, string[] loopPath, string[] loopQualifyer)
        {
            SurveyDataRowCache cache;

            var key = CreateRowKey(tableName, loopPath, loopQualifyer);

            if (!_tableName2Row.TryGetValue(key, out cache))
            {
                cache = CreateRowCache(tableName, loopLevel, loopPath, loopQualifyer);

                _tableName2Row[key] = cache;
            }

            return cache;
        }

        private string CreateRowKey(string tableName, string[] loopPath, string[] loopQualifyer)
        {
            return tableName + "[" + String.Join(",", loopPath.Select((x, i) => String.Format("[{0}]=[{1}]", x, loopQualifyer[i]))) + "]";
        }

        private string CreateRowWhereClause(string[] loopPath, string[] loopQualifyer, List<SqlParameter> parameters)
        {
            parameters.Add(new SqlParameter("@respId", InterviewId));

            var result = "respid = @respId";

            for (int i = 0; i < loopQualifyer.Length; i++)
            {
                var parameterName = String.Format("@l{0}", i);
                result += String.Format(" AND [{0}] = {1}", loopPath[i], parameterName);
                parameters.Add(new SqlParameter(parameterName, loopQualifyer[i]));
            }

            return result;
        }

        private SurveyDataRowCache CreateRowCache(string tableName, string loopLevel, string[] loopPath, string[] loopQualifyer)
        {
            var parameters = new List<SqlParameter>();
            var loopWhereClause = CreateRowWhereClause(loopPath, loopQualifyer, parameters);

            var query = $"SELECT * FROM <Schema>.[{tableName}] WHERE {loopWhereClause}";

            var table = _surveyDatabaseEngine.ExecuteQuery(SurveyId, query, parameters.ToArray());

            if (table.Rows.Count >= 1)
            {
                var row = table.Rows.Cast<DataRow>().First();

                return new SurveyDataRowCache(tableName, loopLevel, loopPath, loopQualifyer, true, row);
            }

            return new SurveyDataRowCache(tableName, loopLevel, loopPath, loopQualifyer, false, table.NewRow());
        }
    }
}