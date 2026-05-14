using System;
using System.Collections.Generic;
using System.Linq;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public class SurveyMetadataCache : ISurveyMetadataCache
    {
        private static readonly Object _lock = new Object();  

        private readonly string _projectID;
        private readonly int _surveySid;

        private readonly Dictionary<string, FormDescBase> _formDescMap = new Dictionary<string, FormDescBase>();
        private readonly Dictionary<string, FormDescBase> _replDescMap = new Dictionary<string, FormDescBase>();
        private readonly Dictionary<string, SurveyDatabaseFieldInfo> _respondentDescMap = new Dictionary<string, SurveyDatabaseFieldInfo>();

        private readonly ISurveyDatabaseInfoProvider _surveyDatabaseInfoProvider = ServiceLocator.Resolve<ISurveyDatabaseInfoProvider>();
        private readonly IAuthoringService _authoringService = ServiceLocator.Resolve<IAuthoringService>();

        internal SurveyMetadataCache(int surveyId)
        {
            var survey = SurveyRepository.GetById(surveyId);

            var evt = new InitializeSurveyMetadataCacheEvent(surveyId, survey.Name);
            using (new EventDetailsScope(evt.Details))
            {
                _projectID = survey.Name;
                _surveySid = survey.SID;

                // read BE replication scheme
                var columns = ReplicationColumnsRepository.GetBySurveyId(_surveySid);
                var formInfos = _authoringService.GetFormInfos(_projectID, columns.Select(x => x.ColumnName), SchemaSourceType.RuntimeProduction);
                foreach (var column in columns)
                {
                    _replDescMap.Add(column.ColumnName, CreateFormDescForReplicatedColumn(_surveySid, _projectID, column, formInfos.FirstOrDefault(x => x != null && x.Name == column.ColumnName)));
                }

                // read respondent table info
                _respondentDescMap =
                    _surveyDatabaseInfoProvider.GetRespondentFieldsInfo(survey.SID)
                        .ToDictionary(x => x.FieldName, StringComparer.OrdinalIgnoreCase);
            }
            evt.Finish();
        }

        public FormDescBase GetFormDesc(string name)
        {
            lock (_lock)
            {
                FormDescBase result = _formDescMap.GetValueOrDefault(name);

                if (result == null)
                {
                    var formInfo = _authoringService.GetFormInfos(_projectID, new[] { name }, SchemaSourceType.RuntimeProduction).FirstOrDefault();
                
                    if( formInfo == null )
                    {
                        return null;
                    }

                    var databaseFormInfo = _surveyDatabaseInfoProvider.GetFormInfo(_surveySid, name);

                    result = CreateFormDesc(formInfo, databaseFormInfo);

                    _formDescMap.Add(name, result);
                }

                return result;
            }
        }

        private FormDescBase CreateFormDescForReplicatedColumn(int surveyId, string projectId, BvReplicationColumnsEntity column, FormBase formInfo)
        {
            if (formInfo != null)
            {
                var databaseFormInfo = _surveyDatabaseInfoProvider.GetFormInfo(_surveySid, column.ColumnName);

                var resultFormDescBase = CreateFormDesc(formInfo, databaseFormInfo);
                resultFormDescBase.IsReplicated = true;
                resultFormDescBase.FormLevel = ReplicationSchemaService.GetDestinationTableName(surveyId);

                return resultFormDescBase;
            }
            else
            {
                return new SystemFormDesc(surveyId, projectId, column);
            }
        }

        private FormDescBase CreateFormDesc(FormBase formInfo, SurveyDatabaseFormInfo databaseFormInfo)
        {
            return FormDescBase.CreateInstance(_surveySid, _projectID, formInfo, databaseFormInfo);
        }

        public FormDescBase GetReplFormDesc(string name)
        {
            return _replDescMap.GetValueOrDefault(name);
        }

        public SurveyDatabaseFieldInfo GetRespondentFieldDesc(string fieldName)
        {
            return _respondentDescMap.GetValueOrDefault(fieldName);
        }
    }
}
