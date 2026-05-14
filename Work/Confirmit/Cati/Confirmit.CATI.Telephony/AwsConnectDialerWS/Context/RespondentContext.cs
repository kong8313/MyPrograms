namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Context
{
    public class RespondentContext : SurveyContext
    {
        public int AgentId { get; set; }
        public int InterviewId { get; set; }
        public long CallId { get; set; }

        public RespondentContext(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId)
            : base(companyId, dialerId, campaignId)
        {
            AgentId = agentId;
            InterviewId = interviewId;
            CallId = callId;
        }

        public override string ToString()
        {
            return $"{base.ToString()}_A:{AgentId}_R:{InterviewId}_CID:{CallId}";
        }
        
        public static implicit operator string(RespondentContext obj)
        {
            return obj.ToString();
        }

        public SurveyContext ToSurveyContext()
        {
            return new SurveyContext(CompanyId, DialerId, CampaignId);
        }
    }
}