using System.Runtime.Serialization;

namespace DialerWsLogParserLibrary
{
    [DataContract]
    public class GroupsFilter
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string StartTime { get; private set; }
        [DataMember]
        public string FinishTime { get; private set; }
        [DataMember]
        public string CompanyId { get; private set; }
        [DataMember]
        public string DialerId { get; private set; }
        [DataMember]
        public string CampaignId { get; private set; }
        [DataMember]
        public string AgentId { get; private set; }
        [DataMember]
        public string CallId { get; private set; }
        [DataMember]
        public string InterviewId { get; private set; }
        [DataMember]
        public string Duration { get; private set; }
        [DataMember]
        public string AllInfo { get; private set; }

        public GroupsFilter(string name,
            string startTime,
            string finishTime,
            string companyId,
            string dialerId,
            string campaignId,
            string agentId,
            string callId,
            string interviewId,
            string duration,
            string all)
        {
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
            AllInfo = all;
        }

        public void SetColumn(string columnName, string value)
        {
            switch (columnName)
            {
                case ("Name"):
                    Name = value;
                    break;
                case ("StartTime"):
                    StartTime = value;
                    break;
                case ("FinishTime"):
                    FinishTime = value;
                    break;
                case ("CompanyId"):
                    CompanyId = value;
                    break;
                case ("DialerId"):
                    DialerId = value;
                    break;
                case ("CampaignId"):
                    CampaignId = value;
                    break;
                case ("AgentId"):
                    AgentId = value;
                    break;
                case ("CallId"):
                    CallId = value;
                    break;
                case ("InterviewId"):
                    InterviewId = value;
                    break;
                case ("Duration"):
                    Duration = value;
                    break;
                case ("AllInfo"):
                    AllInfo = value;
                    break;
                default:
                    return;
            }
        }
    }
}
