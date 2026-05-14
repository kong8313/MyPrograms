using System;
using Confirmit.CATI.Core.Mail.Feedback;
using Confirmit.CATI.Core.SupervisorService;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using DialerCommon.DialerParameters;
using System.Data;
using Confirmit.CATI.Core.Services.PersonImport;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Telephony;
using DialerCommon;
using Confirmit.CATI.Monitoring.Common.Contracts;

namespace Confirmit.CATI.Core.SupervisorService.Fakes
{
    public class StubISupervisorService : ISupervisorService 
    {
        private ISupervisorService _inner;

        public StubISupervisorService()
        {
            _inner = null;
        }

        public ISupervisorService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendMessageFeedbackFormDelegate(FeedbackForm feedback);
        public SendMessageFeedbackFormDelegate SendMessageFeedbackForm;

        void ISupervisorService.SendMessage(FeedbackForm feedback)
        {

            if (SendMessageFeedbackForm != null)
            {
                SendMessageFeedbackForm(feedback);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).SendMessage(feedback);
            }
        }

        public delegate void OpenSurveyInt32Delegate(int surveySid);
        public OpenSurveyInt32Delegate OpenSurveyInt32;

        void ISupervisorService.OpenSurvey(int surveySid)
        {

            if (OpenSurveyInt32 != null)
            {
                OpenSurveyInt32(surveySid);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).OpenSurvey(surveySid);
            }
        }

        public delegate void CloseSurveyInt32Delegate(int surveySid);
        public CloseSurveyInt32Delegate CloseSurveyInt32;

        void ISupervisorService.CloseSurvey(int surveySid)
        {

            if (CloseSurveyInt32 != null)
            {
                CloseSurveyInt32(surveySid);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).CloseSurvey(surveySid);
            }
        }

        public delegate void ShutdownSurveyInt32Delegate(int surveySid);
        public ShutdownSurveyInt32Delegate ShutdownSurveyInt32;

        void ISupervisorService.ShutdownSurvey(int surveySid)
        {

            if (ShutdownSurveyInt32 != null)
            {
                ShutdownSurveyInt32(surveySid);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).ShutdownSurvey(surveySid);
            }
        }

        public delegate BvTasksEntity TerminateTaskByPersonInt32NullableOfCallOutcomeDelegate(int personSid, CallOutcome? explicitIts);
        public TerminateTaskByPersonInt32NullableOfCallOutcomeDelegate TerminateTaskByPersonInt32NullableOfCallOutcome;

        BvTasksEntity ISupervisorService.TerminateTaskByPerson(int personSid, CallOutcome? explicitIts)
        {


            if (TerminateTaskByPersonInt32NullableOfCallOutcome != null)
            {
                return TerminateTaskByPersonInt32NullableOfCallOutcome(personSid, explicitIts);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).TerminateTaskByPerson(personSid, explicitIts);
            }

            return default(BvTasksEntity);
        }

        public delegate void TerminateTasksByDialerIdInt32Delegate(int dialerId);
        public TerminateTasksByDialerIdInt32Delegate TerminateTasksByDialerIdInt32;

        void ISupervisorService.TerminateTasksByDialerId(int dialerId)
        {

            if (TerminateTasksByDialerIdInt32 != null)
            {
                TerminateTasksByDialerIdInt32(dialerId);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).TerminateTasksByDialerId(dialerId);
            }
        }

        public delegate bool EnableDialerInt32Delegate(int dialerId);
        public EnableDialerInt32Delegate EnableDialerInt32;

        bool ISupervisorService.EnableDialer(int dialerId)
        {


            if (EnableDialerInt32 != null)
            {
                return EnableDialerInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).EnableDialer(dialerId);
            }

            return default(bool);
        }

        public delegate bool DisableDialerInt32Delegate(int dialerId);
        public DisableDialerInt32Delegate DisableDialerInt32;

        bool ISupervisorService.DisableDialer(int dialerId)
        {


            if (DisableDialerInt32 != null)
            {
                return DisableDialerInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).DisableDialer(dialerId);
            }

            return default(bool);
        }

        public delegate void SetPersonParentGroupsInt32ArrayOfInt32Delegate(int personSid, int[] parentGroupsSids);
        public SetPersonParentGroupsInt32ArrayOfInt32Delegate SetPersonParentGroupsInt32ArrayOfInt32;

        void ISupervisorService.SetPersonParentGroups(int personSid, int[] parentGroupsSids)
        {

            if (SetPersonParentGroupsInt32ArrayOfInt32 != null)
            {
                SetPersonParentGroupsInt32ArrayOfInt32(personSid, parentGroupsSids);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).SetPersonParentGroups(personSid, parentGroupsSids);
            }
        }

        public delegate void DeletePersonInt32Delegate(int personSid);
        public DeletePersonInt32Delegate DeletePersonInt32;

        void ISupervisorService.DeletePerson(int personSid)
        {

            if (DeletePersonInt32 != null)
            {
                DeletePersonInt32(personSid);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).DeletePerson(personSid);
            }
        }

        public delegate void DeletePersonsListOfInt32Delegate(List<int> personSids);
        public DeletePersonsListOfInt32Delegate DeletePersonsListOfInt32;

        void ISupervisorService.DeletePersons(List<int> personSids)
        {

            if (DeletePersonsListOfInt32 != null)
            {
                DeletePersonsListOfInt32(personSids);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).DeletePersons(personSids);
            }
        }

        public delegate void LockPersonBySupervisorInt32Delegate(int personId);
        public LockPersonBySupervisorInt32Delegate LockPersonBySupervisorInt32;

        void ISupervisorService.LockPersonBySupervisor(int personId)
        {

            if (LockPersonBySupervisorInt32 != null)
            {
                LockPersonBySupervisorInt32(personId);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).LockPersonBySupervisor(personId);
            }
        }

        public delegate void LockPersonsBySupervisorListOfInt32Delegate(List<int> personIds);
        public LockPersonsBySupervisorListOfInt32Delegate LockPersonsBySupervisorListOfInt32;

        void ISupervisorService.LockPersonsBySupervisor(List<int> personIds)
        {

            if (LockPersonsBySupervisorListOfInt32 != null)
            {
                LockPersonsBySupervisorListOfInt32(personIds);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).LockPersonsBySupervisor(personIds);
            }
        }

        public delegate bool IsDialerOperationalInt32Delegate(int dialerId);
        public IsDialerOperationalInt32Delegate IsDialerOperationalInt32;

        bool ISupervisorService.IsDialerOperational(int dialerId)
        {


            if (IsDialerOperationalInt32 != null)
            {
                return IsDialerOperationalInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).IsDialerOperational(dialerId);
            }

            return default(bool);
        }

        public delegate void SaveScheduleInt32StringDelegate(int scheduleSid, string serializedSchedule);
        public SaveScheduleInt32StringDelegate SaveScheduleInt32String;

        void ISupervisorService.SaveSchedule(int scheduleSid, string serializedSchedule)
        {

            if (SaveScheduleInt32String != null)
            {
                SaveScheduleInt32String(scheduleSid, serializedSchedule);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).SaveSchedule(scheduleSid, serializedSchedule);
            }
        }

        public delegate void LaunchScheduleInt32Delegate(int scheduleSid);
        public LaunchScheduleInt32Delegate LaunchScheduleInt32;

        void ISupervisorService.LaunchSchedule(int scheduleSid)
        {

            if (LaunchScheduleInt32 != null)
            {
                LaunchScheduleInt32(scheduleSid);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).LaunchSchedule(scheduleSid);
            }
        }

        public delegate void CheckScheduleStringDelegate(string serializedSchedule);
        public CheckScheduleStringDelegate CheckScheduleString;

        void ISupervisorService.CheckSchedule(string serializedSchedule)
        {

            if (CheckScheduleString != null)
            {
                CheckScheduleString(serializedSchedule);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).CheckSchedule(serializedSchedule);
            }
        }

        public delegate bool ScheduleDelegate();
        public ScheduleDelegate Schedule;

        bool ISupervisorService.Schedule()
        {


            if (Schedule != null)
            {
                return Schedule();
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).Schedule();
            }

            return default(bool);
        }

        public delegate void ForceCallDeliveryDelegate();
        public ForceCallDeliveryDelegate ForceCallDelivery;

        void ISupervisorService.ForceCallDelivery()
        {

            if (ForceCallDelivery != null)
            {
                ForceCallDelivery();
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).ForceCallDelivery();
            }
        }

        public delegate void StartMonitorStringInt32StringDelegate(string supervisorName, int interviewerId, string telephoneNumber);
        public StartMonitorStringInt32StringDelegate StartMonitorStringInt32String;

        void ISupervisorService.StartMonitor(string supervisorName, int interviewerId, string telephoneNumber)
        {

            if (StartMonitorStringInt32String != null)
            {
                StartMonitorStringInt32String(supervisorName, interviewerId, telephoneNumber);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).StartMonitor(supervisorName, interviewerId, telephoneNumber);
            }
        }

        public delegate void StopMonitorStringInt32Delegate(string supervisorName, int interviewerId);
        public StopMonitorStringInt32Delegate StopMonitorStringInt32;

        void ISupervisorService.StopMonitor(string supervisorName, int interviewerId)
        {

            if (StopMonitorStringInt32 != null)
            {
                StopMonitorStringInt32(supervisorName, interviewerId);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).StopMonitor(supervisorName, interviewerId);
            }
        }

        public delegate void SetLiveMonitoringModeStringInt32MonitorModeDelegate(string supervisorName, int interviewerId, MonitorMode mode);
        public SetLiveMonitoringModeStringInt32MonitorModeDelegate SetLiveMonitoringModeStringInt32MonitorMode;

        void ISupervisorService.SetLiveMonitoringMode(string supervisorName, int interviewerId, MonitorMode mode)
        {

            if (SetLiveMonitoringModeStringInt32MonitorMode != null)
            {
                SetLiveMonitoringModeStringInt32MonitorMode(supervisorName, interviewerId, mode);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).SetLiveMonitoringMode(supervisorName, interviewerId, mode);
            }
        }

        public delegate int CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfStringDelegate(int callCenterId, int personSid, string name, string description, string fullName, string password, AgentTaskChoiceMode mode, PersonAssignmentListMode assignmentListMode, TaskChoicePermissions? permissions, List<int> parentGroups, int? autoSurveyId, int callGroupId, string location, DialType dialType, AgentType agentType, bool enableSoftphoneIntegration, string[] attributes);
        public CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfStringDelegate CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfString;

        int ISupervisorService.CreateOrUpdatePerson(int callCenterId, int personSid, string name, string description, string fullName, string password, AgentTaskChoiceMode mode, PersonAssignmentListMode assignmentListMode, TaskChoicePermissions? permissions, List<int> parentGroups, int? autoSurveyId, int callGroupId, string location, DialType dialType, AgentType agentType, bool enableSoftphoneIntegration, string[] attributes)
        {


            if (CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfString != null)
            {
                return CreateOrUpdatePersonInt32Int32StringStringStringStringAgentTaskChoiceModePersonAssignmentListModeNullableOfTaskChoicePermissionsListOfInt32NullableOfInt32Int32StringDialTypeAgentTypeBooleanArrayOfString(callCenterId, personSid, name, description, fullName, password, mode, assignmentListMode, permissions, parentGroups, autoSurveyId, callGroupId, location, dialType, agentType, enableSoftphoneIntegration, attributes);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).CreateOrUpdatePerson(callCenterId, personSid, name, description, fullName, password, mode, assignmentListMode, permissions, parentGroups, autoSurveyId, callGroupId, location, dialType, agentType, enableSoftphoneIntegration, attributes);
            }

            return default(int);
        }

        public delegate IEnumerable<AudioRecordInfo> GetInterviewRecordingsInt32Int32Delegate(int surveyId, int interviewId);
        public GetInterviewRecordingsInt32Int32Delegate GetInterviewRecordingsInt32Int32;

        IEnumerable<AudioRecordInfo> ISupervisorService.GetInterviewRecordings(int surveyId, int interviewId)
        {


            if (GetInterviewRecordingsInt32Int32 != null)
            {
                return GetInterviewRecordingsInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetInterviewRecordings(surveyId, interviewId);
            }

            return default(IEnumerable<AudioRecordInfo>);
        }

        public delegate bool[] AreRecordsExistsInt32ArrayOfInt32Delegate(int surveySid, int[] interviewIds);
        public AreRecordsExistsInt32ArrayOfInt32Delegate AreRecordsExistsInt32ArrayOfInt32;

        bool[] ISupervisorService.AreRecordsExists(int surveySid, int[] interviewIds)
        {


            if (AreRecordsExistsInt32ArrayOfInt32 != null)
            {
                return AreRecordsExistsInt32ArrayOfInt32(surveySid, interviewIds);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).AreRecordsExists(surveySid, interviewIds);
            }

            return default(bool[]);
        }

        public delegate void SetDialerDefaultSurveyParametersIEnumerableOfDialerParameterDelegate(IEnumerable<DialerParameter> parameters);
        public SetDialerDefaultSurveyParametersIEnumerableOfDialerParameterDelegate SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter;

        void ISupervisorService.SetDialerDefaultSurveyParameters(IEnumerable<DialerParameter> parameters)
        {

            if (SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter != null)
            {
                SetDialerDefaultSurveyParametersIEnumerableOfDialerParameter(parameters);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).SetDialerDefaultSurveyParameters(parameters);
            }
        }

        public delegate void ValidateDialerSurveyParametersIEnumerableOfDialerParameterDelegate(IEnumerable<DialerParameter> parameters);
        public ValidateDialerSurveyParametersIEnumerableOfDialerParameterDelegate ValidateDialerSurveyParametersIEnumerableOfDialerParameter;

        void ISupervisorService.ValidateDialerSurveyParameters(IEnumerable<DialerParameter> parameters)
        {

            if (ValidateDialerSurveyParametersIEnumerableOfDialerParameter != null)
            {
                ValidateDialerSurveyParametersIEnumerableOfDialerParameter(parameters);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).ValidateDialerSurveyParameters(parameters);
            }
        }

        public delegate void SetDialerSurveyParametersInt32IEnumerableOfDialerParameterDelegate(int surveySid, IEnumerable<DialerParameter> parameters);
        public SetDialerSurveyParametersInt32IEnumerableOfDialerParameterDelegate SetDialerSurveyParametersInt32IEnumerableOfDialerParameter;

        void ISupervisorService.SetDialerSurveyParameters(int surveySid, IEnumerable<DialerParameter> parameters)
        {

            if (SetDialerSurveyParametersInt32IEnumerableOfDialerParameter != null)
            {
                SetDialerSurveyParametersInt32IEnumerableOfDialerParameter(surveySid, parameters);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).SetDialerSurveyParameters(surveySid, parameters);
            }
        }

        public delegate ImportResult ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptionsDelegate(int callCenterId, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions);
        public ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptionsDelegate ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions;

        ImportResult ISupervisorService.ImportPersons(int callCenterId, DataTable dataTable, Dictionary<string, ColumnRole> columnRoleMap, ImportOptions importOptions)
        {


            if (ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions != null)
            {
                return ImportPersonsInt32DataTableDictionaryOfStringColumnRoleImportOptions(callCenterId, dataTable, columnRoleMap, importOptions);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).ImportPersons(callCenterId, dataTable, columnRoleMap, importOptions);
            }

            return default(ImportResult);
        }

        public delegate void ConfigureInboundDdiNumbersInt32Delegate(int dialerId);
        public ConfigureInboundDdiNumbersInt32Delegate ConfigureInboundDdiNumbersInt32;

        void ISupervisorService.ConfigureInboundDdiNumbers(int dialerId)
        {

            if (ConfigureInboundDdiNumbersInt32 != null)
            {
                ConfigureInboundDdiNumbersInt32(dialerId);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).ConfigureInboundDdiNumbers(dialerId);
            }
        }

        public delegate IEnumerable<LogFileInfo> GetLogFilesInt32Delegate(int dialerId);
        public GetLogFilesInt32Delegate GetLogFilesInt32;

        IEnumerable<LogFileInfo> ISupervisorService.GetLogFiles(int dialerId)
        {


            if (GetLogFilesInt32 != null)
            {
                return GetLogFilesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetLogFiles(dialerId);
            }

            return default(IEnumerable<LogFileInfo>);
        }

        public delegate byte[] GetLogFileBodyZippedInt32StringDelegate(int dialerId, string fileName);
        public GetLogFileBodyZippedInt32StringDelegate GetLogFileBodyZippedInt32String;

        byte[] ISupervisorService.GetLogFileBodyZipped(int dialerId, string fileName)
        {


            if (GetLogFileBodyZippedInt32String != null)
            {
                return GetLogFileBodyZippedInt32String(dialerId, fileName);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetLogFileBodyZipped(dialerId, fileName);
            }

            return default(byte[]);
        }

        public delegate string GetDialerVersionInt32Delegate(int dialerId);
        public GetDialerVersionInt32Delegate GetDialerVersionInt32;

        string ISupervisorService.GetDialerVersion(int dialerId)
        {


            if (GetDialerVersionInt32 != null)
            {
                return GetDialerVersionInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetDialerVersion(dialerId);
            }

            return default(string);
        }

        public delegate DialerAvailableExtendedFunctionality GetAvailableExtendedFunctionalityInt32Delegate(int dialerId);
        public GetAvailableExtendedFunctionalityInt32Delegate GetAvailableExtendedFunctionalityInt32;

        DialerAvailableExtendedFunctionality ISupervisorService.GetAvailableExtendedFunctionality(int dialerId)
        {


            if (GetAvailableExtendedFunctionalityInt32 != null)
            {
                return GetAvailableExtendedFunctionalityInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetAvailableExtendedFunctionality(dialerId);
            }

            return default(DialerAvailableExtendedFunctionality);
        }

        public delegate DialerFeatures GetDialerSupportedFeaturesInt32Delegate(int dialerId);
        public GetDialerSupportedFeaturesInt32Delegate GetDialerSupportedFeaturesInt32;

        DialerFeatures ISupervisorService.GetDialerSupportedFeatures(int dialerId)
        {


            if (GetDialerSupportedFeaturesInt32 != null)
            {
                return GetDialerSupportedFeaturesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetDialerSupportedFeatures(dialerId);
            }

            return default(DialerFeatures);
        }

        public delegate AudioFile GetAudioFileInt32StringDelegate(int dialerId, string audioUrl);
        public GetAudioFileInt32StringDelegate GetAudioFileInt32String;

        AudioFile ISupervisorService.GetAudioFile(int dialerId, string audioUrl)
        {


            if (GetAudioFileInt32String != null)
            {
                return GetAudioFileInt32String(dialerId, audioUrl);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetAudioFile(dialerId, audioUrl);
            }

            return default(AudioFile);
        }

        public delegate IEnumerable<DialerOverridenFeature> GetOverridenDialerSupportedFeaturesInt32Delegate(int dialerId);
        public GetOverridenDialerSupportedFeaturesInt32Delegate GetOverridenDialerSupportedFeaturesInt32;

        IEnumerable<DialerOverridenFeature> ISupervisorService.GetOverridenDialerSupportedFeatures(int dialerId)
        {


            if (GetOverridenDialerSupportedFeaturesInt32 != null)
            {
                return GetOverridenDialerSupportedFeaturesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetOverridenDialerSupportedFeatures(dialerId);
            }

            return default(IEnumerable<DialerOverridenFeature>);
        }

        public delegate void UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBooleanDelegate(int dialerId, string featureName, bool? overridenFeatureValue);
        public UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBooleanDelegate UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean;

        void ISupervisorService.UpdateOverridenDialerSupportedFeature(int dialerId, string featureName, bool? overridenFeatureValue)
        {

            if (UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean != null)
            {
                UpdateOverridenDialerSupportedFeatureInt32StringNullableOfBoolean(dialerId, featureName, overridenFeatureValue);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).UpdateOverridenDialerSupportedFeature(dialerId, featureName, overridenFeatureValue);
            }
        }

        public delegate void SendMessageToInterviewersIEnumerableOfInt32BooleanStringStringDelegate(IEnumerable<int> interviewerIds, bool onlineOnly, string message, string supervisorName);
        public SendMessageToInterviewersIEnumerableOfInt32BooleanStringStringDelegate SendMessageToInterviewersIEnumerableOfInt32BooleanStringString;

        void ISupervisorService.SendMessageToInterviewers(IEnumerable<int> interviewerIds, bool onlineOnly, string message, string supervisorName)
        {

            if (SendMessageToInterviewersIEnumerableOfInt32BooleanStringString != null)
            {
                SendMessageToInterviewersIEnumerableOfInt32BooleanStringString(interviewerIds, onlineOnly, message, supervisorName);
            } else if (_inner != null)
            {
                ((ISupervisorService)_inner).SendMessageToInterviewers(interviewerIds, onlineOnly, message, supervisorName);
            }
        }

        public delegate IEnumerable<AudioIdentityObject> GetAudioIdentitiesInt64Delegate(long recordId);
        public GetAudioIdentitiesInt64Delegate GetAudioIdentitiesInt64;

        IEnumerable<AudioIdentityObject> ISupervisorService.GetAudioIdentities(long recordId)
        {


            if (GetAudioIdentitiesInt64 != null)
            {
                return GetAudioIdentitiesInt64(recordId);
            } else if (_inner != null)
            {
                return ((ISupervisorService)_inner).GetAudioIdentities(recordId);
            }

            return default(IEnumerable<AudioIdentityObject>);
        }

    }
}