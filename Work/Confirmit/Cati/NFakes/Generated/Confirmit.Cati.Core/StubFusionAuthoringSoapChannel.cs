using System;
using Confirmit.CATI.Core.AuthoringService;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Confirmit.CATI.Core.AuthoringService.Fakes
{
    public class StubFusionAuthoringSoapChannel : FusionAuthoringSoapChannel 
    {
        private FusionAuthoringSoapChannel _inner;

        public StubFusionAuthoringSoapChannel()
        {
            _inner = null;
        }

        public FusionAuthoringSoapChannel Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CatiSupervisorInfo GetCatiSupervisorInfoStringStringDelegate(string key, string xConfirmitApiKey);
        public GetCatiSupervisorInfoStringStringDelegate GetCatiSupervisorInfoStringString;

        CatiSupervisorInfo FusionAuthoringSoap.GetCatiSupervisorInfo(string key, string xConfirmitApiKey)
        {


            if (GetCatiSupervisorInfoStringString != null)
            {
                return GetCatiSupervisorInfoStringString(key, xConfirmitApiKey);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetCatiSupervisorInfo(key, xConfirmitApiKey);
            }

            return default(CatiSupervisorInfo);
        }

        public delegate string GetCatiSupervisorNameStringStringDelegate(string key, string xConfirmitApiKey);
        public GetCatiSupervisorNameStringStringDelegate GetCatiSupervisorNameStringString;

        string FusionAuthoringSoap.GetCatiSupervisorName(string key, string xConfirmitApiKey)
        {


            if (GetCatiSupervisorNameStringString != null)
            {
                return GetCatiSupervisorNameStringString(key, xConfirmitApiKey);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetCatiSupervisorName(key, xConfirmitApiKey);
            }

            return default(string);
        }

        public delegate CatiIdentityValidationResult ValidateCatiIdentityStringStringStringStringBooleanDelegate(string key, string confirmitCookieData, string catiUserName, string catiClientKey, bool isCatiHttps);
        public ValidateCatiIdentityStringStringStringStringBooleanDelegate ValidateCatiIdentityStringStringStringStringBoolean;

        CatiIdentityValidationResult FusionAuthoringSoap.ValidateCatiIdentity(string key, string confirmitCookieData, string catiUserName, string catiClientKey, bool isCatiHttps)
        {


            if (ValidateCatiIdentityStringStringStringStringBoolean != null)
            {
                return ValidateCatiIdentityStringStringStringStringBoolean(key, confirmitCookieData, catiUserName, catiClientKey, isCatiHttps);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).ValidateCatiIdentity(key, confirmitCookieData, catiUserName, catiClientKey, isCatiHttps);
            }

            return default(CatiIdentityValidationResult);
        }

        public delegate SurveySchema GetQuestionnaireStringStringBooleanDelegate(string key, string projectId, bool projectSpecific);
        public GetQuestionnaireStringStringBooleanDelegate GetQuestionnaireStringStringBoolean;

        SurveySchema FusionAuthoringSoap.GetQuestionnaire(string key, string projectId, bool projectSpecific)
        {


            if (GetQuestionnaireStringStringBoolean != null)
            {
                return GetQuestionnaireStringStringBoolean(key, projectId, projectSpecific);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetQuestionnaire(key, projectId, projectSpecific);
            }

            return default(SurveySchema);
        }

        public delegate bool HasCatiAddonStringInt32Delegate(string key, int companyId);
        public HasCatiAddonStringInt32Delegate HasCatiAddonStringInt32;

        bool FusionAuthoringSoap.HasCatiAddon(string key, int companyId)
        {


            if (HasCatiAddonStringInt32 != null)
            {
                return HasCatiAddonStringInt32(key, companyId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).HasCatiAddon(key, companyId);
            }

            return default(bool);
        }

        public delegate bool HasDedicatedIPAddonStringInt32Delegate(string key, int companyId);
        public HasDedicatedIPAddonStringInt32Delegate HasDedicatedIPAddonStringInt32;

        bool FusionAuthoringSoap.HasDedicatedIPAddon(string key, int companyId)
        {


            if (HasDedicatedIPAddonStringInt32 != null)
            {
                return HasDedicatedIPAddonStringInt32(key, companyId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).HasDedicatedIPAddon(key, companyId);
            }

            return default(bool);
        }

        public delegate string GetCapiInterviewerNameStringInt32Delegate(string key, int capiInterviewerId);
        public GetCapiInterviewerNameStringInt32Delegate GetCapiInterviewerNameStringInt32;

        string FusionAuthoringSoap.GetCapiInterviewerName(string key, int capiInterviewerId)
        {


            if (GetCapiInterviewerNameStringInt32 != null)
            {
                return GetCapiInterviewerNameStringInt32(key, capiInterviewerId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetCapiInterviewerName(key, capiInterviewerId);
            }

            return default(string);
        }

        public delegate bool IsTelephonyEnabledStringInt32Delegate(string key, int companyId);
        public IsTelephonyEnabledStringInt32Delegate IsTelephonyEnabledStringInt32;

        bool FusionAuthoringSoap.IsTelephonyEnabled(string key, int companyId)
        {


            if (IsTelephonyEnabledStringInt32 != null)
            {
                return IsTelephonyEnabledStringInt32(key, companyId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).IsTelephonyEnabled(key, companyId);
            }

            return default(bool);
        }

        public delegate int GetMaximumCatiInterviewersStringInt32Delegate(string key, int companyId);
        public GetMaximumCatiInterviewersStringInt32Delegate GetMaximumCatiInterviewersStringInt32;

        int FusionAuthoringSoap.GetMaximumCatiInterviewers(string key, int companyId)
        {


            if (GetMaximumCatiInterviewersStringInt32 != null)
            {
                return GetMaximumCatiInterviewersStringInt32(key, companyId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetMaximumCatiInterviewers(key, companyId);
            }

            return default(int);
        }

        public delegate InterviewHistoryEntry[] GetInterviewHistoryWithValidBackToStringStringStringInt32StringDelegate(string key, string projectId, string respondentIdentity, int languageId, string domainOverride);
        public GetInterviewHistoryWithValidBackToStringStringStringInt32StringDelegate GetInterviewHistoryWithValidBackToStringStringStringInt32String;

        InterviewHistoryEntry[] FusionAuthoringSoap.GetInterviewHistoryWithValidBackTo(string key, string projectId, string respondentIdentity, int languageId, string domainOverride)
        {


            if (GetInterviewHistoryWithValidBackToStringStringStringInt32String != null)
            {
                return GetInterviewHistoryWithValidBackToStringStringStringInt32String(key, projectId, respondentIdentity, languageId, domainOverride);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetInterviewHistoryWithValidBackTo(key, projectId, respondentIdentity, languageId, domainOverride);
            }

            return default(InterviewHistoryEntry[]);
        }

        public delegate Language[] GetSurveyLanguagesStringStringDelegate(string key, string projectId);
        public GetSurveyLanguagesStringStringDelegate GetSurveyLanguagesStringString;

        Language[] FusionAuthoringSoap.GetSurveyLanguages(string key, string projectId)
        {


            if (GetSurveyLanguagesStringString != null)
            {
                return GetSurveyLanguagesStringString(key, projectId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetSurveyLanguages(key, projectId);
            }

            return default(Language[]);
        }

        public delegate int GetCatiCompanyIdStringStringDelegate(string key, string catiCompanyIdentifier);
        public GetCatiCompanyIdStringStringDelegate GetCatiCompanyIdStringString;

        int FusionAuthoringSoap.GetCatiCompanyId(string key, string catiCompanyIdentifier)
        {


            if (GetCatiCompanyIdStringString != null)
            {
                return GetCatiCompanyIdStringString(key, catiCompanyIdentifier);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetCatiCompanyId(key, catiCompanyIdentifier);
            }

            return default(int);
        }

        public delegate string[] GetProjectsWithSuperviseCATIProjectPermissionForUserStringStringInt32Delegate(string key, string userId, int companyId);
        public GetProjectsWithSuperviseCATIProjectPermissionForUserStringStringInt32Delegate GetProjectsWithSuperviseCATIProjectPermissionForUserStringStringInt32;

        string[] FusionAuthoringSoap.GetProjectsWithSuperviseCATIProjectPermissionForUser(string key, string userId, int companyId)
        {


            if (GetProjectsWithSuperviseCATIProjectPermissionForUserStringStringInt32 != null)
            {
                return GetProjectsWithSuperviseCATIProjectPermissionForUserStringStringInt32(key, userId, companyId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetProjectsWithSuperviseCATIProjectPermissionForUser(key, userId, companyId);
            }

            return default(string[]);
        }

        public delegate int GetDBVersionStringStringDelegate(string key, string projectId);
        public GetDBVersionStringStringDelegate GetDBVersionStringString;

        int FusionAuthoringSoap.GetDBVersion(string key, string projectId)
        {


            if (GetDBVersionStringString != null)
            {
                return GetDBVersionStringString(key, projectId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetDBVersion(key, projectId);
            }

            return default(int);
        }

        public delegate string[] GetQuotaNamesStringStringQuotaModeDelegate(string key, string projectId, QuotaMode quotaMode);
        public GetQuotaNamesStringStringQuotaModeDelegate GetQuotaNamesStringStringQuotaMode;

        string[] FusionAuthoringSoap.GetQuotaNames(string key, string projectId, QuotaMode quotaMode)
        {


            if (GetQuotaNamesStringStringQuotaMode != null)
            {
                return GetQuotaNamesStringStringQuotaMode(key, projectId, quotaMode);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetQuotaNames(key, projectId, quotaMode);
            }

            return default(string[]);
        }

        public delegate QuotaList GetQuotaListStringStringStringQuotaModeDelegate(string key, string projectId, string quotaName, QuotaMode quotaMode);
        public GetQuotaListStringStringStringQuotaModeDelegate GetQuotaListStringStringStringQuotaMode;

        QuotaList FusionAuthoringSoap.GetQuotaList(string key, string projectId, string quotaName, QuotaMode quotaMode)
        {


            if (GetQuotaListStringStringStringQuotaMode != null)
            {
                return GetQuotaListStringStringStringQuotaMode(key, projectId, quotaName, quotaMode);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetQuotaList(key, projectId, quotaName, quotaMode);
            }

            return default(QuotaList);
        }

        public delegate FormBase[] GetQuotaFormsStringStringStringDelegate(string key, string projectId, string quotaName);
        public GetQuotaFormsStringStringStringDelegate GetQuotaFormsStringStringString;

        FormBase[] FusionAuthoringSoap.GetQuotaForms(string key, string projectId, string quotaName)
        {


            if (GetQuotaFormsStringStringString != null)
            {
                return GetQuotaFormsStringStringString(key, projectId, quotaName);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetQuotaForms(key, projectId, quotaName);
            }

            return default(FormBase[]);
        }

        public delegate void UpdateQuotaListStringStringStringQuotaListDatabaseTypeDelegate(string key, string projectId, string quotaName, QuotaList quotaList, DatabaseType databaseType);
        public UpdateQuotaListStringStringStringQuotaListDatabaseTypeDelegate UpdateQuotaListStringStringStringQuotaListDatabaseType;

        void FusionAuthoringSoap.UpdateQuotaList(string key, string projectId, string quotaName, QuotaList quotaList, DatabaseType databaseType)
        {

            if (UpdateQuotaListStringStringStringQuotaListDatabaseType != null)
            {
                UpdateQuotaListStringStringStringQuotaListDatabaseType(key, projectId, quotaName, quotaList, databaseType);
            } else if (_inner != null)
            {
                ((FusionAuthoringSoap)_inner).UpdateQuotaList(key, projectId, quotaName, quotaList, databaseType);
            }
        }

        public delegate void SynchronizeQuotaStringStringStringDatabaseTypeDelegate(string key, string projectId, string quotaName, DatabaseType databaseType);
        public SynchronizeQuotaStringStringStringDatabaseTypeDelegate SynchronizeQuotaStringStringStringDatabaseType;

        void FusionAuthoringSoap.SynchronizeQuota(string key, string projectId, string quotaName, DatabaseType databaseType)
        {

            if (SynchronizeQuotaStringStringStringDatabaseType != null)
            {
                SynchronizeQuotaStringStringStringDatabaseType(key, projectId, quotaName, databaseType);
            } else if (_inner != null)
            {
                ((FusionAuthoringSoap)_inner).SynchronizeQuota(key, projectId, quotaName, databaseType);
            }
        }

        public delegate Tabs GetTabPermissionsStringStringInt32Delegate(string key, string loginName, int companyId);
        public GetTabPermissionsStringStringInt32Delegate GetTabPermissionsStringStringInt32;

        Tabs FusionAuthoringSoap.GetTabPermissions(string key, string loginName, int companyId)
        {


            if (GetTabPermissionsStringStringInt32 != null)
            {
                return GetTabPermissionsStringStringInt32(key, loginName, companyId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetTabPermissions(key, loginName, companyId);
            }

            return default(Tabs);
        }

        public delegate SendMailResponse SendMailSendMailRequestDelegate(SendMailRequest request);
        public SendMailSendMailRequestDelegate SendMailSendMailRequest;

        SendMailResponse FusionAuthoringSoap.SendMail(SendMailRequest request)
        {


            if (SendMailSendMailRequest != null)
            {
                return SendMailSendMailRequest(request);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).SendMail(request);
            }

            return default(SendMailResponse);
        }

        public delegate SendMailHtmlResponse SendMailHtmlSendMailHtmlRequestDelegate(SendMailHtmlRequest request);
        public SendMailHtmlSendMailHtmlRequestDelegate SendMailHtmlSendMailHtmlRequest;

        SendMailHtmlResponse FusionAuthoringSoap.SendMailHtml(SendMailHtmlRequest request)
        {


            if (SendMailHtmlSendMailHtmlRequest != null)
            {
                return SendMailHtmlSendMailHtmlRequest(request);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).SendMailHtml(request);
            }

            return default(SendMailHtmlResponse);
        }

        public delegate FormBase[] GetFormInfosStringStringArrayOfStringSchemaSourceTypeDelegate(string key, string projectId, string[] formNames, SchemaSourceType schemaSourceType);
        public GetFormInfosStringStringArrayOfStringSchemaSourceTypeDelegate GetFormInfosStringStringArrayOfStringSchemaSourceType;

        FormBase[] FusionAuthoringSoap.GetFormInfos(string key, string projectId, string[] formNames, SchemaSourceType schemaSourceType)
        {


            if (GetFormInfosStringStringArrayOfStringSchemaSourceType != null)
            {
                return GetFormInfosStringStringArrayOfStringSchemaSourceType(key, projectId, formNames, schemaSourceType);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetFormInfos(key, projectId, formNames, schemaSourceType);
            }

            return default(FormBase[]);
        }

        public delegate FormBase[] GetFormInfosWithTextsStringStringArrayOfStringSchemaSourceTypeDelegate(string key, string projectId, string[] formNames, SchemaSourceType schemaSourceType);
        public GetFormInfosWithTextsStringStringArrayOfStringSchemaSourceTypeDelegate GetFormInfosWithTextsStringStringArrayOfStringSchemaSourceType;

        FormBase[] FusionAuthoringSoap.GetFormInfosWithTexts(string key, string projectId, string[] formNames, SchemaSourceType schemaSourceType)
        {


            if (GetFormInfosWithTextsStringStringArrayOfStringSchemaSourceType != null)
            {
                return GetFormInfosWithTextsStringStringArrayOfStringSchemaSourceType(key, projectId, formNames, schemaSourceType);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetFormInfosWithTexts(key, projectId, formNames, schemaSourceType);
            }

            return default(FormBase[]);
        }

        public delegate CatiSupervisor[] GetCompanyCatiSupervisorsStringInt32Delegate(string key, int companyId);
        public GetCompanyCatiSupervisorsStringInt32Delegate GetCompanyCatiSupervisorsStringInt32;

        CatiSupervisor[] FusionAuthoringSoap.GetCompanyCatiSupervisors(string key, int companyId)
        {


            if (GetCompanyCatiSupervisorsStringInt32 != null)
            {
                return GetCompanyCatiSupervisorsStringInt32(key, companyId);
            } else if (_inner != null)
            {
                return ((FusionAuthoringSoap)_inner).GetCompanyCatiSupervisors(key, companyId);
            }

            return default(CatiSupervisor[]);
        }

        public delegate void DisplayInitializationUIDelegate();
        public DisplayInitializationUIDelegate DisplayInitializationUI;

        void IClientChannel.DisplayInitializationUI()
        {

            if (DisplayInitializationUI != null)
            {
                DisplayInitializationUI();
            } else if (_inner != null)
            {
                ((IClientChannel)_inner).DisplayInitializationUI();
            }
        }

        public delegate IAsyncResult BeginDisplayInitializationUIAsyncCallbackObjectDelegate(AsyncCallback callback, Object state);
        public BeginDisplayInitializationUIAsyncCallbackObjectDelegate BeginDisplayInitializationUIAsyncCallbackObject;

        IAsyncResult IClientChannel.BeginDisplayInitializationUI(AsyncCallback callback, Object state)
        {


            if (BeginDisplayInitializationUIAsyncCallbackObject != null)
            {
                return BeginDisplayInitializationUIAsyncCallbackObject(callback, state);
            } else if (_inner != null)
            {
                return ((IClientChannel)_inner).BeginDisplayInitializationUI(callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate void EndDisplayInitializationUIIAsyncResultDelegate(IAsyncResult result);
        public EndDisplayInitializationUIIAsyncResultDelegate EndDisplayInitializationUIIAsyncResult;

        void IClientChannel.EndDisplayInitializationUI(IAsyncResult result)
        {

            if (EndDisplayInitializationUIIAsyncResult != null)
            {
                EndDisplayInitializationUIIAsyncResult(result);
            } else if (_inner != null)
            {
                ((IClientChannel)_inner).EndDisplayInitializationUI(result);
            }
        }

        T IChannel.GetProperty<T>()
        {


            return default(T);
        }

        public delegate void AbortDelegate();
        public AbortDelegate Abort;

        void ICommunicationObject.Abort()
        {

            if (Abort != null)
            {
                Abort();
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Abort();
            }
        }

        public delegate IAsyncResult BeginCloseAsyncCallbackObjectDelegate(AsyncCallback callback, Object state);
        public BeginCloseAsyncCallbackObjectDelegate BeginCloseAsyncCallbackObject;

        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, Object state)
        {


            if (BeginCloseAsyncCallbackObject != null)
            {
                return BeginCloseAsyncCallbackObject(callback, state);
            } else if (_inner != null)
            {
                return ((ICommunicationObject)_inner).BeginClose(callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate IAsyncResult BeginCloseTimeSpanAsyncCallbackObjectDelegate(TimeSpan timeout, AsyncCallback callback, Object state);
        public BeginCloseTimeSpanAsyncCallbackObjectDelegate BeginCloseTimeSpanAsyncCallbackObject;

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, Object state)
        {


            if (BeginCloseTimeSpanAsyncCallbackObject != null)
            {
                return BeginCloseTimeSpanAsyncCallbackObject(timeout, callback, state);
            } else if (_inner != null)
            {
                return ((ICommunicationObject)_inner).BeginClose(timeout, callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate void EndCloseIAsyncResultDelegate(IAsyncResult result);
        public EndCloseIAsyncResultDelegate EndCloseIAsyncResult;

        void ICommunicationObject.EndClose(IAsyncResult result)
        {

            if (EndCloseIAsyncResult != null)
            {
                EndCloseIAsyncResult(result);
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).EndClose(result);
            }
        }

        public delegate void OpenDelegate();
        public OpenDelegate Open;

        void ICommunicationObject.Open()
        {

            if (Open != null)
            {
                Open();
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Open();
            }
        }

        public delegate void OpenTimeSpanDelegate(TimeSpan timeout);
        public OpenTimeSpanDelegate OpenTimeSpan;

        void ICommunicationObject.Open(TimeSpan timeout)
        {

            if (OpenTimeSpan != null)
            {
                OpenTimeSpan(timeout);
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Open(timeout);
            }
        }

        public delegate IAsyncResult BeginOpenAsyncCallbackObjectDelegate(AsyncCallback callback, Object state);
        public BeginOpenAsyncCallbackObjectDelegate BeginOpenAsyncCallbackObject;

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, Object state)
        {


            if (BeginOpenAsyncCallbackObject != null)
            {
                return BeginOpenAsyncCallbackObject(callback, state);
            } else if (_inner != null)
            {
                return ((ICommunicationObject)_inner).BeginOpen(callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate IAsyncResult BeginOpenTimeSpanAsyncCallbackObjectDelegate(TimeSpan timeout, AsyncCallback callback, Object state);
        public BeginOpenTimeSpanAsyncCallbackObjectDelegate BeginOpenTimeSpanAsyncCallbackObject;

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, Object state)
        {


            if (BeginOpenTimeSpanAsyncCallbackObject != null)
            {
                return BeginOpenTimeSpanAsyncCallbackObject(timeout, callback, state);
            } else if (_inner != null)
            {
                return ((ICommunicationObject)_inner).BeginOpen(timeout, callback, state);
            }

            return default(IAsyncResult);
        }

        public delegate void EndOpenIAsyncResultDelegate(IAsyncResult result);
        public EndOpenIAsyncResultDelegate EndOpenIAsyncResult;

        void ICommunicationObject.EndOpen(IAsyncResult result)
        {

            if (EndOpenIAsyncResult != null)
            {
                EndOpenIAsyncResult(result);
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).EndOpen(result);
            }
        }

        public delegate void CloseDelegate();
        public CloseDelegate Close;

        void ICommunicationObject.Close()
        {

            if (Close != null)
            {
                Close();
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Close();
            }
        }

        public delegate void CloseTimeSpanDelegate(TimeSpan timeout);
        public CloseTimeSpanDelegate CloseTimeSpan;

        void ICommunicationObject.Close(TimeSpan timeout)
        {

            if (CloseTimeSpan != null)
            {
                CloseTimeSpan(timeout);
            } else if (_inner != null)
            {
                ((ICommunicationObject)_inner).Close(timeout);
            }
        }

        public delegate void DisposeDelegate();
        public DisposeDelegate Dispose;

        void IDisposable.Dispose()
        {

            if (Dispose != null)
            {
                Dispose();
            } else if (_inner != null)
            {
                ((IDisposable)_inner).Dispose();
            }
        }

        private bool _AllowInitializationUI;
        public Func<bool> AllowInitializationUIGet;
        public Action<bool> AllowInitializationUISetBoolean;

        bool IClientChannel.AllowInitializationUI
        {
            get
            {
                if (AllowInitializationUIGet != null)
                {
                    return AllowInitializationUIGet();
                } else if (_inner != null)
                {
                    return ((IClientChannel)_inner).AllowInitializationUI;
                }

                if (AllowInitializationUISetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AllowInitializationUI;
                }

                return default(bool);
            }

            set
            {
                if (AllowInitializationUISetBoolean != null)
                {
                    AllowInitializationUISetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IClientChannel)_inner).AllowInitializationUI = value;
                    return;
                }

                if (AllowInitializationUIGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AllowInitializationUI = value;
                }

            }
        }

        private bool _DidInteractiveInitialization;
        public Func<bool> DidInteractiveInitializationGet;
        public Action<bool> DidInteractiveInitializationSetBoolean;

        bool IClientChannel.DidInteractiveInitialization
        {
            get
            {
                if (DidInteractiveInitializationGet != null)
                {
                    return DidInteractiveInitializationGet();
                } else if (_inner != null)
                {
                    return ((IClientChannel)_inner).DidInteractiveInitialization;
                }

                if (DidInteractiveInitializationSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DidInteractiveInitialization;
                }

                return default(bool);
            }

        }

        private Uri _Via;
        public Func<Uri> ViaGet;
        public Action<Uri> ViaSetUri;

        Uri IClientChannel.Via
        {
            get
            {
                if (ViaGet != null)
                {
                    return ViaGet();
                } else if (_inner != null)
                {
                    return ((IClientChannel)_inner).Via;
                }

                if (ViaSetUri == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Via;
                }

                return default(Uri);
            }

        }

        private bool _AllowOutputBatching;
        public Func<bool> AllowOutputBatchingGet;
        public Action<bool> AllowOutputBatchingSetBoolean;

        bool IContextChannel.AllowOutputBatching
        {
            get
            {
                if (AllowOutputBatchingGet != null)
                {
                    return AllowOutputBatchingGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).AllowOutputBatching;
                }

                if (AllowOutputBatchingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AllowOutputBatching;
                }

                return default(bool);
            }

            set
            {
                if (AllowOutputBatchingSetBoolean != null)
                {
                    AllowOutputBatchingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IContextChannel)_inner).AllowOutputBatching = value;
                    return;
                }

                if (AllowOutputBatchingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AllowOutputBatching = value;
                }

            }
        }

        private IInputSession _InputSession;
        public Func<IInputSession> InputSessionGet;
        public Action<IInputSession> InputSessionSetIInputSession;

        IInputSession IContextChannel.InputSession
        {
            get
            {
                if (InputSessionGet != null)
                {
                    return InputSessionGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).InputSession;
                }

                if (InputSessionSetIInputSession == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InputSession;
                }

                return default(IInputSession);
            }

        }

        private EndpointAddress _LocalAddress;
        public Func<EndpointAddress> LocalAddressGet;
        public Action<EndpointAddress> LocalAddressSetEndpointAddress;

        EndpointAddress IContextChannel.LocalAddress
        {
            get
            {
                if (LocalAddressGet != null)
                {
                    return LocalAddressGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).LocalAddress;
                }

                if (LocalAddressSetEndpointAddress == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LocalAddress;
                }

                return default(EndpointAddress);
            }

        }

        private TimeSpan _OperationTimeout;
        public Func<TimeSpan> OperationTimeoutGet;
        public Action<TimeSpan> OperationTimeoutSetTimeSpan;

        TimeSpan IContextChannel.OperationTimeout
        {
            get
            {
                if (OperationTimeoutGet != null)
                {
                    return OperationTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).OperationTimeout;
                }

                if (OperationTimeoutSetTimeSpan == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OperationTimeout;
                }

                return default(TimeSpan);
            }

            set
            {
                if (OperationTimeoutSetTimeSpan != null)
                {
                    OperationTimeoutSetTimeSpan(value);
                    return;
                } else if (_inner != null)
                {
                    ((IContextChannel)_inner).OperationTimeout = value;
                    return;
                }

                if (OperationTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _OperationTimeout = value;
                }

            }
        }

        private IOutputSession _OutputSession;
        public Func<IOutputSession> OutputSessionGet;
        public Action<IOutputSession> OutputSessionSetIOutputSession;

        IOutputSession IContextChannel.OutputSession
        {
            get
            {
                if (OutputSessionGet != null)
                {
                    return OutputSessionGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).OutputSession;
                }

                if (OutputSessionSetIOutputSession == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _OutputSession;
                }

                return default(IOutputSession);
            }

        }

        private EndpointAddress _RemoteAddress;
        public Func<EndpointAddress> RemoteAddressGet;
        public Action<EndpointAddress> RemoteAddressSetEndpointAddress;

        EndpointAddress IContextChannel.RemoteAddress
        {
            get
            {
                if (RemoteAddressGet != null)
                {
                    return RemoteAddressGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).RemoteAddress;
                }

                if (RemoteAddressSetEndpointAddress == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RemoteAddress;
                }

                return default(EndpointAddress);
            }

        }

        private string _SessionId;
        public Func<string> SessionIdGet;
        public Action<string> SessionIdSetString;

        string IContextChannel.SessionId
        {
            get
            {
                if (SessionIdGet != null)
                {
                    return SessionIdGet();
                } else if (_inner != null)
                {
                    return ((IContextChannel)_inner).SessionId;
                }

                if (SessionIdSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SessionId;
                }

                return default(string);
            }

        }

        private CommunicationState _State;
        public Func<CommunicationState> StateGet;
        public Action<CommunicationState> StateSetCommunicationState;

        CommunicationState ICommunicationObject.State
        {
            get
            {
                if (StateGet != null)
                {
                    return StateGet();
                } else if (_inner != null)
                {
                    return ((ICommunicationObject)_inner).State;
                }

                if (StateSetCommunicationState == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _State;
                }

                return default(CommunicationState);
            }

        }

        private IExtensionCollection<IContextChannel> _Extensions;
        public Func<IExtensionCollection<IContextChannel>> ExtensionsGet;
        public Action<IExtensionCollection<IContextChannel>> ExtensionsSetIExtensionCollectionOfIContextChannel;

        IExtensionCollection<IContextChannel> IExtensibleObject<IContextChannel>.Extensions
        {
            get
            {
                if (ExtensionsGet != null)
                {
                    return ExtensionsGet();
                } else if (_inner != null)
                {
                    return ((IExtensibleObject<IContextChannel>)_inner).Extensions;
                }

                if (ExtensionsSetIExtensionCollectionOfIContextChannel == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Extensions;
                }

                return default(IExtensionCollection<IContextChannel>);
            }

        }

        public event EventHandler<UnknownMessageReceivedEventArgs> UnknownMessageReceived;
        public void OnUnknownMessageReceived(UnknownMessageReceivedEventArgs args)
        {
            if (UnknownMessageReceived != null)
            {
                UnknownMessageReceived(this, args);
            }
        }

        public event EventHandler Closed;
        public void OnClosed(EventArgs args)
        {
            if (Closed != null)
            {
                Closed(this, args);
            }
        }

        public event EventHandler Closing;
        public void OnClosing(EventArgs args)
        {
            if (Closing != null)
            {
                Closing(this, args);
            }
        }

        public event EventHandler Faulted;
        public void OnFaulted(EventArgs args)
        {
            if (Faulted != null)
            {
                Faulted(this, args);
            }
        }

        public event EventHandler Opened;
        public void OnOpened(EventArgs args)
        {
            if (Opened != null)
            {
                Opened(this, args);
            }
        }

        public event EventHandler Opening;
        public void OnOpening(EventArgs args)
        {
            if (Opening != null)
            {
                Opening(this, args);
            }
        }

    }
}