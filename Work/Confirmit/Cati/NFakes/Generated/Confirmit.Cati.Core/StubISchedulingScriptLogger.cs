using System;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISchedulingScriptLogger : ISchedulingScriptLogger 
    {
        private ISchedulingScriptLogger _inner;

        public StubISchedulingScriptLogger()
        {
            _inner = null;
        }

        public ISchedulingScriptLogger Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void LogErrorExceptionInt32Int32Int32SchedulingScriptExecutionReasonStringBooleanDelegate(Exception exception, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason, string currentITS, bool notificationSent);
        public LogErrorExceptionInt32Int32Int32SchedulingScriptExecutionReasonStringBooleanDelegate LogErrorExceptionInt32Int32Int32SchedulingScriptExecutionReasonStringBoolean;

        void ISchedulingScriptLogger.LogError(Exception exception, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason, string currentITS, bool notificationSent)
        {

            if (LogErrorExceptionInt32Int32Int32SchedulingScriptExecutionReasonStringBoolean != null)
            {
                LogErrorExceptionInt32Int32Int32SchedulingScriptExecutionReasonStringBoolean(exception, interviewId, surveyId, scheduleId, executionReason, currentITS, notificationSent);
            } else if (_inner != null)
            {
                ((ISchedulingScriptLogger)_inner).LogError(exception, interviewId, surveyId, scheduleId, executionReason, currentITS, notificationSent);
            }
        }

    }
}