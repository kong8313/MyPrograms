using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Represents result of getting bulk of audio.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public class BulkAudioResult
    {
        /// <summary>
        /// Initializes a new instance of the BulkAudioResult class.
        /// </summary>
        public BulkAudioResult()
        {
            CampaignInterviewIdentities = new CampaignInterviewIdentity[0];
            AudioRecords = new AudioRecordInfo[0][];
        }
        /// <summary>
        /// Gets or sets list of survey-interview identities for which
        /// audio data is returned.
        /// </summary>
        [DataMember]
        public CampaignInterviewIdentity[] CampaignInterviewIdentities
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets collection of audio records. First index of
        /// this property corresponds to index of CampaignInterviewIdentities property.
        /// </summary>
        [DataMember]
        public AudioRecordInfo[][] AudioRecords
        {
            get;
            set;
        }
    }
}
