using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class ConfigureInboundDdiNumbersResponse : DialerResponse
    {
        public DialerErrorCode[] DialerErrorCodes { get; set; }
    }
}