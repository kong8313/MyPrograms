using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.DAL.Handmade.Cache
{
    public class SystemSettingCache : ISystemSettingCache, ITableCache
    {
        private readonly IIpFilterCache _ipFilterCache;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;
        private Dictionary<string, BvSystemSettingsEntity> _cache;
        private ICompanyInfo _companyInfo;

        private readonly object _lockObject = new object();

        private DateTime _expiredTime;

        public SystemSettingCache(IIpFilterCache ipFilterCache, ISqlTableUpdatedPublisher sqlTableUpdatedPublisher, ICompanyInfo companyInfo)
        {
            _ipFilterCache = ipFilterCache;
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
            _companyInfo = companyInfo;
        }

        public string Get(string settingSystemName)
        {
            ReloadCacheIfNeeds();

            return _cache[settingSystemName].Value;
        }

        public void Set<T>(string settingSystemName, T typedValue)
        {
            string value = typedValue == null ? null : typedValue.ToString();

            lock (_lockObject)
            {
                BvSpSystemSetting_UpdateAdapter.ExecuteNonQuery(settingSystemName, value);

                if (_cache != null)
                {
                    _cache[settingSystemName].Value = value;
                }
            }

            if (DatabaseTransactionScope.Current != null)
            {
                DatabaseTransactionScope.Current.AddCacheToExpireAfterSuccessfullCommit(this);
            }
            else
            {
                _sqlTableUpdatedPublisher.PublishSystemSettingsUpdated();
            }
        }

        public void Reset()
        {
            lock (_lockObject)
            {
                ReloadCache();
            }
        }

        private void ReloadCacheIfNeeds()
        {
            // We use double-checked locking for cache reloading
            if (IsCacheValid())
            {
                return;
            }

            lock (_lockObject)
            {
                if (IsCacheValid())
                {
                    return;
                }

                ReloadCache();
            }
        }

        private bool IsCacheValid()
        {
            if (IsTestCompany())//needed for integration tests which work with different system setting values,
                return false;//otherwise cached system setting value breaks test logic.  
            
            return _cache != null && DateTime.UtcNow < _expiredTime;
        }

        private bool IsTestCompany()
        {
            return _companyInfo.CompanyName?.StartsWith("TestCompany") ?? false;
        }

        private void ReloadCache()
        {
            var localSettings = GetSettings(BackendInstance.Current.ConnectionString).ToDictionary(x => x.SystemName);

            if (!BackendInstance.Current.IsDefaultInstance)
            {
                var defaultSettings = GetSettings(BackendInstance.Current.DefaultInstanceConnectionString).ToDictionary(x => x.SystemName);
                foreach (var local in localSettings)
                {
                    defaultSettings[local.Key] = local.Value;
                }

                localSettings = defaultSettings;
            }

            var prevCache = _cache;
            _cache = localSettings;
            _expiredTime = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5));

            try
            {
                ChangedSettingsCollector changedSettingsCollector = DetectChanges(prevCache, _cache);
                if (changedSettingsCollector.TotalChangesCount > 0)
                {
                    LogChanges(changedSettingsCollector);

                    if (changedSettingsCollector.IsAccessAllowedIpAddressesChanged)
                    {
                        _ipFilterCache.Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex, "An error occurred during logging of system settings changes:");
            }
        }

        internal ChangedSettingsCollector DetectChanges(Dictionary<string, BvSystemSettingsEntity> prevSettings, Dictionary<string, BvSystemSettingsEntity> curSettings)
        {
            var changesInformation = new ChangedSettingsCollector();
            if (prevSettings == null)
            {
                return changesInformation;
            }

            foreach (var prev in prevSettings)
            {
                if (!curSettings.ContainsKey(prev.Key))
                {
                    changesInformation.AddInformationAboutRemovedSetting(prev.Key, prev.Value.Value);
                    continue;
                }

                var cur = curSettings[prev.Key];
                if (cur.Value != prev.Value.Value)
                {
                    changesInformation.AddInformationAboutChangedSetting(cur.SystemName, prev.Value.Value, cur.Value);
                }
            }

            foreach (var cur in curSettings)
            {
                if (!prevSettings.ContainsKey(cur.Key))
                {
                    changesInformation.AddInformationAboutAddedSetting(cur.Key, cur.Value.Value);
                }
            }

            return changesInformation;
        }

        private void LogChanges(ChangedSettingsCollector changedSettingsCollector)
        {
            ServiceLocator.Resolve<ISystemSettingsNotifyChanged>().OnChanged();

            Trace.TraceInformation(changedSettingsCollector.GetMessageAboutChanges());
        }

        private static IEnumerable<BvSystemSettingsEntity> GetSettings(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = BvSystemSettingsAdapter.selectSql,
                    CommandType = CommandType.Text,
                    CommandTimeout = 120
                };
                return BvSystemSettingsAdapter.ReadList(command.ExecuteReader());
            }
        }

        public string CachedTableName => "BvSystemSetting";

        public void OnTableChanged()
        {
            Reset();
        }

        public void OnCacheExpired()
        {
            _sqlTableUpdatedPublisher.PublishSystemSettingsUpdated();
            Reset();
        }
    }
}
