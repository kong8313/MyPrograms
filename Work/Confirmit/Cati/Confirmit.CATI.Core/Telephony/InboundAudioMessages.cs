using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony.Inbound;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public class InboundAudioMessages : IInboundAudioMessages
    {
        private readonly IDialerSettings _dialerSettings;
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;

        public readonly List<AudioMessageType> DdiNumbersMessageTypes = new List<AudioMessageType>
        {
            AudioMessageType.IncomingCall,
            AudioMessageType.IncomingCallMandatory,
            AudioMessageType.DropCallSystemFault,
        };

        public InboundAudioMessages(
            IDialerSettings dialerSettings,
            IInboundTelephoneNumberRepository inboundTelephoneNumberRepository)
        {
            _dialerSettings = dialerSettings;
            _inboundTelephoneNumberRepository = inboundTelephoneNumberRepository;
        }

        /// <summary>
        /// If AudioMessageDescriptor from inboundTelephoneNumberEntity doesn't exist then get it from BvSystemSettings
        /// If RepeatCount of AudioMessageDescriptor from inboundTelephoneNumberEntity is null then get it from RepeatCount of AudioMessageDescriptor from BvSystemSettings
        /// </summary>
        /// <param name="inboundTelephoneNumberEntity"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public AudioMessageDescriptor Get(BvInboundTelephoneNumberEntity inboundTelephoneNumberEntity, AudioMessageType messageType)
        {
            var defaultDescriptor = _dialerSettings.GetInboundAudioMessage(messageType);

            defaultDescriptor = defaultDescriptor?.GetActiveOrNull();

            if (inboundTelephoneNumberEntity == null)
                return defaultDescriptor;

            var ddiSpecificDescriptor = inboundTelephoneNumberEntity.GetInboundAudioMessage(messageType);

            ddiSpecificDescriptor = ddiSpecificDescriptor?.GetEmptySourceCheckedOrNull();

            return ddiSpecificDescriptor == null
                ? defaultDescriptor
                : ddiSpecificDescriptor.GetActiveOrDefault(defaultDescriptor);
        }

        public AudioMessageDescriptor Get(string inboundDdiNumber, AudioMessageType messageType)
        {
            var inboundTelephoneNumberEntity = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(inboundDdiNumber);
            return Get(inboundTelephoneNumberEntity, messageType);
        }

        public IEnumerable<KeyValuePair<AudioMessageType, AudioMessageDescriptor>> DdiNumbersMessages(
            BvInboundTelephoneNumberEntity inboundTelephoneNumberEntity)
        {
            return DdiNumbersMessageTypes
                .Select(messageType =>
                {
                    var audioMessageDescriptor = Get(inboundTelephoneNumberEntity, messageType);

                    return new KeyValuePair<AudioMessageType, AudioMessageDescriptor>(
                        messageType, audioMessageDescriptor);
                })
                .Where(x => x.Value != null);
        }

        public AudioMessageDescriptor FromDropCallReason(
            string inboundDdiNumber,
            DropInboundCallReason dropInboundCallReason)
        {
            var inboundTelephoneNumberEntity = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(inboundDdiNumber);

            switch (dropInboundCallReason)
            {
                case DropInboundCallReason.DdiRecordIsNotFound:
                case DropInboundCallReason.NotAcceptedBySchedulingScript:
                case DropInboundCallReason.InterviewIsNotFound:
                    return Get(inboundTelephoneNumberEntity, AudioMessageType.DropCallInterviewNotFound);

                case DropInboundCallReason.SurveyIsNotFound:
                case DropInboundCallReason.SurveyIsNotOpened:
                    return Get(inboundTelephoneNumberEntity, AudioMessageType.DropCallCampaignNotAvailable);

                case DropInboundCallReason.ShiftIsNotFound:
                case DropInboundCallReason.NoAgentsAvailable:
                    return Get(inboundTelephoneNumberEntity, AudioMessageType.DropCallOutOfShift);
            }

            return Get(inboundTelephoneNumberEntity, AudioMessageType.DropCallSystemFault);
        }
    }
}
