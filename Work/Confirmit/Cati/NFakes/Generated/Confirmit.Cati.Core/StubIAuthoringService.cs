using System;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.AuthoringService;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.WcfServices.Clients.Fakes
{
    public class StubIAuthoringService : IAuthoringService 
    {
        private IAuthoringService _inner;

        public StubIAuthoringService()
        {
            _inner = null;
        }

        public IAuthoringService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int GetCatiCompanyIdStringDelegate(string companyAlias);
        public GetCatiCompanyIdStringDelegate GetCatiCompanyIdString;

        int IAuthoringService.GetCatiCompanyId(string companyAlias)
        {


            if (GetCatiCompanyIdString != null)
            {
                return GetCatiCompanyIdString(companyAlias);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetCatiCompanyId(companyAlias);
            }

            return default(int);
        }

        public delegate CatiSupervisorInfo GetCatiSupervisorInfoStringDelegate(string xConfirmitApiKey);
        public GetCatiSupervisorInfoStringDelegate GetCatiSupervisorInfoString;

        CatiSupervisorInfo IAuthoringService.GetCatiSupervisorInfo(string xConfirmitApiKey)
        {


            if (GetCatiSupervisorInfoString != null)
            {
                return GetCatiSupervisorInfoString(xConfirmitApiKey);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetCatiSupervisorInfo(xConfirmitApiKey);
            }

            return default(CatiSupervisorInfo);
        }

        public delegate IEnumerable<CatiSupervisor> GetCompanyCatiSupervisorsNamesInt32Delegate(int companyId);
        public GetCompanyCatiSupervisorsNamesInt32Delegate GetCompanyCatiSupervisorsNamesInt32;

        IEnumerable<CatiSupervisor> IAuthoringService.GetCompanyCatiSupervisorsNames(int companyId)
        {


            if (GetCompanyCatiSupervisorsNamesInt32 != null)
            {
                return GetCompanyCatiSupervisorsNamesInt32(companyId);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetCompanyCatiSupervisorsNames(companyId);
            }

            return default(IEnumerable<CatiSupervisor>);
        }

        public delegate int GetDBVersionStringDelegate(string projectId);
        public GetDBVersionStringDelegate GetDBVersionString;

        int IAuthoringService.GetDBVersion(string projectId)
        {


            if (GetDBVersionString != null)
            {
                return GetDBVersionString(projectId);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetDBVersion(projectId);
            }

            return default(int);
        }

        public delegate FormBase[] GetFormInfosStringIEnumerableOfStringSchemaSourceTypeDelegate(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType);
        public GetFormInfosStringIEnumerableOfStringSchemaSourceTypeDelegate GetFormInfosStringIEnumerableOfStringSchemaSourceType;

        FormBase[] IAuthoringService.GetFormInfos(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType)
        {


            if (GetFormInfosStringIEnumerableOfStringSchemaSourceType != null)
            {
                return GetFormInfosStringIEnumerableOfStringSchemaSourceType(projectId, formNames, schemaSourceType);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetFormInfos(projectId, formNames, schemaSourceType);
            }

            return default(FormBase[]);
        }

        public delegate FormBase[] GetFormInfosWithTextStringIEnumerableOfStringSchemaSourceTypeDelegate(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType);
        public GetFormInfosWithTextStringIEnumerableOfStringSchemaSourceTypeDelegate GetFormInfosWithTextStringIEnumerableOfStringSchemaSourceType;

        FormBase[] IAuthoringService.GetFormInfosWithText(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType)
        {


            if (GetFormInfosWithTextStringIEnumerableOfStringSchemaSourceType != null)
            {
                return GetFormInfosWithTextStringIEnumerableOfStringSchemaSourceType(projectId, formNames, schemaSourceType);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetFormInfosWithText(projectId, formNames, schemaSourceType);
            }

            return default(FormBase[]);
        }

        public delegate InterviewHistoryEntry[] GetInterviewHistoryWithValidBackToStringStringInt32StringDelegate(string projectId, string respondentIdentity, int languageId, string domainOverride);
        public GetInterviewHistoryWithValidBackToStringStringInt32StringDelegate GetInterviewHistoryWithValidBackToStringStringInt32String;

        InterviewHistoryEntry[] IAuthoringService.GetInterviewHistoryWithValidBackTo(string projectId, string respondentIdentity, int languageId, string domainOverride)
        {


            if (GetInterviewHistoryWithValidBackToStringStringInt32String != null)
            {
                return GetInterviewHistoryWithValidBackToStringStringInt32String(projectId, respondentIdentity, languageId, domainOverride);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetInterviewHistoryWithValidBackTo(projectId, respondentIdentity, languageId, domainOverride);
            }

            return default(InterviewHistoryEntry[]);
        }

        public delegate int GetMaximumCatiInterviewersInt32Delegate(int companyId);
        public GetMaximumCatiInterviewersInt32Delegate GetMaximumCatiInterviewersInt32;

        int IAuthoringService.GetMaximumCatiInterviewers(int companyId)
        {


            if (GetMaximumCatiInterviewersInt32 != null)
            {
                return GetMaximumCatiInterviewersInt32(companyId);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetMaximumCatiInterviewers(companyId);
            }

            return default(int);
        }

        public delegate string[] GetProjectsWithSuperviseCATIProjectPermissionForUserStringInt32Delegate(string userName, int companyId);
        public GetProjectsWithSuperviseCATIProjectPermissionForUserStringInt32Delegate GetProjectsWithSuperviseCATIProjectPermissionForUserStringInt32;

        string[] IAuthoringService.GetProjectsWithSuperviseCATIProjectPermissionForUser(string userName, int companyId)
        {


            if (GetProjectsWithSuperviseCATIProjectPermissionForUserStringInt32 != null)
            {
                return GetProjectsWithSuperviseCATIProjectPermissionForUserStringInt32(userName, companyId);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetProjectsWithSuperviseCATIProjectPermissionForUser(userName, companyId);
            }

            return default(string[]);
        }

        public delegate SurveySchema GetQuestionnaireStringBooleanDelegate(string projectId, bool projectSpecific);
        public GetQuestionnaireStringBooleanDelegate GetQuestionnaireStringBoolean;

        SurveySchema IAuthoringService.GetQuestionnaire(string projectId, bool projectSpecific)
        {


            if (GetQuestionnaireStringBoolean != null)
            {
                return GetQuestionnaireStringBoolean(projectId, projectSpecific);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetQuestionnaire(projectId, projectSpecific);
            }

            return default(SurveySchema);
        }

        public delegate FormBase[] GetQuotaFormsStringStringDelegate(string projectId, string quotaName);
        public GetQuotaFormsStringStringDelegate GetQuotaFormsStringString;

        FormBase[] IAuthoringService.GetQuotaForms(string projectId, string quotaName)
        {


            if (GetQuotaFormsStringString != null)
            {
                return GetQuotaFormsStringString(projectId, quotaName);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetQuotaForms(projectId, quotaName);
            }

            return default(FormBase[]);
        }

        public delegate QuotaList GetQuotaListStringStringQuotaModeDelegate(string projectId, string quotaName, QuotaMode quotaMode);
        public GetQuotaListStringStringQuotaModeDelegate GetQuotaListStringStringQuotaMode;

        QuotaList IAuthoringService.GetQuotaList(string projectId, string quotaName, QuotaMode quotaMode)
        {


            if (GetQuotaListStringStringQuotaMode != null)
            {
                return GetQuotaListStringStringQuotaMode(projectId, quotaName, quotaMode);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetQuotaList(projectId, quotaName, quotaMode);
            }

            return default(QuotaList);
        }

        public delegate string[] GetQuotaNamesStringQuotaModeDelegate(string projectId, QuotaMode quotaMode);
        public GetQuotaNamesStringQuotaModeDelegate GetQuotaNamesStringQuotaMode;

        string[] IAuthoringService.GetQuotaNames(string projectId, QuotaMode quotaMode)
        {


            if (GetQuotaNamesStringQuotaMode != null)
            {
                return GetQuotaNamesStringQuotaMode(projectId, quotaMode);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetQuotaNames(projectId, quotaMode);
            }

            return default(string[]);
        }

        public delegate Language[] GetSurveyLanguagesStringDelegate(string projectId);
        public GetSurveyLanguagesStringDelegate GetSurveyLanguagesString;

        Language[] IAuthoringService.GetSurveyLanguages(string projectId)
        {


            if (GetSurveyLanguagesString != null)
            {
                return GetSurveyLanguagesString(projectId);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetSurveyLanguages(projectId);
            }

            return default(Language[]);
        }

        public delegate Tabs GetTabPermissionsStringInt32Delegate(string loginName, int companyId);
        public GetTabPermissionsStringInt32Delegate GetTabPermissionsStringInt32;

        Tabs IAuthoringService.GetTabPermissions(string loginName, int companyId)
        {


            if (GetTabPermissionsStringInt32 != null)
            {
                return GetTabPermissionsStringInt32(loginName, companyId);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetTabPermissions(loginName, companyId);
            }

            return default(Tabs);
        }

        public delegate Tabs GetTabPermissionsStringInt32StringDelegate(string loginName, int companyId, string clientKey);
        public GetTabPermissionsStringInt32StringDelegate GetTabPermissionsStringInt32String;

        Tabs IAuthoringService.GetTabPermissions(string loginName, int companyId, string clientKey)
        {


            if (GetTabPermissionsStringInt32String != null)
            {
                return GetTabPermissionsStringInt32String(loginName, companyId, clientKey);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).GetTabPermissions(loginName, companyId, clientKey);
            }

            return default(Tabs);
        }

        public delegate bool HasCatiAddonInt32Delegate(int companyId);
        public HasCatiAddonInt32Delegate HasCatiAddonInt32;

        bool IAuthoringService.HasCatiAddon(int companyId)
        {


            if (HasCatiAddonInt32 != null)
            {
                return HasCatiAddonInt32(companyId);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).HasCatiAddon(companyId);
            }

            return default(bool);
        }

        public delegate bool IsCompanyTelephonyEnabledInt32Delegate(int companyId);
        public IsCompanyTelephonyEnabledInt32Delegate IsCompanyTelephonyEnabledInt32;

        bool IAuthoringService.IsCompanyTelephonyEnabled(int companyId)
        {


            if (IsCompanyTelephonyEnabledInt32 != null)
            {
                return IsCompanyTelephonyEnabledInt32(companyId);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).IsCompanyTelephonyEnabled(companyId);
            }

            return default(bool);
        }

        public delegate void SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteStringDelegate(string[] addressesTo, string addressBcc, string messageSubject, string messageBody, string messageBodyHtml, byte[] attachment, string attachmentName);
        public SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteStringDelegate SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteString;

        void IAuthoringService.SendMailHtml(string[] addressesTo, string addressBcc, string messageSubject, string messageBody, string messageBodyHtml, byte[] attachment, string attachmentName)
        {

            if (SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteString != null)
            {
                SendMailHtmlArrayOfStringStringStringStringStringArrayOfByteString(addressesTo, addressBcc, messageSubject, messageBody, messageBodyHtml, attachment, attachmentName);
            } else if (_inner != null)
            {
                ((IAuthoringService)_inner).SendMailHtml(addressesTo, addressBcc, messageSubject, messageBody, messageBodyHtml, attachment, attachmentName);
            }
        }

        public delegate void SynchronizeQuotaStringStringDatabaseTypeDelegate(string projectId, string quotaName, DatabaseType databaseType);
        public SynchronizeQuotaStringStringDatabaseTypeDelegate SynchronizeQuotaStringStringDatabaseType;

        void IAuthoringService.SynchronizeQuota(string projectId, string quotaName, DatabaseType databaseType)
        {

            if (SynchronizeQuotaStringStringDatabaseType != null)
            {
                SynchronizeQuotaStringStringDatabaseType(projectId, quotaName, databaseType);
            } else if (_inner != null)
            {
                ((IAuthoringService)_inner).SynchronizeQuota(projectId, quotaName, databaseType);
            }
        }

        public delegate void UpdateQuotaListStringStringQuotaListDatabaseTypeDelegate(string projectId, string quotaName, QuotaList quotaList, DatabaseType databaseType);
        public UpdateQuotaListStringStringQuotaListDatabaseTypeDelegate UpdateQuotaListStringStringQuotaListDatabaseType;

        void IAuthoringService.UpdateQuotaList(string projectId, string quotaName, QuotaList quotaList, DatabaseType databaseType)
        {

            if (UpdateQuotaListStringStringQuotaListDatabaseType != null)
            {
                UpdateQuotaListStringStringQuotaListDatabaseType(projectId, quotaName, quotaList, databaseType);
            } else if (_inner != null)
            {
                ((IAuthoringService)_inner).UpdateQuotaList(projectId, quotaName, quotaList, databaseType);
            }
        }

        public delegate CatiIdentityValidationResult ValidateCatiIdentityStringStringStringBooleanDelegate(string confirmitCookieData, string catiUserName, string catiClientKey, bool isCatiHttps);
        public ValidateCatiIdentityStringStringStringBooleanDelegate ValidateCatiIdentityStringStringStringBoolean;

        CatiIdentityValidationResult IAuthoringService.ValidateCatiIdentity(string confirmitCookieData, string catiUserName, string catiClientKey, bool isCatiHttps)
        {


            if (ValidateCatiIdentityStringStringStringBoolean != null)
            {
                return ValidateCatiIdentityStringStringStringBoolean(confirmitCookieData, catiUserName, catiClientKey, isCatiHttps);
            } else if (_inner != null)
            {
                return ((IAuthoringService)_inner).ValidateCatiIdentity(confirmitCookieData, catiUserName, catiClientKey, isCatiHttps);
            }

            return default(CatiIdentityValidationResult);
        }

    }
}