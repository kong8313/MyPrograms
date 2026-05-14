using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.ActivityViews;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.CallManagement;
using Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings
{
    public interface ISupervisorSettingsRepository
    {
        QuotaPageViewSettings ReadQuotaSettings(int surveyId);
        void WriteQuotaSettings(int surveyId, QuotaPageViewSettings settings);

        CallManagementColumnSettings ReadCallManagementColumnSettings();
        void WriteCallManagementColumnSettings(CallManagementColumnSettings settings);

        CallManagementViews ReadCallManagementViews();
        void WriteCallManagementViews(CallManagementViews views);

        List<ColumnDescription> ReadSurveyActivityViewColumnSettings();
        void WriteSurveyActivityViewColumnSettings(List<ColumnDescription> columns);

        string ReadTableDensity();

        void WriteBooleanSetting(string settingType, bool value);
        bool? ReadBooleanSetting(string settingType);
    }
}