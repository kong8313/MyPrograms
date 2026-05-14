using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    [Serializable]
    public class InternalCallTransferFormClosedStateData : BaseStateData
    {
        /// <summary>
        /// Gets/sets interview mode
        /// </summary>
        public ConsoleState ConsoleState
        {
            get;
            set;
        }
    }
}
