using System;
using System.Collections.Generic;
using BvDotNetEngine.Events;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetEngine
{
    public class SchedulingScriptExecutionOptions
    {
        public ICallProvider CallProvider;
        public DateTime EventTime;
        public bool IsExecuteSchedulingScript;
        public bool IsLogToHistory;
        public int BatchID;

        public List<SchedulingScriptNotificatorExceptionDescription> SchedulingScriptNotificatorExceptions;

        public SchedulingScriptExecutionReason ExecutionReason;

        public OperationType opType;
        /// <summary>
        /// New its which should be used for scheduling. 0 meens that should be used current invetview ITS
        /// </summary>
        public int ITS;

        /// <summary>
        /// New LastCallTime. null meens that should be used current invetview LastCallTime
        /// </summary>
        public DateTime? LastCallTime;

        /// <summary>
        /// New LastCallPersonSID. null meens that should be used current invetview LastCallPersonSID
        /// </summary>
        public int? LastCallPersonSID;

        /// <summary>
        /// ID of call center
        /// </summary>
        public int CallCenterID;

        /// <summary>
        /// ID of role that shows who initialized the ITS change in interview history
        /// </summary>
        public int? RoleID;

        /// <summary>
        /// Set to true when scheduling script is executed for the sample update mode
        /// </summary>
        public ProcessSampleMode ProcessSampleMode;

        /// <summary>
        /// Caller telephone number if scheduling script is exeuted for first processing of inbound call.
        /// </summary>
        public string CliNumber;

        /// <summary>
        /// DDI(Direct Inward Dialing) telephone number if scheduling script is exeuted for first processing of inbound call.
        /// </summary>
        public string DdiNumber;

        public BvInterviewTimings Timings;

        public int ConfirmitDuration;

        public int? LinkedInterviewSessionId;
        public int? CallAttemptNumber;

        public CatiDialingAttempt[] DialingAttempts;

        /// <summary>
        /// Action which will be executed in the end of exeuction of scheduling script before saving/aplying changes
        /// </summary>
        public Action<EventSchedule> PostSchedulingAction { get; set; }

        public SchedulingScriptExecutionOptions()
        {
            CallProvider = new CallDatabaseProvider();
            EventTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            IsExecuteSchedulingScript = true;
            IsLogToHistory = true;
            BatchID = 0;
            SchedulingScriptNotificatorExceptions = new List<SchedulingScriptNotificatorExceptionDescription>();
            ExecutionReason = SchedulingScriptExecutionReason.Unspecified;
            ITS = 0;
            LastCallTime = null;
            LastCallPersonSID = null;
            CallCenterID = 0;
            RoleID = null;
            CliNumber = null;
            DdiNumber = null;
            PostSchedulingAction = null;
            Timings = new BvInterviewTimings()
            {
                CallCenterID = 0,
                InterviewDurationTime = 0,
                OpenEndReviewDurationTime = 0,
                TimeCallDelivered = null,
                WaitingTime = 0
            };
            ConfirmitDuration = 0;
            LinkedInterviewSessionId = null;
            CallAttemptNumber = null;
        }
    }
}
