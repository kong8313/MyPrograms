using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents state data of telephony command result.
    /// </summary>
    [Serializable]
    public class TelephonyResultData : BaseStateData
    {
        #region Constructors

        /// <summary>
        /// Intitializes new instance of TelephonyResultData class.
        /// </summary>
        public TelephonyResultData()
            : base()
        {
        }

        /// <summary>
        /// Initializes new instance of TelephonyResultData class and fills it with given data.
        /// </summary>
        /// <param name="status">Result status.</param>
        public TelephonyResultData(int status)
            : base()
        {
            Status = status;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets result status of telephony command.
        /// </summary>
        public int Status
        {
            get;
            set;
        }

        #endregion
    }
}
