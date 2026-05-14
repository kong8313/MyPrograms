using SimulatorDialerDriver.Models;

namespace SimulatorDialerDriver.Distribution
{
    public class ContextInfo
    {
        public ContextInfo(Interviewer interviewer, int? interviewId = null)
        {
            CompanyId = interviewer.CompanyId;
            DialerId = interviewer.DialerId;
            CampaignId = interviewer.CampaignId;
            AgentId = interviewer.AgentId;
            InterviewId = interviewId;
        }

        public ContextInfo(int companyId, int dialerId, long campaignId, int agentId, int? interviewId = null)
        {
            CompanyId = companyId;
            DialerId = dialerId;
            CampaignId = campaignId;
            AgentId = agentId;
            InterviewId = interviewId;
        }

        public long CompanyId { get; set; }
        public long DialerId { get; set; }
        public long CampaignId { get; set; }
        public int AgentId { get; set; }
        public int? InterviewId { get; set; }
    }
}