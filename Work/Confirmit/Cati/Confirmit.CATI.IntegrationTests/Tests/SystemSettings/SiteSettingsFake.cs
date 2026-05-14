using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.IntegrationTests.Tests.SystemSettings
{
    public class SiteSettingsFake : ISiteSettingsGroup
    {
        public string StartSurveyURL { get; set; }

        public int TimeZoneID { get; set; }

        public bool IsChanged = false;

        public void OnChanged()
        {
            this.IsChanged = true;
        }
    }
}