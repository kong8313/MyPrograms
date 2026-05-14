using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Supervisor.Script.Classes
{
    public interface IScheduleManager
    {
        Schedule DeserializeSchedule(string xmlSchedule);
    }
}