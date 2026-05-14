using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [ManagementEventAttribute(ManagementEvent.Schedule)]
    public class ScheduleEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ScheduleEvent():
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.Schedule, true)
        {
        }
    }

    [Serializable]
    public class QuotaBalancingEventParameters : ManagementActivityEventDetails
    {
        public class PromotedCell
        {
            public int QuotaId;

            public int CellId;

            public string CellInfo;

            public int Count;
        }
        

        public List<PromotedCell> PromotedCells = new List<PromotedCell>();
    }
    
    [Serializable]
    public class SetQuotaBalancingEventParameters : ManagementActivityEventDetails
    {
        public QuotaBalancingConfiguration Configuration;
    }

    [ManagementEventAttribute(ManagementEvent.SetQuotaBalancing)]
    public class SetQuotaBalancingEvent : ManagementActivityEvent<SetQuotaBalancingEventParameters>
    {
        public SetQuotaBalancingEvent(int surveySid, string surveyName, QuotaBalancingConfiguration configuration):
            base(ManagementEventCategory.Quota, ManagementEvent.SetQuotaBalancing)
        {
            ObjectId = surveySid;
            ObjectName = surveyName;
            Details = new SetQuotaBalancingEventParameters
            {
                Configuration = configuration
            };
        }

    }

    [ManagementEventAttribute(ManagementEvent.ResetQuotaBalancing)]
    public class ResetQuotaBalancingEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ResetQuotaBalancingEvent(int surveySid, string surveyName):
            base(ManagementEventCategory.Quota, ManagementEvent.ResetQuotaBalancing)
        {
            ObjectId = surveySid;
            ObjectName = surveyName;
        }
    }

    [ManagementEventAttribute(ManagementEvent.PeriodicalReplication)]
    public class PeriodicalReplicationEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public PeriodicalReplicationEvent():
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.PeriodicalReplication, true)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.SurveyReplication)]
    public class SurveyReplicationEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public SurveyReplicationEvent():
            base(ManagementEventCategory.Survey, ManagementEvent.SurveyReplication, true)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.AutoLogoutThread)]
    public class AutoLogoutThreadEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public AutoLogoutThreadEvent():
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.AutoLogoutThread, true)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.AutoLogoutWebConsoleThread)]
    public class AutoLogoutWebConsoleThreadEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public AutoLogoutWebConsoleThreadEvent():
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.AutoLogoutWebConsoleThread, true)
        {
        }
    }

    [Serializable]
    public class TerminateTaskWhileAutoLogoutParameters : ManagementActivityEventDetails
    {
        public BvTasksEntity Task { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.TerminateTaskWhileAutoLogout)]
    public class TerminateTaskWhileAutoLogoutEvent : ManagementActivityEvent<TerminateTaskWhileAutoLogoutParameters>
    {
        public TerminateTaskWhileAutoLogoutEvent(int interviewerId, string interviewerName, BvTasksEntity task):
            base(ManagementEventCategory.InterviewerSession, ManagementEvent.TerminateTaskWhileAutoLogout, true)
        {
            ObjectId = interviewerId;
            ObjectName = interviewerName;
            Details = new TerminateTaskWhileAutoLogoutParameters { Task = task };
        }
    }

    [ManagementEventAttribute(ManagementEvent.DialerHealthControlThread)]
    public class DialerHealthControlThreadEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public DialerHealthControlThreadEvent():
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.DialerHealthControlThread, true)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.ScheduledReportEmail)]
    public class ScheduledReportEmailEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public ScheduledReportEmailEvent(ReportType reportType):
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.ScheduledReportEmail, true)
        {
            ObjectId = (int)reportType;
            ObjectName = reportType.ToString();
        }
    }

    [Serializable]
    public class AsyncOperationDequeueEventParameters : ManagementActivityEventDetails
    {
        public BvAsyncOperationQueueEntity AsyncOperationEntity { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AsyncOperationDequeue)]
    public class AsyncOperationDequeueEvent : ManagementActivityEvent<AsyncOperationDequeueEventParameters>
    {
        public AsyncOperationDequeueEvent() : 
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.AsyncOperationDequeue, true)
        {
        }
    }

    [Serializable]
    public class AsyncOperationAbortEventParameters : ManagementActivityEventDetails
    {
        public BvAsyncOperationQueueEntity AsyncOperationEntity { get; set; }
    }

    [ManagementEventAttribute(ManagementEvent.AsyncOperationAbort)]
    public class AsyncOperationAbortEvent : ManagementActivityEvent<AsyncOperationAbortEventParameters>
    {
        public AsyncOperationAbortEvent() : 
            base(ManagementEventCategory.System, ManagementEvent.AsyncOperationAbort, true)
        {
        }
    }

    [Serializable]
    public class RoutineMaintenanceEventParameters : ManagementActivityEventDetails
    {
    }

    [ManagementEventAttribute(ManagementEvent.RoutineMaintenance)]
    public class RoutineMaintenanceEvent : ManagementActivityEvent<RoutineMaintenanceEventParameters>
    {
        public RoutineMaintenanceEvent() : 
            base(ManagementEventCategory.BackgroundTasks, ManagementEvent.RoutineMaintenance, true)
        {
        }
    }

    [ManagementEventAttribute(ManagementEvent.RereadReplication)]
    public class RereadReplicationEvent : ManagementActivityEvent<NoManagementParameters>
    {
        public RereadReplicationEvent():
            base(ManagementEventCategory.Survey, ManagementEvent.RereadReplication, true)
        {
        }
    }
}