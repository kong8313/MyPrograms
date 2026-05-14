using System;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.ManagementService.Fakes
{
    public class StubIManagementService : IManagementService 
    {
        private IManagementService _inner;

        public StubIManagementService()
        {
            _inner = null;
        }

        public IManagementService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int Telephony_IvrRenderVoiceXmlInt32Int32Int64Int32Int32StringDelegate(int companyId, int dialerId, long campaignId, int agentId, int contactId, string voiceXml);
        public Telephony_IvrRenderVoiceXmlInt32Int32Int64Int32Int32StringDelegate Telephony_IvrRenderVoiceXmlInt32Int32Int64Int32Int32String;

        int IManagementService.Telephony_IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, int contactId, string voiceXml)
        {


            if (Telephony_IvrRenderVoiceXmlInt32Int32Int64Int32Int32String != null)
            {
                return Telephony_IvrRenderVoiceXmlInt32Int32Int64Int32Int32String(companyId, dialerId, campaignId, agentId, contactId, voiceXml);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_IvrRenderVoiceXml(companyId, dialerId, campaignId, agentId, contactId, voiceXml);
            }

            return default(int);
        }

        public delegate int Telephony_StartCustomIvrInterviewInt32Int64StringInt32Int64StringDelegate(int dialerId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink);
        public Telephony_StartCustomIvrInterviewInt32Int64StringInt32Int64StringDelegate Telephony_StartCustomIvrInterviewInt32Int64StringInt32Int64String;

        int IManagementService.Telephony_StartCustomIvrInterview(int dialerId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink)
        {


            if (Telephony_StartCustomIvrInterviewInt32Int64StringInt32Int64String != null)
            {
                return Telephony_StartCustomIvrInterviewInt32Int64StringInt32Int64String(dialerId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_StartCustomIvrInterview(dialerId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            }

            return default(int);
        }

        public delegate bool IsInboundCallInt32Delegate(int catiInterviewerId);
        public IsInboundCallInt32Delegate IsInboundCallInt32;

        bool IManagementService.IsInboundCall(int catiInterviewerId)
        {


            if (IsInboundCallInt32 != null)
            {
                return IsInboundCallInt32(catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).IsInboundCall(catiInterviewerId);
            }

            return default(bool);
        }

        public delegate bool IsIvrCallInt32Delegate(int catiInterviewerId);
        public IsIvrCallInt32Delegate IsIvrCallInt32;

        bool IManagementService.IsIvrCall(int catiInterviewerId)
        {


            if (IsIvrCallInt32 != null)
            {
                return IsIvrCallInt32(catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).IsIvrCall(catiInterviewerId);
            }

            return default(bool);
        }

        public delegate bool SetNextLinkedInterviewStringInt32Int32Delegate(string projectId, int respondentId, int catiInterviewerId);
        public SetNextLinkedInterviewStringInt32Int32Delegate SetNextLinkedInterviewStringInt32Int32;

        bool IManagementService.SetNextLinkedInterview(string projectId, int respondentId, int catiInterviewerId)
        {


            if (SetNextLinkedInterviewStringInt32Int32 != null)
            {
                return SetNextLinkedInterviewStringInt32Int32(projectId, respondentId, catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).SetNextLinkedInterview(projectId, respondentId, catiInterviewerId);
            }

            return default(bool);
        }

        public delegate bool SetNextLinkedInterviewToPreviousInt32Delegate(int catiInterviewerId);
        public SetNextLinkedInterviewToPreviousInt32Delegate SetNextLinkedInterviewToPreviousInt32;

        bool IManagementService.SetNextLinkedInterviewToPrevious(int catiInterviewerId)
        {


            if (SetNextLinkedInterviewToPreviousInt32 != null)
            {
                return SetNextLinkedInterviewToPreviousInt32(catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).SetNextLinkedInterviewToPrevious(catiInterviewerId);
            }

            return default(bool);
        }

        public delegate CatiInterview[] GetInterviewsArrayOfStringStringStringStringInt32Delegate(string[] projectList, string telephoneNumber, string respondentName, string filter, int catiInterviewerId);
        public GetInterviewsArrayOfStringStringStringStringInt32Delegate GetInterviewsArrayOfStringStringStringStringInt32;

        CatiInterview[] IManagementService.GetInterviews(string[] projectList, string telephoneNumber, string respondentName, string filter, int catiInterviewerId)
        {


            if (GetInterviewsArrayOfStringStringStringStringInt32 != null)
            {
                return GetInterviewsArrayOfStringStringStringStringInt32(projectList, telephoneNumber, respondentName, filter, catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetInterviews(projectList, telephoneNumber, respondentName, filter, catiInterviewerId);
            }

            return default(CatiInterview[]);
        }

        public delegate CatiInterview[] GetLinkedInterviewsInt32Delegate(int catiInterviewerId);
        public GetLinkedInterviewsInt32Delegate GetLinkedInterviewsInt32;

        CatiInterview[] IManagementService.GetLinkedInterviews(int catiInterviewerId)
        {


            if (GetLinkedInterviewsInt32 != null)
            {
                return GetLinkedInterviewsInt32(catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetLinkedInterviews(catiInterviewerId);
            }

            return default(CatiInterview[]);
        }

        public delegate void AddSurveyStringStringStringStringDelegate(string confirmitProjectID, string confirmitSurveyName, string cfSqlServerConnectionString, string userName);
        public AddSurveyStringStringStringStringDelegate AddSurveyStringStringStringString;

        void IManagementService.AddSurvey(string confirmitProjectID, string confirmitSurveyName, string cfSqlServerConnectionString, string userName)
        {

            if (AddSurveyStringStringStringString != null)
            {
                AddSurveyStringStringStringString(confirmitProjectID, confirmitSurveyName, cfSqlServerConnectionString, userName);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).AddSurvey(confirmitProjectID, confirmitSurveyName, cfSqlServerConnectionString, userName);
            }
        }

        public delegate int DeleteSurveyStringDelegate(string confirmitProjectID);
        public DeleteSurveyStringDelegate DeleteSurveyString;

        int IManagementService.DeleteSurvey(string confirmitProjectID)
        {


            if (DeleteSurveyString != null)
            {
                return DeleteSurveyString(confirmitProjectID);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).DeleteSurvey(confirmitProjectID);
            }

            return default(int);
        }

        public delegate void SoftDeleteSurveyStringDelegate(string confirmitProjectID);
        public SoftDeleteSurveyStringDelegate SoftDeleteSurveyString;

        void IManagementService.SoftDeleteSurvey(string confirmitProjectID)
        {

            if (SoftDeleteSurveyString != null)
            {
                SoftDeleteSurveyString(confirmitProjectID);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).SoftDeleteSurvey(confirmitProjectID);
            }
        }

        public delegate void RestoreSoftDeletedSurveyStringDelegate(string confirmitProjectID);
        public RestoreSoftDeletedSurveyStringDelegate RestoreSoftDeletedSurveyString;

        void IManagementService.RestoreSoftDeletedSurvey(string confirmitProjectID)
        {

            if (RestoreSoftDeletedSurveyString != null)
            {
                RestoreSoftDeletedSurveyString(confirmitProjectID);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).RestoreSoftDeletedSurvey(confirmitProjectID);
            }
        }

        public delegate void AddRespondentStringInt32Int32Delegate(string projectId, int respondentId, int its);
        public AddRespondentStringInt32Int32Delegate AddRespondentStringInt32Int32;

        void IManagementService.AddRespondent(string projectId, int respondentId, int its)
        {

            if (AddRespondentStringInt32Int32 != null)
            {
                AddRespondentStringInt32Int32(projectId, respondentId, its);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).AddRespondent(projectId, respondentId, its);
            }
        }

        public delegate void AddRespondentFromConsoleStringInt32Int32Delegate(string projectId, int respondentId, int personId);
        public AddRespondentFromConsoleStringInt32Int32Delegate AddRespondentFromConsoleStringInt32Int32;

        void IManagementService.AddRespondentFromConsole(string projectId, int respondentId, int personId)
        {

            if (AddRespondentFromConsoleStringInt32Int32 != null)
            {
                AddRespondentFromConsoleStringInt32Int32(projectId, respondentId, personId);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).AddRespondentFromConsole(projectId, respondentId, personId);
            }
        }

        public delegate void AddSampleStringInt32Int32Int32Delegate(string projectdID, int batchID, int mode, int recordsCount);
        public AddSampleStringInt32Int32Int32Delegate AddSampleStringInt32Int32Int32;

        void IManagementService.AddSample(string projectdID, int batchID, int mode, int recordsCount)
        {

            if (AddSampleStringInt32Int32Int32 != null)
            {
                AddSampleStringInt32Int32Int32(projectdID, batchID, mode, recordsCount);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).AddSample(projectdID, batchID, mode, recordsCount);
            }
        }

        public delegate void ProcessSampleStringInt32Int32Int32Delegate(string projectdId, int batchId, int sampleMode, int schedulingMode);
        public ProcessSampleStringInt32Int32Int32Delegate ProcessSampleStringInt32Int32Int32;

        void IManagementService.ProcessSample(string projectdId, int batchId, int sampleMode, int schedulingMode)
        {

            if (ProcessSampleStringInt32Int32Int32 != null)
            {
                ProcessSampleStringInt32Int32Int32(projectdId, batchId, sampleMode, schedulingMode);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).ProcessSample(projectdId, batchId, sampleMode, schedulingMode);
            }
        }

        public delegate void UpdateSurveyAccessListStringStringBooleanDelegate(string userId, string surveyId, bool enabled);
        public UpdateSurveyAccessListStringStringBooleanDelegate UpdateSurveyAccessListStringStringBoolean;

        void IManagementService.UpdateSurveyAccessList(string userId, string surveyId, bool enabled)
        {

            if (UpdateSurveyAccessListStringStringBoolean != null)
            {
                UpdateSurveyAccessListStringStringBoolean(userId, surveyId, enabled);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).UpdateSurveyAccessList(userId, surveyId, enabled);
            }
        }

        public delegate void UpdateSurveyPropertiesStringStringNullableOfInt32NullableOfBooleanNullableOfBooleanNullableOfBooleanBooleanBooleanStringBooleanDelegate(string confirmitProjectID, string confirmitProjectName, int? dialingMode, bool? openEndReview, bool? voiceRecording, bool? screenRecording, bool supportBlacklist, bool allowRespondentsDynamicCreation, string notificationEmail, bool enforceHttps);
        public UpdateSurveyPropertiesStringStringNullableOfInt32NullableOfBooleanNullableOfBooleanNullableOfBooleanBooleanBooleanStringBooleanDelegate UpdateSurveyPropertiesStringStringNullableOfInt32NullableOfBooleanNullableOfBooleanNullableOfBooleanBooleanBooleanStringBoolean;

        void IManagementService.UpdateSurveyProperties(string confirmitProjectID, string confirmitProjectName, int? dialingMode, bool? openEndReview, bool? voiceRecording, bool? screenRecording, bool supportBlacklist, bool allowRespondentsDynamicCreation, string notificationEmail, bool enforceHttps)
        {

            if (UpdateSurveyPropertiesStringStringNullableOfInt32NullableOfBooleanNullableOfBooleanNullableOfBooleanBooleanBooleanStringBoolean != null)
            {
                UpdateSurveyPropertiesStringStringNullableOfInt32NullableOfBooleanNullableOfBooleanNullableOfBooleanBooleanBooleanStringBoolean(confirmitProjectID, confirmitProjectName, dialingMode, openEndReview, voiceRecording, screenRecording, supportBlacklist, allowRespondentsDynamicCreation, notificationEmail, enforceHttps);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).UpdateSurveyProperties(confirmitProjectID, confirmitProjectName, dialingMode, openEndReview, voiceRecording, screenRecording, supportBlacklist, allowRespondentsDynamicCreation, notificationEmail, enforceHttps);
            }
        }

        public delegate bool IsSurveyOpenStringDelegate(string confirmitProjectID);
        public IsSurveyOpenStringDelegate IsSurveyOpenString;

        bool IManagementService.IsSurveyOpen(string confirmitProjectID)
        {


            if (IsSurveyOpenString != null)
            {
                return IsSurveyOpenString(confirmitProjectID);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).IsSurveyOpen(confirmitProjectID);
            }

            return default(bool);
        }

        public delegate void OnCATIOptionsChangedBooleanDelegate(bool bTelephonyEnabled);
        public OnCATIOptionsChangedBooleanDelegate OnCATIOptionsChangedBoolean;

        void IManagementService.OnCATIOptionsChanged(bool bTelephonyEnabled)
        {

            if (OnCATIOptionsChangedBoolean != null)
            {
                OnCATIOptionsChangedBoolean(bTelephonyEnabled);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).OnCATIOptionsChanged(bTelephonyEnabled);
            }
        }

        public delegate int DeleteRespondentsAsyncArrayOfInt32StringDelegate(int[] respIDs, string confirmitProjectID);
        public DeleteRespondentsAsyncArrayOfInt32StringDelegate DeleteRespondentsAsyncArrayOfInt32String;

        int IManagementService.DeleteRespondentsAsync(int[] respIDs, string confirmitProjectID)
        {


            if (DeleteRespondentsAsyncArrayOfInt32String != null)
            {
                return DeleteRespondentsAsyncArrayOfInt32String(respIDs, confirmitProjectID);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).DeleteRespondentsAsync(respIDs, confirmitProjectID);
            }

            return default(int);
        }

        public delegate void UpdateSurveyReplicationSchemeStringArrayOfTableInfoDelegate(string projectId, TableInfo[] tables);
        public UpdateSurveyReplicationSchemeStringArrayOfTableInfoDelegate UpdateSurveyReplicationSchemeStringArrayOfTableInfo;

        void IManagementService.UpdateSurveyReplicationScheme(string projectId, TableInfo[] tables)
        {

            if (UpdateSurveyReplicationSchemeStringArrayOfTableInfo != null)
            {
                UpdateSurveyReplicationSchemeStringArrayOfTableInfo(projectId, tables);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).UpdateSurveyReplicationScheme(projectId, tables);
            }
        }

        public delegate void UpdateSurveyReplicationStatusStringBooleanDelegate(string projectId, bool isReplicationEnabled);
        public UpdateSurveyReplicationStatusStringBooleanDelegate UpdateSurveyReplicationStatusStringBoolean;

        void IManagementService.UpdateSurveyReplicationStatus(string projectId, bool isReplicationEnabled)
        {

            if (UpdateSurveyReplicationStatusStringBoolean != null)
            {
                UpdateSurveyReplicationStatusStringBoolean(projectId, isReplicationEnabled);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).UpdateSurveyReplicationStatus(projectId, isReplicationEnabled);
            }
        }

        public delegate void SaveInterviewHistoryAndControlDataInterviewHistoryDataInterviewControlDataDelegate(InterviewHistoryData historyData, InterviewControlData controlData);
        public SaveInterviewHistoryAndControlDataInterviewHistoryDataInterviewControlDataDelegate SaveInterviewHistoryAndControlDataInterviewHistoryDataInterviewControlData;

        void IManagementService.SaveInterviewHistoryAndControlData(InterviewHistoryData historyData, InterviewControlData controlData)
        {

            if (SaveInterviewHistoryAndControlDataInterviewHistoryDataInterviewControlData != null)
            {
                SaveInterviewHistoryAndControlDataInterviewHistoryDataInterviewControlData(historyData, controlData);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).SaveInterviewHistoryAndControlData(historyData, controlData);
            }
        }

        public delegate int AddSampleGetStateInt32StringOutDelegate(int batchId, out string stateDescription);
        public AddSampleGetStateInt32StringOutDelegate AddSampleGetStateInt32StringOut;

        int IManagementService.AddSampleGetState(int batchId, out string stateDescription)
        {
            stateDescription = default(string);


            if (AddSampleGetStateInt32StringOut != null)
            {
                return AddSampleGetStateInt32StringOut(batchId, out stateDescription);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).AddSampleGetState(batchId, out stateDescription);
            }

            return default(int);
        }

        public delegate int ProcessSampleGetStateInt32Int32StringOutDelegate(int batchId, int sampleMode, out string stateDescription);
        public ProcessSampleGetStateInt32Int32StringOutDelegate ProcessSampleGetStateInt32Int32StringOut;

        int IManagementService.ProcessSampleGetState(int batchId, int sampleMode, out string stateDescription)
        {
            stateDescription = default(string);


            if (ProcessSampleGetStateInt32Int32StringOut != null)
            {
                return ProcessSampleGetStateInt32Int32StringOut(batchId, sampleMode, out stateDescription);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).ProcessSampleGetState(batchId, sampleMode, out stateDescription);
            }

            return default(int);
        }

        public delegate void OnQuotaChangedStringInt32Delegate(string cfProjectId, int cfQuotaId);
        public OnQuotaChangedStringInt32Delegate OnQuotaChangedStringInt32;

        void IManagementService.OnQuotaChanged(string cfProjectId, int cfQuotaId)
        {

            if (OnQuotaChangedStringInt32 != null)
            {
                OnQuotaChangedStringInt32(cfProjectId, cfQuotaId);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).OnQuotaChanged(cfProjectId, cfQuotaId);
            }
        }

        public delegate void OnQuotaCellsChangedStringInt32ArrayOfInt32ArrayOfInt32ArrayOfInt32Delegate(string cfProjectId, int cfQuotaId, int[] openedCfCellIds, int[] closedCfCellIds, int[] optimisticallyClosedCfCellIds);
        public OnQuotaCellsChangedStringInt32ArrayOfInt32ArrayOfInt32ArrayOfInt32Delegate OnQuotaCellsChangedStringInt32ArrayOfInt32ArrayOfInt32ArrayOfInt32;

        void IManagementService.OnQuotaCellsChanged(string cfProjectId, int cfQuotaId, int[] openedCfCellIds, int[] closedCfCellIds, int[] optimisticallyClosedCfCellIds)
        {

            if (OnQuotaCellsChangedStringInt32ArrayOfInt32ArrayOfInt32ArrayOfInt32 != null)
            {
                OnQuotaCellsChangedStringInt32ArrayOfInt32ArrayOfInt32ArrayOfInt32(cfProjectId, cfQuotaId, openedCfCellIds, closedCfCellIds, optimisticallyClosedCfCellIds);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).OnQuotaCellsChanged(cfProjectId, cfQuotaId, openedCfCellIds, closedCfCellIds, optimisticallyClosedCfCellIds);
            }
        }

        public delegate void OnQuotaCellsStateChangedStringInt32ListOfCatiQuotaCellCountersStateDelegate(string projectId, int quotaId, List<CatiQuotaCellCountersState> quotaCellsCountersList);
        public OnQuotaCellsStateChangedStringInt32ListOfCatiQuotaCellCountersStateDelegate OnQuotaCellsStateChangedStringInt32ListOfCatiQuotaCellCountersState;

        void IManagementService.OnQuotaCellsStateChanged(string projectId, int quotaId, List<CatiQuotaCellCountersState> quotaCellsCountersList)
        {

            if (OnQuotaCellsStateChangedStringInt32ListOfCatiQuotaCellCountersState != null)
            {
                OnQuotaCellsStateChangedStringInt32ListOfCatiQuotaCellCountersState(projectId, quotaId, quotaCellsCountersList);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).OnQuotaCellsStateChanged(projectId, quotaId, quotaCellsCountersList);
            }
        }

        public delegate void OnQuotasCellsStatesChangedStringListOfCatiQuotaCellsCountersStatesDelegate(string projectId, List<CatiQuotaCellsCountersStates> quotasCellsCountersStates);
        public OnQuotasCellsStatesChangedStringListOfCatiQuotaCellsCountersStatesDelegate OnQuotasCellsStatesChangedStringListOfCatiQuotaCellsCountersStates;

        void IManagementService.OnQuotasCellsStatesChanged(string projectId, List<CatiQuotaCellsCountersStates> quotasCellsCountersStates)
        {

            if (OnQuotasCellsStatesChangedStringListOfCatiQuotaCellsCountersStates != null)
            {
                OnQuotasCellsStatesChangedStringListOfCatiQuotaCellsCountersStates(projectId, quotasCellsCountersStates);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).OnQuotasCellsStatesChanged(projectId, quotasCellsCountersStates);
            }
        }

        public delegate string GetCATIInterviewerNameInt32Delegate(int catiInterviewerId);
        public GetCATIInterviewerNameInt32Delegate GetCATIInterviewerNameInt32;

        string IManagementService.GetCATIInterviewerName(int catiInterviewerId)
        {


            if (GetCATIInterviewerNameInt32 != null)
            {
                return GetCATIInterviewerNameInt32(catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetCATIInterviewerName(catiInterviewerId);
            }

            return default(string);
        }

        public delegate string GetCatiInterviewerDisplayNameInt32Delegate(int catiInterviewerId);
        public GetCatiInterviewerDisplayNameInt32Delegate GetCatiInterviewerDisplayNameInt32;

        string IManagementService.GetCatiInterviewerDisplayName(int catiInterviewerId)
        {


            if (GetCatiInterviewerDisplayNameInt32 != null)
            {
                return GetCatiInterviewerDisplayNameInt32(catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetCatiInterviewerDisplayName(catiInterviewerId);
            }

            return default(string);
        }

        public delegate string GetCATIStationIdInt32Delegate(int catiInterviewerId);
        public GetCATIStationIdInt32Delegate GetCATIStationIdInt32;

        string IManagementService.GetCATIStationId(int catiInterviewerId)
        {


            if (GetCATIStationIdInt32 != null)
            {
                return GetCATIStationIdInt32(catiInterviewerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetCATIStationId(catiInterviewerId);
            }

            return default(string);
        }

        public delegate DateTime? GetCATIAppointmentTimeStringInt32Delegate(string projectId, int respondentId);
        public GetCATIAppointmentTimeStringInt32Delegate GetCATIAppointmentTimeStringInt32;

        DateTime? IManagementService.GetCATIAppointmentTime(string projectId, int respondentId)
        {


            if (GetCATIAppointmentTimeStringInt32 != null)
            {
                return GetCATIAppointmentTimeStringInt32(projectId, respondentId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetCATIAppointmentTime(projectId, respondentId);
            }

            return default(DateTime?);
        }

        public delegate CatiDialingAttempt[] GetCatiInterviewDialingAttemptsStringInt32Delegate(string projectId, int respondentId);
        public GetCatiInterviewDialingAttemptsStringInt32Delegate GetCatiInterviewDialingAttemptsStringInt32;

        CatiDialingAttempt[] IManagementService.GetCatiInterviewDialingAttempts(string projectId, int respondentId)
        {


            if (GetCatiInterviewDialingAttemptsStringInt32 != null)
            {
                return GetCatiInterviewDialingAttemptsStringInt32(projectId, respondentId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetCatiInterviewDialingAttempts(projectId, respondentId);
            }

            return default(CatiDialingAttempt[]);
        }

        public delegate void StopRecordingStringInt32StringDelegate(string projectId, int respondentId, string stopRecordingMode);
        public StopRecordingStringInt32StringDelegate StopRecordingStringInt32String;

        void IManagementService.StopRecording(string projectId, int respondentId, string stopRecordingMode)
        {

            if (StopRecordingStringInt32String != null)
            {
                StopRecordingStringInt32String(projectId, respondentId, stopRecordingMode);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).StopRecording(projectId, respondentId, stopRecordingMode);
            }
        }

        public delegate void StartRecordingStringInt32StringDelegate(string projectId, int respondentId, string label);
        public StartRecordingStringInt32StringDelegate StartRecordingStringInt32String;

        void IManagementService.StartRecording(string projectId, int respondentId, string label)
        {

            if (StartRecordingStringInt32String != null)
            {
                StartRecordingStringInt32String(projectId, respondentId, label);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).StartRecording(projectId, respondentId, label);
            }
        }

        public delegate void EnableLiveMonitoringStringInt32Delegate(string projectId, int catiInterviewerId);
        public EnableLiveMonitoringStringInt32Delegate EnableLiveMonitoringStringInt32;

        void IManagementService.EnableLiveMonitoring(string projectId, int catiInterviewerId)
        {

            if (EnableLiveMonitoringStringInt32 != null)
            {
                EnableLiveMonitoringStringInt32(projectId, catiInterviewerId);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).EnableLiveMonitoring(projectId, catiInterviewerId);
            }
        }

        public delegate int GetDialingModeStringInt32Delegate(string projectId, int respondentId);
        public GetDialingModeStringInt32Delegate GetDialingModeStringInt32;

        int IManagementService.GetDialingMode(string projectId, int respondentId)
        {


            if (GetDialingModeStringInt32 != null)
            {
                return GetDialingModeStringInt32(projectId, respondentId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetDialingMode(projectId, respondentId);
            }

            return default(int);
        }

        public delegate int GetExtendedStatusStringInt32Delegate(string projectId, int respondentId);
        public GetExtendedStatusStringInt32Delegate GetExtendedStatusStringInt32;

        int IManagementService.GetExtendedStatus(string projectId, int respondentId)
        {


            if (GetExtendedStatusStringInt32 != null)
            {
                return GetExtendedStatusStringInt32(projectId, respondentId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetExtendedStatus(projectId, respondentId);
            }

            return default(int);
        }

        public delegate void TransferToIvrStringInt32StringIEnumerableOfKeyValuePairOfStringStringDelegate(string projectId, int respondentId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes);
        public TransferToIvrStringInt32StringIEnumerableOfKeyValuePairOfStringStringDelegate TransferToIvrStringInt32StringIEnumerableOfKeyValuePairOfStringString;

        void IManagementService.TransferToIvr(string projectId, int respondentId, string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {

            if (TransferToIvrStringInt32StringIEnumerableOfKeyValuePairOfStringString != null)
            {
                TransferToIvrStringInt32StringIEnumerableOfKeyValuePairOfStringString(projectId, respondentId, endpoint, attributes);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).TransferToIvr(projectId, respondentId, endpoint, attributes);
            }
        }

        public delegate void AddToCATIBlacklistStringStringInt32Delegate(string telephoneNumber, string projectId, int respondentId);
        public AddToCATIBlacklistStringStringInt32Delegate AddToCATIBlacklistStringStringInt32;

        void IManagementService.AddToCATIBlacklist(string telephoneNumber, string projectId, int respondentId)
        {

            if (AddToCATIBlacklistStringStringInt32 != null)
            {
                AddToCATIBlacklistStringStringInt32(telephoneNumber, projectId, respondentId);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).AddToCATIBlacklist(telephoneNumber, projectId, respondentId);
            }
        }

        public delegate string BackupSurveyToArchiveStringDelegate(string projectId);
        public BackupSurveyToArchiveStringDelegate BackupSurveyToArchiveString;

        string IManagementService.BackupSurveyToArchive(string projectId)
        {


            if (BackupSurveyToArchiveString != null)
            {
                return BackupSurveyToArchiveString(projectId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).BackupSurveyToArchive(projectId);
            }

            return default(string);
        }

        public delegate int BeginRestoreSurveyFromArchiveStringStringDelegate(string projectId, string data);
        public BeginRestoreSurveyFromArchiveStringStringDelegate BeginRestoreSurveyFromArchiveStringString;

        int IManagementService.BeginRestoreSurveyFromArchive(string projectId, string data)
        {


            if (BeginRestoreSurveyFromArchiveStringString != null)
            {
                return BeginRestoreSurveyFromArchiveStringString(projectId, data);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).BeginRestoreSurveyFromArchive(projectId, data);
            }

            return default(int);
        }

        public delegate AsyncOperationInfo GetAsyncOperationInfoInt32Delegate(int operationId);
        public GetAsyncOperationInfoInt32Delegate GetAsyncOperationInfoInt32;

        AsyncOperationInfo IManagementService.GetAsyncOperationInfo(int operationId)
        {


            if (GetAsyncOperationInfoInt32 != null)
            {
                return GetAsyncOperationInfoInt32(operationId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetAsyncOperationInfo(operationId);
            }

            return default(AsyncOperationInfo);
        }

        public delegate string[] GetSurveyCallCentersStringStringDelegate(string projectId, string supervisorName);
        public GetSurveyCallCentersStringStringDelegate GetSurveyCallCentersStringString;

        string[] IManagementService.GetSurveyCallCenters(string projectId, string supervisorName)
        {


            if (GetSurveyCallCentersStringString != null)
            {
                return GetSurveyCallCentersStringString(projectId, supervisorName);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetSurveyCallCenters(projectId, supervisorName);
            }

            return default(string[]);
        }

        public delegate int LaunchSurveyStringLaunchSurveyParametersDelegate(string projectId, LaunchSurveyParameters parameters);
        public LaunchSurveyStringLaunchSurveyParametersDelegate LaunchSurveyStringLaunchSurveyParameters;

        int IManagementService.LaunchSurvey(string projectId, LaunchSurveyParameters parameters)
        {


            if (LaunchSurveyStringLaunchSurveyParameters != null)
            {
                return LaunchSurveyStringLaunchSurveyParameters(projectId, parameters);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).LaunchSurvey(projectId, parameters);
            }

            return default(int);
        }

        public delegate string GetVersionDelegate();
        public GetVersionDelegate GetVersion;

        string IManagementService.GetVersion()
        {


            if (GetVersion != null)
            {
                return GetVersion();
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetVersion();
            }

            return default(string);
        }

        public delegate Task UpdateActiveQuestionStringInt32StringDelegate(string projectId, int catiInterviewerId, string qId);
        public UpdateActiveQuestionStringInt32StringDelegate UpdateActiveQuestionStringInt32String;

        Task IManagementService.UpdateActiveQuestion(string projectId, int catiInterviewerId, string qId)
        {


            if (UpdateActiveQuestionStringInt32String != null)
            {
                return UpdateActiveQuestionStringInt32String(projectId, catiInterviewerId, qId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).UpdateActiveQuestion(projectId, catiInterviewerId, qId);
            }

            return default(Task);
        }

        public delegate void ScheduleInterviewSchedulingScriptExecutionParametersDelegate(SchedulingScriptExecutionParameters parameters);
        public ScheduleInterviewSchedulingScriptExecutionParametersDelegate ScheduleInterviewSchedulingScriptExecutionParameters;

        void IManagementService.ScheduleInterview(SchedulingScriptExecutionParameters parameters)
        {

            if (ScheduleInterviewSchedulingScriptExecutionParameters != null)
            {
                ScheduleInterviewSchedulingScriptExecutionParameters(parameters);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).ScheduleInterview(parameters);
            }
        }

        public delegate bool IsTimeInShiftStringInt32DateTimeDelegate(string projectId, int timezoneId, DateTime dateTime);
        public IsTimeInShiftStringInt32DateTimeDelegate IsTimeInShiftStringInt32DateTime;

        bool IManagementService.IsTimeInShift(string projectId, int timezoneId, DateTime dateTime)
        {


            if (IsTimeInShiftStringInt32DateTime != null)
            {
                return IsTimeInShiftStringInt32DateTime(projectId, timezoneId, dateTime);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).IsTimeInShift(projectId, timezoneId, dateTime);
            }

            return default(bool);
        }

        public delegate TimeInShift[] AreTimesInShiftStringInt32ArrayOfDateTimeDelegate(string projectId, int timezoneId, DateTime[] dateTimes);
        public AreTimesInShiftStringInt32ArrayOfDateTimeDelegate AreTimesInShiftStringInt32ArrayOfDateTime;

        TimeInShift[] IManagementService.AreTimesInShift(string projectId, int timezoneId, DateTime[] dateTimes)
        {


            if (AreTimesInShiftStringInt32ArrayOfDateTime != null)
            {
                return AreTimesInShiftStringInt32ArrayOfDateTime(projectId, timezoneId, dateTimes);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).AreTimesInShift(projectId, timezoneId, dateTimes);
            }

            return default(TimeInShift[]);
        }

        public delegate bool IsCatiGroupMemberInt32StringDelegate(int catiInterviewerId, string groupName);
        public IsCatiGroupMemberInt32StringDelegate IsCatiGroupMemberInt32String;

        bool IManagementService.IsCatiGroupMember(int catiInterviewerId, string groupName)
        {


            if (IsCatiGroupMemberInt32String != null)
            {
                return IsCatiGroupMemberInt32String(catiInterviewerId, groupName);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).IsCatiGroupMember(catiInterviewerId, groupName);
            }

            return default(bool);
        }

        public delegate void SaveAlternativeNumberInt32Int32StringDelegate(int surveyId, int interviewId, string newPhoneNumber);
        public SaveAlternativeNumberInt32Int32StringDelegate SaveAlternativeNumberInt32Int32String;

        void IManagementService.SaveAlternativeNumber(int surveyId, int interviewId, string newPhoneNumber)
        {

            if (SaveAlternativeNumberInt32Int32String != null)
            {
                SaveAlternativeNumberInt32Int32String(surveyId, interviewId, newPhoneNumber);
            } else if (_inner != null)
            {
                ((IManagementService)_inner).SaveAlternativeNumber(surveyId, interviewId, newPhoneNumber);
            }
        }

        public delegate string GetInterviewVariableValueStringInt32StringDelegate(string projectId, int interviewId, string variableName);
        public GetInterviewVariableValueStringInt32StringDelegate GetInterviewVariableValueStringInt32String;

        string IManagementService.GetInterviewVariableValue(string projectId, int interviewId, string variableName)
        {


            if (GetInterviewVariableValueStringInt32String != null)
            {
                return GetInterviewVariableValueStringInt32String(projectId, interviewId, variableName);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).GetInterviewVariableValue(projectId, interviewId, variableName);
            }

            return default(string);
        }

        public delegate int Telephony_LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringStringDelegate(int dialerId, long campaignId, string agentId, string agentName, AgentType agentType, string agentExtension, string userId, bool isPredictive, bool isLocal, IEnumerable<KeyValuePair<string, string>> agentAttributes);
        public Telephony_LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringStringDelegate Telephony_LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString;

        int IManagementService.Telephony_Login(int dialerId, long campaignId, string agentId, string agentName, AgentType agentType, string agentExtension, string userId, bool isPredictive, bool isLocal, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {


            if (Telephony_LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString != null)
            {
                return Telephony_LoginInt32Int64StringStringAgentTypeStringStringBooleanBooleanIEnumerableOfKeyValuePairOfStringString(dialerId, campaignId, agentId, agentName, agentType, agentExtension, userId, isPredictive, isLocal, agentAttributes);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_Login(dialerId, campaignId, agentId, agentName, agentType, agentExtension, userId, isPredictive, isLocal, agentAttributes);
            }

            return default(int);
        }

        public delegate int Telephony_SetGroupsInt32Int64StringArrayOfInt32Delegate(int dialerId, long campaignId, string agentId, int[] groups);
        public Telephony_SetGroupsInt32Int64StringArrayOfInt32Delegate Telephony_SetGroupsInt32Int64StringArrayOfInt32;

        int IManagementService.Telephony_SetGroups(int dialerId, long campaignId, string agentId, int[] groups)
        {


            if (Telephony_SetGroupsInt32Int64StringArrayOfInt32 != null)
            {
                return Telephony_SetGroupsInt32Int64StringArrayOfInt32(dialerId, campaignId, agentId, groups);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_SetGroups(dialerId, campaignId, agentId, groups);
            }

            return default(int);
        }

        public delegate int Telephony_LogoutInt32Int64BooleanStringDelegate(int dialerId, long campaignId, bool isPredictive, string agentId);
        public Telephony_LogoutInt32Int64BooleanStringDelegate Telephony_LogoutInt32Int64BooleanString;

        int IManagementService.Telephony_Logout(int dialerId, long campaignId, bool isPredictive, string agentId)
        {


            if (Telephony_LogoutInt32Int64BooleanString != null)
            {
                return Telephony_LogoutInt32Int64BooleanString(dialerId, campaignId, isPredictive, agentId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_Logout(dialerId, campaignId, isPredictive, agentId);
            }

            return default(int);
        }

        public delegate int Telephony_KillAgentInt32Int64StringDelegate(int dialerId, long campaignId, string agentId);
        public Telephony_KillAgentInt32Int64StringDelegate Telephony_KillAgentInt32Int64String;

        int IManagementService.Telephony_KillAgent(int dialerId, long campaignId, string agentId)
        {


            if (Telephony_KillAgentInt32Int64String != null)
            {
                return Telephony_KillAgentInt32Int64String(dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_KillAgent(dialerId, campaignId, agentId);
            }

            return default(int);
        }

        public delegate int Telephony_SetCampaignInt32Int64Int32Delegate(int dialerId, long campaignId, int agentId);
        public Telephony_SetCampaignInt32Int64Int32Delegate Telephony_SetCampaignInt32Int64Int32;

        int IManagementService.Telephony_SetCampaign(int dialerId, long campaignId, int agentId)
        {


            if (Telephony_SetCampaignInt32Int64Int32 != null)
            {
                return Telephony_SetCampaignInt32Int64Int32(dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_SetCampaign(dialerId, campaignId, agentId);
            }

            return default(int);
        }

        public delegate int Telephony_GoReadyInt32Int64StringDelegate(int dialerId, long campaignId, string agentId);
        public Telephony_GoReadyInt32Int64StringDelegate Telephony_GoReadyInt32Int64String;

        int IManagementService.Telephony_GoReady(int dialerId, long campaignId, string agentId)
        {


            if (Telephony_GoReadyInt32Int64String != null)
            {
                return Telephony_GoReadyInt32Int64String(dialerId, campaignId, agentId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_GoReady(dialerId, campaignId, agentId);
            }

            return default(int);
        }

        public delegate int Telephony_GoNotReadyInt32Int64StringStringDelegate(int dialerId, long campaignId, string agentId, string breakName);
        public Telephony_GoNotReadyInt32Int64StringStringDelegate Telephony_GoNotReadyInt32Int64StringString;

        int IManagementService.Telephony_GoNotReady(int dialerId, long campaignId, string agentId, string breakName)
        {


            if (Telephony_GoNotReadyInt32Int64StringString != null)
            {
                return Telephony_GoNotReadyInt32Int64StringString(dialerId, campaignId, agentId, breakName);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_GoNotReady(dialerId, campaignId, agentId, breakName);
            }

            return default(int);
        }

        public delegate int Telephony_SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObjectDelegate(int dialerId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, Object> respondentVariables);
        public Telephony_SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObjectDelegate Telephony_SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject;

        int IManagementService.Telephony_SendNumberToAgent(int dialerId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, Object> respondentVariables)
        {


            if (Telephony_SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject != null)
            {
                return Telephony_SendNumberToAgentInt32Int64StringDialingModeInt32Int32StringBooleanStringDictionaryOfStringObject(dialerId, campaignId, agentId, dialingMode, contactId, callId, phoneNumber, isRecording, callerId, respondentVariables);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_SendNumberToAgent(dialerId, campaignId, agentId, dialingMode, contactId, callId, phoneNumber, isRecording, callerId, respondentVariables);
            }

            return default(int);
        }

        public delegate int Telephony_SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32BooleanDelegate(int dialerId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording);
        public Telephony_SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32BooleanDelegate Telephony_SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32Boolean;

        int IManagementService.Telephony_SendNumberToAgentEx(int dialerId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {


            if (Telephony_SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32Boolean != null)
            {
                return Telephony_SendNumberToAgentExInt32Int64StringDialingModeInt32Int32StringInt32Boolean(dialerId, campaignId, agentId, dialingMode, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_SendNumberToAgentEx(dialerId, campaignId, agentId, dialingMode, contactId, callId, phoneNumber, callAgingTimeout, isRecording);
            }

            return default(int);
        }

        public delegate int Telephony_RedialInt32Int64StringInt32Int32StringBooleanStringDelegate(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerId);
        public Telephony_RedialInt32Int64StringInt32Int32StringBooleanStringDelegate Telephony_RedialInt32Int64StringInt32Int32StringBooleanString;

        int IManagementService.Telephony_Redial(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerId)
        {


            if (Telephony_RedialInt32Int64StringInt32Int32StringBooleanString != null)
            {
                return Telephony_RedialInt32Int64StringInt32Int32StringBooleanString(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording, callerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_Redial(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording, callerId);
            }

            return default(int);
        }

        public delegate int Telephony_HangupInt32Int64StringInt32Int64Delegate(int dialerId, long campaignId, string agentId, int interviewId, long callId);
        public Telephony_HangupInt32Int64StringInt32Int64Delegate Telephony_HangupInt32Int64StringInt32Int64;

        int IManagementService.Telephony_Hangup(int dialerId, long campaignId, string agentId, int interviewId, long callId)
        {


            if (Telephony_HangupInt32Int64StringInt32Int64 != null)
            {
                return Telephony_HangupInt32Int64StringInt32Int64(dialerId, campaignId, agentId, interviewId, callId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_Hangup(dialerId, campaignId, agentId, interviewId, callId);
            }

            return default(int);
        }

        public delegate int Telephony_CompleteCallInt32Int64StringBooleanStringInterviewStatusInt32Int64Delegate(int dialerId, long campaignId, string agentId, bool makeAgentReady, string breakName, InterviewStatus status, int interviewId, long callId);
        public Telephony_CompleteCallInt32Int64StringBooleanStringInterviewStatusInt32Int64Delegate Telephony_CompleteCallInt32Int64StringBooleanStringInterviewStatusInt32Int64;

        int IManagementService.Telephony_CompleteCall(int dialerId, long campaignId, string agentId, bool makeAgentReady, string breakName, InterviewStatus status, int interviewId, long callId)
        {


            if (Telephony_CompleteCallInt32Int64StringBooleanStringInterviewStatusInt32Int64 != null)
            {
                return Telephony_CompleteCallInt32Int64StringBooleanStringInterviewStatusInt32Int64(dialerId, campaignId, agentId, makeAgentReady, breakName, status, interviewId, callId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_CompleteCall(dialerId, campaignId, agentId, makeAgentReady, breakName, status, interviewId, callId);
            }

            return default(int);
        }

        public delegate int Telephony_SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64Delegate(int dialerId, long campaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId);
        public Telephony_SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64Delegate Telephony_SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64;

        int IManagementService.Telephony_SetNextInterview(int dialerId, long campaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {


            if (Telephony_SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64 != null)
            {
                return Telephony_SetNextInterviewInt32Int64StringInterviewStatusInt64Int32Int64(dialerId, campaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_SetNextInterview(dialerId, campaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            }

            return default(int);
        }

        public delegate int Telephony_StopMonitorInt32StringDelegate(int dialerId, string sessionId);
        public Telephony_StopMonitorInt32StringDelegate Telephony_StopMonitorInt32String;

        int IManagementService.Telephony_StopMonitor(int dialerId, string sessionId)
        {


            if (Telephony_StopMonitorInt32String != null)
            {
                return Telephony_StopMonitorInt32String(dialerId, sessionId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_StopMonitor(dialerId, sessionId);
            }

            return default(int);
        }

        public delegate int Telephony_CompletePreviewInt32Int64StringInt32Int32StringBooleanDelegate(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording);
        public Telephony_CompletePreviewInt32Int64StringInt32Int32StringBooleanDelegate Telephony_CompletePreviewInt32Int64StringInt32Int32StringBoolean;

        int IManagementService.Telephony_CompletePreview(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording)
        {


            if (Telephony_CompletePreviewInt32Int64StringInt32Int32StringBoolean != null)
            {
                return Telephony_CompletePreviewInt32Int64StringInt32Int32StringBoolean(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_CompletePreview(dialerId, campaignId, agentId, contactId, callId, phoneNumber, isRecording);
            }

            return default(int);
        }

        public delegate int Telephony_ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptorDelegate(int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor);
        public Telephony_ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptorDelegate Telephony_ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptor;

        int IManagementService.Telephony_ConnectInboundCallToAgent(int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (Telephony_ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptor != null)
            {
                return Telephony_ConnectInboundCallToAgentInt32Int64StringCallInfoAudioMessageDescriptor(dialerId, campaignId, inboundCallId, callInfo, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_ConnectInboundCallToAgent(dialerId, campaignId, inboundCallId, callInfo, audioMessageDescriptor);
            }

            return default(int);
        }

        public delegate int Telephony_DropInboundCallInt32StringAudioMessageDescriptorDelegate(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor);
        public Telephony_DropInboundCallInt32StringAudioMessageDescriptorDelegate Telephony_DropInboundCallInt32StringAudioMessageDescriptor;

        int IManagementService.Telephony_DropInboundCall(int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {


            if (Telephony_DropInboundCallInt32StringAudioMessageDescriptor != null)
            {
                return Telephony_DropInboundCallInt32StringAudioMessageDescriptor(dialerId, inboundCallId, audioMessageDescriptor);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_DropInboundCall(dialerId, inboundCallId, audioMessageDescriptor);
            }

            return default(int);
        }

        public delegate int Telephony_TransferStartInt32Int64StringInt32TransferTypeDelegate(int dialerId, long campaignId, string transferId, int agentId, TransferType transferType);
        public Telephony_TransferStartInt32Int64StringInt32TransferTypeDelegate Telephony_TransferStartInt32Int64StringInt32TransferType;

        int IManagementService.Telephony_TransferStart(int dialerId, long campaignId, string transferId, int agentId, TransferType transferType)
        {


            if (Telephony_TransferStartInt32Int64StringInt32TransferType != null)
            {
                return Telephony_TransferStartInt32Int64StringInt32TransferType(dialerId, campaignId, transferId, agentId, transferType);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_TransferStart(dialerId, campaignId, transferId, agentId, transferType);
            }

            return default(int);
        }

        public delegate int Telephony_TransferSetTargetInt32Int64StringTargetTypeStringBooleanDelegate(int dialerId, long campaignId, string transferId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns);
        public Telephony_TransferSetTargetInt32Int64StringTargetTypeStringBooleanDelegate Telephony_TransferSetTargetInt32Int64StringTargetTypeStringBoolean;

        int IManagementService.Telephony_TransferSetTarget(int dialerId, long campaignId, string transferId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {


            if (Telephony_TransferSetTargetInt32Int64StringTargetTypeStringBoolean != null)
            {
                return Telephony_TransferSetTargetInt32Int64StringTargetTypeStringBoolean(dialerId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_TransferSetTarget(dialerId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);
            }

            return default(int);
        }

        public delegate int Telephony_TransferSetConnectionStateInt32Int64StringConnectionStateDelegate(int dialerId, long campaignId, string transferId, ConnectionState state);
        public Telephony_TransferSetConnectionStateInt32Int64StringConnectionStateDelegate Telephony_TransferSetConnectionStateInt32Int64StringConnectionState;

        int IManagementService.Telephony_TransferSetConnectionState(int dialerId, long campaignId, string transferId, ConnectionState state)
        {


            if (Telephony_TransferSetConnectionStateInt32Int64StringConnectionState != null)
            {
                return Telephony_TransferSetConnectionStateInt32Int64StringConnectionState(dialerId, campaignId, transferId, state);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_TransferSetConnectionState(dialerId, campaignId, transferId, state);
            }

            return default(int);
        }

        public delegate int Telephony_TransferCompleteInt32Int64StringDelegate(int dialerId, long campaignId, string transferId);
        public Telephony_TransferCompleteInt32Int64StringDelegate Telephony_TransferCompleteInt32Int64String;

        int IManagementService.Telephony_TransferComplete(int dialerId, long campaignId, string transferId)
        {


            if (Telephony_TransferCompleteInt32Int64String != null)
            {
                return Telephony_TransferCompleteInt32Int64String(dialerId, campaignId, transferId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_TransferComplete(dialerId, campaignId, transferId);
            }

            return default(int);
        }

        public delegate int Telephony_TransferCancelInt32Int64StringDelegate(int dialerId, long campaignId, string transferId);
        public Telephony_TransferCancelInt32Int64StringDelegate Telephony_TransferCancelInt32Int64String;

        int IManagementService.Telephony_TransferCancel(int dialerId, long campaignId, string transferId)
        {


            if (Telephony_TransferCancelInt32Int64String != null)
            {
                return Telephony_TransferCancelInt32Int64String(dialerId, campaignId, transferId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_TransferCancel(dialerId, campaignId, transferId);
            }

            return default(int);
        }

        public delegate int Telephony_StartPlaybackInt32Int64StringInt32Int32StringInt32OutDelegate(int dialerId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds);
        public Telephony_StartPlaybackInt32Int64StringInt32Int32StringInt32OutDelegate Telephony_StartPlaybackInt32Int64StringInt32Int32StringInt32Out;

        int IManagementService.Telephony_StartPlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds)
        {
            timeOfPlayingInSeconds = default(int);


            if (Telephony_StartPlaybackInt32Int64StringInt32Int32StringInt32Out != null)
            {
                return Telephony_StartPlaybackInt32Int64StringInt32Int32StringInt32Out(dialerId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_StartPlayback(dialerId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlayingInSeconds);
            }

            return default(int);
        }

        public delegate int Telephony_StopPlaybackInt32Int64StringInt32Delegate(int dialerId, long campaignId, string agentId, int callId);
        public Telephony_StopPlaybackInt32Int64StringInt32Delegate Telephony_StopPlaybackInt32Int64StringInt32;

        int IManagementService.Telephony_StopPlayback(int dialerId, long campaignId, string agentId, int callId)
        {


            if (Telephony_StopPlaybackInt32Int64StringInt32 != null)
            {
                return Telephony_StopPlaybackInt32Int64StringInt32(dialerId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_StopPlayback(dialerId, campaignId, agentId, callId);
            }

            return default(int);
        }

        public delegate int Telephony_PauseOrResumePlaybackInt32Int64StringInt32Delegate(int dialerId, long campaignId, string agentId, int callId);
        public Telephony_PauseOrResumePlaybackInt32Int64StringInt32Delegate Telephony_PauseOrResumePlaybackInt32Int64StringInt32;

        int IManagementService.Telephony_PauseOrResumePlayback(int dialerId, long campaignId, string agentId, int callId)
        {


            if (Telephony_PauseOrResumePlaybackInt32Int64StringInt32 != null)
            {
                return Telephony_PauseOrResumePlaybackInt32Int64StringInt32(dialerId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_PauseOrResumePlayback(dialerId, campaignId, agentId, callId);
            }

            return default(int);
        }

        public delegate int Telephony_ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Delegate(int dialerId, long campaignId, string agentId, int callId);
        public Telephony_ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32Delegate Telephony_ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32;

        int IManagementService.Telephony_ToggleInterviewerListensToPlaybackOrRespondent(int dialerId, long campaignId, string agentId, int callId)
        {


            if (Telephony_ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32 != null)
            {
                return Telephony_ToggleInterviewerListensToPlaybackOrRespondentInt32Int64StringInt32(dialerId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_ToggleInterviewerListensToPlaybackOrRespondent(dialerId, campaignId, agentId, callId);
            }

            return default(int);
        }

        public delegate bool Telephony_IsPersonModeSupportedInt32AgentTaskChoiceModeDelegate(int dialerId, AgentTaskChoiceMode mode);
        public Telephony_IsPersonModeSupportedInt32AgentTaskChoiceModeDelegate Telephony_IsPersonModeSupportedInt32AgentTaskChoiceMode;

        bool IManagementService.Telephony_IsPersonModeSupported(int dialerId, AgentTaskChoiceMode mode)
        {


            if (Telephony_IsPersonModeSupportedInt32AgentTaskChoiceMode != null)
            {
                return Telephony_IsPersonModeSupportedInt32AgentTaskChoiceMode(dialerId, mode);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_IsPersonModeSupported(dialerId, mode);
            }

            return default(bool);
        }

        public delegate bool Telephony_IsReloginNeededOnSurveyChangeInt32Delegate(int dialerId);
        public Telephony_IsReloginNeededOnSurveyChangeInt32Delegate Telephony_IsReloginNeededOnSurveyChangeInt32;

        bool IManagementService.Telephony_IsReloginNeededOnSurveyChange(int dialerId)
        {


            if (Telephony_IsReloginNeededOnSurveyChangeInt32 != null)
            {
                return Telephony_IsReloginNeededOnSurveyChangeInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_IsReloginNeededOnSurveyChange(dialerId);
            }

            return default(bool);
        }

        public delegate bool Telephony_IsPauseOrResumePlaybackSupportedInt32Delegate(int dialerId);
        public Telephony_IsPauseOrResumePlaybackSupportedInt32Delegate Telephony_IsPauseOrResumePlaybackSupportedInt32;

        bool IManagementService.Telephony_IsPauseOrResumePlaybackSupported(int dialerId)
        {


            if (Telephony_IsPauseOrResumePlaybackSupportedInt32 != null)
            {
                return Telephony_IsPauseOrResumePlaybackSupportedInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_IsPauseOrResumePlaybackSupported(dialerId);
            }

            return default(bool);
        }

        public delegate bool Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupportedInt32Delegate(int dialerId);
        public Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupportedInt32Delegate Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupportedInt32;

        bool IManagementService.Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupported(int dialerId)
        {


            if (Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupportedInt32 != null)
            {
                return Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupportedInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_IsToggleInterviewerListensToPlaybackOrRespondentSupported(dialerId);
            }

            return default(bool);
        }

        public delegate bool Telephony_IsDynamicExtensionNumberAllowedInt32BooleanDelegate(int dialerId, bool isAgentLocal);
        public Telephony_IsDynamicExtensionNumberAllowedInt32BooleanDelegate Telephony_IsDynamicExtensionNumberAllowedInt32Boolean;

        bool IManagementService.Telephony_IsDynamicExtensionNumberAllowed(int dialerId, bool isAgentLocal)
        {


            if (Telephony_IsDynamicExtensionNumberAllowedInt32Boolean != null)
            {
                return Telephony_IsDynamicExtensionNumberAllowedInt32Boolean(dialerId, isAgentLocal);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_IsDynamicExtensionNumberAllowed(dialerId, isAgentLocal);
            }

            return default(bool);
        }

        public delegate int Telephony_RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOutDelegate(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl);
        public Telephony_RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOutDelegate Telephony_RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOut;

        int IManagementService.Telephony_RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            login = default(string);
            password = default(string);
            host = default(string);
            extension = default(string);
            frontendUrl = default(string);


            if (Telephony_RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOut != null)
            {
                return Telephony_RegisterAgentSoftphoneInt32Int32Int32StringStringOutStringOutStringOutStringOutStringOut(companyId, dialerId, agentId, agentName, out login, out password, out host, out extension, out frontendUrl);
            } else if (_inner != null)
            {
                return ((IManagementService)_inner).Telephony_RegisterAgentSoftphone(companyId, dialerId, agentId, agentName, out login, out password, out host, out extension, out frontendUrl);
            }

            return default(int);
        }

    }
}