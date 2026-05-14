using System;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Backend.WcfServices.External.MonitoringService.Diallers
{
    public interface IAudioRecordsManager
    {
        /// <summary>
        /// Gets audio record of interview's call.
        /// </summary>
        /// <param name="surveyId">Survey ID</param>
        /// <param name="interviewId">Interview ID</param>
        /// <param name="deferredMonitoringRecordTimeStamp">TimeStamp</param>
        /// <returns>Audio record of an interview's call. Null, if none.</returns>
        InterviewAudioRecord GetAudioRecord(int surveyId, int interviewId,
            DateTime deferredMonitoringRecordTimeStamp);

        /// <summary>
        /// Returns information about audio records for interview.
        /// </summary>
        /// <param name="surveyId">Survey ID.</param>
        /// <param name="interviewId">Interview ID</param>
        /// <returns>Array of objects with information about audio records for interview.</returns>
        InterviewAudioRecord[] GetInterviewAudioRecords(int surveyId, int interviewId);

        /// <summary>
        /// Gets the interview audio recording file
        /// </summary>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="audioUrl">Url to the audio file which was returned by GetAudioRecords method</param>
        /// <returns>An object with the content of audio file</returns>
        AudioFile GetAudioFile(int dialerId, string audioUrl);
        
        /// <summary>
        /// Returns information about audio records for interview.
        /// </summary>
        /// <param name="surveyId">Survey ID.</param>
        /// <param name="interviewId">Interview ID</param>
        /// <param name="deferredMonitoringRecordTimeStamp">TimeStamp</param>
        /// <param name="interviewDuration">Interview duration</param>
        /// <returns>Array of objects with information about audio records for interview.</returns>
        InterviewAudioRecord[] GetAudioRecords(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp, int interviewDuration);
        List<InterviewAudioRecord> GetAudioRecordsInsideInterviewInterval(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp, int interviewDuration);
    }
}