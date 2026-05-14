using System;
using Confirmit.CATI.Common.Contracts.ErrorReportingService;
using System.Collections.Generic;
using DialerCommon.Logging;

namespace DialerCommon.Logging.Fakes
{
    public class StubIErrorSender : IErrorSender 
    {
        private IErrorSender _inner;

        public StubIErrorSender()
        {
            _inner = null;
        }

        public IErrorSender Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendErrorMessagesIErrorReportingServiceIEnumerableOfErrorMessageDelegate(IErrorReportingService x, IEnumerable<ErrorMessage> messages);
        public SendErrorMessagesIErrorReportingServiceIEnumerableOfErrorMessageDelegate SendErrorMessagesIErrorReportingServiceIEnumerableOfErrorMessage;

        void IErrorSender.SendErrorMessages(IErrorReportingService x, IEnumerable<ErrorMessage> messages)
        {

            if (SendErrorMessagesIErrorReportingServiceIEnumerableOfErrorMessage != null)
            {
                SendErrorMessagesIErrorReportingServiceIEnumerableOfErrorMessage(x, messages);
            } else if (_inner != null)
            {
                ((IErrorSender)_inner).SendErrorMessages(x, messages);
            }
        }

    }
}