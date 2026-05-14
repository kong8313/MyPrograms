using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface ISchedulingScriptLogger
    {
        void LogError(Exception exception, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason = SchedulingScriptExecutionReason.Unspecified, string currentITS = "", bool notificationSent = false);
    }
}
