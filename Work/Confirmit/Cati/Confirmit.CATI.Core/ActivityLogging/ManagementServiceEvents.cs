using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class UpdateSurveyAccessEventParameters : ManagementActivityEventDetails
    {
        public string SupervisorName { get; set; }
    }

    [Serializable]
    public class UpdateSurveyPropertiesViaMsEventParameters : ManagementActivityEventDetails
    {
        public string ProjectName { get; set; }
        public int? DialingMode { get; set; }
        public bool? OpenEndReview { get; set; }
        public bool? VoiceRecording { get; set; }
        public bool? ScreenRecording { get; set; }
        public bool SupportBlacklist { get; set; }
        public bool AllowRespondentsDynamicCreation { get; set; }
        public string NotificationEmail { get; set; }
    }

    [Serializable]
    public class UpdateSurveyReplicationSchemeViaMsEventParameters : ManagementActivityEventDetails
    {
        public TableInfo[] Tables { get; set; }
    }

    [Serializable]
    public class UpdateSurveyReplicationStatusViaMsEventParameters : ManagementActivityEventDetails
    {
        public bool ReplicationEnabled { get; set; }
    }

    [Serializable]
    public class QuotaChangedViaMsEventParameters : ManagementActivityEventDetails
    {
        public int ConfirmitQuotaId { get; set; }
    }

    [Serializable]
    public class QuotaCellsChangedEventViaMsEventParameters : ManagementActivityEventDetails
    {
        public int ConfirmitQuotaId { get; set; }
        public int[] OpenedCfCellIds { get; set; }
        public int[] ClosedCfCellIds { get; set; }
        public int[] OptimisticallyClosedCfCellIds { get; set; }
    }

    [Serializable]
    public class QuotaCellsStateChangedEventParameters : ManagementActivityEventDetails
    {
        public int ConfirmitQuotaId { get; set; }
        public List<CatiQuotaCellCountersState> QuotaCellsStates { get; set; }
    }

    [Serializable]
    public class CatiOptionsChangedViaMsEventParameters : ManagementActivityEventDetails
    {
        public bool TelephonyEnabled { get; set; }
    }

    /// <summary>
    /// Occurs when survey access is updated
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.AddSurveyAccessViaMs)]
    public class AddSurveyAccessViaMsEvent : ManagementActivityEvent<UpdateSurveyAccessEventParameters>
    {
        public AddSurveyAccessViaMsEvent(string supervisorName, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.AddSurveyAccessViaMs)
        {
            ObjectName = projectId;
            Details = new UpdateSurveyAccessEventParameters { SupervisorName = supervisorName };
        }
    }

    /// <summary>
    /// Occurs when survey access is deleted
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.DeleteSurveyAccessViaMs)]
    public class DeleteSurveyAccessViaMsEvent : ManagementActivityEvent<UpdateSurveyAccessEventParameters>
    {
        public DeleteSurveyAccessViaMsEvent(string supervisorName, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.DeleteSurveyAccessViaMs)
        {
            ObjectName = projectId;
            Details = new UpdateSurveyAccessEventParameters { SupervisorName = supervisorName };
        }
    }

    /// <summary>
    /// Occurs when survey properties are updated
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.UpdateSurveyPropertiesViaMs)]
    public class UpdateSurveyPropertiesViaMsEvent : ManagementActivityEvent<UpdateSurveyPropertiesViaMsEventParameters>
    {
        public UpdateSurveyPropertiesViaMsEvent(
              string projectID
            , string projectName
            , int? dialingMode
            , bool? openEndReview
            , bool? voiceRecording
            , bool? screenRecording
            , bool supportBlacklist
            , bool allowRespondentsDynamicCreation
            , string notificationEmail
        ):
            base(ManagementEventCategory.Survey, ManagementEvent.UpdateSurveyPropertiesViaMs)
        {
            ObjectName = projectID;
            Details = new UpdateSurveyPropertiesViaMsEventParameters()
            {
                ProjectName = projectName,
                DialingMode = dialingMode,
                OpenEndReview = openEndReview,
                VoiceRecording = voiceRecording,
                ScreenRecording = screenRecording,
                SupportBlacklist = supportBlacklist,
                AllowRespondentsDynamicCreation = allowRespondentsDynamicCreation,
                NotificationEmail = notificationEmail
            };
        }
    }

    /// <summary>
    /// Occurs when Survey replication scheme is updated via MS
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.UpdateSurveyReplicationSchemeViaMs)]
    public class UpdateSurveyReplicationSchemeViaMsEvent : ManagementActivityEvent<UpdateSurveyReplicationSchemeViaMsEventParameters>
    {
        public UpdateSurveyReplicationSchemeViaMsEvent(int surveyId, string surveyName, TableInfo[] tables):
            base(ManagementEventCategory.Survey, ManagementEvent.UpdateSurveyReplicationSchemeViaMs)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
            Details = new UpdateSurveyReplicationSchemeViaMsEventParameters() { Tables = tables };
        }
    }

    /// <summary>
    /// Occurs when Survey replication status is updated via MS
    /// </summary>
    [ManagementEventAttribute(ManagementEvent.UpdateSurveyReplicationStatusViaMs)]
    public class UpdateSurveyReplicationStatusViaMsEvent : ManagementActivityEvent<UpdateSurveyReplicationStatusViaMsEventParameters>
    {
        public UpdateSurveyReplicationStatusViaMsEvent(int surveyId, string surveyName, bool replicationEnabled):
            base(ManagementEventCategory.Survey, ManagementEvent.UpdateSurveyReplicationStatusViaMs)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
            Details = new UpdateSurveyReplicationStatusViaMsEventParameters() { ReplicationEnabled = replicationEnabled };
        }
    }

    [ManagementEventAttribute(ManagementEvent.QuotaChangedViaMs)]
    public class QuotaChangedViaMsEvent : ManagementActivityEvent<QuotaChangedViaMsEventParameters>
    {
        public QuotaChangedViaMsEvent(int surveyId, string surveyName, int confirmitQuotaId):
            base(ManagementEventCategory.Quota, ManagementEvent.QuotaChangedViaMs)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
            Details = new QuotaChangedViaMsEventParameters() { ConfirmitQuotaId = confirmitQuotaId };
        }
    }

    [ManagementEventAttribute(ManagementEvent.QuotaCellsChangedEventViaMs)]
    public class QuotaCellsChangedEventViaMsEvent : ManagementActivityEvent<QuotaCellsChangedEventViaMsEventParameters>
    {
        public QuotaCellsChangedEventViaMsEvent(int surveyId, string surveyName, int confirmitQuotaId, int[] openedCellsIds, int[] closedCellsIds, int[] optimisticallyClosedCfCellIds)
            : base(ManagementEventCategory.Quota, ManagementEvent.QuotaCellsChangedEventViaMs)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
            Details = new QuotaCellsChangedEventViaMsEventParameters()
            {
                ConfirmitQuotaId = confirmitQuotaId, 
                OpenedCfCellIds = openedCellsIds, 
                ClosedCfCellIds = closedCellsIds,
                OptimisticallyClosedCfCellIds = optimisticallyClosedCfCellIds
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.QuotaCellsStateChangedEvent)]
    public class QuotaCellsStateChangedEvent : ManagementActivityEvent<QuotaCellsStateChangedEventParameters>
    {
        public QuotaCellsStateChangedEvent(int surveyId, string surveyName, int confirmitQuotaId, List<CatiQuotaCellCountersState> quotaCellsStates):
            base(ManagementEventCategory.Quota, ManagementEvent.QuotaCellsStateChangedEvent)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
            Details = new QuotaCellsStateChangedEventParameters()
            {
                ConfirmitQuotaId = confirmitQuotaId, 
                QuotaCellsStates = quotaCellsStates
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.CatiOptionsChangedViaMs)]
    public class CatiOptionsChangedViaMsEvent : ManagementActivityEvent<CatiOptionsChangedViaMsEventParameters>
    {
        public CatiOptionsChangedViaMsEvent(string siteName, bool telephonyEnabled):
            base(ManagementEventCategory.SystemSettings, ManagementEvent.CatiOptionsChangedViaMs)
        {
            ObjectId = 0;
            ObjectName = siteName;
            Details = new CatiOptionsChangedViaMsEventParameters() { TelephonyEnabled = telephonyEnabled };
        }
    }    
}