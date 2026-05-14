using System;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation.Fakes
{
    public class StubISchedulingScriptNotificator : ISchedulingScriptNotificator 
    {
        private ISchedulingScriptNotificator _inner;

        public StubISchedulingScriptNotificator()
        {
            _inner = null;
        }

        public ISchedulingScriptNotificator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void NotifyIfNeededExceptionInt32Int32Int32Int32SchedulingScriptExecutionReasonStringDelegate(Exception exception, int batchId, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason, string currentITS);
        public NotifyIfNeededExceptionInt32Int32Int32Int32SchedulingScriptExecutionReasonStringDelegate NotifyIfNeededExceptionInt32Int32Int32Int32SchedulingScriptExecutionReasonString;

        void ISchedulingScriptNotificator.NotifyIfNeeded(Exception exception, int batchId, int interviewId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason, string currentITS)
        {

            if (NotifyIfNeededExceptionInt32Int32Int32Int32SchedulingScriptExecutionReasonString != null)
            {
                NotifyIfNeededExceptionInt32Int32Int32Int32SchedulingScriptExecutionReasonString(exception, batchId, interviewId, surveyId, scheduleId, executionReason, currentITS);
            } else if (_inner != null)
            {
                ((ISchedulingScriptNotificator)_inner).NotifyIfNeeded(exception, batchId, interviewId, surveyId, scheduleId, executionReason, currentITS);
            }
        }

        public delegate void NotifyListOfSchedulingScriptNotificatorExceptionDescriptionInt32Int32Int32SchedulingScriptExecutionReasonStringDelegate(List<SchedulingScriptNotificatorExceptionDescription> exceptionList, int batchId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason, string currentITS);
        public NotifyListOfSchedulingScriptNotificatorExceptionDescriptionInt32Int32Int32SchedulingScriptExecutionReasonStringDelegate NotifyListOfSchedulingScriptNotificatorExceptionDescriptionInt32Int32Int32SchedulingScriptExecutionReasonString;

        void ISchedulingScriptNotificator.Notify(List<SchedulingScriptNotificatorExceptionDescription> exceptionList, int batchId, int surveyId, int scheduleId, SchedulingScriptExecutionReason executionReason, string currentITS)
        {

            if (NotifyListOfSchedulingScriptNotificatorExceptionDescriptionInt32Int32Int32SchedulingScriptExecutionReasonString != null)
            {
                NotifyListOfSchedulingScriptNotificatorExceptionDescriptionInt32Int32Int32SchedulingScriptExecutionReasonString(exceptionList, batchId, surveyId, scheduleId, executionReason, currentITS);
            } else if (_inner != null)
            {
                ((ISchedulingScriptNotificator)_inner).Notify(exceptionList, batchId, surveyId, scheduleId, executionReason, currentITS);
            }
        }

    }
}