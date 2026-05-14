namespace DialerWsLogParserLibrary
{
    public class EventsGroup
    {
        public long RequestId { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string FinishTime { get; set; }
        public string CompanyId { get; set; }
        public string DialerId { get; set; }
        public string CampaignId { get; set; }
        public string AgentId { get; set; }
        public string CallId { get; set; }
        public string InterviewId { get; set; }
        public string Duration { get; set; }

        public EventsGroup(long requestId,
            string name,
            string startTime,
            string finishTime,
            string companyId = "",
            string dialerId = "",
            string campaignId = "",
            string agentId = "",
            string callId = "",
            string interviewId = "",
            string duration = "")
        {
            RequestId = requestId;
            Name = name;
            StartTime = startTime;
            FinishTime = finishTime;
            CompanyId = companyId;
            DialerId = dialerId;
            CampaignId = campaignId;
            AgentId = agentId;
            CallId = callId;
            InterviewId = interviewId;
            Duration = duration;
        }
    }
}
