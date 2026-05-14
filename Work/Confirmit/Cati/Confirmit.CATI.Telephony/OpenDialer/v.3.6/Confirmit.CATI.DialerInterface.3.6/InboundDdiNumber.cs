using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Inbound DDI number descriptor
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public class InboundDdiNumber
    {
        /// <summary> DDI number </summary>
        [DataMember]
        public string Number;

        /// <summary> Message that should be automatically played to respondent by dialer </summary>
        [DataMember]
        public IEnumerable<KeyValuePair<AudioMessageType, AudioMessageDescriptor>> AudioMessages;

        public override string ToString()
        {
            var audioMessages = (AudioMessages == null)
                ? "[null]"
                : string.Join(", ", AudioMessages.Select(x => x.Key.ToString() + ":" + x.Value.NullableToString()));

            return "[Number=" + Number + ", " +
                   "AudioMessages=[" + audioMessages + "]]";
        }
    }
}