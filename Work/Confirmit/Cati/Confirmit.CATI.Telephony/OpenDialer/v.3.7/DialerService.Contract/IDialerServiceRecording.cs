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
        /// <param name="dialerId">Dialer id</param>
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        void InitializeRecording(int dialerId);

        /// <summary>
        /// Retrieves record URLs for the concrete interview and returns them as a list of <see cref="AudioRecordInfo"/> objects.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="interviewId">The unique identifier of the interview </param>
        /// <param name="dialerId">Dialer id</param>
        /// <returns>IEnumerable of <see cref="AudioRecordInfo"/> objects. <see cref="DateTime"/> field contains UTC time</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId, int dialerId);

        /// <summary>
        /// Gets the interview audio recording file
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="audioUrl">Url to the audio file which was returned by GetAudioRecords method</param>
        /// <returns>An object with the content of audio file</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl);
        
        /// <summary>
        /// Gets audio records for given collection of interviews.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="interviewIdentities">Collection of interview identities</param>
        /// <param name="dialerId">Dialer id</param>
        /// <returns>Audio data</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        BulkAudioResult GetBulkAudioRecords(
            int companyId,
            IEnumerable<CampaignInterviewIdentity> interviewIdentities, 
            int dialerId);

        /// <summary>
        /// Gets the list of boolean flags indicating whether there are some records are available for the specific interview.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="interviewIds">The list of interview identifiers to determine whether recordings are available for.</param>
        /// <param name="dialerId">Dialer id</param>
        /// <returns>
        /// The list of boolean flags. Flags count is always equal to the count of interview identifiers list.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds, int dialerId);
    }
}
