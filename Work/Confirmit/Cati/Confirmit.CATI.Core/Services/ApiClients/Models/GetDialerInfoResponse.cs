namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class GetDialerInfoResponse : DialerResponse
    {
        public string CodiMajorVersion { get; set; }
        public string CodiFullVersion { get; set; }
        public string DialerDriverNameAndVersion { get; set; }
    }
}