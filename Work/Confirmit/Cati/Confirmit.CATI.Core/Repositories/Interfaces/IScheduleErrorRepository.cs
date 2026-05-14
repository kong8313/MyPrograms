using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IScheduleErrorRepository
    {
        void Insert(BvScheduleErrorEntity entity);
        int GetErrorsCountByScheduleID(int scheduleId);
        BvScheduleErrorEntity GetByRowNumber(int rowNumber, int scheduleId);
        List<BvScheduleErrorEntity> GetByScheduleId(int scheduleId);
        List<BvScheduleErrorEntity> GetNotSentErrors();
        void SetNotificationSent(IEnumerable<int> ids);
        void DeleteOldErrors(BvScheduleErrorEntity lastErrorToDelete, int scheduleId);
    }
}
