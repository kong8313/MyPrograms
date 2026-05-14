using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport
{
    [Serializable]
    public class InterviewerProductivityReportTemplate
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("creatorName")]
        public string CreatorName { get; set; }

        [JsonProperty("creatorLogin")]
        public string CreatorLogin { get; set; }

        [JsonProperty("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("isPortrait")]
        public bool IsPortrait { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("includeZeroValues")]
        public bool IncludeZeroValues { get; set; }

        [JsonProperty("accessType")]
        public byte AccessType { get; set; }

        [JsonProperty("showDialerAttempts")]
        public bool ShowDialerAttempts { get; set; }

        [JsonProperty("includeBreakTimeInCalculations")]
        public bool IncludeBreakTimeInCalculations { get; set; }

        [JsonProperty("columns")]
        public List<ProductivityReportTemplateColumn> Columns { get; set; }
    }
}
