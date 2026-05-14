using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    /// <summary>
    /// Defines a list of features that can be supported by the dialer. This is used by CATI to automatically enable / disable functionality that requires support by the dialer
    /// </summary>
    public class YourDialerFeaturesRealization : IDialerFeatures
    {
        /// <summary>
        /// Gets a value indicating whether Interactive Voice Response (IVR) technology is supported. IDialerCoreApi.IvrRenderVoiceXml method has to be implemented
        /// </summary>
        public bool IsIVRSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether inbound calls handling is supported
        /// </summary>
        public bool IsInboundSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether transferring calls to external numbers is supported
        /// </summary>
        public bool IsExternalTransferSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether transferring calls to another agents is supported
        /// </summary>
        public bool IsInternalTransferSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether coaching monitoring mode is supported
        /// </summary>
        public bool IsCoachingSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether barging monitoring mode is supported
        /// </summary>
        public bool IsBargingSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether muting the monitoring session is supported
        /// </summary>
        public bool IsMonitoringMuteSupported { get; set; }

        public bool IsSoftphoneSingleSignOnSupported { get; set; }

        public bool IsAudioContentDownloadSupported { get; set; }
        
        public bool CustomIvrPipeline { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public YourDialerFeaturesRealization()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialerFeatures"></param>
        public YourDialerFeaturesRealization(IDialerFeatures dialerFeatures)
        {
            IsIVRSupported = dialerFeatures.IsIVRSupported;
            IsInboundSupported = dialerFeatures.IsInboundSupported;
            IsExternalTransferSupported = dialerFeatures.IsExternalTransferSupported;
            IsInternalTransferSupported = dialerFeatures.IsInternalTransferSupported;
            IsCoachingSupported = dialerFeatures.IsCoachingSupported;
            IsBargingSupported = dialerFeatures.IsBargingSupported;
            IsMonitoringMuteSupported = dialerFeatures.IsMonitoringMuteSupported;
            IsSoftphoneSingleSignOnSupported = dialerFeatures.IsSoftphoneSingleSignOnSupported;
            IsAudioContentDownloadSupported = dialerFeatures.IsAudioContentDownloadSupported;
            CustomIvrPipeline = dialerFeatures.CustomIvrPipeline;
        }
    }
}
