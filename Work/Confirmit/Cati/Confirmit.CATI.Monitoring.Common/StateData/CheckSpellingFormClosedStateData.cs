using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class CheckSpellingFormClosedStateData : BaseStateData
    {
        #region Properties

        /// <summary>
        /// Gets/sets interview mode
        /// </summary>
        public ConsoleState ConsoleState
        {
            get;
            set;
        }

        #endregion
    }
}
