using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ITimezoneRepository
    {
        BvTimezoneEntity Get(int timezoneId);
        BvTimezoneEntity GetMasterTimezone(int timezoneId);
        List<BvTimezoneEntity> GetActiveList();
        List<BvTimezoneEntity> GetMasterList();
        List<BvTimezoneEntity> GetMasterListFromDefaultDatabase();
        void InsertMasterEntity(BvTimezoneEntity entity);
        void UpdateMasterEntity(BvTimezoneEntity entity, bool isActiveTimezone);
        List<BvTimezoneEntity> GetCustomTimezones(int parentTimezoneId);
        void InsertCustomTimezone(BvTimezoneEntity customTimezone);
        void UpdateCustomTimezone(BvTimezoneEntity customTimezone);
    }
}
