using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public abstract class InterviewerEventParameters : ManagementActivityEventDetails
    {
        public int[] PagentGroups { get; set; }
        public AgentTaskChoiceMode TaskChoice { get; set; }
        public TaskChoicePermissions? TaskChoicePermissions { get; set; }
        public string Location { get; set; }
        public DialType DialType { get; set; }
        public AgentType AgentType { get; set; }
    }

    [Serializable]
    public class CreateInterviewerEventParameters : InterviewerEventParameters
    {
    }

    [Serializable]
    public class UpdateInterviewerEventParameters : InterviewerEventParameters
    {
    }

    [Serializable]
    public class DeleteInterviewerEventParameters : ManagementActivityEventDetails
    {
        public AgentType AgentType { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.CreateInterviewer)]
    public class CreateInterviewerEvent : ManagementActivityEvent<CreateInterviewerEventParameters>
    {
        public CreateInterviewerEvent(
            int interviewerSid,
            string interviewerName,
            IEnumerable<int> pagentGroups,
            AgentTaskChoiceMode taskChoice,
            TaskChoicePermissions? taskChoicePermissions,
            string location,
            DialType dialType,
            AgentType agentType):
            base(ManagementEventCategory.Interviewer, ManagementEvent.CreateInterviewer)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
            Details = new CreateInterviewerEventParameters
            {
                PagentGroups = pagentGroups.ToArray(),
                TaskChoice = taskChoice,
                TaskChoicePermissions = taskChoicePermissions,
                Location = location,
                DialType = dialType,
                AgentType = agentType
            };
        }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateInterviewer)]
    public class UpdateInterviewerEvent : ManagementActivityEvent<UpdateInterviewerEventParameters>
    {
        public UpdateInterviewerEvent(
            int interviewerSid,
            string interviewerName,
            IEnumerable<int> pagentGroups,
            AgentTaskChoiceMode taskChoice,
            TaskChoicePermissions? taskChoicePermissions,
            string location,
            DialType dialType,
            AgentType agentType):
            base(ManagementEventCategory.Interviewer, ManagementEvent.UpdateInterviewer)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
            Details = new UpdateInterviewerEventParameters
            {
                PagentGroups = pagentGroups.ToArray(),
                TaskChoice = taskChoice,
                TaskChoicePermissions = taskChoicePermissions,
                Location = location,
                DialType = dialType,
                AgentType = agentType
            };
        }
    }

    [Serializable]
    public class InterviewerImportDetails : ManagementActivityEventDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] GroupIds { get; set; }
        public AgentTaskChoiceMode TaskChoice { get; set; }
    }

    [Serializable]
    public class ImportInterviewersEventParameters : ManagementActivityEventDetails
    {
        public InterviewerImportDetails[] Interviewers { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ImportInterviewers)]
    public class ImportInterviewersEvent : ManagementActivityEvent<ImportInterviewersEventParameters>
    {
        public ImportInterviewersEvent(IEnumerable<InterviewerImportDetails> interviewers):
            base(ManagementEventCategory.Interviewer, ManagementEvent.ImportInterviewers)
        {
            Details = new ImportInterviewersEventParameters { Interviewers = interviewers.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteInterviewer)]
    public class DeleteInterviewerEvent : ManagementActivityEvent<DeleteInterviewerEventParameters>
    {
        public DeleteInterviewerEvent(int interviewerSid, string interviewerName, AgentType agentType):
            base(ManagementEventCategory.Interviewer, ManagementEvent.DeleteInterviewer)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
            Details = new DeleteInterviewerEventParameters
            {
                AgentType = agentType
            };
        }
    }

    [Serializable]
    public class ChangeInterviewerTaskChoiceEventParameters : ManagementActivityEventDetails
    {
        public AgentTaskChoiceMode TaskChoice { get; set; }
        public TaskChoicePermissions? TaskChoicePermissions { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ChangeInterviewerTaskChoice)]
    public class ChangeInterviewerTaskChoiceEvent : ManagementActivityEvent<ChangeInterviewerTaskChoiceEventParameters>
    {
        public ChangeInterviewerTaskChoiceEvent(int interviewerSid, string interviewerName, AgentTaskChoiceMode taskChoice, TaskChoicePermissions? permissions):
            base(ManagementEventCategory.Interviewer, ManagementEvent.ChangeInterviewerTaskChoice)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
            Details = new ChangeInterviewerTaskChoiceEventParameters { TaskChoice = taskChoice, TaskChoicePermissions = permissions };
        }
    }

    [Serializable]
    public class ChangeInterviewerLocationEventParameters : ManagementActivityEventDetails
    {
        public string Location { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.ChangeInterviewerLocation)]
    public class ChangeInterviewerLocationEvent : ManagementActivityEvent<ChangeInterviewerLocationEventParameters>
    {
        public ChangeInterviewerLocationEvent(int interviewerSid, string interviewerName, string location):
            base(ManagementEventCategory.Interviewer, ManagementEvent.ChangeInterviewerLocation)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
            Details = new ChangeInterviewerLocationEventParameters { Location = location };
        }
    }

    [ManagementEventAttribute(ManagementEvent.ChangeInterviewerSSOType)]
    public class ChangeInterviewerSoftphoneIntegrationEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ChangeInterviewerSoftphoneIntegrationEvent(int interviewerSid, string interviewerName):
            base(ManagementEventCategory.Interviewer, ManagementEvent.ChangeInterviewerSSOType)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
        }
    }

    [ManagementEvent(ManagementEvent.ChangeInterviewerPassword)]
    public class ChangeInterviewerPasswordEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ChangeInterviewerPasswordEvent(int interviewerSid, string interviewerName):
            base(ManagementEventCategory.Interviewer, ManagementEvent.ChangeInterviewerPassword)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
        }
    }

    [Serializable]
    public class SetInterviewerAutomaticSurveyEventParameters : ManagementActivityEventDetails
    {
        public int SurveySid { get; set; }
        public string SurveyName { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SetInterviewerAutomaticSurvey)]
    public class SetInterviewerAutomaticSurveyEvent : ManagementActivityEvent<SetInterviewerAutomaticSurveyEventParameters>
    {
        public SetInterviewerAutomaticSurveyEvent(int interviewerSid, string interviewerName, int autoSurveySid, string autoSurveyName):
            base(ManagementEventCategory.Interviewer, ManagementEvent.SetInterviewerAutomaticSurvey)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
            Details = new SetInterviewerAutomaticSurveyEventParameters { SurveySid = autoSurveySid, SurveyName = autoSurveyName };
        }
    }

    [ManagementEventAttribute(ManagementEvent.ClearInterviewerAutomaticSurvey)]
    public class ClearInterviewerAutomaticSurveyEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ClearInterviewerAutomaticSurveyEvent(int interviewerSid, string interviewerName):
            base(ManagementEventCategory.Interviewer, ManagementEvent.ClearInterviewerAutomaticSurvey)
        {
            ObjectId = interviewerSid;
            ObjectName = interviewerName;
        }
    }

    public class SetInterviewerDialTypeEventParameters : ManagementActivityEventDetails
    {
        public DialType DialType { get; set; }
        public int[] PersonSiDs { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.SetInterviewerDialType)]
    public class SetInterviewerDialTypeEvent : ManagementActivityEvent<SetInterviewerDialTypeEventParameters>
    {
        public SetInterviewerDialTypeEvent(int[] personSids, DialType dialType):
            base(ManagementEventCategory.Interviewer, ManagementEvent.SetInterviewerDialType)
        {
            Details = new SetInterviewerDialTypeEventParameters { DialType = dialType, PersonSiDs = personSids };
        }
    }

    [Serializable]
    public class InterviewerLockedUnlockedEventParameters : ManagementActivityEventDetails
    {
        public int[] InterviewerIds { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.InterviewerLockedBySupervisor)]
    public class InterviewerLockedBySupervisorEvent : ManagementActivityEvent<InterviewerLockedUnlockedEventParameters>
    {
        public InterviewerLockedBySupervisorEvent(IEnumerable<int> interviewerIds):
            base(ManagementEventCategory.Interviewer, ManagementEvent.InterviewerLockedBySupervisor)
        {
            Details = new InterviewerLockedUnlockedEventParameters { InterviewerIds = interviewerIds.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.InterviewerUnLockedBySupervisor)]
    public class InterviewerUnLockedBySupervisorEvent : ManagementActivityEvent<InterviewerLockedUnlockedEventParameters>
    {
        public InterviewerUnLockedBySupervisorEvent(IEnumerable<int> interviewerIds):
            base(ManagementEventCategory.Interviewer, ManagementEvent.InterviewerUnLockedBySupervisor)
        {
            Details = new InterviewerLockedUnlockedEventParameters { InterviewerIds = interviewerIds.ToArray() };
        }
    }

    [Serializable]
    public abstract class InterviewerGroupEventParameters : ManagementActivityEventDetails
    {
        public int[] PagentGroups { get; set; }
        public int[] ChildInterviewers { get; set; }
    }

    [Serializable]
    public class CreateInterviewerGroupEventParameters : InterviewerGroupEventParameters
    {
    }

    [Serializable]
    public class UpdateInterviewerGroupEventParameters : InterviewerGroupEventParameters
    {
    }

    [ManagementEventAttribute(ManagementEvent.CreateInterviewerGroup)]
    public class CreateInterviewerGroupEvent : ManagementActivityEvent<CreateInterviewerGroupEventParameters>
    {
        public CreateInterviewerGroupEvent(int groupSid, string groupName, IEnumerable<int> pagentGroups, IEnumerable<int> childInterviewers):
            base(ManagementEventCategory.Interviewer, ManagementEvent.CreateInterviewerGroup)
        {
            ObjectId = groupSid;
            ObjectName = groupName;
            Details = new CreateInterviewerGroupEventParameters { PagentGroups = pagentGroups.ToArray(), ChildInterviewers = childInterviewers.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.UpdateInterviewerGroup)]
    public class UpdateInterviewerGroupEvent : ManagementActivityEvent<UpdateInterviewerGroupEventParameters>
    {
        public UpdateInterviewerGroupEvent(int groupSid, string groupName, IEnumerable<int> pagentGroups, IEnumerable<int> childInterviewers):
            base(ManagementEventCategory.Interviewer, ManagementEvent.UpdateInterviewerGroup)
        {
            ObjectId = groupSid;
            ObjectName = groupName;
            Details = new UpdateInterviewerGroupEventParameters { PagentGroups = pagentGroups.ToArray(), ChildInterviewers = childInterviewers.ToArray() };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteInterviewerGroup)]
    public class DeleteInterviewerGroupEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeleteInterviewerGroupEvent(int groupSid, string groupName):
            base(ManagementEventCategory.Interviewer, ManagementEvent.DeleteInterviewerGroup)
        {
            ObjectId = groupSid;
            ObjectName = groupName;
        }
    }
}