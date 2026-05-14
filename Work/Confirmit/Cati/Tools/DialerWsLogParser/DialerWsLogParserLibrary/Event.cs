using System.Text.RegularExpressions;

namespace DialerWsLogParserLibrary
{
    public class Event
    {
        public long Id { get; set; }
        public long RequestId { get; set; }
        public IconType Icon { get; set; }
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
        public bool IsHighlighted { get; set; }

        public Event(long id,
            long requestId,
            string name,
            string time,
            string companyId = "",
            string dialerId = "",
            string campaignId = "",
            string agentId = "",
            string callId = "",
            string interviewId = "",
            string duration = "", 
            string all = "",
            bool isHighlighted = false)
        {
            Id = id;
            RequestId = requestId;
            Name = name;
            Time = time;
            CompanyId = companyId;
            DialerId = dialerId;
            CampaignId = campaignId;
            AgentId = agentId;
            CallId = callId;
            InterviewId = interviewId;
            Duration = duration;
            AllInfo = all;
            IsHighlighted = isHighlighted;

            SetIcon();
        }

        private void SetIcon()
        {
            if (Regex.IsMatch(AllInfo, @"^DialerService Information:"))
                Icon = IconType.Info;
            else if (Regex.IsMatch(AllInfo, @"^DialerService Verbose:"))
                Icon = IconType.Verbose;
            else if (Regex.IsMatch(AllInfo, @"^DialerService Warning:"))
                Icon = IconType.Warning;
            else if (AllInfo.Contains("Error"))
                Icon = IconType.Error;
        }
    }
}
