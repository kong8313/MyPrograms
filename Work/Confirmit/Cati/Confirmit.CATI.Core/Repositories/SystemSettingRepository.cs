using System.Collections.Generic;
using System.Data.SqlClient;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private const int DefaultCompanyId = 0;

        private readonly IConnectionStrings _connectionStrings;
        private readonly ICompanyInfo _companyInfo;

        public SystemSettingRepository(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
            _companyInfo = ServiceLocator.Resolve<ICompanyInfo>();
        }

        public string Get(string settingSystemName, int companyId)
        {
            var setting = GetSetting(settingSystemName, companyId);

            if (companyId != 0 && setting == null)
            {
                setting = GetSetting(settingSystemName, DefaultCompanyId);
            }

            return setting.Value;
        }

        public BvSystemSettingsEntity GetSettingForCurrentCompany(string systemName)
        {
            return GetSetting(systemName, _companyInfo.CompanyId);
        }

        public IEnumerable<BvSystemSettingsEntity> GetAllSettingsForCurrentCompany()
        {
            return GetAllSettings(_companyInfo.CompanyId);
        }

        public void InsertSettingForCurrentCompany(BvSystemSettingsEntity entity)
        {
            Insert(entity, _companyInfo.CompanyId);
        }

        public void UpdateSettingForCurrentCompany(BvSystemSettingsEntity entity)
        {
            Update(entity, _companyInfo.CompanyId);
        }

        public void DeleteSettingForCurrentCompany(string systemName)
        {
            Delete(systemName, _companyInfo.CompanyId);
        }

        public BvSystemSettingsEntity GetSettingForDefaultCompany(string systemName)
        {
            return GetSetting(systemName, DefaultCompanyId);
        }

        public IEnumerable<BvSystemSettingsEntity> GetAllSettingsForDefaultCompany()
        {
            return GetAllSettings(DefaultCompanyId);
        }

        public void InsertSettingForDefaultCompany(BvSystemSettingsEntity entity)
        {
            Insert(entity, DefaultCompanyId);
        }

        public void UpdateSettingForDefaultCompany(BvSystemSettingsEntity entity)
        {
            Update(entity, DefaultCompanyId);
        }

        public void DeleteSettingForDefaultCompany(string systemName)
        {
            Delete(systemName, DefaultCompanyId);
        }

        [CanBeNull]
        private BvSystemSettingsEntity GetSetting(string systemName, int companyId)
        {
            // Don't use ConnectionScope here because this code can be used within an another connection scope with different connection settings
            // This situation can be a reason of ecxeption in connection scope

            var query = BvSystemSettingsAdapter.selectSql + " WHERE SystemName = @SystemName";
            var connectionString = _connectionStrings.GetConnectionStringForSpecificCompany(companyId);

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@SystemName", systemName));
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    return BvSystemSettingsAdapter.Read(reader);
                }
            }
        }

        private IEnumerable<BvSystemSettingsEntity> GetAllSettings(int companyId)
        {
            using (new ConnectionScope(
                _connectionStrings.GetConnectionStringForSpecificCompany(companyId)))
            {
                return BvSystemSettingsAdapter.GetAll();
            }
        }

        private void Insert(BvSystemSettingsEntity entity, int companyId)
        {
            using (new ConnectionScope(
                _connectionStrings.GetConnectionStringForSpecificCompany(companyId)))
            {
                BvSystemSettingsAdapter.Insert(entity);
            }

            ServiceLocator.Resolve<ISystemSettingCache>().Reset();
        }

        private void Update(BvSystemSettingsEntity entity, int companyId)
        {
            using (new ConnectionScope(
                _connectionStrings.GetConnectionStringForSpecificCompany(companyId)))
            {
                BvSystemSettingsAdapter.Update(entity);
            }

            ServiceLocator.Resolve<ISystemSettingCache>().Reset();
        }

        private void Delete(string systemName, int companyId)
        {
            using (new ConnectionScope(
                _connectionStrings.GetConnectionStringForSpecificCompany(companyId)))
            {
                BvSystemSettingsAdapter.DeleteByCondition("SystemName = @SystemName", new SqlParameter("@SystemName", systemName));
            }

            ServiceLocator.Resolve<ISystemSettingCache>().Reset();
        }
    }
}