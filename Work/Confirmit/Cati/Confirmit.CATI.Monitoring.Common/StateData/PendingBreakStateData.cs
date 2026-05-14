using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{    
    [Serializable]
    public class PendingBreakStateData : BaseStateData
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
