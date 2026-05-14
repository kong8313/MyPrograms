using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public class InterviewQuotaStatusItem
    {
        public int QuotaId { get; set; }
        public string QuotaName { get; set; }
        public bool IsOpen { get; set; }
        public bool IsNormalCell { get; set; }
        public bool IsFcdQuota { get; set; }
        public bool HasEmptyAnswers { get; set; }
        public bool IsZeroLimit { get; set; }
        public IReadOnlyDictionary<string, string> Fields { get; set; }
    }
}