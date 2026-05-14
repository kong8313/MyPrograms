using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.ApiClients.Models
{
    public class GetAudioRecordsResponse : DialerResponse
    {
        public AudioRecordInfo[] AudioRecords { get; set; }
    }
}