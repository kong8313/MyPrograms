using System;

namespace Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InterviewerActivityEventAttribute : Attribute
    {
        private readonly InterviewerActivityEventType _currentEvent;

        public InterviewerActivityEventAttribute(InterviewerActivityEventType currentEvent)
        {
            _currentEvent = currentEvent;
        }

        public InterviewerActivityEventType CurrentEvent
        {
            get
            {
                return _currentEvent;
            }
        }
    }
}
