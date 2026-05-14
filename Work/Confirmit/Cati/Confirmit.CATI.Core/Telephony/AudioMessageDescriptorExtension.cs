using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    internal static class AudioMessageDescriptorExtension
    {
        public static AudioMessageDescriptor GetActiveOrNull(this AudioMessageDescriptor descriptor)
        {
            return descriptor.IsPlayBehaviorOff() ? null : descriptor;
        }

        public static AudioMessageDescriptor GetActiveOrDefault(this AudioMessageDescriptor descriptor, AudioMessageDescriptor @default)
        {
            return descriptor.IsPlayBehaviorDefault()
                ? descriptor.GetDefaulted(@default)
                : descriptor.GetActiveOrNull();
        }

        public static AudioMessageDescriptor GetEmptySourceCheckedOrNull(this AudioMessageDescriptor descriptor)
        {
            //if Source is blank and playBehavior is default not adding this audio message
            if (string.IsNullOrWhiteSpace(descriptor.Source) && descriptor.IsPlayBehaviorDefault())
                return null;

            //if Source is blank and playBehavior is not default then drop play behavior to Off
            if (string.IsNullOrWhiteSpace(descriptor.Source) && !descriptor.IsPlayBehaviorDefault())
            {
                descriptor.Source = string.Empty;
                descriptor.SetPlayBehaviorOff();
            }

            return descriptor;
        }

        private static bool IsPlayBehaviorOff(this AudioMessageDescriptor descriptor)
        {
            return descriptor.RepeatCount == -1;
        }

        private static void SetPlayBehaviorOff(this AudioMessageDescriptor descriptor)
        {
            descriptor.RepeatCount = -1;
        }

        private static bool IsPlayBehaviorDefault(this AudioMessageDescriptor descriptor)
        {
            return descriptor.RepeatCount == null;
        }

        private static AudioMessageDescriptor GetDefaulted(this AudioMessageDescriptor descriptor, AudioMessageDescriptor @default)
        {
            if (@default == null)
                return null;

            descriptor.RepeatCount = @default.RepeatCount;
            return descriptor;
        }
    }
}