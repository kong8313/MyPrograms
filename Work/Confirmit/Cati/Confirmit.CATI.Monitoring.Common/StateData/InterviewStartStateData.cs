using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents interview starting state data. Contains interview identifier and Backend survey identifier.
    /// </summary>
    [Serializable]
    public class InterviewStartStateData : BaseStateData
    {
        #region Properties

        /// <summary>
        /// Gets/sets interview identifier.
        /// </summary>
        public int InterviewID { get; set; }

        /// <summary>
        /// Gets/sets Backend survey identifier.
        /// </summary>
        public int SurveyID { get; set; }

        /// <summary>
        /// Provides validation of record
        /// </summary>
        public bool IsJumpingSupported { get; set; }

        #endregion
    }
}
