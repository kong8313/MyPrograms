using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Confirmit.CATI.Common;
using ConfirmitDialerInterface;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.SystemSettings
{
    public partial interface IDialerSettings
    {
        DiallerType Dialer { get; }
        AudioMessageDescriptor GetInboundAudioMessage(AudioMessageType audioMessageType);
        string GetInboundAudioMessageSource(AudioMessageType audioMessageType);
        int GetInboundAudioMessageRepeatCount(AudioMessageType audioMessageType);

        ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor> InboundAudioMessagesDictionary { get; set; }
    }

    public partial class DialerSettings
    {
        private ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor> _inboundAudioDictionary;

        public DiallerType Dialer
        {
            get { return (DiallerType)Enum.Parse(typeof(DiallerType), DialerType); }
        }

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

        public int GetInboundAudioMessageRepeatCount(AudioMessageType audioMessageType)
        {
            return InboundAudioMessagesDictionary.TryGetValue(audioMessageType, out var inboundAudioMessage) && inboundAudioMessage.RepeatCount.HasValue
                ? inboundAudioMessage.RepeatCount.Value
                : -1; // -1 value for Off mode
        }

        public ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor> InboundAudioMessagesDictionary
        {
            get
            {
                if (_inboundAudioDictionary != null)
                {
                    return _inboundAudioDictionary;
                }

                try
                {
                    _inboundAudioDictionary = 
                        JsonConvert.DeserializeObject<ConcurrentDictionary<AudioMessageType, AudioMessageDescriptor>>(InboundAudioMessagesJson);
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
                InboundAudioMessagesJson = JsonConvert.SerializeObject(value);
            }
        }

        partial void OnSettingsChanged()
        {
            _inboundAudioDictionary = null;
            ResetSystemSettingCache();
        }

        private void ResetSystemSettingCache()
        {
            _systemSettingCache.Reset();
        }

    }
}
