using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Export
{
    /// <summary>
    /// Represents base interface providing single record for export.
    /// </summary>
    public interface IExportRecordProvider : IEnumerable<ExportItem>
    {
        #region Properties

        /// <summary>
        /// Gets descendant records for this record.
        /// </summary>
        IExportRecordProvider Descendants
        {
            get;
        }

        /// <summary>
        /// Returns value of export item with given name.
        /// </summary>
        /// <param name="name">Export item name.</param>
        /// <returns>Export value.</returns>
        object this[string name] 
        {
            get;
        }

        #endregion
    }
}
