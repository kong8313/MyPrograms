using Confirmit.CATI.Telephony;

namespace BvCallHandlerLibrary.Tools
{
    public interface IMnTciTools
    {
        IDialerRecordingAPI CreateDialerRecording(int dialerId);
        bool IsDialerConfigured();
        bool DoesCompanyUseTelephony();
    }
}