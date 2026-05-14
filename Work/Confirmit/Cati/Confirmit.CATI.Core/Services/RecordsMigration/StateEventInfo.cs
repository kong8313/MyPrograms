using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Monitoring.Common.Contracts
{
    /// <summary>
    /// Class of information about Interviewer state event.
    /// </summary>
    [DataContract(Namespace = "http://www.confirmit.com/")]
    [Serializable]
    public class StateEventInfo
    {
        /// <summary>
        /// Time of state event.
        /// </summary>
        [DataMember]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Type of state event.
        /// </summary>
        [DataMember]
        public MonitoringMessageTypes MessageType { get; set; }

        /// <summary>
        /// Serialized object with state event details.
        /// </summary>
        [DataMember]
        public byte[] State { get; set; }

        public override string ToString()
        {
            return $"{TimeStamp:o}: {MessageType}";
        }
    }
}