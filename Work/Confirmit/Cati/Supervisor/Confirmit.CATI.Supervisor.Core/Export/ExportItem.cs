using System;

namespace Confirmit.CATI.Supervisor.Core.Export
{
    /// <summary>
    /// Represents single item for export. Item contains assosiated name and value to be exported.
    /// </summary>
    public class ExportItem
    {
        #region Properties

        /// <summary>
        /// Gets/sets item name needed for referencing the item.
        /// </summary>
        public string Name 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets/sets value which should be exported.
        /// </summary>
        public object Value 
        { 
            get; 
            set; 
        }

        #endregion
    }
}
