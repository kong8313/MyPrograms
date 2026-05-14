using System;

namespace Confirmit.CATI.Supervisor.Controls.Grid
{
    /// <summary>
    /// Represents information about controls in the filtration bar
    /// </summary>
    [Serializable]
    public class ColumnHeaderState
    {
        /// <summary>
        /// Gets/sets Unique ID for value control
        /// </summary>
        public string ValueControlUniqueId
        {
            get;
            set;
        }

        public string ValueControlClientId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets Unique ID for operator control
        /// </summary>        
        public string OperatorControlUniqueId
        {
            get;
            set;
        }
    }            
}