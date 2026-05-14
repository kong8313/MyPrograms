using System.Collections.Generic;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Contracts.ErrorReportingService
{
    /// <summary>
    /// Logs the error message from the client on the server.
    /// </summary>
    [ServiceContract(Name = "ErrorReportingService", Namespace = "http://www.confirmit.com/ErrorReportingService/02/27/2010")]
    public interface IErrorReportingService
    {
        [OperationContract]
        void SendConsoleErrorMessage(
            string companyAlias,
            ClientErrorSource source,
            string errorMessage,
            byte[] hash);

        [OperationContract]
        void SendMonitoringErrorMessage(
            string companyAlias,
            ClientErrorSource source,
            string errorMessage,
            byte[] hash);


        [OperationContract]
        void SendDialerErrorMessages(IEnumerable<ErrorMessage> messages);

        [OperationContract]
        void SendLoadUtilityErrorMessages(IEnumerable<ErrorMessage> messages);
    }
}