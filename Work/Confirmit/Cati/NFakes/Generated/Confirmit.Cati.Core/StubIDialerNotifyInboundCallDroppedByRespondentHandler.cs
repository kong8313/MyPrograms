using System;
using Confirmit.CATI.Core.Telephony.Inbound;

namespace Confirmit.CATI.Core.Telephony.Inbound.Fakes
{
    public class StubIDialerNotifyInboundCallDroppedByRespondentHandler : IDialerNotifyInboundCallDroppedByRespondentHandler 
    {
        private IDialerNotifyInboundCallDroppedByRespondentHandler _inner;

        public StubIDialerNotifyInboundCallDroppedByRespondentHandler()
        {
            _inner = null;
        }

        public IDialerNotifyInboundCallDroppedByRespondentHandler Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteInt32Int32StringDelegate(int dialerId, int companyId, string inboundCallId);
        public ExecuteInt32Int32StringDelegate ExecuteInt32Int32String;

        void IDialerNotifyInboundCallDroppedByRespondentHandler.Execute(int dialerId, int companyId, string inboundCallId)
        {

            if (ExecuteInt32Int32String != null)
            {
                ExecuteInt32Int32String(dialerId, companyId, inboundCallId);
            } else if (_inner != null)
            {
                ((IDialerNotifyInboundCallDroppedByRespondentHandler)_inner).Execute(dialerId, companyId, inboundCallId);
            }
        }

    }
}