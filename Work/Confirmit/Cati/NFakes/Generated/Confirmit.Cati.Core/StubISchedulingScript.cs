using System;
using BvDotNetScript.Interfaces;

namespace BvDotNetScript.Interfaces.Fakes
{
    public class StubISchedulingScript : ISchedulingScript 
    {
        private ISchedulingScript _inner;

        public StubISchedulingScript()
        {
            _inner = null;
        }

        public ISchedulingScript Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteIEventScheduleDelegate(IEventSchedule BvEvent);
        public ExecuteIEventScheduleDelegate ExecuteIEventSchedule;

        void ISchedulingScript.Execute(IEventSchedule BvEvent)
        {

            if (ExecuteIEventSchedule != null)
            {
                ExecuteIEventSchedule(BvEvent);
            } else if (_inner != null)
            {
                ((ISchedulingScript)_inner).Execute(BvEvent);
            }
        }

    }
}