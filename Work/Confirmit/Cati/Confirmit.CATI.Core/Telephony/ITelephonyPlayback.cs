using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{

    /// <summary>
    /// This interface describes operations of so called jingle playback (i.e. playing voice files to respondents).
    /// Unlike other interfaces inherited by ITelephony interface this one does not have self-initialization. 
    /// This means that all functions below must be called on TelephonyProvider object which initialized before.
    /// The interfcae is created to keep jingle playback concerning functions together.
    /// </summary>
    public interface ITelephonyPlayback
    {
        /// <summary>
        /// Start playing voice file to respondent
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="interviewId"></param>
        /// <param name="callId"></param>
        /// <param name="fileName"></param>
        /// <param name="timeOfPlayingInSeconds"></param>
        /// <returns></returns>
        DialerErrorCode StartPlayback(
            int dialerId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            string fileName,
            out int timeOfPlayingInSeconds);

        /// <summary>
        /// Stop playing voice file to respondent
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="interviewId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        DialerErrorCode StopPlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId);

        /// <summary>
        /// Returns true if pause/resume playback operation is supported by dialer, false otherwise.
        /// </summary>
        /// <returns></returns>
        bool IsPauseOrResumePlaybackSupported(int? dialerId = null);

        /// <summary>
        /// Returns true if toggle voice source operation is supported by dialer, false otherwise.
        /// </summary>
        /// <returns></returns>
        bool IsToggleInterviewerListensToPlaybackOrRespondentSupported(int? dialerId = null);

        /// <summary>
        /// Pauses or resumes playing voice file to respondent
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="interviewId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        DialerErrorCode PauseOrResumePlayback(int dialerId, long campaignId, string agentId, int interviewId,
            int callId);

        /// <summary>
        /// Switch agent from hear respondent to hear playing and back
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="interviewId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int dialerId, long campaignId, string agentId,
            int interviewId, int callId);
    }
}
