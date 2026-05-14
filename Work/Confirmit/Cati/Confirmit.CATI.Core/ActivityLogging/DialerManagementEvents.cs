using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class DialerRequestCallsEventParameters : ManagementActivityEventDetails
    {
        public string RequestId { get; set; }
        public int? GroupId { get; set; }
        public int CallsRequested { get; set; }
        public int CallsSent { get; set; }
        public CallsSelectionAlgorithm CallsSelectionAlgorithm { get; set; }
        public List<GroupInfo> AggregatedGroupsInfo { get; set; }
        public string TenantId { get; set; }
        public bool IsRecording { get; set; }
    }

    [Serializable]
    public class EnableDialerEventParameters : ManagementActivityEventDetails
    {
        public bool IsInvokedFromSupervisor { get; set; }
        public bool IsSuccessful { get; set; }
        
        public string ErrorMessage { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ActivateDialer)]
    public class ActivateDialerEvent : ManagementActivityEvent<EnableDialerEventParameters>
    {
        public ActivateDialerEvent(int dialerId, bool isInvokedFromSupervisor):
            base(ManagementEventCategory.Dialer, ManagementEvent.ActivateDialer)
        {
            ObjectId = dialerId;
            Details = new EnableDialerEventParameters
            {
                IsInvokedFromSupervisor = isInvokedFromSupervisor,
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DectivateDialer)]
    public class DectivateDialerEvent : ManagementActivityEvent<EnableDialerEventParameters>
    {
        public DectivateDialerEvent(int dialerId, bool isInvokedFromSupervisor):
            base(ManagementEventCategory.Dialer, ManagementEvent.DectivateDialer)
        {
            ObjectId = dialerId;
            Details = new EnableDialerEventParameters
            {
                IsInvokedFromSupervisor = isInvokedFromSupervisor,
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.EnableDialer)]
    public class EnableDialerEvent : ManagementActivityEvent<EnableDialerEventParameters>
    {
        public EnableDialerEvent(int dialerId, bool isInvokedFromSupervisor):
            base(ManagementEventCategory.Dialer, ManagementEvent.EnableDialer)
        {
            ObjectId = dialerId;
            Details = new EnableDialerEventParameters
            {
                IsInvokedFromSupervisor = isInvokedFromSupervisor,
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DisableDialer)]
    public class DisableDialerEvent : ManagementActivityEvent<EnableDialerEventParameters>
    {
        public DisableDialerEvent(int dialerId, bool isInvokedFromSupervisor):
            base(ManagementEventCategory.Dialer, ManagementEvent.DisableDialer)
        {
            ObjectId = dialerId;
            Details = new EnableDialerEventParameters
            {
                IsInvokedFromSupervisor = isInvokedFromSupervisor,
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.ReconnectDialer)]
    public class ReconnectDialerEvent : ManagementActivityEvent<EnableDialerEventParameters>
    {
        public ReconnectDialerEvent(int dialerId) : base(ManagementEventCategory.Dialer, ManagementEvent.ReconnectDialer) 
        { 
            ObjectId = dialerId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.StopDialerReconnection)]
    public class StopDialerReconnectionEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public StopDialerReconnectionEvent(int dialerId) : base(ManagementEventCategory.Dialer, ManagementEvent.StopDialerReconnection)
        {
            ObjectId = dialerId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DialerRequestCalls)]
    public class DialerRequestCallsEvent : ManagementActivityEvent<DialerRequestCallsEventParameters>
    {
        public DialerRequestCallsEvent(
            string requestId,
            int? groupId,
            int callsRequested,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            string tenantId,
            int dialerId):
            base(ManagementEventCategory.DialerCommunication, ManagementEvent.DialerRequestCalls, true)
        {
            Details = new DialerRequestCallsEventParameters
            {
                RequestId = requestId,
                GroupId = groupId,
                CallsRequested = callsRequested,
                CallsSelectionAlgorithm = callsSelectionAlgorithm,
                TenantId = tenantId
            };
            Supervisor = dialerId.ToString();
        }
    }

    [Serializable]
    public class SetDialerDefaultSurveyParametersEventParameters : ManagementActivityEventDetails
    {
        public List<DialerParameter> Parameters { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SetDialerDefaultSurveyParameters)]
    public class SetDialerDefaultSurveyParametersEvent : ManagementActivityEvent<SetDialerDefaultSurveyParametersEventParameters>
    {
        public SetDialerDefaultSurveyParametersEvent(IEnumerable<DialerParameter> parameters):
            base(ManagementEventCategory.Dialer, ManagementEvent.SetDialerDefaultSurveyParameters)
        {
            Details = new SetDialerDefaultSurveyParametersEventParameters { Parameters = parameters.ToList() };
        }
    }

    [Serializable]
    public class SetDialerNotificationsEmailEventParameters : ManagementActivityEventDetails
    {
        public string Email { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SetDialerNotificationsEmail)]
    public class SetDialerNotificationsEmailEvent : ManagementActivityEvent<SetDialerNotificationsEmailEventParameters>
    {
        public SetDialerNotificationsEmailEvent(string email):
            base(ManagementEventCategory.Dialer, ManagementEvent.SetDialerNotificationsEmail)
        {
            Details = new SetDialerNotificationsEmailEventParameters { Email = email };
        }
    }
    
    [Serializable]
    public class SetRespondentVariablesToSendToTheDialerEventParameters : ManagementActivityEventDetails
    {
        public string RespondentVariables { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SetRespondentVariablesToSendToDialer)]
    public class SetRespondentVariablesToSendToTheDialerEvent : ManagementActivityEvent<SetRespondentVariablesToSendToTheDialerEventParameters>
    {
        public SetRespondentVariablesToSendToTheDialerEvent(string respondentVariables):
            base(ManagementEventCategory.Dialer, ManagementEvent.SetDialerNotificationsEmail)
        {
            Details = new SetRespondentVariablesToSendToTheDialerEventParameters { RespondentVariables = respondentVariables };
        }
    }

    [Serializable]
    public class SendNumbersEventParameters : ManagementActivityEventDetails
    {
        public string RequestId { get; set; }
        public int? GroupId { get; set; }
        public int CallsRequested { get; set; }
        public int CallsSent { get; set; }
        public CallsSelectionAlgorithm CallsSelectionAlgorithm { get; set; }
        public string TenantId { get; set; }
        public bool IsRecording { get; set; }
        public DialerErrorCode SendNumbersResult { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SendNumbersEvent)]
    public class SendNumbersEvent : ManagementActivityEvent<SendNumbersEventParameters>
    {
        public SendNumbersEvent(
            string requestId,
            int? groupId,
            int callsRequested,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            string tenantId,
            int dialerId):
            base(ManagementEventCategory.DialerCommunication, ManagementEvent.SendNumbersEvent)
        {
            Details = new SendNumbersEventParameters
            {
                RequestId = requestId,
                GroupId = groupId,
                CallsRequested = callsRequested,
                CallsSelectionAlgorithm = callsSelectionAlgorithm,
                TenantId = tenantId
            };
            Supervisor = dialerId.ToString();
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteDialer)]
    public class DeleteDialerEvent : ManagementActivityEvent<DeleteDialerEventParameters>
    {
        public DeleteDialerEvent(BvDialersEntity dialer):
            base(ManagementEventCategory.Dialer, ManagementEvent.DeleteDialer)
        {
            ObjectId = dialer.Id;
            Details = new DeleteDialerEventParameters
            {
                Dialer = dialer,
            };
        }
    }

    [Serializable]
    public class AddDialerEventParameters : ManagementActivityEventDetails
    {
        public BvDialersEntity Dialer { get; set; }
    }

    [Serializable]
    public class DeleteDialerEventParameters : ManagementActivityEventDetails
    {
        public BvDialersEntity Dialer { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AddDialer)]
    public class AddDialerEvent : ManagementActivityEvent<AddDialerEventParameters>
    {
        public AddDialerEvent(BvDialersEntity dialer):
            base(ManagementEventCategory.Dialer, ManagementEvent.AddDialer)
        {
            ObjectId = dialer.Id;
            Details = new AddDialerEventParameters
            {
                Dialer = dialer,
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.EditDialer)]
    public class EditDialerEvent : ManagementActivityEvent<EditDialerEventParameters>
    {
        public EditDialerEvent(BvDialersEntity entity):
            base(ManagementEventCategory.Dialer, ManagementEvent.EditDialer)
        {
            ObjectId = entity.Id;
            Details = new EditDialerEventParameters
            {
                BeforeChanging = entity
            };
        }
    }

    [Serializable]
    public class EditDialerEventParameters : ManagementActivityEventDetails
    {
        public BvDialersEntity BeforeChanging { get; set; }
        public BvDialersEntity AfterChanging { get; set; }
    }

    [Serializable]
    public class GetLogFilesEventParameters : ManagementActivityEventDetails
    {
        public bool IsSuccessful { get; set; }
        public int Count { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.GetLogFilesEvent)]
    public class GetLogFilesEvent : ManagementActivityEvent<GetLogFilesEventParameters>
    {
        public GetLogFilesEvent(int dialerId):
            base(ManagementEventCategory.Dialer, ManagementEvent.GetLogFilesEvent)
        {
            ObjectId = dialerId;
            Details = new GetLogFilesEventParameters();
        }
    }

    [Serializable]
    public class GetLogFileBodyZippedEventParameters : ManagementActivityEventDetails
    {
        public bool IsSuccessful { get; set; }
        public string FileName { get; set; }
        public int Length { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.GetLogFileBodyZippedEvent)]
    public class GetLogFileBodyZippedEvent : ManagementActivityEvent<GetLogFileBodyZippedEventParameters>
    {
        public GetLogFileBodyZippedEvent(int dialerId, string fileName):
            base(ManagementEventCategory.Dialer, ManagementEvent.GetLogFileBodyZippedEvent)
        {
            ObjectId = dialerId;
            Details = new GetLogFileBodyZippedEventParameters
            {
                FileName = fileName
            };
        }
    }

    [ManagementEvent(ManagementEvent.GetAvailableExtendedFunctionalityEvent)]
    public class GetAvailableExtendedFunctionalityEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public GetAvailableExtendedFunctionalityEvent(int dialerId):
            base(ManagementEventCategory.Dialer, ManagementEvent.GetAvailableExtendedFunctionalityEvent)
        {
            ObjectId = dialerId;
            Details = new NoManagementParameters();
        }
    }

    [ManagementEvent(ManagementEvent.GetDialerSupportedFeaturesEvent)]
    public class GetDialerSupportedFeaturesEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public GetDialerSupportedFeaturesEvent(int dialerId):
            base(ManagementEventCategory.Dialer, ManagementEvent.GetDialerSupportedFeaturesEvent)
        {
            ObjectId = dialerId;
            Details = new NoManagementParameters();
        }
    }

    [ManagementEvent(ManagementEvent.GetOverridenDialerSupportedFeaturesEvent)]
    public class GetOverridenDialerSupportedFeaturesEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public GetOverridenDialerSupportedFeaturesEvent(int dialerId):
            base(ManagementEventCategory.Dialer, ManagementEvent.GetOverridenDialerSupportedFeaturesEvent)
        {
            ObjectId = dialerId;
            Details = new NoManagementParameters();
        }
    }

    [ManagementEvent(ManagementEvent.UpdateOverridenDialerSupportedFeatureEvent)]
    public class UpdateOverridenDialerSupportedFeatureEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public UpdateOverridenDialerSupportedFeatureEvent(int dialerId):
            base(ManagementEventCategory.Dialer, ManagementEvent.UpdateOverridenDialerSupportedFeatureEvent)
        {
            ObjectId = dialerId;
            Details = new NoManagementParameters();
        }
    }
}