using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Telephony.Inbound;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IInboundAudioMessages
    {
        AudioMessageDescriptor Get(BvInboundTelephoneNumberEntity inboundTelephoneNumberEntity, AudioMessageType messageType);
        AudioMessageDescriptor Get(string inboundDdiNumber, AudioMessageType messageType);

        IEnumerable<KeyValuePair<AudioMessageType, AudioMessageDescriptor>> DdiNumbersMessages(
            BvInboundTelephoneNumberEntity inboundTelephoneNumberEntity);

        AudioMessageDescriptor FromDropCallReason(string inboundDdiNumber, DropInboundCallReason dropInboundCallReason);
    }
}