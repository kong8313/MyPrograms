using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Telephony.Inbound;

namespace Confirmit.CATI.Core.Telephony.Inbound.Fakes
{
    public class StubIInboundCallService : IInboundCallService 
    {
        private IInboundCallService _inner;

        public StubIInboundCallService()
        {
            _inner = null;
        }

        public IInboundCallService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CreateCallHistoryBvActiveDialEntityInboundHandlerOperationTypeDelegate(BvActiveDialEntity activeDial, InboundHandlerOperationType operationType);
        public CreateCallHistoryBvActiveDialEntityInboundHandlerOperationTypeDelegate CreateCallHistoryBvActiveDialEntityInboundHandlerOperationType;

        void IInboundCallService.CreateCallHistory(BvActiveDialEntity activeDial, InboundHandlerOperationType operationType)
        {

            if (CreateCallHistoryBvActiveDialEntityInboundHandlerOperationType != null)
            {
                CreateCallHistoryBvActiveDialEntityInboundHandlerOperationType(activeDial, operationType);
            } else if (_inner != null)
            {
                ((IInboundCallService)_inner).CreateCallHistory(activeDial, operationType);
            }
        }

        public delegate void CheckAndSearchInterviewStringStringInterviewWithCallDelegate(string inboundLinePhoneNumber, string callerPhoneNumber, InterviewWithCall result);
        public CheckAndSearchInterviewStringStringInterviewWithCallDelegate CheckAndSearchInterviewStringStringInterviewWithCall;

        void IInboundCallService.CheckAndSearchInterview(string inboundLinePhoneNumber, string callerPhoneNumber, InterviewWithCall result)
        {

            if (CheckAndSearchInterviewStringStringInterviewWithCall != null)
            {
                CheckAndSearchInterviewStringStringInterviewWithCall(inboundLinePhoneNumber, callerPhoneNumber, result);
            } else if (_inner != null)
            {
                ((IInboundCallService)_inner).CheckAndSearchInterview(inboundLinePhoneNumber, callerPhoneNumber, result);
            }
        }

        public delegate InboundHandlerOperationType InboundHandlerOperationTypeFromDropInboundCallReasonDropInboundCallReasonDelegate(DropInboundCallReason dropInboundCallReason);
        public InboundHandlerOperationTypeFromDropInboundCallReasonDropInboundCallReasonDelegate InboundHandlerOperationTypeFromDropInboundCallReasonDropInboundCallReason;

        InboundHandlerOperationType IInboundCallService.InboundHandlerOperationTypeFromDropInboundCallReason(DropInboundCallReason dropInboundCallReason)
        {


            if (InboundHandlerOperationTypeFromDropInboundCallReasonDropInboundCallReason != null)
            {
                return InboundHandlerOperationTypeFromDropInboundCallReasonDropInboundCallReason(dropInboundCallReason);
            } else if (_inner != null)
            {
                return ((IInboundCallService)_inner).InboundHandlerOperationTypeFromDropInboundCallReason(dropInboundCallReason);
            }

            return default(InboundHandlerOperationType);
        }

    }
}