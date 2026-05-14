using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class GetStateResponse : DialerResponse
    {
        public DialerState DialerState { get; set; }
    }
}