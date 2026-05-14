using System.Collections.Generic;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public interface ITelephonyRecording
    {
        void InitializeRecording();

        IEnumerable<AudioRecordInfo> GetAudioRecords(int surveyId, int interviewId);

        bool[] AreRecordsExists(int surveyId, int[] interviewIds);
        
        AudioFile GetAudioFile(int dialerId, string audioUrl);
    }
}
