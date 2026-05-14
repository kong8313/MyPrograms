using System;
using Confirmit.CATI.Backend.WcfServices.External.MonitoringService.Diallers;
using ConfirmitDialerInterface;
using System.Collections.Generic;

namespace Confirmit.CATI.Backend.WcfServices.External.MonitoringService.Diallers.Fakes
{
    public class StubIAudioRecordsManager : IAudioRecordsManager 
    {
        private IAudioRecordsManager _inner;

        public StubIAudioRecordsManager()
        {
            _inner = null;
        }

        public IAudioRecordsManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate InterviewAudioRecord GetAudioRecordInt32Int32DateTimeDelegate(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp);
        public GetAudioRecordInt32Int32DateTimeDelegate GetAudioRecordInt32Int32DateTime;

        InterviewAudioRecord IAudioRecordsManager.GetAudioRecord(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp)
        {


            if (GetAudioRecordInt32Int32DateTime != null)
            {
                return GetAudioRecordInt32Int32DateTime(surveyId, interviewId, deferredMonitoringRecordTimeStamp);
            } else if (_inner != null)
            {
                return ((IAudioRecordsManager)_inner).GetAudioRecord(surveyId, interviewId, deferredMonitoringRecordTimeStamp);
            }

            return default(InterviewAudioRecord);
        }

        public delegate InterviewAudioRecord[] GetInterviewAudioRecordsInt32Int32Delegate(int surveyId, int interviewId);
        public GetInterviewAudioRecordsInt32Int32Delegate GetInterviewAudioRecordsInt32Int32;

        InterviewAudioRecord[] IAudioRecordsManager.GetInterviewAudioRecords(int surveyId, int interviewId)
        {


            if (GetInterviewAudioRecordsInt32Int32 != null)
            {
                return GetInterviewAudioRecordsInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((IAudioRecordsManager)_inner).GetInterviewAudioRecords(surveyId, interviewId);
            }

            return default(InterviewAudioRecord[]);
        }

        public delegate AudioFile GetAudioFileInt32StringDelegate(int dialerId, string audioUrl);
        public GetAudioFileInt32StringDelegate GetAudioFileInt32String;

        AudioFile IAudioRecordsManager.GetAudioFile(int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32String != null)
            {
                return GetAudioFileInt32String(dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((IAudioRecordsManager)_inner).GetAudioFile(dialerId, audioUrl);
            }

            return default(AudioFile);
        }

        public delegate InterviewAudioRecord[] GetAudioRecordsInt32Int32DateTimeInt32Delegate(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp, int interviewDuration);
        public GetAudioRecordsInt32Int32DateTimeInt32Delegate GetAudioRecordsInt32Int32DateTimeInt32;

        InterviewAudioRecord[] IAudioRecordsManager.GetAudioRecords(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp, int interviewDuration)
        {


            if (GetAudioRecordsInt32Int32DateTimeInt32 != null)
            {
                return GetAudioRecordsInt32Int32DateTimeInt32(surveyId, interviewId, deferredMonitoringRecordTimeStamp, interviewDuration);
            } else if (_inner != null)
            {
                return ((IAudioRecordsManager)_inner).GetAudioRecords(surveyId, interviewId, deferredMonitoringRecordTimeStamp, interviewDuration);
            }

            return default(InterviewAudioRecord[]);
        }

        public delegate List<InterviewAudioRecord> GetAudioRecordsInsideInterviewIntervalInt32Int32DateTimeInt32Delegate(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp, int interviewDuration);
        public GetAudioRecordsInsideInterviewIntervalInt32Int32DateTimeInt32Delegate GetAudioRecordsInsideInterviewIntervalInt32Int32DateTimeInt32;

        List<InterviewAudioRecord> IAudioRecordsManager.GetAudioRecordsInsideInterviewInterval(int surveyId, int interviewId, DateTime deferredMonitoringRecordTimeStamp, int interviewDuration)
        {


            if (GetAudioRecordsInsideInterviewIntervalInt32Int32DateTimeInt32 != null)
            {
                return GetAudioRecordsInsideInterviewIntervalInt32Int32DateTimeInt32(surveyId, interviewId, deferredMonitoringRecordTimeStamp, interviewDuration);
            } else if (_inner != null)
            {
                return ((IAudioRecordsManager)_inner).GetAudioRecordsInsideInterviewInterval(surveyId, interviewId, deferredMonitoringRecordTimeStamp, interviewDuration);
            }

            return default(List<InterviewAudioRecord>);
        }

    }
}