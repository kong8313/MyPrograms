using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.UnitTests.Telephony
{
    internal class AudioMessageDescriptorContext
    {
        private readonly AudioMessageType _audioMessageType;
        private AudioMessageDescriptor _defaultDescriptor;
        private StubIDialerSettings _stubIDialerSettings;
        private AudioMessageDescriptor _overrideDescriptor;
        private BvInboundTelephoneNumberEntity _telephoneNumberEntity;
        private InboundAudioMessages _inboundAudioMessages;

        public AudioMessageDescriptorContext(AudioMessageType audioMessageType)
        {
            _audioMessageType = audioMessageType;
            _stubIDialerSettings = new StubIDialerSettings();
            _inboundAudioMessages = new InboundAudioMessages(_stubIDialerSettings, null);
        }

        public AudioMessageDescriptorContext SetDefault(AudioMessageDescriptor descriptor)
        {
            if (descriptor != null && descriptor.RepeatCount == null)
                throw new InvalidOperationException($"Null value of {nameof(descriptor.RepeatCount)} not available for the default descriptor");

            _defaultDescriptor = descriptor;
            _stubIDialerSettings.GetInboundAudioMessageAudioMessageType =
                type => type == _audioMessageType ? _defaultDescriptor : null;
            return this;
        }

        public AudioMessageDescriptorContext SetDefault(int sourceType, string source, int? repeatCount)
        {
            var descriptor = CreateDescriptor((AudioSourceType)sourceType, source, repeatCount);
            return SetDefault(descriptor);
        }

        public AudioMessageDescriptorContext SetOverride(AudioMessageDescriptor descriptor)
        {
            _overrideDescriptor = descriptor;

            var descriptorDict = new Dictionary<AudioMessageType, AudioMessageDescriptor> { { _audioMessageType, _overrideDescriptor } };

            _telephoneNumberEntity = new BvInboundTelephoneNumberEntity
            {
                AudioMessagesJson = JsonConvert.SerializeObject(descriptorDict)
            };

            return this;
        }

        public AudioMessageDescriptorContext SetOverride(int sourceType, string source, int? repeatCount)
        {
            var descriptor = CreateDescriptor((AudioSourceType)sourceType, source, repeatCount);
            return SetOverride(descriptor);
        }

        public bool ResultEqualsTo(AudioMessageDescriptor descriptor)
        {
            var result = _inboundAudioMessages.Get(_telephoneNumberEntity, _audioMessageType);

            return (result == null && descriptor == null) ||
                   (result != null && descriptor != null &&
                    result.Type == descriptor.Type &&
                    result.Source == descriptor.Source &&
                    result.RepeatCount == descriptor.RepeatCount);
        }

        public bool ResultEqualsTo(int sourceType, string source, int? repeatCount)
        {
            var descriptor = CreateDescriptor((AudioSourceType)sourceType, source, repeatCount);
            return ResultEqualsTo(descriptor);
        }

        private static AudioMessageDescriptor CreateDescriptor(AudioSourceType sourceType, string source, int? repeatCount)
        {
            var descriptor = new AudioMessageDescriptor
            {
                Type = sourceType,
                Source = source,
                RepeatCount = repeatCount
            };
            return descriptor;
        }
    }
}