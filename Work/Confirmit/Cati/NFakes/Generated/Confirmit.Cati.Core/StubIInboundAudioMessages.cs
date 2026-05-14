using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using Confirmit.CATI.Core.Telephony.Inbound;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIInboundAudioMessages : IInboundAudioMessages 
    {
        private IInboundAudioMessages _inner;

        public StubIInboundAudioMessages()
        {
            _inner = null;
        }

        public IInboundAudioMessages Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate AudioMessageDescriptor GetBvInboundTelephoneNumberEntityAudioMessageTypeDelegate(BvInboundTelephoneNumberEntity inboundTelephoneNumberEntity, AudioMessageType messageType);
        public GetBvInboundTelephoneNumberEntityAudioMessageTypeDelegate GetBvInboundTelephoneNumberEntityAudioMessageType;

        AudioMessageDescriptor IInboundAudioMessages.Get(BvInboundTelephoneNumberEntity inboundTelephoneNumberEntity, AudioMessageType messageType)
        {


            if (GetBvInboundTelephoneNumberEntityAudioMessageType != null)
            {
                return GetBvInboundTelephoneNumberEntityAudioMessageType(inboundTelephoneNumberEntity, messageType);
            } else if (_inner != null)
            {
                return ((IInboundAudioMessages)_inner).Get(inboundTelephoneNumberEntity, messageType);
            }

            return default(AudioMessageDescriptor);
        }

        public delegate AudioMessageDescriptor GetStringAudioMessageTypeDelegate(string inboundDdiNumber, AudioMessageType messageType);
        public GetStringAudioMessageTypeDelegate GetStringAudioMessageType;

        AudioMessageDescriptor IInboundAudioMessages.Get(string inboundDdiNumber, AudioMessageType messageType)
        {


            if (GetStringAudioMessageType != null)
            {
                return GetStringAudioMessageType(inboundDdiNumber, messageType);
            } else if (_inner != null)
            {
                return ((IInboundAudioMessages)_inner).Get(inboundDdiNumber, messageType);
            }

            return default(AudioMessageDescriptor);
        }

        public delegate IEnumerable<KeyValuePair<AudioMessageType, AudioMessageDescriptor>> DdiNumbersMessagesBvInboundTelephoneNumberEntityDelegate(BvInboundTelephoneNumberEntity inboundTelephoneNumberEntity);
        public DdiNumbersMessagesBvInboundTelephoneNumberEntityDelegate DdiNumbersMessagesBvInboundTelephoneNumberEntity;

        IEnumerable<KeyValuePair<AudioMessageType, AudioMessageDescriptor>> IInboundAudioMessages.DdiNumbersMessages(BvInboundTelephoneNumberEntity inboundTelephoneNumberEntity)
        {


            if (DdiNumbersMessagesBvInboundTelephoneNumberEntity != null)
            {
                return DdiNumbersMessagesBvInboundTelephoneNumberEntity(inboundTelephoneNumberEntity);
            } else if (_inner != null)
            {
                return ((IInboundAudioMessages)_inner).DdiNumbersMessages(inboundTelephoneNumberEntity);
            }

            return default(IEnumerable<KeyValuePair<AudioMessageType, AudioMessageDescriptor>>);
        }

        public delegate AudioMessageDescriptor FromDropCallReasonStringDropInboundCallReasonDelegate(string inboundDdiNumber, DropInboundCallReason dropInboundCallReason);
        public FromDropCallReasonStringDropInboundCallReasonDelegate FromDropCallReasonStringDropInboundCallReason;

        AudioMessageDescriptor IInboundAudioMessages.FromDropCallReason(string inboundDdiNumber, DropInboundCallReason dropInboundCallReason)
        {


            if (FromDropCallReasonStringDropInboundCallReason != null)
            {
                return FromDropCallReasonStringDropInboundCallReason(inboundDdiNumber, dropInboundCallReason);
            } else if (_inner != null)
            {
                return ((IInboundAudioMessages)_inner).FromDropCallReason(inboundDdiNumber, dropInboundCallReason);
            }

            return default(AudioMessageDescriptor);
        }

    }
}