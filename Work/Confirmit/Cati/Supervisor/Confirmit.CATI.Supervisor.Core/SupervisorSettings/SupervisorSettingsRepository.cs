using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.ActivityViews;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;
using Newtonsoft.Json;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings
{
    public class SupervisorSettingsRepository : ISupervisorSettingsRepository
    {
        private readonly ISupervisorNameProvider _supervisorNameProvider;
        private readonly ICallManagementViewsProvider _callManagementViewProvider;
        private readonly IConnectionStrings _connectionStrings;

        public SupervisorSettingsRepository(ISupervisorNameProvider supervisorNameProvider, ICallManagementViewsProvider callManagementViewProvider, IConnectionStrings connectionStrings)
        {
            _supervisorNameProvider = supervisorNameProvider;
            _callManagementViewProvider = callManagementViewProvider;
            _connectionStrings = connectionStrings;
        }

        [NotNull]
        public QuotaPageViewSettings ReadQuotaSettings(int surveyId)
        {
            var userName = _supervisorNameProvider.Name;
            var settings = ReadSettings<QuotaPageViewSettings>(SupervisorSettingType.QuotaColumns, userName, surveyId);

            if (settings == null)
            {
                return new QuotaPageViewSettings();
            }

            return settings;
        }

        public void WriteQuotaSettings(int surveyId, QuotaPageViewSettings settings)
        {
            var userName = _supervisorNameProvider.Name;
            WriteSettings(settings, SupervisorSettingType.QuotaColumns, userName, surveyId);
        }

        [NotNull]
        public CallManagementViews ReadCallManagementViews()
        {
            var currentSetting = ReadSettings<CallManagementViews>(SupervisorSettingType.CallManagementCustomViews);
            var customViews = currentSetting ?? new CallManagementViews { Views = new List<CallManagementView>() };
            var defaultViews = _callManagementViewProvider.GetDefaultViews();

            return _callManagementViewProvider.MergeViews(defaultViews, customViews);
        }

        public void WriteCallManagementViews(CallManagementViews views)
        {
            CallManagementViews customViews = _callManagementViewProvider.RemoveDefaultViews(views);
            WriteSettings(customViews, SupervisorSettingType.CallManagementCustomViews, string.Empty);
        }

        public List<ColumnDescription> ReadSurveyActivityViewColumnSettings()
        {
            return ReadSettings<List<ColumnDescription>>(SupervisorSettingType.SurveyActivityViewColumnSettings) ?? new List<ColumnDescription>();
        }

        public void WriteSurveyActivityViewColumnSettings(List<ColumnDescription> columns)
        {
            WriteSettings(columns, SupervisorSettingType.SurveyActivityViewColumnSettings);
        }

        [NotNull]
        public CallManagementColumnSettings ReadCallManagementColumnSettings()
        {
            var currentSetting = ReadSettings<CallManagementColumnSettings>(SupervisorSettingType.CallManagementColumnWidth, _supervisorNameProvider.Name);

            return currentSetting ?? new CallManagementColumnSettings
            {
                Columns = new Dictionary<string, List<ColumnSetting>>()
            };
        }

        public void WriteCallManagementColumnSettings(CallManagementColumnSettings settings)
        {
            var name = _supervisorNameProvider.Name;
            WriteSettings(settings, SupervisorSettingType.CallManagementColumnWidth, name);
        }

        [CanBeNull]
        private BvSupervisorSettingsEntity ReadSettingsEnity(string settingType, string userName = null, int? surveyId = null)
        {
            var sql = "[SettingType] = @SettingType";
            var sqlParameters = new List<SqlParameter>() { new SqlParameter("@SettingType", settingType) };

            if (userName != null)
            {
                sql += " AND [UserName] = @UserName";
                sqlParameters.Add(new SqlParameter("@UserName", userName));
            }

            if (surveyId.HasValue)
            {
                sql += " AND [SurveyId] = @SurveyId";
                sqlParameters.Add(new SqlParameter("@SurveyId", surveyId));
            }

            var entity = BvSupervisorSettingsAdapter.GetByCondition(sql, sqlParameters.ToArray()).FirstOrDefault();

            return entity;
        }

        public void WriteBooleanSetting(string settingType, bool value)
        {
            using (new ConnectionScope(_connectionStrings.DefaultInstanceConnectionString))
            {
                var name = _supervisorNameProvider.Name;
                WriteSettings(value.ToString(), settingType, name);
            }
        }

        [CanBeNull]
        public bool? ReadBooleanSetting(string settingType)
        {
            using (new ConnectionScope(_connectionStrings.DefaultInstanceConnectionString))
            {
                var currentSetting = ReadSettingsEnity(settingType, _supervisorNameProvider.Name);

                if (currentSetting == null)
                    return null;

                return bool.Parse(JsonConvert.DeserializeObject<string>(currentSetting.Settings));
            }
        }

        public string ReadTableDensity()
        {
            using (new ConnectionScope(_connectionStrings.DefaultInstanceConnectionString))
            {
                var currentSetting = ReadSettingsEnity(SupervisorSettingType.SupervisorTableDensity, _supervisorNameProvider.Name);

                return currentSetting == null ? string.Empty : JsonConvert.DeserializeObject<string>(currentSetting.Settings);
            }
        }

        private T ReadSettings<T>(string settingType, string userName = null, int? surveyId = null) where T : class
        {
            var dbSettings = ReadSettingsEnity(settingType, userName, surveyId);
            if (dbSettings != null)
            {
                return JsonConvert.DeserializeObject<T>(dbSettings.Settings);
            }

            return null;
        }

        private void WriteSettings<T>(T settings, string settingType, string userName = null, int? surveyId = null) where T : class
        {
            var currentSetting = ReadSettingsEnity(settingType, userName, surveyId);

            if (currentSetting == null)
            {
                currentSetting = new BvSupervisorSettingsEntity
                {
                    SettingType = settingType,
                    Settings = JsonConvert.SerializeObject(settings)
                };

                if (userName != null)
                    currentSetting.UserName = userName;
                if (surveyId.HasValue)
                    currentSetting.SurveyId = surveyId;

                BvSupervisorSettingsAdapter.Insert(currentSetting);
            }
            else
            {
                var sql = "[SettingType] = @SettingType";
                if (userName != null)
                    sql += " AND [UserName] = @UserName";
                if (surveyId.HasValue)
                    sql += " AND [SurveyId] = @SurveyId";

                currentSetting.Settings = JsonConvert.SerializeObject(settings);
                BvSupervisorSettingsAdapter.UpdateByCondition(currentSetting, sql);
            }
        }
    }
}
