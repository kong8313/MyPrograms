using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public interface IInboundCallService
    {
        void CreateCallHistory(BvActiveDialEntity activeDial, InboundHandlerOperationType operationType);
        void CheckAndSearchInterview(string inboundLinePhoneNumber, string callerPhoneNumber, InterviewWithCall result);
        InboundHandlerOperationType InboundHandlerOperationTypeFromDropInboundCallReason(DropInboundCallReason dropInboundCallReason);
    }
}