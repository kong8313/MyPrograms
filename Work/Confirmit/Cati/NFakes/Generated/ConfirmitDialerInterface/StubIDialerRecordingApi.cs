using System;
using ConfirmitDialerInterface;
using System.Collections.Generic;

namespace ConfirmitDialerInterface.Fakes
{
    public class StubIDialerRecordingApi : IDialerRecordingApi 
    {
        private IDialerRecordingApi _inner;

        public StubIDialerRecordingApi()
        {
            _inner = null;
        }

        public IDialerRecordingApi Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeRecordingInt32Delegate(int dialerId);
        public InitializeRecordingInt32Delegate InitializeRecordingInt32;

        void IDialerRecordingApi.InitializeRecording(int dialerId)
        {

            if (InitializeRecordingInt32 != null)
            {
                InitializeRecordingInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerRecordingApi)_inner).InitializeRecording(dialerId);
            }
        }

        public delegate IEnumerable<AudioRecordInfo> GetAudioRecordsInt32Int64Int32Int32Delegate(int companyId, long campaignId, int interviewId, int dialerId);
        public GetAudioRecordsInt32Int64Int32Int32Delegate GetAudioRecordsInt32Int64Int32Int32;

        IEnumerable<AudioRecordInfo> IDialerRecordingApi.GetAudioRecords(int companyId, long campaignId, int interviewId, int dialerId)
        {


            if (GetAudioRecordsInt32Int64Int32Int32 != null)
            {
                return GetAudioRecordsInt32Int64Int32Int32(companyId, campaignId, interviewId, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerRecordingApi)_inner).GetAudioRecords(companyId, campaignId, interviewId, dialerId);
            }

            return default(IEnumerable<AudioRecordInfo>);
        }

        public delegate BulkAudioResult GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32Delegate(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities, int dialerId);
        public GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32Delegate GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32;

        BulkAudioResult IDialerRecordingApi.GetBulkAudioRecords(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities, int dialerId)
        {


            if (GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32 != null)
            {
                return GetBulkAudioRecordsInt32IEnumerableOfCampaignInterviewIdentityInt32(companyId, interviewIdentities, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerRecordingApi)_inner).GetBulkAudioRecords(companyId, interviewIdentities, dialerId);
            }

            return default(BulkAudioResult);
        }

        public delegate bool[] AreRecordsExistsInt32Int64ArrayOfInt32Int32Delegate(int companyId, long campaignId, int[] interviewIds, int dialerId);
        public AreRecordsExistsInt32Int64ArrayOfInt32Int32Delegate AreRecordsExistsInt32Int64ArrayOfInt32Int32;

        bool[] IDialerRecordingApi.AreRecordsExists(int companyId, long campaignId, int[] interviewIds, int dialerId)
        {


            if (AreRecordsExistsInt32Int64ArrayOfInt32Int32 != null)
            {
                return AreRecordsExistsInt32Int64ArrayOfInt32Int32(companyId, campaignId, interviewIds, dialerId);
            } else if (_inner != null)
            {
                return ((IDialerRecordingApi)_inner).AreRecordsExists(companyId, campaignId, interviewIds, dialerId);
            }

            return default(bool[]);
        }

        public delegate AudioFile GetAudioFileInt32Int32StringDelegate(int companyId, int dialerId, string audioUrl);
        public GetAudioFileInt32Int32StringDelegate GetAudioFileInt32Int32String;

        AudioFile IDialerRecordingApi.GetAudioFile(int companyId, int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32Int32String != null)
            {
                return GetAudioFileInt32Int32String(companyId, dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((IDialerRecordingApi)_inner).GetAudioFile(companyId, dialerId, audioUrl);
            }

            return default(AudioFile);
        }

    }
}