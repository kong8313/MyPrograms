using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents state data of check redo question action. Contains single field - question identifier.
    /// </summary>
    [Serializable]
    public class PendingLogoutStateData : BaseStateData
    {
        #region Properties

        /// <summary>
        /// True if pending logout is enabled, otherwise false
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }

        #endregion
    }
}
