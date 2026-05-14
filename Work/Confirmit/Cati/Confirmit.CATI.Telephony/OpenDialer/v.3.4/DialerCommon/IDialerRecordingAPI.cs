using System;
using System.Collections.Generic;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony
{
    public interface IDialerRecordingAPI
    {
        /// <summary>
        /// Initialization of the <see cref="IDialerRecordingAPI"/>. 
        /// Must be called before any other methods
        /// </summary>
        /// <param name="connectionParametersXml"></param>
        /// <param name="configurationParametersXml"></param>
        void Initialize(string connectionParametersXml, string configurationParametersXml);

        /// <summary>
        /// Retrieves recordings URLs and returns them as a list of <see cref="AudioRecordInfo"/> objects.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="surveyId">Campaign ID.</param>
        /// <param name="interviewId"></param>
        /// <returns>IEnumerable of <see cref="AudioRecordInfo"/> objects. <see cref="DateTime"/> field contains UTC time</returns>
        IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long surveyId, int interviewId);

        /// <summary>
        /// Gets audio records for given collection of interviews.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="interviewIdentities">Collection of interview identities.</param>
        /// <returns>Audio data.</returns>
        BulkAudioResult GetBulkAudioRecords(
            int companyId,
            IEnumerable<CampaignInterviewIdentity> interviewIdentities);

        /// <summary>
        /// Gets the list of boolean flags indicating whether there are some recordings are available for the specific interview ID.
        /// </summary>
        /// <param name="companyId">The company ID.</param>
        /// <param name="surveyId">Campaign ID.</param>
        /// <param name="interviewIds">The list of interview IDs to determine whether recordings are available for.</param>
        /// <returns>
        /// The list of boolean flags. Flags count is always equal to the count of interview IDs list.
        /// </returns>
        bool[] AreRecordsExists(int companyId, long surveyId, int[] interviewIds);
    }
}
