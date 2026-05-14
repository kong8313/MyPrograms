namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Defines a list of features that can be supported by the dialer. This is used by CATI to automatically enable / disable functionality that requires support by the dialer
    /// </summary>
    public interface IDialerFeatures
    {
        /// <summary>
        /// Gets a value indicating whether Interactive Voice Response (IVR) technology is supported. IDialerCoreApi.IvrRenderVoiceXml method has to be implemented
        /// </summary>
        bool IsIVRSupported { get; }

        /// <summary>
        /// Gets a value indicating whether inbound calls handling is supported
        /// </summary>
        bool IsInboundSupported { get; }

        /// <summary>
        /// Gets a value indicating whether transferring calls to external numbers is supported
        /// </summary>
        bool IsExternalTransferSupported { get; }

        /// <summary>
        /// Gets a value indicating whether transferring calls to another agents is supported
        /// </summary>
        bool IsInternalTransferSupported { get; }

        /// <summary>
        /// Gets a value indicating whether coaching monitoring mode is supported
        /// </summary>
        bool IsCoachingSupported { get; }

        /// <summary>
        /// Gets a value indicating whether barging monitoring mode is supported
        /// </summary>
        bool IsBargingSupported { get; }

        /// <summary>
        /// Gets a value indicating whether muting the monitoring session is supported
        /// </summary>
        bool IsMonitoringMuteSupported { get; }

        /// <summary>
        /// Gets a value indicating whether dialer supports web-based softphone for agents and have an ability to perform
        /// single sign-on with the interviewer application.
        /// <seealso cref="IDialerCoreApi.RegisterAgentSoftphone"/> method has to be implemented.
        /// </summary>
        bool IsSoftphoneSingleSignOnSupported { get; }
        
        /// <summary>
        /// Get a value indicating whether dialer supports download of the interview audio recording file through the dialer API
        /// <seealso cref="IDialerRecordingApi.GetAudioFile"/> method has to be implemented.
        /// </summary>
        bool IsAudioContentDownloadSupported { get; }
        
        /// <summary>
        /// Gets a value indicating whether custom ivr pipeline is supported. For internal use only
        /// </summary>
        bool CustomIvrPipeline { get; }
        
    }
}
