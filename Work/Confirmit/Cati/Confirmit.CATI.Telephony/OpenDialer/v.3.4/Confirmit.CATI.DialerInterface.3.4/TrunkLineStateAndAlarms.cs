using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Possible TrunkLineState
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum TrunkLineState
    {
        /// <summary>
        /// Line is up
        /// </summary>
        [EnumMember]
        Up = 1,

        /// <summary>
        /// Line is down
        /// </summary>
        [EnumMember]
        Down = 2
    };

    /// <summary>
    /// Trunk line alarm details
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public class AlarmEntry
    {
        /// <summary>
        /// The line state
        /// </summary>
        [DataMember]
        public TrunkLineState State;

        /// <summary>
        /// Time passed after the alarm occured
        /// </summary>
        [DataMember]
        public int Duration;

        /// <summary>
        /// The alarm time
        /// </summary>
        [DataMember]
        public DateTime Time;
    }

    /// <summary>
    /// Trunk Line state and alarms for a concrete line
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public class TrunkLineStateAndAlarms
    {
        /// <summary>
        /// The line name
        /// </summary>
        [DataMember]
        public string LineName;

        /// <summary>
        /// Current line state
        /// </summary>
        [DataMember]
        public AlarmEntry State;

        /// <summary>
        /// Alarms cached after the previous method call
        /// </summary>
        [DataMember]
        public List<AlarmEntry> AlarmsList = new List<AlarmEntry>();
    }
}
