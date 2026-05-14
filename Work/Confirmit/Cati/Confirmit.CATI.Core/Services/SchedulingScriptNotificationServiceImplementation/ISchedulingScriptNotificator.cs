using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation
{
    public interface ISchedulingScriptNotificator
    {
        void NotifyIfNeeded(Exception exception, int batchId, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason = SchedulingScriptExecutionReason.Unspecified, string currentITS = "");

        void Notify(List<SchedulingScriptNotificatorExceptionDescription> exceptionList, int batchId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason = SchedulingScriptExecutionReason.Unspecified, string currentITS = "");
    }
}