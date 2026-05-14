using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Core.Services.PersonImport;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Monitoring.Common.Contracts;
using ConfirmitDialerInterface;
using DialerCommon;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Core.SupervisorService
{
    [ServiceContract(Name = "supervisorService", Namespace = "http://www.confirmit.com/supervisorService/05/05/2009")]
    public interface ISupervisorService
    {
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void SendMessage(FeedbackForm feedback);

        [OperationContract]
        [FaultContract(typeof(DialerStartCampaignExceptionDetails))]
        void OpenSurvey(int surveySid);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void CloseSurvey(int surveySid);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void ShutdownSurvey(int surveySid);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        BvTasksEntity TerminateTaskByPerson(int personSid, CallOutcome? explicitIts);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void TerminateTasksByDialerId(int dialerId);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool EnableDialer(int dialerId);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool DisableDialer(int dialerId);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void SetPersonParentGroups(
            int personSid,
            int[] parentGroupsSids);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void DeletePerson(int personSid);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void DeletePersons(List<int> personSids);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void LockPersonBySupervisor(int personId);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void LockPersonsBySupervisor(List<int> personIds);
        
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool IsDialerOperational(int dialerId);

        [OperationContract]
        [FaultContract(typeof(SchedulingScriptSyntaxErrorExceptionDetails))]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void SaveSchedule(int scheduleSid, string serializedSchedule);

        [OperationContract]
        [FaultContract(typeof(SchedulingScriptSyntaxErrorExceptionDetails))]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void LaunchSchedule(int scheduleSid);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void CheckSchedule(string serializedSchedule);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool Schedule();

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void ForceCallDelivery();

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void StartMonitor(string supervisorName, int interviewerId, string telephoneNumber);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void StopMonitor(string supervisorName, int interviewerId);


        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void SetLiveMonitoringMode(string supervisorName, int interviewerId, MonitorMode mode);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        int CreateOrUpdatePerson(
            int callCenterId,
            int personSid,
            string name,
            string description,
            string fullName,
            string password,
            AgentTaskChoiceMode mode,
            PersonAssignmentListMode assignmentListMode,
            TaskChoicePermissions? permissions,
            List<int> parentGroups,
            int? autoSurveyId,
            int callGroupId,
            string location,
            DialType dialType,
            AgentType agentType,
            bool enableSoftphoneIntegration = true,
            string[] attributes = null);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        IEnumerable<AudioRecordInfo> GetInterviewRecordings(int surveyId, int interviewId);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        bool[] AreRecordsExists(int surveySid, int[] interviewIds);

        /// <summary>
        /// Sets default dialer survey parameters.
        /// </summary>
        /// <param name="parameters"></param>
        [OperationContract]
        [FaultContract(typeof(DialerParametersExceptionDetails))]
        void SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters);

        /// <summary>
        /// Validates dialer survey parameters.
        /// </summary>
        /// <param name="parameters"></param>
        [OperationContract]
        [FaultContract(typeof(DialerParametersExceptionDetails))]
        void ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters);

        /// <summary>
        /// Sets dialer parameters for the specified survey.
        /// </summary>
        /// <param name="surveySid"></param>
        /// <param name="parameters"></param>
        [OperationContract]
        [FaultContract(typeof(DialerParametersExceptionDetails))]
        void SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters);

        /// <summary>
        /// Imports persons.
        /// </summary>
        /// <param name="callCenterId">Id of call center.</param>
        /// <param name="dataTable">DataTable with imported information.</param>
        /// <param name="columnRoleMap">Dictionary containing column name to role map.</param>        
        /// <param name="importOptions">Import options.</param>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        ImportResult ImportPersons(int callCenterId, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions);

        /// <summary>
        /// Informs dialer about configuring DDI numbers
        /// </summary>
        /// <param name="dialerId">Id of dialer</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void ConfigureInboundDdiNumbers(
            int dialerId);

        /// <summary>
        /// Get list of all files from logs folder.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <returns>List of file info.</returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        IEnumerable<LogFileInfo> GetLogFiles(int dialerId);

        /// <summary>
        /// Get zipped body of specified file from logs folder.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <param name="fileName">File name with extension in logs folder.</param>
        /// <returns>Zip archive contained one specified file.</returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        byte[] GetLogFileBodyZipped(int dialerId, string fileName);

        /// <summary>
        /// Get dialer version.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <returns>String represents full version of dialer.</returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        string GetDialerVersion(int dialerId);

        /// <summary>
        /// Get available extended functionality.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <returns>Special object contains flags of availability of extended functionality.</returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionality(int dialerId);

        /// <summary>
        /// Get dialer supported features.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <returns>Special object contains flags of supported features.</returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        DialerFeatures GetDialerSupportedFeatures(int dialerId);

        /// <summary>
        /// Gets the interview audio recording file
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <param name="audioUrl">The URL to audio file on dialer side</param>
        /// <returns>An object with the content of audio file</returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        AudioFile GetAudioFile(int dialerId, string audioUrl);
        
        /// <summary>
        /// Get dialer supported features overriden by local values.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <returns>List of flags of supported default and overriden features.</returns>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeatures(int dialerId);

        /// <summary>
        /// Update dialer supported feature overriden by local value.
        /// </summary>
        /// <param name="dialerId">Dialer identifier.</param>
        /// <param name="featureName"></param>
        /// <param name="overridenFeatureValue"></param>
        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        void SendMessageToInterviewers(IEnumerable<int> interviewerIds, bool onlineOnly, string message, string supervisorName);

        [OperationContract]
        [FaultContract(typeof(UserMessageExceptionDetails))]
        IEnumerable<AudioIdentityObject> GetAudioIdentities(long recordId);
    }
}
