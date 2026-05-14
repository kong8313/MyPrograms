using System;
using System.Collections.Generic;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Interface to work with interview records
    /// </summary>
    public interface IDialerRecordingApi
    {
        /// <summary>
        /// Initialization of the <see cref="IDialerRecordingApi"/>. 
        /// It is called before any other methods. Dialer initializes any resources it can require to work with interview records.
        /// </summary>
        void InitializeRecording();

        /// <summary>
        /// Retrieves record URLs for the concrete interview and returns them as a list of <see cref="AudioRecordInfo"/> objects.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="interviewId">The unique identifier of the interview </param>
        /// <returns>
        /// IEnumerable of <see cref="AudioRecordInfo"/> objects. <see cref="DateTime"/> field contains UTC time
        /// Note: The returned AudioRecordInfo.DateTime field 
        /// should contain UTC date and time when the recording actually began, 
        /// not some kind of file creation/conversion/encoding/.. time.
        /// </returns>
        IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId);

        /// <summary>
        /// Gets audio records for given collection of interviews.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="interviewIdentities">Collection of interview identities</param>
        /// <returns>
        /// Audio data
        /// Note: The AudioRecordInfo.DateTime field in the returned BulkAudioResult.AudioRecords items
        /// should contain UTC date and time when the recording actually began, 
        /// not some kind of file creation/conversion/encoding/.. time.
        /// </returns>
        BulkAudioResult GetBulkAudioRecords(
            int companyId,
            IEnumerable<CampaignInterviewIdentity> interviewIdentities);

        /// <summary>
        /// Gets the list of boolean flags indicating whether there are some records are available for the specific interview.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="interviewIds">The list of interview identifiers to determine whether recordings are available for.</param>
        /// <returns>
        /// The list of boolean flags. Flags count is always equal to the count of interview identifiers list.
        /// </returns>
        bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds);
    }
}
