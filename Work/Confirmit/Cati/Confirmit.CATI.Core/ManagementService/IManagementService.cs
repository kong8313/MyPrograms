using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.ManagementService
{
    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/08/06/2009/Interviewer")]
    public struct Interviewer
    {
        [DataMember]
        public int interviewerID;

        [DataMember]
        public string name;

        [DataMember]
        public int role;
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/08/06/2009/SurveyProperties")]
    public class SurveyProperties
    {
        [DataMember]
        public string CfSqlServerConnectionString { get; set; }
        [DataMember]
        public string CreatedUserName { get; set; }
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public int? DialingMode { get; set; }
        [DataMember]
        public bool? OpenEndReview { get; set; }
        [DataMember]
        public bool? LiveMonitoring { get; set; }
        [DataMember]
        public bool? VoiceRecording { get; set; }
        [DataMember]
        public bool? ScreenRecording { get; set; }
        [DataMember]
        public bool SupportBlacklist { get; set; }
        [DataMember]
        public bool AllowRespondentsDynamicCreation { get; set; }
        [DataMember]
        public string NotificationEmail { get; set; }
        [DataMember]
        public bool EnforceHttps { get; set; }
        [DataMember]
        public bool ReplicationStatus { get; set; }
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/08/06/2009/LaunchSurveyParameters")]
    public class LaunchSurveyParameters
    {
        [DataMember]
        public bool RemoveData { get; set; }
        [DataMember]
        public SurveyProperties SurveyProperties { get; set; }
        [DataMember]
        public string[] PermittedUsers { get; set; }
        [DataMember]
        public TableInfo[] ReplicatedTables { get; set; }
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/02/09/2009/InterviewHistoryData")]
    public class InterviewHistoryData
    {
        [DataMember]
        public string projectID;

        [DataMember]
        public string respondentPhone;

        [DataMember]
        public System.DateTime time;

        [DataMember]
        public int interviewID;

        [DataMember]
        public string status;

        [DataMember]
        public int appointmentID;

        [DataMember]
        public int netDuration;

        [DataMember]
        public int grossDuration;

        [DataMember]
        public int totalDuration;

        [DataMember]
        public int interviewerID;

        [DataMember]
        public int roleID;

        public override string ToString()
        {
            return string.Format("ProjectId: {0}, RespondentPhone: {1}, Time: {2}, InterviewId: {3}, Status: {4}, AppointmentId: {5}, NetDuration: {6}, GrossDuration: {7}, TotalDuration: {8}, InterviewerId: {9}, RoleId: {10}", this.projectID, this.respondentPhone, this.time, this.interviewID, this.status, this.appointmentID, this.netDuration, this.grossDuration, this.totalDuration, this.interviewerID, this.roleID);
        }
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/02/09/2009/InterviewControlData")]
    public class InterviewControlData
    {
        [DataMember]
        public string projectID;

        [DataMember]
        public int interviewID;

        [DataMember]
        public string status;

        [DataMember]
        public string respondentName;

        [DataMember]
        public string respondentPhone;

        [DataMember]
        public System.DateTime lastCallTime;

        [DataMember]
        public int totalDuration;

        [DataMember]
        public int interviewerID;

        [DataMember]
        public int roleID;

        [DataMember]
        public byte lastChannelID;

        public override string ToString()
        {
            return string.Format("ProjectId: {0}, InterviewId: {1}, Status: {2}, RespondentName: {3}, RespondentPhone: {4}, LastCallTime: {5}, TotalDuration: {6}, InterviewerId: {7}, RoleId: {8}, LastChannelId: {9}", this.projectID, this.interviewID, this.status, this.respondentName, this.respondentPhone, this.lastCallTime, this.totalDuration, this.interviewerID, this.roleID, this.lastChannelID);
        }
    }

    [Serializable]
    [DataContract]
    public enum ProcessSampleMode
    {
        [EnumMember(Value = "0")]
        Add,
        [EnumMember(Value = "1")]
        Update,
        [EnumMember(Value = "2")]
        Merge
    }

    [Serializable]
    [DataContract]
    public enum SampleSchedulingMode
    {
        [EnumMember(Value = "0")]
        Full,
        [EnumMember(Value = "1")]
        Simple
    }

    [ServiceContract(Name = "ManagementService", Namespace = "http://www.confirmit.com/ManagementService/15/05/2009")]
    public interface IManagementService
    {
        [OperationContract]
        bool IsInboundCall(int catiInterviewerId);

        [OperationContract]
        bool IsIvrCall(int catiInterviewerId);

        [OperationContract]
        bool SetNextLinkedInterview(string projectId, int respondentId, int catiInterviewerId);

        [OperationContract]
        bool SetNextLinkedInterviewToPrevious(int catiInterviewerId);

        [OperationContract]
        CatiInterview[] GetInterviews(string[] projectList, string telephoneNumber, string respondentName, string filter, int catiInterviewerId);

        [OperationContract]
        CatiInterview[] GetLinkedInterviews(int catiInterviewerId);

        [OperationContract]
        void AddSurvey(string confirmitProjectID, string confirmitSurveyName, string cfSqlServerConnectionString, string userName);

        [OperationContract]
        int DeleteSurvey(string confirmitProjectID);

        [OperationContract]
        void SoftDeleteSurvey(string confirmitProjectID);
        
        [OperationContract]
        void RestoreSoftDeletedSurvey(string confirmitProjectID);
        
        [OperationContract]
        void AddRespondent(string projectId, int respondentId, int its);

        [OperationContract]
        void AddRespondentFromConsole(string projectId, int respondentId, int personId);

        // Old sample addition function, TODO: Remove
        [OperationContract]
        void AddSample(string projectdID, int batchID, int mode, int recordsCount);

        // New sample addition function
        [OperationContract]
        void ProcessSample(string projectdId, int batchId, int sampleMode, int schedulingMode);

        [OperationContract]
        void UpdateSurveyAccessList(string userId, string surveyId, bool enabled);

        [OperationContract]
        void UpdateSurveyProperties(
            string confirmitProjectID,
            string confirmitProjectName,
            int? dialingMode,
            bool? openEndReview,
            bool? voiceRecording,
            bool? screenRecording,
            bool supportBlacklist,
            bool allowRespondentsDynamicCreation,
            string notificationEmail,
            bool enforceHttps);


        [OperationContract]
        bool IsSurveyOpen(string confirmitProjectID);

        [OperationContract]
        void OnCATIOptionsChanged(bool bTelephonyEnabled);

        [OperationContract]
        int DeleteRespondentsAsync(int[] respIDs, string confirmitProjectID);

        /// <summary>
        /// Updates the survey data replication scheme.
        /// </summary>
        /// <param name="projectId">Confirmit project ID.</param>
        /// <param name="tables">Array of <see cref="TableInfo"/> objects with list of columns to replicate data.</param>
        [OperationContract]
        void UpdateSurveyReplicationScheme(string projectId, TableInfo[] tables);

        /// <summary>
        /// Updates the survey replication status.
        /// </summary>
        /// <param name="projectId">Confirmit project ID.</param>
        /// <param name="isReplicationEnabled">If set to <c>true</c> CATI will replicate data for specified survey.</param>
        [OperationContract]
        void UpdateSurveyReplicationStatus(string projectId, bool isReplicationEnabled);

        /// <summary>
        /// Saves History and Control data for interview and runs OnSchedule event
        /// if Control data specified
        /// </summary>
        /// <param name="historyData">interview history data</param>
        /// <param name="controlData">interview control data (can be null)</param>
        [OperationContract]
        void SaveInterviewHistoryAndControlData(InterviewHistoryData historyData, InterviewControlData controlData);

        /// <summary>
        /// Returns current state and its text for specific sample
        /// </summary>
        /// <param name="batchId">batchId</param>
        /// <param name="stateDescription">state description text</param>
        /// <returns>AddSampleAsyncResult</returns>
        [OperationContract]
        int AddSampleGetState(int batchId, out string stateDescription);

        /// <summary>
        /// Returns current state and its text for specific sample
        /// </summary>
        /// <param name="batchId">batchId</param>
        /// <param name="sampleMode">mode sample added with 0 - Add, 1 - Update</param>
        /// <param name="stateDescription">state description text</param>
        /// <returns>AddSampleAsyncResult</returns>
        [OperationContract]
        int ProcessSampleGetState(int batchId, int sampleMode, out string stateDescription);

        [OperationContract]
        void OnQuotaChanged(string cfProjectId, int cfQuotaId);

        [OperationContract]
        void OnQuotaCellsChanged(string cfProjectId, int cfQuotaId, int[] openedCfCellIds, int[] closedCfCellIds, int[] optimisticallyClosedCfCellIds);

        [OperationContract]
        void OnQuotaCellsStateChanged(string projectId, int quotaId, List<CatiQuotaCellCountersState> quotaCellsCountersList);

        [OperationContract]
        void OnQuotasCellsStatesChanged(string projectId, List<CatiQuotaCellsCountersStates> quotasCellsCountersStates);

        /// <summary>
        /// Gets cati interviewer name.
        /// </summary>
        /// <param name="catiInterviewerId">CATI interviewer ID</param>
        /// <returns>Name of CATI interviewer</returns>
        [OperationContract]
        string GetCATIInterviewerName(int catiInterviewerId);

        /// <summary>
        /// Gets cati interviewer display name.
        /// </summary>
        /// <param name="catiInterviewerId">CATI interviewer ID</param>
        /// <returns>Name of CATI interviewer</returns>
        [OperationContract]
        string GetCatiInterviewerDisplayName(int catiInterviewerId);
        
        /// <summary>
        /// Gets cati station id.
        /// </summary>
        /// <param name="catiInterviewerId">CATI interviewer ID</param>
        /// <returns>Station of CATI interviewer</returns>
        [OperationContract]
        string GetCATIStationId(int catiInterviewerId);

        /// <summary>
        /// Gets appointment time for the respondent.
        /// Returns null if no appointment exist for the respondent.
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="respondentId">Respondent ID</param>
        /// <returns>Time in the respondent time zone of the appointment set for this interview in CATI</returns>
        [OperationContract]
        DateTime? GetCATIAppointmentTime(string projectId, int respondentId);

        /// <summary>
        /// Gets all dialing attempts for last cati interview attempt for the respondent.
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="respondentId">Respondent ID</param>
        /// <returns>Dialing attempts for the respondent</returns>
        [OperationContract]
        CatiDialingAttempt[] GetCatiInterviewDialingAttempts(string projectId, int respondentId);

        /// <summary>
        /// Stops interview recording.
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="respondentId">Respondent ID</param>
        /// <param name="stopRecordingMode">
        /// StopRecordingMode: stop whole interview recording, or sectional or both?
        /// Either specific string or concrete interger can be passed:
        /// "WholeInterview" = 1
        /// "Sectional" = 2
        /// "Both" = 3
        /// </param>
        [OperationContract]
        void StopRecording(string projectId, int respondentId, string stopRecordingMode);

        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        /// <param name="projectId">The project ID (pXXXXXXX).</param>
        /// <param name="respondentId">The respondent ID (interview ID in CATI).</param>
        /// <param name="label">The label. It will be included in the name of the recorded audio file.</param>
        /// <remarks>It should work both if whole interview recording is enabled or not.
        /// If whole interview recording if in process when this method is called - it will be automatically paused.</remarks>
        [OperationContract]
        void StartRecording(string projectId, int respondentId, string label);

        [OperationContract]
        void EnableLiveMonitoring(string projectId, int catiInterviewerId);

        /// <summary>
        /// return dialing mode for specified interview
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="respondentId">Respondent ID</param>
        /// <returns></returns>
        [OperationContract]
        int GetDialingMode(string projectId, int respondentId);

        /// <summary>
        /// Returns transient state for specified interview
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <param name="respondentId">Respondent ID</param>
        /// <returns>Transient state of interview</returns>
        [OperationContract]
        int GetExtendedStatus(string projectId, int respondentId);

        /// <summary>
        /// Transfers current respondent call to an IVR endpoint
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="respondentId"></param>
        /// <param name="endpoint"></param>
        /// <param name="attributes"></param>
        [OperationContract]
        void TransferToIvr(string projectId, int respondentId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes);

        /// <summary>
        /// Adds telephone number to the CATI Blacklist of this company.
        /// </summary>
        /// <param name="telephoneNumber">Telephone number to add in the blacklist</param>
        /// <param name="projectId">The project ID (pXXXXXXX)</param>
        /// <param name="respondentId">Respondent ID</param>
        [OperationContract]
        void AddToCATIBlacklist(string telephoneNumber, string projectId, int respondentId);

        [OperationContract]
        string BackupSurveyToArchive(string projectId);

        [OperationContract]
        int BeginRestoreSurveyFromArchive(string projectId, string data);

        [OperationContract]
        AsyncOperationInfo GetAsyncOperationInfo(int operationId);

        [OperationContract]
        string[] GetSurveyCallCenters(string projectId, string supervisorName);

        [OperationContract]
        int LaunchSurvey(string projectId, LaunchSurveyParameters parameters);

        [OperationContract]
        string GetVersion();

        [OperationContract]
        Task UpdateActiveQuestion(string projectId, int catiInterviewerId, string qId);

        [OperationContract]
        void ScheduleInterview(SchedulingScriptExecutionParameters parameters);

        [OperationContract]
        bool IsTimeInShift(string projectId, int timezoneId, DateTime dateTime);

        [OperationContract]
        TimeInShift[] AreTimesInShift(string projectId, int timezoneId, DateTime[] dateTimes);

        /// <summary>
        /// Gets a value indicating whether interviewer with specified ID is a member of interviewer group with specified name
        /// </summary>
        /// <param name="catiInterviewerId">Id of interviewer</param>
        /// <param name="groupName">Name of group</param>
        /// <returns></returns>
        [OperationContract]
        bool IsCatiGroupMember(int catiInterviewerId, string groupName);

        [OperationContract]
        void SaveAlternativeNumber(int surveyId, int interviewId, string newPhoneNumber);

        [OperationContract]
        string GetInterviewVariableValue(string projectId, int interviewId, string variableName);

        [OperationContract]
        int Telephony_Login(int dialerId, long campaignId, string agentId, string agentName,
            AgentType agentType, string agentExtension, string userId, bool isPredictive, bool isLocal,
            IEnumerable<KeyValuePair<string, string>> agentAttributes);

        [OperationContract]
        int Telephony_SetGroups(int dialerId, long campaignId, string agentId, int[] groups);

        [OperationContract]
        int Telephony_Logout(int dialerId, long campaignId, bool isPredictive, string agentId);

        [OperationContract]
        int Telephony_KillAgent(int dialerId, long campaignId, string agentId);

        [OperationContract]
        int Telephony_SetCampaign(int dialerId, long campaignId, int agentId);

        [OperationContract]
        int Telephony_GoReady(int dialerId, long campaignId, string agentId);

        [OperationContract]
        int Telephony_GoNotReady(int dialerId, long campaignId, string agentId, string breakName);

        [OperationContract]
        int Telephony_SendNumberToAgent(int dialerId, long campaignId, string agentId,
            DialingMode dialingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, object> respondentVariables);

        [OperationContract]
        int Telephony_SendNumberToAgentEx(int dialerId, long campaignId, string agentId,
            DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout,
            bool isRecording);

        [OperationContract]
        int Telephony_Redial(int dialerId, long campaignId, string agentId, int contactId,
            int callId, string phoneNumber, bool isRecording, string callerId);

        [OperationContract]
        int Telephony_Hangup(int dialerId, long campaignId, string agentId, int interviewId, long callId);

        [OperationContract]
        int Telephony_CompleteCall(int dialerId, long campaignId, string agentId,
            bool makeAgentReady, string breakName, InterviewStatus status, int interviewId, long callId);

        [OperationContract]
        int Telephony_SetNextInterview(int dialerId, long campaignId, string agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId);

        [OperationContract]
        int Telephony_StopMonitor(int dialerId, string sessionId);

        [OperationContract]
        int Telephony_CompletePreview(int dialerId, long campaignId, string agentId, int contactId,
            int callId, string phoneNumber, bool isRecording);

        [OperationContract]
        int Telephony_ConnectInboundCallToAgent(int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor);

        [OperationContract]
        int Telephony_DropInboundCall(int dialerId, string inboundCallId,
            AudioMessageDescriptor audioMessageDescriptor);

        [OperationContract]
        int Telephony_TransferStart(int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType);

        [OperationContract]
        int Telephony_TransferSetTarget(int dialerId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns);

        [OperationContract]
        int Telephony_TransferSetConnectionState(int dialerId, long campaignId, string transferId,
            ConnectionState state);

        [OperationContract]
        int Telephony_TransferComplete(int dialerId, long campaignId, string transferId);

        [OperationContract]
        int Telephony_TransferCancel(int dialerId, long campaignId, string transferId);

        [OperationContract]
        int Telephony_StartPlayback(int dialerId, long campaignId, string agentId, int interviewId,
            int callId, string fileName, out int timeOfPlayingInSeconds);

        [OperationContract]
        int Telephony_StopPlayback(int dialerId, long campaignId, string agentId, int callId);

        [OperationContract]
        int Telephony_PauseOrResumePlayback(int dialerId, long campaignId, string agentId, int callId);

        [OperationContract]
        int Telephony_ToggleInterviewerListensToPlaybackOrRespondent(int dialerId, long campaignId,
            string agentId, int callId);

        [OperationContract]
        bool Telephony_IsPersonModeSupported(int dialerId, AgentTaskChoiceMode mode);

        [OperationContract]
        bool Telephony_IsReloginNeededOnSurveyChange(int dialerId);

        [OperationContract]
        bool Telephony_IsPauseOrResumePlaybackSupported(int dialerId);

        [OperationContract]
        bool Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupported(int dialerId);

        [OperationContract]
        bool Telephony_IsDynamicExtensionNumberAllowed(int dialerId, bool isAgentLocal);

        [OperationContract]
        int Telephony_RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl);

        [OperationContract]
        int Telephony_IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, int contactId,
            string voiceXml);

        [OperationContract]
        int Telephony_StartCustomIvrInterview(int dialerId, long campaignId, string agentId, int interviewId, long callId,
            string respondentSurveyLink);
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/08/06/2009/AsyncOperationInfo")]
    public class AsyncOperationInfo
    {
        [DataMember]
        public byte Type;

        [DataMember]
        public string Title;

        [DataMember]
        public AsyncOperationState State;

        [DataMember]
        public int Priority;

        [DataMember]
        public DateTime QueuedDate;

        [DataMember]
        public DateTime? StartedDate;

        [DataMember]
        public DateTime? FinishedDate;

        [DataMember]
        public int TotalItemsCount;

        [DataMember]
        public int ProcessedItemsCount;

        [DataMember]
        public int FailedItemsCount;

        [DataMember]
        public string CreatedBySupervisorName;

        [DataMember]
        public string Error;

        [DataMember]
        public string Text;
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/08/06/2009/CatiInterview")]
    public class CatiInterview
    {
        [DataMember]
        public string ProjectId;

        [DataMember]
        public int RespondentId;

        [DataMember]
        public string RespondentName;

        [DataMember]
        public string TelephoneNumber;

        [DataMember]
        public string Filters;
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/11/28/2019/SchedulingScriptExecutionParameters")]
    public class SchedulingScriptExecutionParameters
    {
        [DataMember]
        public int InterviewId { get; set; }

        [DataMember]
        public int SurveySid { get; set; }

        [DataMember]
        public OperationType OperationType { get; set; }
        
        [DataMember]
        public int ITS { get; set; }

        [DataMember]
        public int? LastCallPersonId { get; set; }

        [DataMember]
        public string CliNumber { get; set; }

        [DataMember]
        public string DdiNumber { get; set; }

        [DataMember]
        public int CallCenterId { get; set; }

        [DataMember]
        public DateTime? TimeCallDelivered { get; set; }

        [DataMember]
        public int InterviewDurationTime { get; set; }

        [DataMember]
        public int WaitingTime { get; set; }

        [DataMember]
        public int OpenEndReviewDurationTime { get; set; }

        [DataMember]
        public int ConfirmitDuration { get; set; }
        
        [DataMember]
        public int WrapTime { get; set; }
        
        [DataMember]
        public int PreviewTime { get; set; }
        
        [DataMember]
        public int ConnectedTime { get; set; }

        [DataMember]
        public int? LinkedInterviewSessionId { get; set; }

        [DataMember]
        public bool? IsLogToHistory { get; set; }

        [DataMember]
        public CatiDialingAttempt[] DialingAttempts { get; set; }
        
        [DataMember]
        public int? CallAttemptNumber { get; set; }
    }
    
    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/21/03/2025/CatiDialingAttempt")]
    public class CatiDialingAttempt
    {
        [DataMember]
        public long DialId { get; set; }
        
        [DataMember]
        public string TelephoneNumber { get; set; }
        
        [DataMember]
        public string DialerTelephoneNumber { get; set; }
        
        [DataMember]
        public DateTime? StartTime { get; set; }
        
        [DataMember]
        public DateTime? FinishTime { get; set; }
        
        [DataMember]
        public string DialerCallerId { get; set; }
        
        [DataMember]
        public int? RingTime { get; set; }
        
        [DataMember]
        public int? DialerCallOutcome { get; set; }
        
        [DataMember]
        public Dictionary<string, string> CallOutcomeMetadata { get; set; }
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/01/17/2020/TimeInShift")]
    public class TimeInShift
    {
        [DataMember]
        public DateTime Time { get; set; }

        [DataMember]
        public bool IsInShift { get; set; }
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/12/15/2020/CatiQuotaCellCountersState")]
    public class CatiQuotaCellCountersState
    {
        [DataMember]
        public int CellId { get; set; }

        [DataMember]
        public CatiQuotaCellCounters OldCounters { get; set; }

        [DataMember]
        public CatiQuotaCellCounters ActualCounters { get; set; }
    }

    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/12/15/2020/CatiQuotaCellsCountersStates")]
    public class CatiQuotaCellsCountersStates
    {
        [DataMember]
        public int QuotaId { get; set; }
        
        [DataMember]
        public List<CatiQuotaCellCountersState> CellsCountersStates { get; set; }
    }
    
    [DataContract(Namespace = "http://www.confirmit.com/ManagementService/12/15/2020/CatiQuotaCellCounters")]
    public class CatiQuotaCellCounters
    {
        [DataMember]
        public int Counter { get; set; }

        [DataMember]
        public int Limit { get; set; }

        [DataMember]
        public bool Disabled { get; set; }

        [DataMember]
        public int LiveCounter { get; set; }

        [DataMember]
        public int LiveLimit { get; set; }

        [DataMember]
        public bool IsOptimistic { get; set; }
    }
}
