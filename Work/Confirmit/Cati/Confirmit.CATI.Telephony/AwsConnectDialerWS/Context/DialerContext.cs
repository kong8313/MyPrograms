namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Context
{
    public class DialerContext
    {
        public int CompanyId { get; set; }
        public int DialerId { get; set; }

        public DialerContext(int companyId, int dialerId)
        {
            CompanyId = companyId;
            DialerId = dialerId;
        }
        
        public override string ToString()
        {
            return $"C:{CompanyId}_D:{DialerId}";
        }
        
        public static implicit operator string(DialerContext obj)
        {
            return obj.ToString();
        }
    }
}