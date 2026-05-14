using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class DialerResponse
    {
        public DialerErrorCode DialerErrorCode { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}