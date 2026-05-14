namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class AreAudioRecordsExistsResponse : DialerResponse
    {
        public bool[] AudioRecordExistenceFlags { get; set; }
    }
}