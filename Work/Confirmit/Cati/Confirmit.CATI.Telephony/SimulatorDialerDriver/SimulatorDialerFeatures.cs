using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class SimulatorDialerFeatures : IDialerFeatures
    {
        public bool IsIVRSupported { get; set; }
        public bool IsInboundSupported { get; set; }
        public bool IsExternalTransferSupported { get; set; }
        public bool IsInternalTransferSupported { get; set; }
        public bool IsCoachingSupported { get; set; }
        public bool IsBargingSupported { get; set; }
        public bool IsMonitoringMuteSupported { get; set; }
        public bool IsSoftphoneSingleSignOnSupported { get; set; }
        public bool IsAudioContentDownloadSupported { get; set; }
        public bool CustomIvrPipeline { get; set; }
    }
}