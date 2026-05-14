using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleDialProcessor : IConsoleDialProcessor 
    {
        private IConsoleDialProcessor _inner;

        public StubIConsoleDialProcessor()
        {
            _inner = null;
        }

        public IConsoleDialProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool DialBvPersonEntityBvTasksEntityStringInt32DialEventDelegate(BvPersonEntity person, BvTasksEntity task, string phoneNumber, int attemptNumber, DialEvent activityEvent);
        public DialBvPersonEntityBvTasksEntityStringInt32DialEventDelegate DialBvPersonEntityBvTasksEntityStringInt32DialEvent;

        bool IConsoleDialProcessor.Dial(BvPersonEntity person, BvTasksEntity task, string phoneNumber, int attemptNumber, DialEvent activityEvent)
        {


            if (DialBvPersonEntityBvTasksEntityStringInt32DialEvent != null)
            {
                return DialBvPersonEntityBvTasksEntityStringInt32DialEvent(person, task, phoneNumber, attemptNumber, activityEvent);
            } else if (_inner != null)
            {
                return ((IConsoleDialProcessor)_inner).Dial(person, task, phoneNumber, attemptNumber, activityEvent);
            }

            return default(bool);
        }

    }
}