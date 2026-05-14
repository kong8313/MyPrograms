using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.SystemSettings;


namespace Confirmit.CATI.Core.Services.Survey.Data
{
    public class RespondentVariablesService : IRespondentVariablesService
    {
        private readonly IInterviewRespondentDataSourceService _respondentDataSourceService;
        private readonly IDialerSettings _dialerSettings;
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;

        private const int BatchSize = 1000;

        public RespondentVariablesService(
            IInterviewRespondentDataSourceService respondentDataSourceService,
            IDialerSettings dialerSettings,
            ISurveyMetadataCacheService surveyMetadataCacheService,
            ISurveyDatabaseEngine surveyDatabaseEngine)
        {
            _respondentDataSourceService = respondentDataSourceService;
            _dialerSettings = dialerSettings;
            _surveyMetadataCacheService = surveyMetadataCacheService;
            _surveyDatabaseEngine = surveyDatabaseEngine;
        }

        public Dictionary<string, object> GetVariablesToSend(int surveyId, int respId)
        {
            var variableNames = GetVariableNames();
            if (!variableNames.Any())
                return null;

            var surveyMetadataCache = _surveyMetadataCacheService.Get(surveyId);
            _respondentDataSourceService.Initialize(surveyId, respId);

            var result = new Dictionary<string, object>();
            foreach (var name in variableNames)
            {
                var fieldDesc = surveyMetadataCache.GetRespondentFieldDesc(name);
                if (fieldDesc == null)
                    continue;

                var value = _respondentDataSourceService.GetRespondentValue(name);

                result[name] = value;
            }

            return result;
        }

        public Dictionary<int, Dictionary<string, object>> GetVariablesToSend(int surveyId, List<int> respIds)
        {
            var variableNames = GetVariableNames();
            if (!variableNames.Any() || !respIds.Any())
                return null;

            var surveyMetadataCache = _surveyMetadataCacheService.Get(surveyId);
            var existingVariables = new List<string>();
            foreach (var name in variableNames)
            {
                var fieldDesc = surveyMetadataCache.GetRespondentFieldDesc(name);
                if (fieldDesc == null)
                    continue;

                existingVariables.Add(name);
            }

            if (!existingVariables.Any())
                return null;

            try
            {
                return GetRespondentVariables(surveyId, existingVariables, respIds);
            }
            catch (SqlException ex)
            {
                TraceHelper.TraceException(ex, "Error while fetching respondent variables");
                return null;
            }
        }

        private IEnumerable<string> GetVariableNames()
        {
            return _dialerSettings.RespondentVariablesToSend.Split(',').Select(x => x.Trim());
        }

        private Dictionary<int, Dictionary<string, object>> GetRespondentVariables(int surveyId, List<string> variableNames, List<int> respIds)
        {
            var result = new Dictionary<int, Dictionary<string, object>>();

            var selectedColumns = String.Join(",", variableNames.Select(x => $"[{x}]"));
            foreach (var batch in respIds.SplitIntoBatches(BatchSize))
            {
                var respIdsString = String.Join(",", batch);
                var query = $"SELECT respid, {selectedColumns} FROM <Schema>.[respondent] WHERE respid IN ({respIdsString})";

                var table = _surveyDatabaseEngine.ExecuteQuery(surveyId, query);

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var respId = (int)row["respId"];

                    var variables = new Dictionary<string, object>();
                    foreach (var variableName in variableNames)
                    {
                        var value = row[variableName] != DBNull.Value ? row[variableName] : null;
                        variables[variableName] = value;
                    }

                    result.Add(respId, variables);
                }
            }

            return result;
        }
    }
}