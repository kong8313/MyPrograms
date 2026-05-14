using System.Collections.Generic;
using System.ServiceModel;
using Confirmit.CATI.Common.Logging;
using DialerCommon.DialerExceptions;

namespace Confirmit.CATI.Telephony.DialerService.Contract
{
    /// <summary>
    /// Dialer facilities contracts. 
    /// </summary>
    /// <remarks>It doesn't part of Dialer, just dialer service maintenance.</remarks>
    [ServiceContract]
    public interface IDialerServiceFacilities
    {
        /// <summary>
        /// Get list of all files from logs folder.
        /// </summary>
        /// <returns>List of file info.</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        IEnumerable<LogFileInfo> GetLogFiles();

        /// <summary>
        /// Get zipped body of specified file from logs folder.
        /// </summary>
        /// <param name="fileName">File name with extension in logs folder.</param>
        /// <returns>Zip archive contained one specified file.</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        byte[] GetLogFileBodyZipped(string fileName);
    }
}