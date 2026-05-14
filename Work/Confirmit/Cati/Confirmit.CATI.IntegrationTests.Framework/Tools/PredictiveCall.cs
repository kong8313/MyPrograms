using System;

namespace Confirmit.CATI.IntegrationTests.Framework.Tools
{
    /// <summary>
    /// Class represents call data returned by 
    /// </summary>
    public class PredictiveCall
    {
        #region Properties

        /// <summary>
        /// Call identifier.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Interview identifier.
        /// </summary>
        public int InterviewID { get; set; }

        /// <summary>
        /// Explicite sid.
        /// </summary>
        public int ExplicitSid { get; set; }

        /// <summary>
        /// Survey identifier.
        /// </summary>
        public int SurveySID { get; set; }

        /// <summary>
        /// Dialing mode.
        /// </summary>
        public int DialingMode { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Time in shift.
        /// </summary>
        public DateTime TimeInShift { get; set; }

        #endregion
    }
}
