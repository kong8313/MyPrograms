using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class AddSurveyViaWsEventParameters : ManagementActivityEventDetails
    {
        public string SurveyName { get; set; }
        public string ConnectionString { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AddSurveyViaWs)]
    public class AddSurveyViaWsEvent : ManagementActivityEvent<AddSurveyViaWsEventParameters>
    {
        public AddSurveyViaWsEvent(int surveySid, string projectId, string surveyName, string connectionString):
            base(ManagementEventCategory.Survey, ManagementEvent.AddSurveyViaWs)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new AddSurveyViaWsEventParameters { SurveyName = surveyName, ConnectionString = connectionString };
        }
    }
    [ManagementEventAttribute(ManagementEvent.DeleteSurveyViaWs)]
    public class DeleteSurveyViaWsEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeleteSurveyViaWsEvent(int surveySid, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.DeleteSurveyViaWs)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteSurvey)]
    public class DeleteSurveyEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.DeleteSurvey.Parameters>
    {
        public DeleteSurveyEvent(int surveyId, string surveyName, AsyncOperations.Operations.DeleteSurvey.Parameters parameters, BvAsyncOperationQueueEntity entity)
             :base(ManagementEventCategory.Survey, ManagementEvent.DeleteSurvey, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }

    
    [ManagementEventAttribute(ManagementEvent.ConfigureClusteredQuota)]
    public class ConfigureClusteredQuotaEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.ConfigureClusteredQuota.Parameters>
    {
        public ConfigureClusteredQuotaEvent(int surveyId, string surveyName, AsyncOperations.Operations.ConfigureClusteredQuota.Parameters parameters, BvAsyncOperationQueueEntity entity)
             :base(ManagementEventCategory.Survey, ManagementEvent.ConfigureClusteredQuota, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }
    
    [ManagementEventAttribute(ManagementEvent.SoftDeleteSurveyViaWs)]
    public class SoftDeleteSurveyViaWsEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public SoftDeleteSurveyViaWsEvent(int surveySid, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.SoftDeleteSurveyViaWs)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.RestoreSoftDeletedSurveyViaWs)]
    public class RestoreSoftDeletedSurveyViaWsEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public RestoreSoftDeletedSurveyViaWsEvent(int surveySid, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.RestoreSoftDeletedSurveyViaWs)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
        }
    }
    
    [Serializable]
    public class AddRespondentEventParameters : ManagementActivityEventDetails
    {
        public int RespondentId { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AddRespondentViaWs)]
    public class AddRespondentViaWsEvent : ManagementActivityEvent<AddRespondentEventParameters>
    {
        public AddRespondentViaWsEvent(int surveySid, string surveyName, int respondentId):
            base(ManagementEventCategory.Call, ManagementEvent.AddRespondentViaWs)
        {
            ObjectId = surveySid;
            ObjectName = surveyName;
            Details = new AddRespondentEventParameters { RespondentId = respondentId };
        }
    }

    [Serializable]
    public class AddRespondentFromConsoleEventParameters : ManagementActivityEventDetails
    {
        public int RespondentId { get; set; }
        public int PersonId { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AddRespondentFromConsole)]
    public class AddRespondentFromConsoleEvent : ManagementActivityEvent<AddRespondentFromConsoleEventParameters>
    {
        public AddRespondentFromConsoleEvent(int surveySid, string surveyName, int respondentId, int personId):
            base(ManagementEventCategory.Call, ManagementEvent.AddRespondentFromConsole)
        {
            ObjectId = surveySid;
            ObjectName = surveyName;
            Details = new AddRespondentFromConsoleEventParameters { RespondentId = respondentId, PersonId = personId};
        }
    }

    [Serializable]
    public class InitUserSurveyPermissionsEventParameters : ManagementActivityEventDetails
    {
        public string[] AllowedSurveys { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.InitUserSurveyPermissions)]
    public class InitUserSurveyPermissionsEvent : ManagementActivityEvent<InitUserSurveyPermissionsEventParameters>
    {
        public InitUserSurveyPermissionsEvent():
            base(ManagementEventCategory.View, ManagementEvent.InitUserSurveyPermissions)
        {
            ObjectId = 0;
            ObjectName = string.Empty;
            Details = new InitUserSurveyPermissionsEventParameters();
        }
    }

    [Serializable]
    public class InitUserTabPermissionsEventParameters : ManagementActivityEventDetails
    {
        public string AllowedTabs { get; set; }
        public string Mode { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.InitUserTabPermissions)]
    public class InitUserTabPermissionsEvent : ManagementActivityEvent<InitUserTabPermissionsEventParameters>
    {
        public InitUserTabPermissionsEvent():
            base(ManagementEventCategory.View, ManagementEvent.InitUserTabPermissions)
        {
            ObjectId = 0;
            ObjectName = string.Empty;
            Details = new InitUserTabPermissionsEventParameters();
        }
    }

    [ManagementEventAttribute(ManagementEvent.StartLegacySupervisor)]
    public class StartLegacySupervisorEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public StartLegacySupervisorEvent():
            base(ManagementEventCategory.View, ManagementEvent.StartLegacySupervisor)
        {
        }
    }

    [Serializable]
    public class AddTelephoneNumberToBlacklistViaWsEventParameters : ManagementActivityEventDetails
    {
        public string TelephoneNumber { get; set; }
        public string ProjectId { get; set; }
        public int InterviewId { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AddTelephoneNumberToBlacklistViaWs)]
    public class AddTelephoneNumberToBlacklistViaWsEvent : ManagementActivityEvent<AddTelephoneNumberToBlacklistViaWsEventParameters>
    {
        public AddTelephoneNumberToBlacklistViaWsEvent(string number, string projectId, int interviewId):
            base(ManagementEventCategory.Blacklist, ManagementEvent.AddTelephoneNumberToBlacklistViaWs)
        {
            Details = new AddTelephoneNumberToBlacklistViaWsEventParameters 
                { 
                    TelephoneNumber = number,
                    ProjectId = projectId,
                    InterviewId = interviewId
                };
        }
    }

    [Serializable]
    public class RestoreSurveyFromArchiveEventDetails : ManagementActivityEventDetails
    {
        public AsyncOperations.Operations.RestoreSurvey.Parameters Parameters { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.RestoreSurveyFromArchive)]
    public class RestoreSurveyFromArchiveEvent : ManagementActivityEvent<RestoreSurveyFromArchiveEventDetails>
    {
        public RestoreSurveyFromArchiveEvent():
            base(ManagementEventCategory.Survey, ManagementEvent.RestoreSurveyFromArchive)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.LaunchSurvey)]
    public class LaunchSurveyEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.LaunchSurvey.Parameters>
    {
        public LaunchSurveyEvent(int surveyId, string surveyName, AsyncOperations.Operations.LaunchSurvey.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Survey, ManagementEvent.LaunchSurvey, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }
    
    [ManagementEventAttribute(ManagementEvent.SampleUpload)]
    public class SampleUploadEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.SampleUpload.Parameters>
    {
        public SampleUploadEvent(int surveyId, string surveyName, AsyncOperations.Operations.SampleUpload.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Survey, ManagementEvent.SampleUpload, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = surveyName;
        }
    }
    
    [ManagementEventAttribute(ManagementEvent.BackupSurveyToArchive)]
    public class BackupSurveyToArchiveEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public BackupSurveyToArchiveEvent(int objectId, string name):
            base(ManagementEventCategory.Survey, ManagementEvent.BackupSurveyToArchive)
        {
            ObjectId = objectId;
            ObjectName = name;
        }
    }

    [Serializable]
    public class BulkCopyInterviewerActivityEventsEventtParameters : ManagementActivityEventDetails
    {
        public int EventsCount { get; set; }
    }

     [ManagementEventAttribute(ManagementEvent.BulkCopyInterviewerActivityEventsEvent)]
     public class BulkCopyInterviewerActivityEventsEvent : ManagementActivityEvent<BulkCopyInterviewerActivityEventsEventtParameters>
     {
        public BulkCopyInterviewerActivityEventsEvent():
            base(ManagementEventCategory.System, ManagementEvent.BulkCopyInterviewerActivityEventsEvent)
        {
            // TODO: ! In many places we do not set EventType property and there is no need to create details object as it is created in the base classes
            Details = new BulkCopyInterviewerActivityEventsEventtParameters();
        }
    }

    [Serializable]
    public class GetDeferredRecordAudioInfoEventParameters : ManagementActivityEventDetails
    {
        public long MonitoringSessionId;
        public int InterviewerSid;
        public int InterviewId;
        public string[] AudioFilesNames;
        public string[] AudioFilesUris;
    }

    [ManagementEvent(ManagementEvent.GetDeferredRecordAudioInfo)]
    public class GetDeferredRecordAudioInfoEvent : ManagementActivityEvent<GetDeferredRecordAudioInfoEventParameters>
    {
        public GetDeferredRecordAudioInfoEvent():
            base(ManagementEventCategory.RecordedInterview, ManagementEvent.GetDeferredRecordAudioInfo)
        {
        }

        public void Finish(
            int interviewerSid,
            int surveySid,
            string surveyName,
            int interviewId,
            long sessionId,
            string[] filesNames, 
            string[] filesUris)
        {
            ObjectId = surveySid;
            ObjectName = surveyName;

            Details.InterviewerSid = interviewerSid;
            Details.InterviewId = interviewId;
            Details.MonitoringSessionId = sessionId;
            Details.AudioFilesNames = filesNames;
            Details.AudioFilesUris = filesUris;

            Finish();
        }
    }

    [Serializable]
    public class CreateDefferedMonitoringFileEventParameters : ManagementActivityEventDetails
    {
        public long StartingFileSize;
        public int InterviewerSid;
        public int InterviewId;
    }

    [ManagementEventAttribute(ManagementEvent.CreateDefferedMonitoringFile)]
    public class CreateDefferedMonitoringFileEvent : ManagementActivityEvent<CreateDefferedMonitoringFileEventParameters>
    {
       public CreateDefferedMonitoringFileEvent():
           base(ManagementEventCategory.RecordedInterview, ManagementEvent.CreateDefferedMonitoringFile)
       {
       }

       public void Finish(
            int interviewerSid,
            int surveySid,
            string surveyName,
            int interviewId,
            long startingFileSize)
        {
            Details.InterviewerSid = interviewerSid;
            Details.InterviewId = interviewId;
            Details.StartingFileSize = startingFileSize;
            ObjectId = surveySid;
            ObjectName = surveyName;

            Finish();
        }
    }

    [Serializable]
    public class WebApiCallParameters : ManagementActivityEventDetails
    {
        public string RequestInfo { get; set; }
        public List<string> ExecutionLog { get; set; }
        public int StatusCode { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.WebApiCall)]
    public class WebApiCallEvent : ManagementActivityEvent<WebApiCallParameters>
    {
        public WebApiCallEvent():
            base(ManagementEventCategory.PublicApi, ManagementEvent.WebApiCall)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteRespondentsAsync)]
    public class DeleteRespondentsAsyncEvent : BaseAsyncOperationManagementActivityEvent<AsyncOperations.Operations.DeleteRespondents.Parameters>
    {
        public DeleteRespondentsAsyncEvent(int surveyId, string projectId, AsyncOperations.Operations.DeleteRespondents.Parameters parameters, BvAsyncOperationQueueEntity entity)
            : base(ManagementEventCategory.Call, ManagementEvent.DeleteRespondentsAsync, parameters, entity)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
        }
    }

    public class ProcessSampleEventParameters : ManagementActivityEventDetails
    {
        public string ProjectdId { get; set; }
        public string ProjectName { get; set; }
        public int BatchId { get; set; }
        public ProcessSampleMode ProcessSampleMode { get; set; }
        public SchedulingMode SchedulingMode { get; set; }
        public int ProcessedRecords { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ProcessSample)]
    public class ProcessSampleEvent : ManagementActivityEvent<ProcessSampleEventParameters>
    {
        public ProcessSampleEvent():
            base(ManagementEventCategory.Call, ManagementEvent.ProcessSample)
        {
        }

        public new void Finish()
        {
            ObjectId = Details.BatchId;
            ObjectName = Details.ProjectdId;

            base.Finish();
        }
    }

    [Serializable]
    public class OnInboundCallNotifyEventParameters : ManagementActivityEventDetails
    {
        public int DialerId;
        public int InterviewId;
        public string InboundLinePhoneNumber;
        public string CallerPhoneNumber;
        public string InboundCallId;
        public InboundHandlerOperationType InboundResult;
    }

    [ManagementEvent(ManagementEvent.OnInboundCallNotifyEvent)]
    public class OnInboundCallNotifyEvent : ManagementActivityEvent<OnInboundCallNotifyEventParameters>
    {
        public OnInboundCallNotifyEvent():
            base(ManagementEventCategory.InboundNotification, ManagementEvent.OnInboundCallNotifyEvent)
        {
        }

        public void Save(InboundHandlerOperationType inboundResult)
        {
            Details.InboundResult = inboundResult;

            Save();
        }
    }

    [ManagementEvent(ManagementEvent.OnInboundCallDroppedNotifyEvent)]
    public class OnInboundCallDroppedNotifyEvent : ManagementActivityEvent<OnInboundCallNotifyEventParameters>
    {
        public OnInboundCallDroppedNotifyEvent():
            base(ManagementEventCategory.InboundNotification, ManagementEvent.OnInboundCallDroppedNotifyEvent)
        {
        }

        public void Save(InboundHandlerOperationType inboundResult)
        {
            Details.InboundResult = inboundResult;

            Save();
        }
    }

    [Serializable]
    public class SynchronizeRespondentsEventParameters : ManagementActivityEventDetails
    {
        public int CreatedRecords { get; set; }
        public int DeletedRecords { get; set; }
    }


    [ManagementEventAttribute(ManagementEvent.SynchronizeRespondents)]
    public class SynchronizeRespondentsEvent : ManagementActivityEvent<SynchronizeRespondentsEventParameters>
    {
        public SynchronizeRespondentsEvent(int surveyId, string projectId, BvAsyncOperationQueueEntity entity):
            base(ManagementEventCategory.Survey, ManagementEvent.SynchronizeRespondents)
        {
            Details = new SynchronizeRespondentsEventParameters();
            ObjectId = surveyId;
            ObjectName = projectId;

            this.Supervisor = entity.CreatedBySupervisorName;
        }

        public void Save(int processedRecords)
        {
            Details.CreatedRecords = processedRecords;
            Save();
        }
    }
}