using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IScheduleRepository
    {
        BvScheduleEntity GetById(int scheduleId);
        BvScheduleEntity GetByName(string name);
        int InsertWithSpecificId(BvScheduleEntity schedule);
    }
}