using System;

namespace Confirmit.CATI.Core.Telephony.Inbound
{
    public class InboundCallCantProceedException : Exception
    {
        public DropInboundCallReason DropInboundCallReason { get; private set; }

        public InboundCallCantProceedException(string inboundFeatureIsDisabled, DropInboundCallReason dropInboundCallReason)
            : base(inboundFeatureIsDisabled)
        {
            DropInboundCallReason = dropInboundCallReason;
        }
    }
}