using System;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.ActivityViews;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings.Fakes
{
    public class StubISupervisorSettingsRepository : ISupervisorSettingsRepository 
    {
        private ISupervisorSettingsRepository _inner;

        public StubISupervisorSettingsRepository()
        {
            _inner = null;
        }

        public ISupervisorSettingsRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate QuotaPageViewSettings ReadQuotaSettingsInt32Delegate(int surveyId);
        public ReadQuotaSettingsInt32Delegate ReadQuotaSettingsInt32;

        QuotaPageViewSettings ISupervisorSettingsRepository.ReadQuotaSettings(int surveyId)
        {


            if (ReadQuotaSettingsInt32 != null)
            {
                return ReadQuotaSettingsInt32(surveyId);
            } else if (_inner != null)
            {
                return ((ISupervisorSettingsRepository)_inner).ReadQuotaSettings(surveyId);
            }

            return default(QuotaPageViewSettings);
        }

        public delegate void WriteQuotaSettingsInt32QuotaPageViewSettingsDelegate(int surveyId, QuotaPageViewSettings settings);
        public WriteQuotaSettingsInt32QuotaPageViewSettingsDelegate WriteQuotaSettingsInt32QuotaPageViewSettings;

        void ISupervisorSettingsRepository.WriteQuotaSettings(int surveyId, QuotaPageViewSettings settings)
        {

            if (WriteQuotaSettingsInt32QuotaPageViewSettings != null)
            {
                WriteQuotaSettingsInt32QuotaPageViewSettings(surveyId, settings);
            } else if (_inner != null)
            {
                ((ISupervisorSettingsRepository)_inner).WriteQuotaSettings(surveyId, settings);
            }
        }

        public delegate CallManagementColumnSettings ReadCallManagementColumnSettingsDelegate();
        public ReadCallManagementColumnSettingsDelegate ReadCallManagementColumnSettings;

        CallManagementColumnSettings ISupervisorSettingsRepository.ReadCallManagementColumnSettings()
        {


            if (ReadCallManagementColumnSettings != null)
            {
                return ReadCallManagementColumnSettings();
            } else if (_inner != null)
            {
                return ((ISupervisorSettingsRepository)_inner).ReadCallManagementColumnSettings();
            }

            return default(CallManagementColumnSettings);
        }

        public delegate void WriteCallManagementColumnSettingsCallManagementColumnSettingsDelegate(CallManagementColumnSettings settings);
        public WriteCallManagementColumnSettingsCallManagementColumnSettingsDelegate WriteCallManagementColumnSettingsCallManagementColumnSettings;

        void ISupervisorSettingsRepository.WriteCallManagementColumnSettings(CallManagementColumnSettings settings)
        {

            if (WriteCallManagementColumnSettingsCallManagementColumnSettings != null)
            {
                WriteCallManagementColumnSettingsCallManagementColumnSettings(settings);
            } else if (_inner != null)
            {
                ((ISupervisorSettingsRepository)_inner).WriteCallManagementColumnSettings(settings);
            }
        }

        public delegate CallManagementViews ReadCallManagementViewsDelegate();
        public ReadCallManagementViewsDelegate ReadCallManagementViews;

        CallManagementViews ISupervisorSettingsRepository.ReadCallManagementViews()
        {


            if (ReadCallManagementViews != null)
            {
                return ReadCallManagementViews();
            } else if (_inner != null)
            {
                return ((ISupervisorSettingsRepository)_inner).ReadCallManagementViews();
            }

            return default(CallManagementViews);
        }

        public delegate void WriteCallManagementViewsCallManagementViewsDelegate(CallManagementViews views);
        public WriteCallManagementViewsCallManagementViewsDelegate WriteCallManagementViewsCallManagementViews;

        void ISupervisorSettingsRepository.WriteCallManagementViews(CallManagementViews views)
        {

            if (WriteCallManagementViewsCallManagementViews != null)
            {
                WriteCallManagementViewsCallManagementViews(views);
            } else if (_inner != null)
            {
                ((ISupervisorSettingsRepository)_inner).WriteCallManagementViews(views);
            }
        }

        public delegate List<ColumnDescription> ReadSurveyActivityViewColumnSettingsDelegate();
        public ReadSurveyActivityViewColumnSettingsDelegate ReadSurveyActivityViewColumnSettings;

        List<ColumnDescription> ISupervisorSettingsRepository.ReadSurveyActivityViewColumnSettings()
        {


            if (ReadSurveyActivityViewColumnSettings != null)
            {
                return ReadSurveyActivityViewColumnSettings();
            } else if (_inner != null)
            {
                return ((ISupervisorSettingsRepository)_inner).ReadSurveyActivityViewColumnSettings();
            }

            return default(List<ColumnDescription>);
        }

        public delegate void WriteSurveyActivityViewColumnSettingsListOfColumnDescriptionDelegate(List<ColumnDescription> columns);
        public WriteSurveyActivityViewColumnSettingsListOfColumnDescriptionDelegate WriteSurveyActivityViewColumnSettingsListOfColumnDescription;

        void ISupervisorSettingsRepository.WriteSurveyActivityViewColumnSettings(List<ColumnDescription> columns)
        {

            if (WriteSurveyActivityViewColumnSettingsListOfColumnDescription != null)
            {
                WriteSurveyActivityViewColumnSettingsListOfColumnDescription(columns);
            } else if (_inner != null)
            {
                ((ISupervisorSettingsRepository)_inner).WriteSurveyActivityViewColumnSettings(columns);
            }
        }

        public delegate string ReadTableDensityDelegate();
        public ReadTableDensityDelegate ReadTableDensity;

        string ISupervisorSettingsRepository.ReadTableDensity()
        {


            if (ReadTableDensity != null)
            {
                return ReadTableDensity();
            } else if (_inner != null)
            {
                return ((ISupervisorSettingsRepository)_inner).ReadTableDensity();
            }

            return default(string);
        }

        public delegate void WriteBooleanSettingStringBooleanDelegate(string settingType, bool value);
        public WriteBooleanSettingStringBooleanDelegate WriteBooleanSettingStringBoolean;

        void ISupervisorSettingsRepository.WriteBooleanSetting(string settingType, bool value)
        {

            if (WriteBooleanSettingStringBoolean != null)
            {
                WriteBooleanSettingStringBoolean(settingType, value);
            } else if (_inner != null)
            {
                ((ISupervisorSettingsRepository)_inner).WriteBooleanSetting(settingType, value);
            }
        }

        public delegate bool? ReadBooleanSettingStringDelegate(string settingType);
        public ReadBooleanSettingStringDelegate ReadBooleanSettingString;

        bool? ISupervisorSettingsRepository.ReadBooleanSetting(string settingType)
        {


            if (ReadBooleanSettingString != null)
            {
                return ReadBooleanSettingString(settingType);
            } else if (_inner != null)
            {
                return ((ISupervisorSettingsRepository)_inner).ReadBooleanSetting(settingType);
            }

            return default(bool?);
        }

    }
}