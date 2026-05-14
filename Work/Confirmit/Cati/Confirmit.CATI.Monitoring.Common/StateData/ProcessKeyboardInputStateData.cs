using System;

namespace Confirmit.CATI.Monitoring.Common.StateData
{
    /// <summary>
    /// Represents state data of selected question. 
    /// </summary>
    [Serializable]
    public class ProcessKeyboardInputStateData : BaseStateData
    {
        #region Properties
        /// <summary>
        /// Gets/sets value in KeyboardInput control
        /// </summary>
        public string InputValue
        {
            get;
            set;
        }

        #endregion
    }
}

