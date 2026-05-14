using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Telephony.Console;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleWrapUpProcessor : IConsoleWrapUpProcessor 
    {
        private IConsoleWrapUpProcessor _inner;

        public StubIConsoleWrapUpProcessor()
        {
            _inner = null;
        }

        public IConsoleWrapUpProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void WrapUpBvPersonEntityBvTasksEntityInt32BooleanInt32CompletedInterviewDetailsWrapUpReasonWrapUpEventBvActiveDialEntityDelegate(BvPersonEntity person, BvTasksEntity task, int interviewId, bool lookUpForNewCalls, int attemptNumber, CompletedInterviewDetails details, WrapUpReason reason, WrapUpEvent activityEvent, BvActiveDialEntity deletedActiveDial);
        public WrapUpBvPersonEntityBvTasksEntityInt32BooleanInt32CompletedInterviewDetailsWrapUpReasonWrapUpEventBvActiveDialEntityDelegate WrapUpBvPersonEntityBvTasksEntityInt32BooleanInt32CompletedInterviewDetailsWrapUpReasonWrapUpEventBvActiveDialEntity;

        void IConsoleWrapUpProcessor.WrapUp(BvPersonEntity person, BvTasksEntity task, int interviewId, bool lookUpForNewCalls, int attemptNumber, CompletedInterviewDetails details, WrapUpReason reason, WrapUpEvent activityEvent, BvActiveDialEntity deletedActiveDial)
        {

            if (WrapUpBvPersonEntityBvTasksEntityInt32BooleanInt32CompletedInterviewDetailsWrapUpReasonWrapUpEventBvActiveDialEntity != null)
            {
                WrapUpBvPersonEntityBvTasksEntityInt32BooleanInt32CompletedInterviewDetailsWrapUpReasonWrapUpEventBvActiveDialEntity(person, task, interviewId, lookUpForNewCalls, attemptNumber, details, reason, activityEvent, deletedActiveDial);
            } else if (_inner != null)
            {
                ((IConsoleWrapUpProcessor)_inner).WrapUp(person, task, interviewId, lookUpForNewCalls, attemptNumber, details, reason, activityEvent, deletedActiveDial);
            }
        }

    }
}