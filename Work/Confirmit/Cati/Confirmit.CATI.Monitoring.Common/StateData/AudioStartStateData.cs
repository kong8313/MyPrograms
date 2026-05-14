using System;
using Confirmit.CATI.Monitoring.Common.Contracts;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents state data for audio record start playing message.
    /// </summary>
    [Serializable]
    public class AudioStartStateData : BaseStateData
    {
        /// <summary>
        /// Gets/sets audio record identifying object.
        /// </summary>
        public AudioIdentityObject AudioRecordID { get; set; }

        /// <summary>
        /// Gets/sets time difference between client (Cati console) and server (Backend).
        /// Difference is calculated as client - server.
        /// </summary>
        public TimeSpan? Offset { get; set; }        

        public TimeSpan StartAudioOffset { get; set; }

        public TimeSpan Duration { get; set; }
        public bool HasDuration { get; set; }
    }
}
