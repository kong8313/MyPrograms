using System;

namespace DialerWsLogParser
{
    internal class EventView
    {
        public long Id { get; set; }
        public long RequestId { get; set; }
        public Uri Icon { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public string CompanyId { get; set; }
        public string DialerId { get; set; }
        public string CampaignId { get; set; }
        public string AgentId { get; set; }
        public string CallId { get; set; }
        public string InterviewId { get; set; }
        public string Duration { get; set; }
        public string AllInfo { get; set; }
        public bool IsMatchesCondition { get; set; }
    }
}