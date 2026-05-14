using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerRecordingWrapper : IDialerRecordingWrapper 
    {
        private IDialerRecordingWrapper _inner;

        public StubIDialerRecordingWrapper()
        {
            _inner = null;
        }

        public IDialerRecordingWrapper Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool[] AreRecordsExistsInt32Int32Int32ArrayOfInt32Delegate(int dialerId, int tenantId, int surveySid, int[] interviewIds);
        public AreRecordsExistsInt32Int32Int32ArrayOfInt32Delegate AreRecordsExistsInt32Int32Int32ArrayOfInt32;

        bool[] IDialerRecordingWrapper.AreRecordsExists(int dialerId, int tenantId, int surveySid, int[] interviewIds)
        {


            if (AreRecordsExistsInt32Int32Int32ArrayOfInt32 != null)
            {
                return AreRecordsExistsInt32Int32Int32ArrayOfInt32(dialerId, tenantId, surveySid, interviewIds);
            } else if (_inner != null)
            {
                return ((IDialerRecordingWrapper)_inner).AreRecordsExists(dialerId, tenantId, surveySid, interviewIds);
            }

            return default(bool[]);
        }

        public delegate IDictionary<CampaignInterviewIdentity, IEnumerable<AudioRecordInfo>> GetBulkInterviewRecordingsInt32Int32IEnumerableOfCampaignInterviewIdentityDelegate(int dialerId, int tenantId, IEnumerable<CampaignInterviewIdentity> interviewIdentities);
        public GetBulkInterviewRecordingsInt32Int32IEnumerableOfCampaignInterviewIdentityDelegate GetBulkInterviewRecordingsInt32Int32IEnumerableOfCampaignInterviewIdentity;

        IDictionary<CampaignInterviewIdentity, IEnumerable<AudioRecordInfo>> IDialerRecordingWrapper.GetBulkInterviewRecordings(int dialerId, int tenantId, IEnumerable<CampaignInterviewIdentity> interviewIdentities)
        {


            if (GetBulkInterviewRecordingsInt32Int32IEnumerableOfCampaignInterviewIdentity != null)
            {
                return GetBulkInterviewRecordingsInt32Int32IEnumerableOfCampaignInterviewIdentity(dialerId, tenantId, interviewIdentities);
            } else if (_inner != null)
            {
                return ((IDialerRecordingWrapper)_inner).GetBulkInterviewRecordings(dialerId, tenantId, interviewIdentities);
            }

            return default(IDictionary<CampaignInterviewIdentity, IEnumerable<AudioRecordInfo>>);
        }

        public delegate IEnumerable<AudioRecordInfo> GetInterviewRecordingsInt32Int32Int32Int32Delegate(int dialerId, int tenantId, int surveySid, int interviewId);
        public GetInterviewRecordingsInt32Int32Int32Int32Delegate GetInterviewRecordingsInt32Int32Int32Int32;

        IEnumerable<AudioRecordInfo> IDialerRecordingWrapper.GetInterviewRecordings(int dialerId, int tenantId, int surveySid, int interviewId)
        {


            if (GetInterviewRecordingsInt32Int32Int32Int32 != null)
            {
                return GetInterviewRecordingsInt32Int32Int32Int32(dialerId, tenantId, surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((IDialerRecordingWrapper)_inner).GetInterviewRecordings(dialerId, tenantId, surveySid, interviewId);
            }

            return default(IEnumerable<AudioRecordInfo>);
        }

        public delegate AudioFile GetAudioFileInt32Int32StringDelegate(int companyId, int dialerId, string audioUrl);
        public GetAudioFileInt32Int32StringDelegate GetAudioFileInt32Int32String;

        AudioFile IDialerRecordingWrapper.GetAudioFile(int companyId, int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32Int32String != null)
            {
                return GetAudioFileInt32Int32String(companyId, dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((IDialerRecordingWrapper)_inner).GetAudioFile(companyId, dialerId, audioUrl);
            }

            return default(AudioFile);
        }

    }
}