using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Telephony;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Backend.WcfServices.External.MonitoringService.Diallers
{
    /// <summary>
    /// Dialler manager for a dialer.
    /// </summary>
    public class AudioRecordsManager : IAudioRecordsManager
    {
        /// <summary>
        /// Gets audio record of interview's call.
        /// </summary>
        /// <param name="surveyId">Survey ID</param>
        /// <param name="interviewId">Interview ID</param>
        /// <param name="deferredMonitoringRecordTimeStamp">TimeStamp</param>
        /// <returns>Audio record of an interview's call. Null, if none.</returns>
        public InterviewAudioRecord GetAudioRecord(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp)
        {
            var audioRecords = GetInterviewAudioRecords(surveyId, interviewId);
            return FindClosestRecord(deferredMonitoringRecordTimeStamp, audioRecords);
        }

        /// <summary>
        /// Returns information about audio records for interview.
        /// </summary>
        /// <param name="surveyId">Survey ID.</param>
        /// <param name="interviewId">Interview ID</param>
        /// <param name="deferredMonitoringRecordTimeStamp">TimeStamp</param>
        /// <param name="interviewDuration">Interview duration</param>
        /// <returns>Array of objects with information about audio records for interview.</returns>
        public InterviewAudioRecord[] GetAudioRecords(int surveyId, int interviewId,
            DateTime deferredMonitoringRecordTimeStamp, int interviewDuration)
        {
            var audioRecords = GetInterviewAudioRecords(surveyId, interviewId);
            return FindClosestRecords(deferredMonitoringRecordTimeStamp, interviewDuration, audioRecords);
        }

        public List<InterviewAudioRecord> GetAudioRecordsInsideInterviewInterval(int surveyId, int interviewId,
            DateTime deferredMonitoringRecordTimeStamp, int interviewDuration)
        {
            var audioRecords = GetInterviewAudioRecords(surveyId, interviewId);
            return FindRecordInsideInterviewInterval(deferredMonitoringRecordTimeStamp, interviewDuration, audioRecords);
        }

        private List<InterviewAudioRecord> FindRecordInsideInterviewInterval(DateTime deferredMonitoringRecordTimeStamp, int interviewDuration, InterviewAudioRecord[] audioRecords)
        {
            var resultingAudioRecords = new List<InterviewAudioRecord>();
            if (audioRecords != null && audioRecords.Length > 0)
            {
                // Possible difference between time on client and server
                var allowedOffsetInSeconds = 5 * 60;

                // Get correct single audio record of interview's call.
                foreach (InterviewAudioRecord audioRecord in audioRecords)
                {
                    var offsetSeconds = (audioRecord.TimeStamp - deferredMonitoringRecordTimeStamp).TotalSeconds;
                    // Filter out audios starting and ending 5 minutes outside of interview duration interval
                    if (offsetSeconds < -allowedOffsetInSeconds || offsetSeconds > allowedOffsetInSeconds + interviewDuration)
                    {
                        continue;
                    }
                    resultingAudioRecords.Add(audioRecord);
                }
            }

            return resultingAudioRecords;
        }

        private InterviewAudioRecord[] FindClosestRecords(DateTime deferredMonitoringRecordTimeStamp, int interviewDuration, InterviewAudioRecord[] audioRecords)
        {
            if (audioRecords == null || audioRecords.Length == 0)
            {
                return new InterviewAudioRecord[]{};
            }

            var closestAudioRecords = new List<InterviewAudioRecord>();
            var firstRecord = FindClosestRecord(deferredMonitoringRecordTimeStamp, audioRecords);
            closestAudioRecords.Add(firstRecord);

            // Get correct audio records of interview's call in interval of interview duration.
            foreach (var audioRecord in audioRecords)
            {
                if (audioRecord.TimeStamp <= firstRecord.TimeStamp)
                    continue;

                var offset = (audioRecord.TimeStamp - firstRecord.TimeStamp).TotalSeconds;
                if (offset < interviewDuration)
                {
                    closestAudioRecords.Add(audioRecord);
                }
            }

            return closestAudioRecords.ToArray();
      
        }

        /// <summary>
        /// Returns information about audio records for interview.
        /// </summary>
        /// <param name="surveyId">Survey ID.</param>
        /// <param name="interviewId">Interview ID</param>
        /// <returns>Array of objects with information about audio records for interview.</returns>
        public InterviewAudioRecord[] GetInterviewAudioRecords(int surveyId, int interviewId)
        {
            var audioRecordUrls = ServiceLocator.Resolve<IInterviewRecordingManager>().GetInterviewRecordings(surveyId, interviewId);
            return ConvertAudioRecordInfoToInterviewAudioInfo(audioRecordUrls.ToList());
        }

        /// <summary>
        /// Gets the interview audio recording file
        /// </summary>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="audioUrl">Url to the audio file which was returned by GetAudioRecords method</param>
        /// <returns>An object with the content of audio file</returns>
        public AudioFile GetAudioFile(int dialerId, string audioUrl)
        {
            return ServiceLocator.Resolve<IInterviewRecordingManager>().GetAudioFile(dialerId, audioUrl);
        }

        private InterviewAudioRecord FindClosestRecord(DateTime deferredMonitoringRecordTimeStamp, InterviewAudioRecord[] audioRecords)
        {
            if (audioRecords != null)
            {
                if (audioRecords.Length > 0)
                {
                    InterviewAudioRecord resultingAudioRecord = null;

                    // Get correct single audio record of interview's call.
                    TimeSpan minOffset = TimeSpan.MaxValue;
                    foreach (InterviewAudioRecord audioRecord in audioRecords)
                    {
                        TimeSpan offset = (audioRecord.TimeStamp - deferredMonitoringRecordTimeStamp).Duration();
                        if (offset < minOffset)
                        {
                            minOffset = offset;
                            resultingAudioRecord = audioRecord;
                        }
                    }

                    return resultingAudioRecord;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns file name from URI.
        /// </summary>
        /// <param name="uri">URI string.</param>
        /// <returns>File name from URI. Null on any error.</returns>
        private string GetFileNameFromUri(string uri)
        {
            try
            {
                var uriObject = new Uri(uri);

                if (uriObject.Segments.Length > 0)
                {
                    return uriObject.Segments[uriObject.Segments.Length - 1];
                }

                return null;
            }
            catch
            { return null; }
        }

        private InterviewAudioRecord[] ConvertAudioRecordInfoToInterviewAudioInfo(List<AudioRecordInfo> audioRecordUrls)
        {
            if (audioRecordUrls == null || audioRecordUrls.Count == 0)
            {
                return Array.Empty<InterviewAudioRecord>();
            }

            return (from ar in audioRecordUrls
                    select new InterviewAudioRecord
                    {
                        TimeStamp = ar.DateTime,
                        URI = ar.Url,
                        Name = GetFileNameFromUri(ar.Url),
                        DialerId = ar.DialerId
                    }).ToArray();
        }

    }
}
