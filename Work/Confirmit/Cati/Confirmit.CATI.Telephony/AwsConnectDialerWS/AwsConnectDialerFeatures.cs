using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS
{
    public class AwsConnectDialerFeatures : IDialerFeatures
    {
        public bool CustomIvrPipeline => true;
        public bool IsIVRSupported => false;
        public bool IsInboundSupported => false;
        public bool IsExternalTransferSupported => false;
        public bool IsInternalTransferSupported => false;
        public bool IsCoachingSupported => false;
        public bool IsBargingSupported => false;
        public bool IsMonitoringMuteSupported => false;
        public bool IsSoftphoneSingleSignOnSupported => false;
        public bool IsAudioContentDownloadSupported => false;
    }
}