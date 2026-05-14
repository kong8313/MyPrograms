using System.Collections.Generic;
using System.Data;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Core.Services.PersonImport;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;
using DialerCommon;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Core.SupervisorService
{
    public interface ISupervisorServiceClient
    {
        bool[] AreRecordsExists(int surveySid, int[] interviewIds);
        void CheckSchedule(string serializedSchedule);
        void CloseSurvey(int surveySid);
        void CreateOrUpdatePerson(int callCenterId, int personSid, string name, string description, string fullName, string password, AgentTaskChoiceMode mode, PersonAssignmentListMode assignmentListMode, TaskChoicePermissions? permissions, List<int> parentGroups, int? autoSurveyId, int callGroupId, string location, DialType dialType, AgentType agentType, bool enableSoftphoneIntegration = true, string[] attributes = null);
        void DeletePersons(List<int> personSids);
        bool EnableDialer(int dialerId);
        bool DisableDialer(int dialerId);
        IEnumerable<AudioRecordInfo> GetInterviewRecordings(int surveyId, int interviewId);
        ImportResult ImportPersons(int callCenterID, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions);
        bool IsDialerOperational(int dialerId);
        void LaunchSchedule(int scheduleSid);
        void LockPersonsBySupervisor(List<int> personSids);
        void OpenSurvey(int surveySid);
        void SaveSchedule(int scheduleSid, string serializedSchedule);
        bool Schedule();
        void SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters);
        void SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters);
        void SetPersonParentGroups(int personSid, int[] parentGroupsSids);
        void ShutdownSurvey(int surveySid);
        void StartMonitor(string supervisorName, int interviewerId, string telephoneNumber);
        void StopMonitor(string supervisorName, int interviewerId);
        BvTasksEntity TerminateTaskByPerson(int personSid, CallOutcome? explicitIts);
        void TerminateTasksByDialerId(int dialerId);
        void ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters);
        void SendMessage(FeedbackForm mailMessage);
        void ConfigureInboundDdiNumbers(int dialerId);
        IEnumerable<LogFileInfo> GetLogFiles(int dialerId);
        byte[] GetLogFileBodyZipped(int dialerId, string fileName);
        string GetDialerVersion(int dialerId);
        DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionality(int dialerId);
        DialerFeatures GetDialerSupportedFeatures(int dialerId);
        IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeatures(int dialerId);
        void UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue);
    }
}