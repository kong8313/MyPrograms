using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Audio descriptor gives dialer information about audio to be played
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public class AudioMessageDescriptor
    {
        /// <summary>
        /// Audio source type
        /// </summary>
        [DataMember]
        public AudioSourceType Type { get; set; }

        /// <summary>
        /// Audio source
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        /// Number of repeats. Possible value null, 0 and int.MaxValue.
        /// 0 or null  mean do not repeat (play message once).
        /// int.MaxValue means looping.
        /// </summary>
        [DataMember]
        public int? RepeatCount { get; set; }

        public override string ToString()
        {
            return $"[Type={Type}, Source={Source}, RepeatCount={RepeatCount}]";
        }
    }

    public static class AudioMessageDescriptorExtensions
    {
        public static string NullableToString(this AudioMessageDescriptor descriptor)
        {
            return descriptor == null
                ? "[null]"
                : descriptor.ToString();
        }
    }
}