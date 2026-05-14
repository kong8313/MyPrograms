using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIInterviewRecordingManager : IInterviewRecordingManager 
    {
        private IInterviewRecordingManager _inner;

        public StubIInterviewRecordingManager()
        {
            _inner = null;
        }

        public IInterviewRecordingManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool[] AreRecordsExistsInt32ArrayOfInt32Delegate(int surveySid, int[] interviewIds);
        public AreRecordsExistsInt32ArrayOfInt32Delegate AreRecordsExistsInt32ArrayOfInt32;

        bool[] IInterviewRecordingManager.AreRecordsExists(int surveySid, int[] interviewIds)
        {


            if (AreRecordsExistsInt32ArrayOfInt32 != null)
            {
                return AreRecordsExistsInt32ArrayOfInt32(surveySid, interviewIds);
            } else if (_inner != null)
            {
                return ((IInterviewRecordingManager)_inner).AreRecordsExists(surveySid, interviewIds);
            }

            return default(bool[]);
        }

        public delegate IEnumerable<AudioRecordInfo> GetInterviewRecordingsInt32Int32Delegate(int surveySid, int interviewId);
        public GetInterviewRecordingsInt32Int32Delegate GetInterviewRecordingsInt32Int32;

        IEnumerable<AudioRecordInfo> IInterviewRecordingManager.GetInterviewRecordings(int surveySid, int interviewId)
        {


            if (GetInterviewRecordingsInt32Int32 != null)
            {
                return GetInterviewRecordingsInt32Int32(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((IInterviewRecordingManager)_inner).GetInterviewRecordings(surveySid, interviewId);
            }

            return default(IEnumerable<AudioRecordInfo>);
        }

        public delegate AudioFile GetAudioFileInt32StringDelegate(int dialerId, string audioUrl);
        public GetAudioFileInt32StringDelegate GetAudioFileInt32String;

        AudioFile IInterviewRecordingManager.GetAudioFile(int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32String != null)
            {
                return GetAudioFileInt32String(dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((IInterviewRecordingManager)_inner).GetAudioFile(dialerId, audioUrl);
            }

            return default(AudioFile);
        }

        public delegate void StartRecordingStringInt32StringDelegate(string projectId, int interviewId, string label);
        public StartRecordingStringInt32StringDelegate StartRecordingStringInt32String;

        void IInterviewRecordingManager.StartRecording(string projectId, int interviewId, string label)
        {

            if (StartRecordingStringInt32String != null)
            {
                StartRecordingStringInt32String(projectId, interviewId, label);
            } else if (_inner != null)
            {
                ((IInterviewRecordingManager)_inner).StartRecording(projectId, interviewId, label);
            }
        }

        public delegate void StopRecordingStringInt32StringDelegate(string surveyName, int interviewId, string stopRecordingMode);
        public StopRecordingStringInt32StringDelegate StopRecordingStringInt32String;

        void IInterviewRecordingManager.StopRecording(string surveyName, int interviewId, string stopRecordingMode)
        {

            if (StopRecordingStringInt32String != null)
            {
                StopRecordingStringInt32String(surveyName, interviewId, stopRecordingMode);
            } else if (_inner != null)
            {
                ((IInterviewRecordingManager)_inner).StopRecording(surveyName, interviewId, stopRecordingMode);
            }
        }

        public delegate bool ParseStopRecordingModeStringStopRecordingModeOutDelegate(string strStopRecordingMode, out StopRecordingMode typedStopRecordingMode);
        public ParseStopRecordingModeStringStopRecordingModeOutDelegate ParseStopRecordingModeStringStopRecordingModeOut;

        bool IInterviewRecordingManager.ParseStopRecordingMode(string strStopRecordingMode, out StopRecordingMode typedStopRecordingMode)
        {
            typedStopRecordingMode = default(StopRecordingMode);


            if (ParseStopRecordingModeStringStopRecordingModeOut != null)
            {
                return ParseStopRecordingModeStringStopRecordingModeOut(strStopRecordingMode, out typedStopRecordingMode);
            } else if (_inner != null)
            {
                return ((IInterviewRecordingManager)_inner).ParseStopRecordingMode(strStopRecordingMode, out typedStopRecordingMode);
            }

            return default(bool);
        }

    }
}