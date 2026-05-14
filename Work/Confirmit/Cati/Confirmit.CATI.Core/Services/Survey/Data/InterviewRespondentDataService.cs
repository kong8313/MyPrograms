using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Survey.Data
{
    public class InterviewRespondentDataService : IInterviewRespondentDataSourceService
    {
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;
        private readonly ISurveyDataRowsDatabaseUpdater _surveyDataRowsDatabaseUpdater;
        private SurveyDataRowCache _row;

        public InterviewRespondentDataService(
            ISurveyDatabaseEngine surveyDatabaseEngine,
            ISurveyMetadataCacheService surveyMetadataCacheService,
            ISurveyDataRowsDatabaseUpdater surveyDataRowsDatabaseUpdater
            )
        {
            _surveyDatabaseEngine = surveyDatabaseEngine;
            _surveyMetadataCacheService = surveyMetadataCacheService;
            _surveyDataRowsDatabaseUpdater = surveyDataRowsDatabaseUpdater;
            _row = null;
        }

        public int SurveyId { get; private set; }
        public int InterviewId { get; private set; }

        public void Initialize(int surveyId, int interviewId)
        {
            SurveyId = surveyId;
            InterviewId = interviewId;
        }

        public void Commit()
        {
            lock (this)
            {
                if (_row != null && _row.IsChanged)
                {
                    if (!_surveyDataRowsDatabaseUpdater.Update(SurveyId, InterviewId, new[] { _row }))
                    {
                        throw new Exception("Unable to store respondent data.");
                    }
                }
            }
        }

        public object GetRespondentValue(string fieldName)
        {
            var field = GetRespondentFieldDesc(fieldName);

            lock (this)
            {
                var cache = GetRowCache();

                var result = cache.GetFieldValue(field.FieldName);

                EventDetailsScope.Current.AddTiming("InterviewRespondentDataService.GetRespondentValue");

                return result;
            }
        }

        private SurveyDataRowCache GetRowCache()
        {
            if (_row == null)
            {
                var query = "SELECT * FROM <Schema>.[respondent] WHERE respid = @respId";

                var table = _surveyDatabaseEngine.ExecuteQuery(SurveyId, query, new SqlParameter("@respId", InterviewId));

                var row = table.Rows.Cast<DataRow>().Single();

                _row = new SurveyDataRowCache("respondent", "respondent", new string[] { }, new string[] { }, true, row);
            }

            return _row;
        }

        public void SetRespondentValue(string fieldName, object value)
        {
            var field = GetRespondentFieldDesc(fieldName);

            lock (this)
            {
                var cache = GetRowCache();

                cache.SetFieldValue(field.FieldName, field.FieldName, value);

                EventDetailsScope.Current.AddTiming("InterviewRespondentDataService.SetRespondentValue");
            }
        }

        public string GetDiff()
        {
            return ObjectDiffBuilder.GetDiff(_row);
        }

        private SurveyDatabaseFieldInfo GetRespondentFieldDesc(string fieldName)
        {
            var fieldDesc = _surveyMetadataCacheService.Get(SurveyId).GetRespondentFieldDesc(fieldName);
            if (fieldDesc == null)
            {
                throw new SchedulingScriptExecutionException($"Respondent field '{fieldName}' was not found.");
            }
            return fieldDesc;
        }


    }
}