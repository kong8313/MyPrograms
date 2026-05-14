using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubITelephonyRecording : ITelephonyRecording 
    {
        private ITelephonyRecording _inner;

        public StubITelephonyRecording()
        {
            _inner = null;
        }

        public ITelephonyRecording Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeRecordingDelegate();
        public InitializeRecordingDelegate InitializeRecording;

        void ITelephonyRecording.InitializeRecording()
        {

            if (InitializeRecording != null)
            {
                InitializeRecording();
            } else if (_inner != null)
            {
                ((ITelephonyRecording)_inner).InitializeRecording();
            }
        }

        public delegate IEnumerable<AudioRecordInfo> GetAudioRecordsInt32Int32Delegate(int surveyId, int interviewId);
        public GetAudioRecordsInt32Int32Delegate GetAudioRecordsInt32Int32;

        IEnumerable<AudioRecordInfo> ITelephonyRecording.GetAudioRecords(int surveyId, int interviewId)
        {


            if (GetAudioRecordsInt32Int32 != null)
            {
                return GetAudioRecordsInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((ITelephonyRecording)_inner).GetAudioRecords(surveyId, interviewId);
            }

            return default(IEnumerable<AudioRecordInfo>);
        }

        public delegate bool[] AreRecordsExistsInt32ArrayOfInt32Delegate(int surveyId, int[] interviewIds);
        public AreRecordsExistsInt32ArrayOfInt32Delegate AreRecordsExistsInt32ArrayOfInt32;

        bool[] ITelephonyRecording.AreRecordsExists(int surveyId, int[] interviewIds)
        {


            if (AreRecordsExistsInt32ArrayOfInt32 != null)
            {
                return AreRecordsExistsInt32ArrayOfInt32(surveyId, interviewIds);
            } else if (_inner != null)
            {
                return ((ITelephonyRecording)_inner).AreRecordsExists(surveyId, interviewIds);
            }

            return default(bool[]);
        }

        public delegate AudioFile GetAudioFileInt32StringDelegate(int dialerId, string audioUrl);
        public GetAudioFileInt32StringDelegate GetAudioFileInt32String;

        AudioFile ITelephonyRecording.GetAudioFile(int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32String != null)
            {
                return GetAudioFileInt32String(dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((ITelephonyRecording)_inner).GetAudioFile(dialerId, audioUrl);
            }

            return default(AudioFile);
        }

    }
}