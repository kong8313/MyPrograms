namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Context
{
    public class SurveyContext : DialerContext
    {
        public long CampaignId { get; set; }

        public SurveyContext(int companyId, int dialerId, long campaignId)
            : base(companyId, dialerId)
        {
            CampaignId = campaignId;
        }
        
        public string ProjectId => $"p{CampaignId}";
        
        public override string ToString()
        {
            return $"{base.ToString()}_P:{CampaignId}";
        }
        
        public static implicit operator string(SurveyContext obj)
        {
            return obj.ToString();
        }
    }
}