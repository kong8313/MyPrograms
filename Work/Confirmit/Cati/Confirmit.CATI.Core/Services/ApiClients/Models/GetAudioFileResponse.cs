using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class GetAudioFileResponse : DialerResponse
    {
        public AudioFile AudioFile { get; set; }
    }
}