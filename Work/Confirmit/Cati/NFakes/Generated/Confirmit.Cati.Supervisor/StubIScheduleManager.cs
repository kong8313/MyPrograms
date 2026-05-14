using System;
using Confirmit.CATI.Supervisor.Script.Classes;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Supervisor.Script.Classes.Fakes
{
    public class StubIScheduleManager : IScheduleManager 
    {
        private IScheduleManager _inner;

        public StubIScheduleManager()
        {
            _inner = null;
        }

        public IScheduleManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Schedule DeserializeScheduleStringDelegate(string xmlSchedule);
        public DeserializeScheduleStringDelegate DeserializeScheduleString;

        Schedule IScheduleManager.DeserializeSchedule(string xmlSchedule)
        {


            if (DeserializeScheduleString != null)
            {
                return DeserializeScheduleString(xmlSchedule);
            } else if (_inner != null)
            {
                return ((IScheduleManager)_inner).DeserializeSchedule(xmlSchedule);
            }

            return default(Schedule);
        }

    }
}