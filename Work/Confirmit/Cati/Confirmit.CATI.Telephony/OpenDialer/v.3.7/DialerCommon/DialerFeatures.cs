using ConfirmitDialerInterface;

namespace DialerCommon
{
    /// <summary>
    /// Defines a list of features that can be supported by the dialer. This is used by CATI to automatically enable / disable functionality that requires support by the dialer
    /// </summary>
    public class DialerFeatures
    {
        /// <summary>
        /// Gets a value indicating whether Interactive Voice Response (IVR) technology is supported. IDialerCoreApi.IvrRenderVoiceXml method has to be implemented
        /// </summary>
        public bool? IsIVRSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether inbound calls handling is supported
        /// </summary>
        public bool? IsInboundSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether transferring calls to external numbers is supported
        /// </summary>
        public bool? IsExternalTransferSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether transferring calls to another agents is supported
        /// </summary>
        public bool? IsInternalTransferSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether coaching monitoring mode is supported
        /// </summary>
        public bool? IsCoachingSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether barging monitoring mode is supported
        /// </summary>
        public bool? IsBargingSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether muting the monitoring session is supported
        /// </summary>
        public bool? IsMonitoringMuteSupported { get; set; }

        /// <summary>
        /// Gets a value indicating whether dialer supports web-based softphone for agents and have an ability to perform
        /// single sign-on with the interviewer application.
        /// <seealso cref="IDialerCoreApi.RegisterAgentSoftphone"/> method has to be implemented.
        /// </summary>
        public bool? IsSoftphoneSingleSignOnSupported { get; set; }

        /// <summary>
        /// Get a value indicating whether dialer supports download of the interview audio recording file through the dialer API
        /// <seealso cref="IDialerRecordingApi.GetAudioFile"/> method has to be implemented.
        /// </summary>
        public bool? IsAudioContentDownloadSupported { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether custom ivr pipeline is supported. For internal use only
        /// </summary>
        public bool? CustomIvrPipeline { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DialerFeatures()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialerFeatures"></param>
        public DialerFeatures(IDialerFeatures dialerFeatures)
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
