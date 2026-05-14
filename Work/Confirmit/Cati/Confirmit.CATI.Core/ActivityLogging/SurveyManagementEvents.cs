using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class UpdateSurveyEventParameters : ManagementActivityEventDetails
    {
        public int SchedulingScriptId { get; set; }
        public int StateGroupID { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateSurvey)]
    public class UpdateSurveyEvent : ManagementActivityEvent<UpdateSurveyEventParameters>
    {
        public UpdateSurveyEvent(int surveySid, string projectId, int stateGroupId, int schedulingScriptId):
            base(ManagementEventCategory.Survey, ManagementEvent.UpdateSurvey)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
            Details = new UpdateSurveyEventParameters { SchedulingScriptId = schedulingScriptId, StateGroupID = stateGroupId };
        }
    }

    [ManagementEventAttribute(ManagementEvent.OpenSurvey)]
    public class OpenSurveyEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public OpenSurveyEvent(int surveySid, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.OpenSurvey)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.CloseSurvey)]
    public class CloseSurveyEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public CloseSurveyEvent(int surveySid, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.CloseSurvey)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ShutdownSurvey)]
    public class ShutdownSurveyEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ShutdownSurveyEvent(int surveySid, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.ShutdownSurvey)
        {
            ObjectId = surveySid;
            ObjectName = projectId;
        }
    }

    [Serializable]
    public class SetDialerSurveyParametersEventParameters : ManagementActivityEventDetails
    {
        public List<DialerParameter> Parameters { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SetDialerSurveyParameters)]
    public class SetDialerSurveyParametersEvent : ManagementActivityEvent<SetDialerSurveyParametersEventParameters>
    {
        public SetDialerSurveyParametersEvent(int surveyId, string projectId, IEnumerable<DialerParameter> parameters):
            base(ManagementEventCategory.Survey, ManagementEvent.SetDialerSurveyParameters)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new SetDialerSurveyParametersEventParameters { Parameters = parameters.ToList() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.ResetDialerSurveyParameters)]
    public class ResetDialerSurveyParametersEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ResetDialerSurveyParametersEvent(int surveyId, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.ResetDialerSurveyParameters)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
        }
    }

    [Serializable]
    public class SetSurveySchedulingParameterEventParameters : ManagementActivityEventDetails
    {
        public int ParamId { get; set; }
        public int ParamValue { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SetSurveySchedulingParameters)]
    public class SetSurveySchedulingParametersEvent : ManagementActivityEvent<SetSurveySchedulingParameterEventParameters>
    {
        public SetSurveySchedulingParametersEvent(int surveyId, string projectId, int paramId, int paramValue):
            base(ManagementEventCategory.Survey, ManagementEvent.SetSurveySchedulingParameters)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new SetSurveySchedulingParameterEventParameters { ParamId = paramId, ParamValue = paramValue };
        }
    }

    [ManagementEventAttribute(ManagementEvent.ResetSurveySchedulingParameters)]
    public class ResetSurveySchedulingParametersEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ResetSurveySchedulingParametersEvent(int surveyId, string projectId):
            base(ManagementEventCategory.Survey, ManagementEvent.ResetSurveySchedulingParameters)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
        }
    }

    [Serializable]
    public class SaveConsoleSearchableFieldsEventParameters : ManagementActivityEventDetails
    {
        public string[] Variables { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SaveConsoleSearchableFields)]
    public class SaveConsoleSearchableFieldsEvent : ManagementActivityEvent<SaveConsoleSearchableFieldsEventParameters>
    {
        public SaveConsoleSearchableFieldsEvent(int surveyId, string projectId, string[] variables):
            base(ManagementEventCategory.Survey, ManagementEvent.SaveConsoleSearchableFields)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
            Details = new SaveConsoleSearchableFieldsEventParameters()
            {
                Variables = variables
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.InitializeSurveyMetadataCacheEvent)]
    public class InitializeSurveyMetadataCacheEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public InitializeSurveyMetadataCacheEvent(int surveyId, string projectId):
            base(ManagementEventCategory.System, ManagementEvent.InitializeSurveyMetadataCacheEvent, true)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
        }
    }

    [ManagementEventAttribute(ManagementEvent.ResetSurveyMetadataCacheEvent)]
    public class ResetSurveyMetadataCacheEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ResetSurveyMetadataCacheEvent(int surveyId, string projectId):
            base(ManagementEventCategory.System, ManagementEvent.ResetSurveyMetadataCacheEvent, true)
        {
            ObjectId = surveyId;
            ObjectName = projectId;
        }
    }

    [Serializable]
    public class CallHistoryExportEventParameters : ManagementActivityEventDetails
    {
        public string ProjectIds { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ReplicatedVariables { get; set; }
        public bool IncludeVariables { get; set; }
        public bool IncludeBreakTime { get; set; }
        public bool IncludeLoginLogoutInfo { get; set; }
        public bool IncludeColumnHeadings { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.CallHistoryExport)]
    public class CallHistoryExportEvent : ManagementActivityEvent<CallHistoryExportEventParameters>
    {
        public CallHistoryExportEvent() : base(ManagementEventCategory.Survey, ManagementEvent.CallHistoryExport)
        {
        }
    }

    [Serializable]
    public class CallHistoryDeleteParameters : ManagementActivityEventDetails
    {
        public BvHistoryEntity Entity { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.CallHistoryDelete)]
    public class CallHistoryDeleteEvent : ManagementActivityEvent<CallHistoryDeleteParameters>
    {
        public CallHistoryDeleteEvent(BvHistoryEntity historyEntity) :
            base(ManagementEventCategory.Survey, ManagementEvent.CallHistoryDelete)
        {
            Details = new CallHistoryDeleteParameters
            {
                Entity = historyEntity
            };
        }
    }

    [Serializable]
    public class CallHistoryUpdateParameters : ManagementActivityEventDetails
    {
        public BvHistoryEntity Entity { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.CallHistoryUpdate)]
    public class CallHistoryUpdateEvent : ManagementActivityEvent<CallHistoryUpdateParameters>
    {
        public CallHistoryUpdateEvent(BvHistoryEntity historyEntity):
            base(ManagementEventCategory.Survey, ManagementEvent.CallHistoryUpdate)
        {
            Details = new CallHistoryUpdateParameters
            {
                Entity = historyEntity
            };
        }
    }
}