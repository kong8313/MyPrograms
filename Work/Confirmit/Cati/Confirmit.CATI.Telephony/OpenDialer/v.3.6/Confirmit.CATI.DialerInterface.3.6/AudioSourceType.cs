using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// This enum contains all possible audio source types.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum AudioSourceType
    {
        /// <summary> Text for TTS engine </summary>
        [EnumMember]
        Text = 1,

        /// <summary> VoiceXML </summary>
        [EnumMember]
        VoiceXml = 2,

        /// <summary> URL to get VoiceXml file from </summary>
        [EnumMember]
        VoiceXmlUrl = 3,

        /// <summary> URL to get audio file from </summary>
        [EnumMember]
        AudioUrl = 4
    }
}