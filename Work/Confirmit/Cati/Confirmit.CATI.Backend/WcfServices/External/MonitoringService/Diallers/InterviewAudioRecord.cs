using System;

namespace Confirmit.CATI.Backend.WcfServices.External.MonitoringService.Diallers
{
    /// <summary>
    /// Class describes audio record of interview.
    /// </summary>
    public class InterviewAudioRecord
    {
        #region Properties

        /// <summary>
        /// Gets or sets moment of starting of audio file.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets location of audio file.
        /// </summary>
        public string URI { get; set; }

        /// <summary>
        /// Gets or sets name of audio file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets ID of dialer
        /// </summary>
        public int DialerId { get; set; }

        #endregion
    }
}