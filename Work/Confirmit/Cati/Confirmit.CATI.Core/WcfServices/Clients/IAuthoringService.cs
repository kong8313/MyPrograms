using System.Collections.Generic;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Core.WcfServices.Clients
{
    public interface IAuthoringService
    {
        int GetCatiCompanyId(string companyAlias);
        CatiSupervisorInfo GetCatiSupervisorInfo(string xConfirmitApiKey);
        IEnumerable<CatiSupervisor> GetCompanyCatiSupervisorsNames(int companyId);
        int GetDBVersion(string projectId);
        FormBase[] GetFormInfos(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType);
        FormBase[] GetFormInfosWithText(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType);
        InterviewHistoryEntry[] GetInterviewHistoryWithValidBackTo(string projectId, string respondentIdentity, int languageId, string domainOverride = null);
        int GetMaximumCatiInterviewers(int companyId);
        string[] GetProjectsWithSuperviseCATIProjectPermissionForUser(string userName, int companyId);
        SurveySchema GetQuestionnaire(string projectId, bool projectSpecific);
        FormBase[] GetQuotaForms(string projectId, string quotaName);
        QuotaList GetQuotaList(string projectId, string quotaName, QuotaMode quotaMode);
        string[] GetQuotaNames(string projectId, QuotaMode quotaMode);
        Language[] GetSurveyLanguages(string projectId);
        Tabs GetTabPermissions(string loginName, int companyId);
        Tabs GetTabPermissions(string loginName, int companyId, string clientKey);
        bool HasCatiAddon(int companyId);
        bool IsCompanyTelephonyEnabled(int companyId);
        void SendMailHtml(string[] addressesTo, string addressBcc, string messageSubject, string messageBody, string messageBodyHtml, byte[] attachment, string attachmentName);
        void SynchronizeQuota(string projectId, string quotaName, DatabaseType databaseType);
        void UpdateQuotaList(string projectId, string quotaName, QuotaList quotaList, DatabaseType databaseType);
        CatiIdentityValidationResult ValidateCatiIdentity(string confirmitCookieData, string catiUserName, string catiClientKey, bool isCatiHttps);
    }
}