using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.DAL.Generated.Entity.Table
{
    public partial class BvInboundTelephoneNumberEntity
    {
        private ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor> _inboundAudioDictionary;

        public AudioMessageDescriptor GetInboundAudioMessage(AudioMessageType audioMessageType)
        {
            return InboundAudioMessagesDictionary.TryGetValue(audioMessageType, out var inboundAudioMessage)
                ? inboundAudioMessage
                : null;
        }

        public string GetInboundAudioMessageSource(AudioMessageType audioMessageType)
        {
            return InboundAudioMessagesDictionary.TryGetValue(audioMessageType, out var inboundAudioMessage)
                ? inboundAudioMessage.Source
                : string.Empty;
        }

        public int? GetInboundAudioMessageRepeatCount(AudioMessageType audioMessageType)
        {
            return InboundAudioMessagesDictionary.TryGetValue(audioMessageType, out var inboundAudioMessage)
                ? inboundAudioMessage.RepeatCount
                : null;
        }

        public ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor> InboundAudioMessagesDictionary
        {
            get
            {
                if (_inboundAudioDictionary != null)
                {
                    return _inboundAudioDictionary;
                }

                // deserialization of NULL throws an exception. Lets avoid of extra warning in log
                if (string.IsNullOrEmpty(AudioMessagesJson))
                {
                    _inboundAudioDictionary = new ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>();
                    return _inboundAudioDictionary;
                }

                try
                {
                    _inboundAudioDictionary =
                        JsonConvert.DeserializeObject<ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>>(AudioMessagesJson);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(ex.ToString());
                }

                // JsonConvert.DeserializeObject can return null. Create an empty dictionary in this case.
                if (_inboundAudioDictionary == null)
                {
                    _inboundAudioDictionary = new ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>();
                }

                return _inboundAudioDictionary;
            }

            set
            {
                AudioMessagesJson = JsonConvert.SerializeObject(value);
            }
        }
    }
}