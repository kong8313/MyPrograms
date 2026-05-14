using System;
using System.Collections.Generic;
using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.Survey
{
    public interface ISurveyService
    {
        BvSurveyEntity CreateSurvey(string confirmitProjectId, string confirmitSurveyName, string cfSqlServerConnectionString, string userName, string surveySqlServerName);
        void UpdateReplicationScheme(BvSurveyEntity survey, TableInfo[] tables);
        void UpdateReplicationStatus(int surveySid, bool isReplicationEnabled);
        bool IsReplicationSchemaChanged(int surveySid, TableInfo[] tables);
        void UpdateQuotaBalancingConfiguration(int surveySid, TableInfo[] tables);

        void CleanSurvey(int surveyId, CancellationToken cancellationToken);

        List<CallHistoryDataEntity> GetCallHistoryData(string surveySIDs, DateTime? startTime, DateTime? endTime, string[] replicatedVariables);

        void ValidateProjectId(string projectId);

        string GetProjectIdWithName(int surveyId);

        void OnLaunchSurvey(int sid, Action<string> taskLog = null);

        DialingMode GetDialingMode(int sid);

        BvScheduleEntity GetSchedule(int surveySid);
    }
}