using System.Runtime.Serialization;

namespace DialerWsLogParserLibrary
{
    [DataContract]
    public class ColumnDisplayHandler
    {
        [DataMember]
        public bool StartTime { get; private set; }
        [DataMember]
        public bool FinishTime { get; private set; }
        [DataMember]
        public bool CompanyId { get; private set; }
        [DataMember]
        public bool DialerId { get; private set; }
        [DataMember]
        public bool CampaignId { get; private set; }
        [DataMember]
        public bool AgentId { get; private set; }
        [DataMember]
        public bool CallId { get; private set; }
        [DataMember]
        public bool InterviewId { get; private set; }
        [DataMember]
        public bool Duration { get; private set; }

        public ColumnDisplayHandler(bool startTime, bool finishTime, bool companyId, bool dialerId, bool campaignId, 
            bool agentId, bool callId, bool interviewId, bool duration)
        {
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

        public void SetColumn (string columnName, bool value)
        {
            switch(columnName)
            {
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
                default:
                    return;
            }
        }
    }
}
