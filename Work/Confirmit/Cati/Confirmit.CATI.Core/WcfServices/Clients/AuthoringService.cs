using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.ConfirmitClientKey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Core.WcfServices.Clients
{
    /// <summary>
    /// Client wrapper for Confirmit internal authoring service.
    /// </summary>
    public class AuthoringService : IAuthoringService
    {
        private readonly IConfirmitClientKeyProvider _confirmitClientKeyProvider;
        private readonly IWebServiceUrlSettings _webServiceUrlSettings;

        private volatile FusionAuthoringSoapClient _authoringService;
        public AuthoringService(
            IConfirmitClientKeyProvider confirmitClientKeyProvider,
            IWebServiceUrlSettings webServiceUrlSettings)
        {
            _confirmitClientKeyProvider = confirmitClientKeyProvider;
            _webServiceUrlSettings = webServiceUrlSettings;

            InitializeClient();
        }

        private void InitializeClient()
        {
            HttpBindingBase binding;
            CustomBinding customBinding;
            
            var url = BootstrapConfig.IsContainerEnvironment 
                ? "http://internal-soap-14-api/confirmit/internalwebservices/14.0/FusionAuthoring.asmx" 
                : _webServiceUrlSettings.Authoring;
            
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                binding = new BasicHttpsBinding
                {
                    MaxReceivedMessageSize = 16777216,
                    ReaderQuotas = { MaxArrayLength = 2147483647, MaxStringContentLength = 5242880, MaxDepth = 128 }
                };

                customBinding = new CustomBinding(binding);
                customBinding.Elements.Find<HttpsTransportBindingElement>().KeepAliveEnabled = false;
            }
            else
            {
                binding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = 16777216,
                    ReaderQuotas = { MaxArrayLength = 2147483647, MaxStringContentLength = 5242880, MaxDepth = 128 }
                };

                customBinding = new CustomBinding(binding);
                customBinding.Elements.Find<HttpTransportBindingElement>().KeepAliveEnabled = false;
            }
            
            _authoringService = new FusionAuthoringSoapClient(customBinding, new EndpointAddress(url));
        }
        
        /// <summary>
        /// Releases the service client.
        /// </summary>
        private void ReinitializeClient()
        {
            var authoringService = _authoringService;

            InitializeClient();

            if (authoringService != null)
            {
                authoringService.Abort();
            }
        }

        private void DoServiceCall(Action<FusionAuthoringSoap> action, string methodWithArguments)
        {
            DoServiceCall(action.WrapInFunc<FusionAuthoringSoap, bool>(), methodWithArguments);
        }

        private T DoServiceCall<T>(Func<FusionAuthoringSoap, T> action, string methodWithArguments)
        {
            CheckTransaction(methodWithArguments);

            try
            {
                return action(_authoringService);
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex, methodWithArguments);

                ReinitializeClient();

                throw;
            }
        }

        private void CheckTransaction(string methodName)
        {
            if (DatabaseTransactionScope.Current != null)
            {
                Trace.TraceWarning(
                    "Web service method 'AuthoringService.{0}' is called inside transaction scope '{1}'.",
                    methodName,
                    DatabaseTransactionScope.Current.TransactionName);
            }
        }

        /// <summary>
        /// Get Questionnaire (Routing, PredefinedLists and Quotas).
        /// Note that all information about a survey is retrieved.  If not all
        /// that information is needed, please use a different function that is
        /// suited to retrieve a subset of a survey schema (taking custom PoetReadFilter as the parameter).
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <param name="projectSpecific">True means that the questionnaire should be
        /// linked to the project and can be updated,
        /// False means that the questionnaire should not be linked to the project
        /// and can be used to update other projects</param>
        /// <returns>ObjectStructure containing Questionnaire</returns>
        public SurveySchema GetQuestionnaire(string projectId, bool projectSpecific)
        {
            string methodWithArguments = string.Format(
                "GetQuestionnaire(projectId = {0}, projectSpecific = {1})", projectId, projectSpecific);
            var clientKey = _confirmitClientKeyProvider.Get();
            return DoServiceCall(x => x.GetQuestionnaire(clientKey, projectId, projectSpecific), methodWithArguments);
        }

        /// <summary>
        /// Determines whether company with the specified company id has CATI addon.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns>
        /// 	<c>true</c> if company with the specified company id has CATI addon; otherwise, <c>false</c>.
        /// </returns>
        public bool HasCatiAddon(int companyId)
        {
            string methodWithArguments = string.Format("HasCatiAddon(companyId = {0})", companyId);

            try
            {
                var clientKey = _confirmitClientKeyProvider.Get();
                return DoServiceCall(x => x.HasCatiAddon(clientKey, companyId), methodWithArguments);
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether company with the specified company id has CATI addon.
        /// </summary>
        /// <param name="companyAlias">The company alias.</param>
        /// <returns>
        /// 	<c>true</c> if company with the specified company id has CATI addon; otherwise, <c>false</c>.
        /// </returns>
        public int GetCatiCompanyId(string companyAlias)
        {
            try
            {
                var clientKey = _confirmitClientKeyProvider.Get();
                return _authoringService.GetCatiCompanyId(clientKey, companyAlias);
            }
            catch (Exception ex)
            {
                Trace.TraceError("AuthorizeAndReturnCompanyId(companyAlias = {0}) failed.\n{1}", companyAlias, ex);

                ReinitializeClient();

                throw;
            }
        }

        public Language[] GetSurveyLanguages(string projectId)
        {
            try
            {
                var clientKey = _confirmitClientKeyProvider.Get();
                return _authoringService.GetSurveyLanguages(clientKey, projectId);
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetSurveyLanguages(projectId = {0}) failed.\n{1}", projectId, ex);

                ReinitializeClient();

                throw;
            }
        }

        public InterviewHistoryEntry[] GetInterviewHistoryWithValidBackTo(string projectId, string respondentIdentity, int languageId, string domainOverride = null)
        {
            try
            {
                var clientKey = _confirmitClientKeyProvider.Get();
                return _authoringService.GetInterviewHistoryWithValidBackTo(clientKey, projectId, respondentIdentity, languageId, domainOverride);
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetInterviewHistoryWithValidBackTo(projectId = {0}) failed.\n{1}", projectId, ex);

                ReinitializeClient();

                throw;
            }
        }

        public int GetMaximumCatiInterviewers(int companyId)
        {
            try
            {
                var clientKey = _confirmitClientKeyProvider.Get();
                return _authoringService.GetMaximumCatiInterviewers(clientKey, companyId);
            }
            catch (Exception ex)
            {
                Trace.TraceError("GetMaximumCatiInterviewers(companyId = {0}) failed.\n{1}", companyId, ex);

                ReinitializeClient();

                throw;
            }
        }

        /// <summary>
        /// Sends email message using Confirmit settings
        /// </summary>
        /// <param name="addressesTo">list of addresses of recipients</param>
        /// <param name="addressBcc">address of BCC recipient</param>
        /// <param name="messageSubject">subject of message</param>
        /// <param name="messageBody">body of message</param>
        /// <param name="messageBodyHtml">body of html message</param>
        /// <param name="attachment">byte array attachment </param>
        /// <param name="attachmentName">the attachment name</param>
        public void SendMailHtml(
            string[] addressesTo, string addressBcc, string messageSubject, string messageBody, string messageBodyHtml, byte[] attachment, string attachmentName)
        {
            string methodWithArguments = string.Format(
                "SendMailHtml(addressesTo = {0}, addressBcc = {1}, messageSubject = {2}, messageBody = {3} ), messageBodyHtml = {4}, attachmentSize = {5}, attachmentName= {6}",
                string.Join(",", addressesTo),
                addressBcc,
                messageSubject,
                messageBody,
                messageBodyHtml,
                attachment?.Length ?? 0,
                attachmentName);

            var clientKey = _confirmitClientKeyProvider.Get();
            DoServiceCall(
                x => x.SendMailHtml(new SendMailHtmlRequest(clientKey, BackendInstance.Current.CompanyId, addressesTo, addressBcc, messageSubject, messageBody, messageBodyHtml, attachment, attachmentName)),
                methodWithArguments);
        }

        /// <summary>
        /// Determines whether telephony is enabled for the company with the specified company id.
        /// </summary>
        /// <param name="companyId">The company id.</param>
        /// <returns>
        /// 	<c>true</c> if telephony is enabled for the company with the specified company id; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompanyTelephonyEnabled(int companyId)
        {
            string methodWithArguments = string.Format("IsCompanyTelephonyEnabled(companyId = {0})", companyId);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.IsTelephonyEnabled(clientKey, companyId), methodWithArguments);
        }

        /// <summary>
        /// Gets user tab permissions.
        /// </summary>
        /// <param name="loginName">User login name.</param>
        /// <param name="companyId"></param>
        /// <returns>
        /// User tab permissions.
        /// </returns>
        public Tabs GetTabPermissions(string loginName, int companyId)
        {
            string methodWithArguments = string.Format("GetTabPermissions(loginName = {0}, companyId = {1})", loginName, companyId);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetTabPermissions(clientKey, loginName, companyId), methodWithArguments);
        }

        /// <summary>
        /// Gets user tab permissions.
        /// </summary>
        /// <param name="loginName">User login name.</param>
        /// <param name="companyId"></param>
        /// <param name="clientKey"></param>
        /// <returns>
        /// User tab permissions.
        /// </returns>
        public Tabs GetTabPermissions(string loginName, int companyId, string clientKey)
        {
            string methodWithArguments = string.Format("GetTabPermissions(loginName = {0}, companyId = {1})", loginName, companyId);

            return DoServiceCall(x => x.GetTabPermissions(clientKey, loginName, companyId), methodWithArguments);
        }

        /// <summary>
        /// Gets the projects with supervise CATI project permission for user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <returns>Array of project IDs.</returns>
        public string[] GetProjectsWithSuperviseCATIProjectPermissionForUser(string userName, int companyId)
        {
            string methodWithArguments = string.Format("GetProjectsWithSuperviseCATIProjectPermissionForUser(userName = {0}, companyId = {1})", userName, companyId);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetProjectsWithSuperviseCATIProjectPermissionForUser(clientKey, userName, companyId), methodWithArguments);
        }

        /// <summary>
        /// This method can be used to verify that CF survey database is attached. It attaches it if needed.
        /// </summary>
        /// <param name="projectId">The project id</param>
        /// <returns>Database version</returns>
        public int GetDBVersion(string projectId)
        {
            string methodWithArguments = string.Format("GetDBVersion(projectId = {0})", projectId);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetDBVersion(clientKey, projectId), methodWithArguments);
        }

        /// <summary>
        /// Gets the quota names for a project. 
        /// The user can specify to get the names for either Production, Test, or design (DesignWithProductionCounter and DesignWithTestCounter will both give design name for this method) by the quotaMode property.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="quotaMode">The quota mode.</param>
        public string[] GetQuotaNames(string projectId, QuotaMode quotaMode)
        {
            string methodWithArguments = string.Format("GetQuotaNames(projectId = {0}, quotaMode = {1})", projectId, quotaMode);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetQuotaNames(clientKey, projectId, quotaMode), methodWithArguments);
        }

        /// <summary>
        /// Gets the quota list for a quota. The user can specify to get the quota list for either Production, Test, DesignWithProductionCounter, or DesignWithTestCounter by the quotaMode property.
        /// Note. The QuotaRowId on the QuotaRow object (which is contained by QuotaList) is only set if the quota is synchronized. If not available, it is set to -1.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="quotaName">Name of the quota.</param>
        /// <param name="quotaMode">The quota mode.</param>
        public QuotaList GetQuotaList(string projectId, string quotaName, QuotaMode quotaMode)
        {
            string methodWithArguments = string.Format(
                "GetQuotaList(projectId = {0}, quotaName = {1}, quotaMode = {2})", projectId, quotaName, quotaMode);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetQuotaList(clientKey, projectId, quotaName, quotaMode), methodWithArguments);
        }

        public FormBase[] GetQuotaForms(string projectId, string quotaName)
        {
            string methodWithArguments = string.Format(
                "GetQuotaForms(projectId = {0}, quotaName = {1})", projectId, quotaName);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetQuotaForms(clientKey, projectId, quotaName), methodWithArguments);
        }

        /// <summary>
        /// Updates the quota list in design mode.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="quotaName">Name of the quota.</param>
        /// <param name="quotaList">The quota list to update with.</param>
        /// <param name="databaseType">Type of the database.</param>
        public void UpdateQuotaList(string projectId, string quotaName, QuotaList quotaList, DatabaseType databaseType)
        {
            string methodWithArguments = string.Format(
                "UpdateQuotaList(projectId = {0}, quotaName = {1}, quotaList = {2}, databaseType = {3})", projectId, quotaName, quotaList, databaseType);

            var clientKey = _confirmitClientKeyProvider.Get();

            DoServiceCall(x => x.UpdateQuotaList(clientKey, projectId, quotaName, quotaList, databaseType), methodWithArguments);
        }

        /// <summary>
        /// Synchronizes the quota from design to either test or production.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="quotaName">Name of the quota.</param>
        /// <param name="databaseType">Type of the database.</param>
        public void SynchronizeQuota(string projectId, string quotaName, DatabaseType databaseType)
        {
            string methodWithArguments = string.Format(
                "SynchronizeQuota(projectId = {0}, quotaName = {1}, databaseType = {2})", projectId, quotaName, databaseType);

            var clientKey = _confirmitClientKeyProvider.Get();

            DoServiceCall(x => x.SynchronizeQuota(clientKey, projectId, quotaName, databaseType), methodWithArguments);
        }

        public FormBase[] GetFormInfos(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType)
        {
            var names = formNames.ToArray();
            string methodWithArguments = string.Format(
                "GetFormInfos(projectId = {0}, formNames = {1}, schemaSourceType = {2})",
                projectId,
                string.Join(",", names),
                schemaSourceType);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetFormInfos(clientKey, projectId, names, schemaSourceType), methodWithArguments);
        }

        public FormBase[] GetFormInfosWithText(string projectId, IEnumerable<string> formNames, SchemaSourceType schemaSourceType)
        {
            var names = formNames.ToArray();

            string methodWithArguments = string.Format(
                "GetFormInfosWithText(projectId = {0}, formNames = {1}, schemaSourceType = {2})",
                projectId,
                string.Join(",", names),
                schemaSourceType);

            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetFormInfosWithTexts(clientKey, projectId, names, schemaSourceType), methodWithArguments);
        }

        public CatiIdentityValidationResult ValidateCatiIdentity(string confirmitCookieData, string catiUserName, string catiClientKey, bool isCatiHttps)
        {
            string methodWithArguments = string.Format("ValidateCatiIdentity(confirmitCookieData = {0}, catiUserName = {1}, catiClientKey = {2}, isCatiHttps = {3})",
                                                       confirmitCookieData, catiUserName, catiClientKey, isCatiHttps);

            return DoServiceCall(x => x.ValidateCatiIdentity(catiClientKey, confirmitCookieData, catiUserName, catiClientKey, isCatiHttps), methodWithArguments);
        }

        public IEnumerable<CatiSupervisor> GetCompanyCatiSupervisorsNames(int companyId)
        {
            string methodWithArguments = string.Format("GetCompanyCatiSupervisorsNames(companyId = {0})", companyId);
            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetCompanyCatiSupervisors(clientKey, companyId), methodWithArguments);
        }

        public CatiSupervisorInfo GetCatiSupervisorInfo(string xConfirmitApiKey)
        {
            string methodWithArguments = string.Format("GetCatiSupervisorInfo(xConfirmitApiKey = {0}",
                                                       xConfirmitApiKey);

            return DoServiceCall(x => x.GetCatiSupervisorInfo(xConfirmitApiKey, xConfirmitApiKey), methodWithArguments);
        }
    }
}