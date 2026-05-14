namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Contains information about dialler
    /// </summary>
    public class DiallerInfo
    {
        /// <summary>
        /// True if there is a dialer in the system,
        /// and the person is registered in it, false otherwise.
        /// </summary>
        public bool ConnectedToDialer { get; set; }

        /// <summary>
        /// Login to dialer state (taken from BvTasks table), makes sense only if the person
        /// is logged in to the system (see PersonInfo.AlreadyLoggedIn)
        /// 
        /// </summary>
        public int CurrentLoggedInToDialerState { get; set; }

        /// <summary>
        /// If extenstion number is provided we should not ask user about this number
        /// </summary>
        public bool HasExtensionNumber { get; set; }

        /// <summary>
        /// True if hang up action is available for interviewer otherwise false,
        /// it depends on dialer type.
        /// </summary>
        public bool IsHangUpSupported { get; set; }

        /// Gets/sets Login to dialer type
        /// Makes sense only if alreadyLoggedIn = true 
        /// and the person is logged in to dialer.
        public bool CurrentIsPredictive { get; set; }

        /// <summary>
        /// True if pause/resume playback command is available for interviewer otherwise false,
        /// it depends on dialer type.
        /// </summary>
        public bool IsPauseOrResumePlaybackSupported { get; set; }

        /// <summary>
        /// True if toggle voice source command is available for interviewer otherwise false,
        /// it depends on dialer type.
        /// </summary>
        public bool IsToggleVoiceSourceSupported { get; set; }
    }
}
