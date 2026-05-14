using System;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using System.Threading;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.Survey.Fakes
{
    public class StubISurveyService : ISurveyService 
    {
        private ISurveyService _inner;

        public StubISurveyService()
        {
            _inner = null;
        }

        public ISurveyService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvSurveyEntity CreateSurveyStringStringStringStringStringDelegate(string confirmitProjectId, string confirmitSurveyName, string cfSqlServerConnectionString, string userName, string surveySqlServerName);
        public CreateSurveyStringStringStringStringStringDelegate CreateSurveyStringStringStringStringString;

        BvSurveyEntity ISurveyService.CreateSurvey(string confirmitProjectId, string confirmitSurveyName, string cfSqlServerConnectionString, string userName, string surveySqlServerName)
        {


            if (CreateSurveyStringStringStringStringString != null)
            {
                return CreateSurveyStringStringStringStringString(confirmitProjectId, confirmitSurveyName, cfSqlServerConnectionString, userName, surveySqlServerName);
            } else if (_inner != null)
            {
                return ((ISurveyService)_inner).CreateSurvey(confirmitProjectId, confirmitSurveyName, cfSqlServerConnectionString, userName, surveySqlServerName);
            }

            return default(BvSurveyEntity);
        }

        public delegate void UpdateReplicationSchemeBvSurveyEntityArrayOfTableInfoDelegate(BvSurveyEntity survey, TableInfo[] tables);
        public UpdateReplicationSchemeBvSurveyEntityArrayOfTableInfoDelegate UpdateReplicationSchemeBvSurveyEntityArrayOfTableInfo;

        void ISurveyService.UpdateReplicationScheme(BvSurveyEntity survey, TableInfo[] tables)
        {

            if (UpdateReplicationSchemeBvSurveyEntityArrayOfTableInfo != null)
            {
                UpdateReplicationSchemeBvSurveyEntityArrayOfTableInfo(survey, tables);
            } else if (_inner != null)
            {
                ((ISurveyService)_inner).UpdateReplicationScheme(survey, tables);
            }
        }

        public delegate void UpdateReplicationStatusInt32BooleanDelegate(int surveySid, bool isReplicationEnabled);
        public UpdateReplicationStatusInt32BooleanDelegate UpdateReplicationStatusInt32Boolean;

        void ISurveyService.UpdateReplicationStatus(int surveySid, bool isReplicationEnabled)
        {

            if (UpdateReplicationStatusInt32Boolean != null)
            {
                UpdateReplicationStatusInt32Boolean(surveySid, isReplicationEnabled);
            } else if (_inner != null)
            {
                ((ISurveyService)_inner).UpdateReplicationStatus(surveySid, isReplicationEnabled);
            }
        }

        public delegate bool IsReplicationSchemaChangedInt32ArrayOfTableInfoDelegate(int surveySid, TableInfo[] tables);
        public IsReplicationSchemaChangedInt32ArrayOfTableInfoDelegate IsReplicationSchemaChangedInt32ArrayOfTableInfo;

        bool ISurveyService.IsReplicationSchemaChanged(int surveySid, TableInfo[] tables)
        {


            if (IsReplicationSchemaChangedInt32ArrayOfTableInfo != null)
            {
                return IsReplicationSchemaChangedInt32ArrayOfTableInfo(surveySid, tables);
            } else if (_inner != null)
            {
                return ((ISurveyService)_inner).IsReplicationSchemaChanged(surveySid, tables);
            }

            return default(bool);
        }

        public delegate void UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfoDelegate(int surveySid, TableInfo[] tables);
        public UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfoDelegate UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfo;

        void ISurveyService.UpdateQuotaBalancingConfiguration(int surveySid, TableInfo[] tables)
        {

            if (UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfo != null)
            {
                UpdateQuotaBalancingConfigurationInt32ArrayOfTableInfo(surveySid, tables);
            } else if (_inner != null)
            {
                ((ISurveyService)_inner).UpdateQuotaBalancingConfiguration(surveySid, tables);
            }
        }

        public delegate void CleanSurveyInt32CancellationTokenDelegate(int surveyId, CancellationToken cancellationToken);
        public CleanSurveyInt32CancellationTokenDelegate CleanSurveyInt32CancellationToken;

        void ISurveyService.CleanSurvey(int surveyId, CancellationToken cancellationToken)
        {

            if (CleanSurveyInt32CancellationToken != null)
            {
                CleanSurveyInt32CancellationToken(surveyId, cancellationToken);
            } else if (_inner != null)
            {
                ((ISurveyService)_inner).CleanSurvey(surveyId, cancellationToken);
            }
        }

        public delegate List<CallHistoryDataEntity> GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringDelegate(string surveySIDs, DateTime? startTime, DateTime? endTime, string[] replicatedVariables);
        public GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfStringDelegate GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfString;

        List<CallHistoryDataEntity> ISurveyService.GetCallHistoryData(string surveySIDs, DateTime? startTime, DateTime? endTime, string[] replicatedVariables)
        {


            if (GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfString != null)
            {
                return GetCallHistoryDataStringNullableOfDateTimeNullableOfDateTimeArrayOfString(surveySIDs, startTime, endTime, replicatedVariables);
            } else if (_inner != null)
            {
                return ((ISurveyService)_inner).GetCallHistoryData(surveySIDs, startTime, endTime, replicatedVariables);
            }

            return default(List<CallHistoryDataEntity>);
        }

        public delegate void ValidateProjectIdStringDelegate(string projectId);
        public ValidateProjectIdStringDelegate ValidateProjectIdString;

        void ISurveyService.ValidateProjectId(string projectId)
        {

            if (ValidateProjectIdString != null)
            {
                ValidateProjectIdString(projectId);
            } else if (_inner != null)
            {
                ((ISurveyService)_inner).ValidateProjectId(projectId);
            }
        }

        public delegate string GetProjectIdWithNameInt32Delegate(int surveyId);
        public GetProjectIdWithNameInt32Delegate GetProjectIdWithNameInt32;

        string ISurveyService.GetProjectIdWithName(int surveyId)
        {


            if (GetProjectIdWithNameInt32 != null)
            {
                return GetProjectIdWithNameInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ISurveyService)_inner).GetProjectIdWithName(surveyId);
            }

            return default(string);
        }

        public delegate void OnLaunchSurveyInt32ActionOfStringDelegate(int sid, Action<string> taskLog);
        public OnLaunchSurveyInt32ActionOfStringDelegate OnLaunchSurveyInt32ActionOfString;

        void ISurveyService.OnLaunchSurvey(int sid, Action<string> taskLog)
        {

            if (OnLaunchSurveyInt32ActionOfString != null)
            {
                OnLaunchSurveyInt32ActionOfString(sid, taskLog);
            } else if (_inner != null)
            {
                ((ISurveyService)_inner).OnLaunchSurvey(sid, taskLog);
            }
        }

        public delegate DialingMode GetDialingModeInt32Delegate(int sid);
        public GetDialingModeInt32Delegate GetDialingModeInt32;

        DialingMode ISurveyService.GetDialingMode(int sid)
        {


            if (GetDialingModeInt32 != null)
            {
                return GetDialingModeInt32(sid);
            } else if (_inner != null)
            {
                return ((ISurveyService)_inner).GetDialingMode(sid);
            }

            return default(DialingMode);
        }

        public delegate BvScheduleEntity GetScheduleInt32Delegate(int surveySid);
        public GetScheduleInt32Delegate GetScheduleInt32;

        BvScheduleEntity ISurveyService.GetSchedule(int surveySid)
        {


            if (GetScheduleInt32 != null)
            {
                return GetScheduleInt32(surveySid);
            } else if (_inner != null)
            {
                return ((ISurveyService)_inner).GetSchedule(surveySid);
            }

            return default(BvScheduleEntity);
        }

    }
}