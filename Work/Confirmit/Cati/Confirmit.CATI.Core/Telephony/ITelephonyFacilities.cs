using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    /// <summary>
    /// Dialer facilities contracts. 
    /// </summary>
    /// <remarks>It doesn't part of Dialer, just dialer service maintenance.</remarks>
    public interface ITelephonyFacilities
    {
        /// <summary>
        /// Get list of all files from logs folder.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <returns>List of file info.</returns>
        IEnumerable<LogFileInfo> GetLogFiles(int dialerId);

        /// <summary>
        /// Get zipped body of specified file from logs folder.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <param name="fileName">File name with extension in logs folder.</param>
        /// <returns>Zip archive contained one specified file.</returns>
        byte[] GetLogFileBodyZipped(int dialerId, string fileName);

        /// <summary>
        /// Get dialer product full version.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <returns>Version presented in string</returns>
        string GetDialerVersion(int dialerId);
    }
}
