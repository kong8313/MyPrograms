using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Contracts.ErrorReportingService;
using System.Collections.Generic;

namespace Confirmit.CATI.Common.Contracts.ErrorReportingService.Fakes
{
    public class StubIErrorReportingService : IErrorReportingService 
    {
        private IErrorReportingService _inner;

        public StubIErrorReportingService()
        {
            _inner = null;
        }

        public IErrorReportingService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendConsoleErrorMessageStringClientErrorSourceStringArrayOfByteDelegate(string companyAlias, ClientErrorSource source, string errorMessage, byte[] hash);
        public SendConsoleErrorMessageStringClientErrorSourceStringArrayOfByteDelegate SendConsoleErrorMessageStringClientErrorSourceStringArrayOfByte;

        void IErrorReportingService.SendConsoleErrorMessage(string companyAlias, ClientErrorSource source, string errorMessage, byte[] hash)
        {

            if (SendConsoleErrorMessageStringClientErrorSourceStringArrayOfByte != null)
            {
                SendConsoleErrorMessageStringClientErrorSourceStringArrayOfByte(companyAlias, source, errorMessage, hash);
            } else if (_inner != null)
            {
                ((IErrorReportingService)_inner).SendConsoleErrorMessage(companyAlias, source, errorMessage, hash);
            }
        }

        public delegate void SendMonitoringErrorMessageStringClientErrorSourceStringArrayOfByteDelegate(string companyAlias, ClientErrorSource source, string errorMessage, byte[] hash);
        public SendMonitoringErrorMessageStringClientErrorSourceStringArrayOfByteDelegate SendMonitoringErrorMessageStringClientErrorSourceStringArrayOfByte;

        void IErrorReportingService.SendMonitoringErrorMessage(string companyAlias, ClientErrorSource source, string errorMessage, byte[] hash)
        {

            if (SendMonitoringErrorMessageStringClientErrorSourceStringArrayOfByte != null)
            {
                SendMonitoringErrorMessageStringClientErrorSourceStringArrayOfByte(companyAlias, source, errorMessage, hash);
            } else if (_inner != null)
            {
                ((IErrorReportingService)_inner).SendMonitoringErrorMessage(companyAlias, source, errorMessage, hash);
            }
        }

        public delegate void SendDialerErrorMessagesIEnumerableOfErrorMessageDelegate(IEnumerable<ErrorMessage> messages);
        public SendDialerErrorMessagesIEnumerableOfErrorMessageDelegate SendDialerErrorMessagesIEnumerableOfErrorMessage;

        void IErrorReportingService.SendDialerErrorMessages(IEnumerable<ErrorMessage> messages)
        {

            if (SendDialerErrorMessagesIEnumerableOfErrorMessage != null)
            {
                SendDialerErrorMessagesIEnumerableOfErrorMessage(messages);
            } else if (_inner != null)
            {
                ((IErrorReportingService)_inner).SendDialerErrorMessages(messages);
            }
        }

        public delegate void SendLoadUtilityErrorMessagesIEnumerableOfErrorMessageDelegate(IEnumerable<ErrorMessage> messages);
        public SendLoadUtilityErrorMessagesIEnumerableOfErrorMessageDelegate SendLoadUtilityErrorMessagesIEnumerableOfErrorMessage;

        void IErrorReportingService.SendLoadUtilityErrorMessages(IEnumerable<ErrorMessage> messages)
        {

            if (SendLoadUtilityErrorMessagesIEnumerableOfErrorMessage != null)
            {
                SendLoadUtilityErrorMessagesIEnumerableOfErrorMessage(messages);
            } else if (_inner != null)
            {
                ((IErrorReportingService)_inner).SendLoadUtilityErrorMessages(messages);
            }
        }

    }
}