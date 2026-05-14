using System;
using Confirmit.CATI.Telephony;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.Fakes
{
    public class StubIDialerRecordingAPI : IDialerRecordingAPI 
    {
        private IDialerRecordingAPI _inner;

        public StubIDialerRecordingAPI()
        {
            _inner = null;
        }

        public IDialerRecordingAPI Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeStringStringDelegate(string connectionParametersXml, string configurationParametersXml);
        public InitializeStringStringDelegate InitializeStringString;

        void IDialerRecordingAPI.Initialize(string connectionParametersXml, string configurationParametersXml)
        {

            if (InitializeStringString != null)
            {
                InitializeStringString(connectionParametersXml, configurationParametersXml);
            } else if (_inner != null)
            {
                ((IDialerRecordingAPI)_inner).Initialize(connectionParametersXml, configurationParametersXml);
            }
        }

        public delegate IEnumerable<AudioRecordInfo> GetAudioRecordsInt32Int64Int32Int32Delegate(int companyId, long surveyId, int interviewId, int dialerId);
        public GetAudioRecordsInt32Int64Int32Int32Delegate GetAudioRecordsInt32Int64Int32Int32;

        IEnumerable<AudioRecordInfo> IDialerRecordingAPI.GetAudioRecords(int companyId, long surveyId, int interviewId, int dialerId)
        {


            if (GetAudioRecordsInt32Int64Int32Int32 != null)
            {
                return GetAudioRecordsInt32Int64Int32Int32(companyId, surveyId, interviewId, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerRecordingAPI)_inner).GetAudioRecords(companyId, surveyId, interviewId, dialerId);
            }

            return default(IEnumerable<AudioRecordInfo>);
        }

        public delegate BulkAudioResult GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32Delegate(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities, int dialerId);
        public GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32Delegate GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32;

        BulkAudioResult IDialerRecordingAPI.GetBulkAudioRecords(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities, int dialerId)
        {


            if (GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32 != null)
            {
                return GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32(companyId, interviewIdentities, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerRecordingAPI)_inner).GetBulkAudioRecords(companyId, interviewIdentities, dialerId);
            }

            return default(BulkAudioResult);
        }

        public delegate bool[] AreRecordsExistsInt32Int64ArrayOfInt32Int32Delegate(int companyId, long surveyId, int[] interviewIds, int dialerId);
        public AreRecordsExistsInt32Int64ArrayOfInt32Int32Delegate AreRecordsExistsInt32Int64ArrayOfInt32Int32;

        bool[] IDialerRecordingAPI.AreRecordsExists(int companyId, long surveyId, int[] interviewIds, int dialerId)
        {


            if (AreRecordsExistsInt32Int64ArrayOfInt32Int32 != null)
            {
                return AreRecordsExistsInt32Int64ArrayOfInt32Int32(companyId, surveyId, interviewIds, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerRecordingAPI)_inner).AreRecordsExists(companyId, surveyId, interviewIds, dialerId);
            }

            return default(bool[]);
        }

        public delegate AudioFile GetAudioFileInt32Int32StringDelegate(int companyId, int dialerId, string audioUrl);
        public GetAudioFileInt32Int32StringDelegate GetAudioFileInt32Int32String;

        AudioFile IDialerRecordingAPI.GetAudioFile(int companyId, int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32Int32String != null)
            {
                return GetAudioFileInt32Int32String(companyId, dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((IDialerRecordingAPI)_inner).GetAudioFile(companyId, dialerId, audioUrl);
            }

            return default(AudioFile);
        }

    }
}