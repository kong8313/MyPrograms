using System;
using Confirmit.CATI.Core.SupervisorService;
using ConfirmitDialerInterface;
using Confirmit.CATI.Common;
using System.Collections.Generic;
using System.Data;
using Confirmit.CATI.Core.Services.PersonImport;
using DialerCommon.DialerParameters;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Telephony;
using DialerCommon;

namespace Confirmit.CATI.Core.SupervisorService.Fakes
{
    public class StubISupervisorServiceClient : ISupervisorServiceClient 
    {
        private ISupervisorServiceClient _inner;

        public StubISupervisorServiceClient()
        {
            _inner = null;
        }

        public ISupervisorServiceClient Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool[] AreRecordsExistsInt32ArrayOfInt32Delegate(int surveySid, int[] interviewIds);
        public AreRecordsExistsInt32ArrayOfInt32Delegate AreRecordsExistsInt32ArrayOfInt32;

        bool[] ISupervisorServiceClient.AreRecordsExists(int surveySid, int[] interviewIds)
        {


            if (AreRecordsExistsInt32ArrayOfInt32 != null)
            {
                return AreRecordsExistsInt32ArrayOfInt32(surveySid, interviewIds);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).AreRecordsExists(surveySid, interviewIds);
            }

            return default(bool[]);
        }

        public delegate void CheckScheduleStringDelegate(string serializedSchedule);
        public CheckScheduleStringDelegate CheckScheduleString;

        void ISupervisorServiceClient.CheckSchedule(string serializedSchedule)
        {

            if (CheckScheduleString != null)
            {
                CheckScheduleString(serializedSchedule);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).CheckSchedule(serializedSchedule);
            }
        }

        public delegate void CloseSurveyInt32Delegate(int surveySid);
        public CloseSurveyInt32Delegate CloseSurveyInt32;

        void ISupervisorServiceClient.CloseSurvey(int surveySid)
        {

            if (CloseSurveyInt32 != null)
            {
                CloseSurveyInt32(surveySid);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).CloseSurvey(surveySid);
            }
        }

        public delegate void CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfStringDelegate(int callCenterId, int personSid, string name, string description, string fullName, string password, AgentTaskChoiceMode mode, PersonAssignmentListMode assignmentListMode, TaskChoicePermissions? permissions, List<int> parentGroups, int? autoSurveyId, int callGroupId, string location, DialType dialType, AgentType agentType, bool enableSoftphoneIntegration, string[] attributes);
        public CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfStringDelegate CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfString;

        void ISupervisorServiceClient.CreateOrUpdatePerson(int callCenterId, int personSid, string name, string description, string fullName, string password, AgentTaskChoiceMode mode, PersonAssignmentListMode assignmentListMode, TaskChoicePermissions? permissions, List<int> parentGroups, int? autoSurveyId, int callGroupId, string location, DialType dialType, AgentType agentType, bool enableSoftphoneIntegration, string[] attributes)
        {

            if (CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfString != null)
            {
                CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfString(callCenterId, personSid, name, description, fullName, password, mode, assignmentListMode, permissions, parentGroups, autoSurveyId, callGroupId, location, dialType, agentType, enableSoftphoneIntegration, attributes);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).CreateOrUpdatePerson(callCenterId, personSid, name, description, fullName, password, mode, assignmentListMode, permissions, parentGroups, autoSurveyId, callGroupId, location, dialType, agentType, enableSoftphoneIntegration, attributes);
            }
        }

        public delegate void DeletePersonsListOfInt32Delegate(List<int> personSids);
        public DeletePersonsListOfInt32Delegate DeletePersonsListOfInt32;

        void ISupervisorServiceClient.DeletePersons(List<int> personSids)
        {

            if (DeletePersonsListOfInt32 != null)
            {
                DeletePersonsListOfInt32(personSids);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).DeletePersons(personSids);
            }
        }

        public delegate bool EnableDialerInt32Delegate(int dialerId);
        public EnableDialerInt32Delegate EnableDialerInt32;

        bool ISupervisorServiceClient.EnableDialer(int dialerId)
        {


            if (EnableDialerInt32 != null)
            {
                return EnableDialerInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).EnableDialer(dialerId);
            }

            return default(bool);
        }

        public delegate bool DisableDialerInt32Delegate(int dialerId);
        public DisableDialerInt32Delegate DisableDialerInt32;

        bool ISupervisorServiceClient.DisableDialer(int dialerId)
        {


            if (DisableDialerInt32 != null)
            {
                return DisableDialerInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).DisableDialer(dialerId);
            }

            return default(bool);
        }

        public delegate IEnumerable<AudioRecordInfo> GetInterviewRecordingsInt32Int32Delegate(int surveyId, int interviewId);
        public GetInterviewRecordingsInt32Int32Delegate GetInterviewRecordingsInt32Int32;

        IEnumerable<AudioRecordInfo> ISupervisorServiceClient.GetInterviewRecordings(int surveyId, int interviewId)
        {


            if (GetInterviewRecordingsInt32Int32 != null)
            {
                return GetInterviewRecordingsInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).GetInterviewRecordings(surveyId, interviewId);
            }

            return default(IEnumerable<AudioRecordInfo>);
        }

        public delegate ImportResult ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptionsDelegate(int callCenterID, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions);
        public ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptionsDelegate ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions;

        ImportResult ISupervisorServiceClient.ImportPersons(int callCenterID, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions)
        {


            if (ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions != null)
            {
                return ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions(callCenterID, dataTable, columnRoleMap, importOptions);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).ImportPersons(callCenterID, dataTable, columnRoleMap, importOptions);
            }

            return default(ImportResult);
        }

        public delegate bool IsDialerOperationalInt32Delegate(int dialerId);
        public IsDialerOperationalInt32Delegate IsDialerOperationalInt32;

        bool ISupervisorServiceClient.IsDialerOperational(int dialerId)
        {


            if (IsDialerOperationalInt32 != null)
            {
                return IsDialerOperationalInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).IsDialerOperational(dialerId);
            }

            return default(bool);
        }

        public delegate void LaunchScheduleInt32Delegate(int scheduleSid);
        public LaunchScheduleInt32Delegate LaunchScheduleInt32;

        void ISupervisorServiceClient.LaunchSchedule(int scheduleSid)
        {

            if (LaunchScheduleInt32 != null)
            {
                LaunchScheduleInt32(scheduleSid);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).LaunchSchedule(scheduleSid);
            }
        }

        public delegate void LockPersonsBySupervisorListOfInt32Delegate(List<int> personSids);
        public LockPersonsBySupervisorListOfInt32Delegate LockPersonsBySupervisorListOfInt32;

        void ISupervisorServiceClient.LockPersonsBySupervisor(List<int> personSids)
        {

            if (LockPersonsBySupervisorListOfInt32 != null)
            {
                LockPersonsBySupervisorListOfInt32(personSids);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).LockPersonsBySupervisor(personSids);
            }
        }

        public delegate void OpenSurveyInt32Delegate(int surveySid);
        public OpenSurveyInt32Delegate OpenSurveyInt32;

        void ISupervisorServiceClient.OpenSurvey(int surveySid)
        {

            if (OpenSurveyInt32 != null)
            {
                OpenSurveyInt32(surveySid);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).OpenSurvey(surveySid);
            }
        }

        public delegate void SaveScheduleInt32StringDelegate(int scheduleSid, string serializedSchedule);
        public SaveScheduleInt32StringDelegate SaveScheduleInt32String;

        void ISupervisorServiceClient.SaveSchedule(int scheduleSid, string serializedSchedule)
        {

            if (SaveScheduleInt32String != null)
            {
                SaveScheduleInt32String(scheduleSid, serializedSchedule);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).SaveSchedule(scheduleSid, serializedSchedule);
            }
        }

        public delegate bool ScheduleDelegate();
        public ScheduleDelegate Schedule;

        bool ISupervisorServiceClient.Schedule()
        {


            if (Schedule != null)
            {
                return Schedule();
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).Schedule();
            }

            return default(bool);
        }

        public delegate void SetDialerDefaultSurveyParametersIEnumerableOfDialerParameterDelegate(IEnumerable<DialerParameter> parameters);
        public SetDialerDefaultSurveyParametersIEnumerableOfDialerParameterDelegate SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter;

        void ISupervisorServiceClient.SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters)
        {

            if (SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter != null)
            {
                SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter(parameters);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).SetDialerDefaultSurveyParameters(parameters);
            }
        }

        public delegate void SetDialerSurveyParametersInt32IEnumerableOfDialerParameterDelegate(int surveySid, IEnumerable<DialerParameter> parameters);
        public SetDialerSurveyParametersInt32IEnumerableOfDialerParameterDelegate SetDialerSurveyParametersInt32IEnumerableOfDialerParameter;

        void ISupervisorServiceClient.SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters)
        {

            if (SetDialerSurveyParametersInt32IEnumerableOfDialerParameter != null)
            {
                SetDialerSurveyParametersInt32IEnumerableOfDialerParameter(surveySid, parameters);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).SetDialerSurveyParameters(surveySid, parameters);
            }
        }

        public delegate void SetPersonParentGroupsInt32ArrayOfInt32Delegate(int personSid, int[] parentGroupsSids);
        public SetPersonParentGroupsInt32ArrayOfInt32Delegate SetPersonParentGroupsInt32ArrayOfInt32;

        void ISupervisorServiceClient.SetPersonParentGroups(int personSid, int[] parentGroupsSids)
        {

            if (SetPersonParentGroupsInt32ArrayOfInt32 != null)
            {
                SetPersonParentGroupsInt32ArrayOfInt32(personSid, parentGroupsSids);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).SetPersonParentGroups(personSid, parentGroupsSids);
            }
        }

        public delegate void ShutdownSurveyInt32Delegate(int surveySid);
        public ShutdownSurveyInt32Delegate ShutdownSurveyInt32;

        void ISupervisorServiceClient.ShutdownSurvey(int surveySid)
        {

            if (ShutdownSurveyInt32 != null)
            {
                ShutdownSurveyInt32(surveySid);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).ShutdownSurvey(surveySid);
            }
        }

        public delegate void StartMonitorStringInt32StringDelegate(string supervisorName, int interviewerId, string telephoneNumber);
        public StartMonitorStringInt32StringDelegate StartMonitorStringInt32String;

        void ISupervisorServiceClient.StartMonitor(string supervisorName, int interviewerId, string telephoneNumber)
        {

            if (StartMonitorStringInt32String != null)
            {
                StartMonitorStringInt32String(supervisorName, interviewerId, telephoneNumber);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).StartMonitor(supervisorName, interviewerId, telephoneNumber);
            }
        }

        public delegate void StopMonitorStringInt32Delegate(string supervisorName, int interviewerId);
        public StopMonitorStringInt32Delegate StopMonitorStringInt32;

        void ISupervisorServiceClient.StopMonitor(string supervisorName, int interviewerId)
        {

            if (StopMonitorStringInt32 != null)
            {
                StopMonitorStringInt32(supervisorName, interviewerId);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).StopMonitor(supervisorName, interviewerId);
            }
        }

        public delegate BvTasksEntity TerminateTaskByPersonInt32NullableOfCallOutcomeDelegate(int personSid, CallOutcome? explicitIts);
        public TerminateTaskByPersonInt32NullableOfCallOutcomeDelegate TerminateTaskByPersonInt32NullableOfCallOutcome;

        BvTasksEntity ISupervisorServiceClient.TerminateTaskByPerson(int personSid, CallOutcome? explicitIts)
        {


            if (TerminateTaskByPersonInt32NullableOfCallOutcome != null)
            {
                return TerminateTaskByPersonInt32NullableOfCallOutcome(personSid, explicitIts);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).TerminateTaskByPerson(personSid, explicitIts);
            }

            return default(BvTasksEntity);
        }

        public delegate void TerminateTasksByDialerIdInt32Delegate(int dialerId);
        public TerminateTasksByDialerIdInt32Delegate TerminateTasksByDialerIdInt32;

        void ISupervisorServiceClient.TerminateTasksByDialerId(int dialerId)
        {

            if (TerminateTasksByDialerIdInt32 != null)
            {
                TerminateTasksByDialerIdInt32(dialerId);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).TerminateTasksByDialerId(dialerId);
            }
        }

        public delegate void ValidateDialerSurveyParametersIEnumerableOfDialerParameterDelegate(IEnumerable<DialerParameter> parameters);
        public ValidateDialerSurveyParametersIEnumerableOfDialerParameterDelegate ValidateDialerSurveyParametersIEnumerableOfDialerParameter;

        void ISupervisorServiceClient.ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters)
        {

            if (ValidateDialerSurveyParametersIEnumerableOfDialerParameter != null)
            {
                ValidateDialerSurveyParametersIEnumerableOfDialerParameter(parameters);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).ValidateDialerSurveyParameters(parameters);
            }
        }

        public delegate void SendMessageFeedbackFormDelegate(FeedbackForm mailMessage);
        public SendMessageFeedbackFormDelegate SendMessageFeedbackForm;

        void ISupervisorServiceClient.SendMessage(FeedbackForm mailMessage)
        {

            if (SendMessageFeedbackForm != null)
            {
                SendMessageFeedbackForm(mailMessage);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).SendMessage(mailMessage);
            }
        }

        public delegate void ConfigureInboundDdiNumbersInt32Delegate(int dialerId);
        public ConfigureInboundDdiNumbersInt32Delegate ConfigureInboundDdiNumbersInt32;

        void ISupervisorServiceClient.ConfigureInboundDdiNumbers(int dialerId)
        {

            if (ConfigureInboundDdiNumbersInt32 != null)
            {
                ConfigureInboundDdiNumbersInt32(dialerId);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).ConfigureInboundDdiNumbers(dialerId);
            }
        }

        public delegate IEnumerable<LogFileInfo> GetLogFilesInt32Delegate(int dialerId);
        public GetLogFilesInt32Delegate GetLogFilesInt32;

        IEnumerable<LogFileInfo> ISupervisorServiceClient.GetLogFiles(int dialerId)
        {


            if (GetLogFilesInt32 != null)
            {
                return GetLogFilesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).GetLogFiles(dialerId);
            }

            return default(IEnumerable<LogFileInfo>);
        }

        public delegate byte[] GetLogFileBodyZippedInt32StringDelegate(int dialerId, string fileName);
        public GetLogFileBodyZippedInt32StringDelegate GetLogFileBodyZippedInt32String;

        byte[] ISupervisorServiceClient.GetLogFileBodyZipped(int dialerId, string fileName)
        {


            if (GetLogFileBodyZippedInt32String != null)
            {
                return GetLogFileBodyZippedInt32String(dialerId, fileName);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).GetLogFileBodyZipped(dialerId, fileName);
            }

            return default(byte[]);
        }

        public delegate string GetDialerVersionInt32Delegate(int dialerId);
        public GetDialerVersionInt32Delegate GetDialerVersionInt32;

        string ISupervisorServiceClient.GetDialerVersion(int dialerId)
        {


            if (GetDialerVersionInt32 != null)
            {
                return GetDialerVersionInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).GetDialerVersion(dialerId);
            }

            return default(string);
        }

        public delegate DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionalityInt32Delegate(int dialerId);
        public GetAvailableExtendedFunctionalityInt32Delegate GetAvailableExtendedFunctionalityInt32;

        DialerAvailableExtendedFunctionality ISupervisorServiceClient.GetAvailableExtendedFunctionality(int dialerId)
        {


            if (GetAvailableExtendedFunctionalityInt32 != null)
            {
                return GetAvailableExtendedFunctionalityInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).GetAvailableExtendedFunctionality(dialerId);
            }

            return default(DialerAvailableExtendedFunctionality);
        }

        public delegate DialerFeatures GetDialerSupportedFeaturesInt32Delegate(int dialerId);
        public GetDialerSupportedFeaturesInt32Delegate GetDialerSupportedFeaturesInt32;

        DialerFeatures ISupervisorServiceClient.GetDialerSupportedFeatures(int dialerId)
        {


            if (GetDialerSupportedFeaturesInt32 != null)
            {
                return GetDialerSupportedFeaturesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).GetDialerSupportedFeatures(dialerId);
            }

            return default(DialerFeatures);
        }

        public delegate IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeaturesInt32Delegate(int dialerId);
        public GetOverridenDialerSupportedFeaturesInt32Delegate GetOverridenDialerSupportedFeaturesInt32;

        IEnumerable<DialerOverridenFeature> ISupervisorServiceClient.GetOverridenDialerSupportedFeatures(int dialerId)
        {


            if (GetOverridenDialerSupportedFeaturesInt32 != null)
            {
                return GetOverridenDialerSupportedFeaturesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorServiceClient)_inner).GetOverridenDialerSupportedFeatures(dialerId);
            }

            return default(IEnumerable<DialerOverridenFeature>);
        }

        public delegate void UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBooleanDelegate(int dialerId, string featureName, bool? overridenFeatureValue);
        public UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBooleanDelegate UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean;

        void ISupervisorServiceClient.UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue)
        {

            if (UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean != null)
            {
                UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean(dialerId, featureName, overridenFeatureValue);
            } else if (_inner != null)
            {
                ((ISupervisorServiceClient)_inner).UpdateOverridenDialerSupportedFeature(dialerId, featureName, overridenFeatureValue);
            }
        }

    }
}