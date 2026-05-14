using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ISystemSettingRepository
    {
        string Get(string settingSystemName, int companyId);

        BvSystemSettingsEntity GetSettingForCurrentCompany(string systemName);
        IEnumerable<BvSystemSettingsEntity> GetAllSettingsForCurrentCompany();

        void InsertSettingForCurrentCompany(BvSystemSettingsEntity entity);
        void UpdateSettingForCurrentCompany(BvSystemSettingsEntity entity);
        void DeleteSettingForCurrentCompany(string systemName);

        BvSystemSettingsEntity GetSettingForDefaultCompany(string systemName);
        IEnumerable<BvSystemSettingsEntity> GetAllSettingsForDefaultCompany();

        void InsertSettingForDefaultCompany(BvSystemSettingsEntity entity);
        void UpdateSettingForDefaultCompany(BvSystemSettingsEntity entity);
        void DeleteSettingForDefaultCompany(string systemName);
    }
}