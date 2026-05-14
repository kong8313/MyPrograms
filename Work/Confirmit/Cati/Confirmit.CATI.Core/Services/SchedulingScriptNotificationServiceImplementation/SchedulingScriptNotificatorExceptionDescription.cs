using System;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation
{
    public class SchedulingScriptNotificatorExceptionDescription
    {
        public string RuleNumber { get; private set; }
        public string Action { get; private set; }
        public string Message { get; private set; }
        public int InterviewId { get; private set; }
        public string ExecutionReason { get; private set; }
        public string ExtendedStatus { get; private set; }
        /// <summary>
        /// The local server time when the InnerException is registered.
        /// </summary>
        public DateTime HappenedAt { get; private set; }

        public SchedulingScriptNotificatorExceptionDescription(int interviewId, Exception exception, SchedulingScriptExecutionReason executionReason = SchedulingScriptExecutionReason.Unspecified, string extendedStatus = "")
        {
            ParameterValidator.ValidateNotNull(exception, "exception");
            InterviewId = interviewId;
            Message = exception.Message;
            if (exception is SchedulingExecutionException exceptionWithContext)
            {
                RuleNumber = $"{exceptionWithContext.RuleNumber + 1}.{exceptionWithContext.SubRuleNumber + 1}";
                Action = $"{exceptionWithContext.ActionNumber + 1}";
            }

            ExecutionReason = SchedulingScriptExecutionReasonConverter.ConvertToString(executionReason);
            ExtendedStatus = extendedStatus;
            HappenedAt = TimezoneManager.ConvertToTzLocalTime(
                TimezoneManager.GetDefaultCallCenterTimezoneId(),
                DateTime.UtcNow);
        }

        public SchedulingScriptNotificatorExceptionDescription(BvScheduleErrorEntity entity)
        {
            InterviewId = entity.InterviewId;
            Message = entity.Message;
            if (!string.IsNullOrEmpty(entity.RuleNumber))
            {
                RuleNumber = entity.RuleNumber;
                Action = entity.Action;
            }
            ExtendedStatus = entity.ExtendedStatus;
            ExecutionReason = entity.TriggeredBy;

            HappenedAt = TimezoneManager.ConvertToTzLocalTime(
                TimezoneManager.GetDefaultCallCenterTimezoneId(),
                entity.Timestamp);
        }
    }
}