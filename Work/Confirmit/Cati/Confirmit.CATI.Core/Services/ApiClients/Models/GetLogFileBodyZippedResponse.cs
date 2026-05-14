namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class GetLogFileBodyZippedResponse : DialerResponse
    {
        public byte[] LogFileBodyZipped { get; set; }
    }
}