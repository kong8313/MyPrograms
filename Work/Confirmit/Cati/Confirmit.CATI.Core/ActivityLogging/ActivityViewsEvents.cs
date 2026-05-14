using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [Serializable]
    public class SetActivityAlertEventParameters : ManagementActivityEventDetails
    {
        public int WarningThreshold { get; set; }
        public int RedThreshold { get; set; }
    }

    [Serializable]
    public class SetActivityStatusAlertEventParameters : ManagementActivityEventDetails
    {
        public int WarningThreshold { get; set; }
        public int RedThreshold { get; set; }
    }

    [Serializable]
    public class SetAppointmentListIntervalsEventParameters : ManagementActivityEventDetails
    {
        public TimeSpan LongInterval { get; set; }
        public TimeSpan ShortInterval { get; set; }
    }

    [Serializable]
    public class StartVideoMonitoringEventParameters : ManagementActivityEventDetails
    {
        public string ProjectId { get; set; }
    }

    [Serializable]
    public class StopVideoMonitoringEventParameters : ManagementActivityEventDetails
    {
        public long MonitoringSessionID { get; set; }
    }

    [Serializable]
    public class StartAudioMonitoringEventParameters : ManagementActivityEventDetails
    {
        public string TelephoneNumber { get; set; }

        public string SupervisorName { get; set; }
    }

    [Serializable]
    public class TerminateTaskEventParameters : ManagementActivityEventDetails
    {
        public BvTasksEntity Task { get; set; }
    }

    [Serializable]
    public class TerminateTaskWithReasonEventParameters : TerminateTaskEventParameters
    {
        public string Reason { get; set; }
    }
   
    [ManagementEventAttribute(ManagementEvent.SetActivityAlert)]
    public class SetActivityAlertEvent : ManagementActivityEvent<SetActivityAlertEventParameters>
    {
        public SetActivityAlertEvent(int alertId, string alertName, int warningThreshold, int redThreshold):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.SetActivityAlert)
        {
            ObjectId = alertId;
            ObjectName = alertName;
            Details = new SetActivityAlertEventParameters { WarningThreshold = warningThreshold, RedThreshold = redThreshold };
        }
    }

    [ManagementEventAttribute(ManagementEvent.SetActivityStatusAlert)]
    public class SetActivityStatusAlertEvent : ManagementActivityEvent<SetActivityStatusAlertEventParameters>
    {
        public SetActivityStatusAlertEvent(int alertId, string alertName, int warningThreshold, int redThreshold):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.SetActivityStatusAlert)
        {
            ObjectId = alertId;
            ObjectName = alertName;
            Details = new SetActivityStatusAlertEventParameters { WarningThreshold = warningThreshold, RedThreshold = redThreshold };
        }
    }

    [ManagementEventAttribute(ManagementEvent.SetAppointmentListIntervals)]
    public class SetAppointmentListIntervalsEvent : ManagementActivityEvent<SetAppointmentListIntervalsEventParameters>
    {
        public SetAppointmentListIntervalsEvent(TimeSpan shortInterval, TimeSpan longInterval):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.SetAppointmentListIntervals)
        {
            ObjectId = 0;
            ObjectName = String.Empty;
            Details = new SetAppointmentListIntervalsEventParameters { LongInterval = longInterval, ShortInterval = shortInterval };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteActivityAlert)]
    public class DeleteActivityAlertEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeleteActivityAlertEvent(int alertId, string alertName):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.DeleteActivityAlert)
        {
            ObjectId = alertId;
            ObjectName = alertName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.DeleteActivityStatusAlert)]
    public class DeleteActivityStatusAlertEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DeleteActivityStatusAlertEvent(int alertId, string alertName):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.DeleteActivityStatusAlert)
        {
            ObjectId = alertId;
            ObjectName = alertName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.TerminateTask)]
    public class TerminateTaskEvent : ManagementActivityEvent<TerminateTaskEventParameters>
    {
        public TerminateTaskEvent(int interviewerId, string interviewerName, BvTasksEntity task):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.TerminateTask)
        {
            ObjectId = interviewerId;
            ObjectName = interviewerName;

            Details = new TerminateTaskEventParameters { Task = task };
        }
    }

    [ManagementEventAttribute(ManagementEvent.TerminateTaskWithReason)]
    public class TerminateTaskWithReasonEvent : ManagementActivityEvent<TerminateTaskWithReasonEventParameters>
    {
        public TerminateTaskWithReasonEvent(int interviewerId, string interviewerName, BvTasksEntity task, string reason):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.TerminateTaskWithReason)
        {
            ObjectId = interviewerId;
            ObjectName = interviewerName;

            Details = new TerminateTaskWithReasonEventParameters { Task = task, Reason = reason };
        }
    }

    [ManagementEventAttribute(ManagementEvent.StartVideoMonitoring)]
    public class StartVideoMonitoringEvent : ManagementActivityEvent<StartVideoMonitoringEventParameters>
    {
        public StartVideoMonitoringEvent(int interviewerId, string interviewerName, string projectId):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.StartVideoMonitoring)
        {
            ObjectId = interviewerId;
            ObjectName = interviewerName;
            Details = new StartVideoMonitoringEventParameters { ProjectId = projectId };
        }
    }

    [ManagementEventAttribute(ManagementEvent.StopVideoMonitoring)]
    public class StopVideoMonitoringEvent : ManagementActivityEvent<StopVideoMonitoringEventParameters>
    {
        public StopVideoMonitoringEvent(int interviewerId, string interviewerName, long monitoringSessionId):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.StopVideoMonitoring)
        {
            ObjectId = interviewerId;
            ObjectName = interviewerName;
            Details = new StopVideoMonitoringEventParameters { MonitoringSessionID = monitoringSessionId };
        }
    }

    [ManagementEventAttribute(ManagementEvent.StartAudioMonitoring)]
    public class StartAudioMonitoringEvent : ManagementActivityEvent<StartAudioMonitoringEventParameters>
    {
        public StartAudioMonitoringEvent(string supervisorName, int interviewerId, string interviewerName, string telephoneNumber):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.StartAudioMonitoring)
        {
            ObjectId = interviewerId;
            ObjectName = interviewerName;
            Details = new StartAudioMonitoringEventParameters { SupervisorName = supervisorName, TelephoneNumber = telephoneNumber };
        }
    }

    [ManagementEventAttribute(ManagementEvent.StopAudioMonitoring)]
    public class StopAudioMonitoringEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public StopAudioMonitoringEvent(int interviewerId, string interviewerName):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.StopAudioMonitoring)
        {
            ObjectId = interviewerId;
            ObjectName = interviewerName;
        }
    }
}