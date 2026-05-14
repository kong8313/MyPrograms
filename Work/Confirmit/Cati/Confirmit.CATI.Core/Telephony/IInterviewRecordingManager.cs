using System;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IInterviewRecordingManager
    {
        /// <summary>
        /// Gets the list of boolean flags indicating whether there are some recordings are available for the specific interview ID.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <param name="interviewIds">The list of interview IDs to determine whether recordings are available for.</param>
        /// <returns>
        /// The list of boolean flags. Flags count is always equal to the count of interview IDs list.
        /// </returns>
        bool[] AreRecordsExists(int surveySid, int[] interviewIds);


        /// <summary>
        /// Gets the list of URLs to interview recordings by interview ID.
        /// </summary>
        /// <param name="surveySid">The surveySID.</param>
        /// <param name="interviewId">The interview ID to get recordings for.</param>
        /// <returns>Enumeration of interview recordings</returns>
        /// <exception cref="ArgumentOutOfRangeException"><c>callId</c> is out of range.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><c>surveySid</c> is out of range.</exception>
        /// <exception cref="ArgumentException">Invalid ProjectID</exception>
        IEnumerable<AudioRecordInfo> GetInterviewRecordings(int surveySid, int interviewId);

        /// <summary>
        /// Gets the interview audio recording file
        /// </summary>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="audioUrl">Url to the audio file which was returned by GetAudioRecords method</param>
        /// <returns>An object with the content of audio file</returns>
        AudioFile GetAudioFile(int dialerId, string audioUrl);
        
        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        /// <param name="projectId">The project ID (pXXXXXXX).</param>
        /// <param name="interviewId">The ID of currently active interview to start recording of.</param>
        /// <param name="label">The label. It will be included in the name of the recorded audio file.</param>
        void StartRecording(string projectId, int interviewId, string label);

        /// <summary>
        /// Stops interview recording.
        /// </summary>
        /// <param name="surveyName">Survey name (project ID)</param>
        /// <param name="interviewId">Interview ID</param>
        /// <param name="stopRecordingMode">
        /// StopRecordingMode: stop whole interview recording, or sectional or both?
        /// </param>
        void StopRecording(string surveyName, int interviewId, string stopRecordingMode);

        bool ParseStopRecordingMode(string strStopRecordingMode, out StopRecordingMode typedStopRecordingMode);
    }
}