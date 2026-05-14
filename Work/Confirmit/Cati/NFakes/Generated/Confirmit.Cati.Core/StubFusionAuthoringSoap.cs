using System;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Core.AuthoringService.Fakes
{
    public class StubFusionAuthoringSoap : FusionAuthoringSoap 
    {
        private FusionAuthoringSoap _inner;

        public StubFusionAuthoringSoap()
        {
            _inner = null;
        }

        public FusionAuthoringSoap Inner
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

    }
}