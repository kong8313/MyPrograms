using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public interface IDialerRecordingWrapper
    {
        bool[] AreRecordsExists(int dialerId, int tenantId, int surveySid, int[] interviewIds);
        IDictionary<CampaignInterviewIdentity, IEnumerable<AudioRecordInfo>> GetBulkInterviewRecordings(int dialerId, int tenantId, IEnumerable<CampaignInterviewIdentity> interviewIdentities);
        IEnumerable<AudioRecordInfo> GetInterviewRecordings(int dialerId, int tenantId, int surveySid, int interviewId);
        AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl);
    }
}