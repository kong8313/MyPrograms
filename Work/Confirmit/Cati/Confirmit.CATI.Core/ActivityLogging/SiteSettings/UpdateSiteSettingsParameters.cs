using System;

namespace Confirmit.CATI.Core.ActivityLogging.SiteSettings
{
    [Serializable]
    public class UpdateSiteSettingsParameters : ManagementActivityEventDetails
    {
        public string ChangedSettings { get; set; }
    }
}
