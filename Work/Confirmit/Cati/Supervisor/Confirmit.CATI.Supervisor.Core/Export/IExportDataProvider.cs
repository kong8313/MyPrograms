using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Export
{
    /// <summary>
    /// Represents base interface providing export data.
    /// </summary>
    public interface IExportDataProvider : IEnumerable<IExportRecordProvider>
    {
        /// <summary>
        /// Returns additional parameter by key
        /// </summary>
        /// <param name="key">Parameter key</param>
        /// <returns>Parameter value</returns>
        string GetParameter(string key);
    }
}
