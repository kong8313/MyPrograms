using System;
using System.Collections.Generic;
using System.ServiceModel;
using ConfirmitDialerInterface;
using DialerCommon.DialerExceptions;

namespace Confirmit.CATI.Telephony.DialerService.Contract
{
    [ServiceContract]
    public interface IDialerServiceRecording
    {
        /// <summary>
        /// Initialization of the <see cref="IDialerRecordingApi"/>. 
        /// It is called before any other methods. Dialer initializes any resources it can require to work with interview records.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        void InitializeRecording();

        /// <summary>
        /// Retrieves record URLs for the concrete interview and returns them as a list of <see cref="AudioRecordInfo"/> objects.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="interviewId">The unique identifier of the interview </param>
        /// <returns>IEnumerable of <see cref="AudioRecordInfo"/> objects. <see cref="DateTime"/> field contains UTC time</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId);

        /// <summary>
        /// Gets audio records for given collection of interviews.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="interviewIdentities">Collection of interview identities</param>
        /// <returns>Audio data</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
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
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds);
    }
}
