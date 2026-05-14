using System;

namespace Confirmit.CATI.Core.ActivityLogging
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ManagementEventAttribute : Attribute
    {
        private readonly ManagementEvent _currentEvent;

        public ManagementEventAttribute(ManagementEvent currentEvent)
        {
            _currentEvent = currentEvent;
        }   
 
        public ManagementEvent CurrentEvent
        {
            get
            {
                return _currentEvent;
            }
        }
    }
}
