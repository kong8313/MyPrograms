using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings.Quota
{
    public class QuotaPageViewSettings
    {
        public QuotaPageViewSettings()
        {
            QuotasOrder = new List<string>();
            QuotasExclusion = new List<string>();
        }

        public int NumberOfColumns { get; set; }
        public List<string> QuotasOrder { get; set; }
        public List<string> QuotasExclusion { get; set; }
    }
}