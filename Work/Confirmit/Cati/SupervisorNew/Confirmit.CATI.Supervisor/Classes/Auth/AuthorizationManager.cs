using System;
using System.Web;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Core.Misc;
using Confirmit.Configuration;
using Confirmit.Security.Crypto;

namespace Confirmit.CATI.Supervisor.Classes.Auth
{
    /// <summary>
    /// Class is responsible for authentication in CP.
    /// </summary>
    public static class AuthorizationManager
    {
        /// <summary>
        /// Logs confirmit user into CP, sets authentication cookie and stores company ID
        /// and project ID, associated with request.
        /// </summary>
        /// <param name="name">Supervisor login name.</param>
        /// <param name="clientKey">External security client key of confirmit user.</param>
        /// <param name="company">Company identifier of confirmit user.</param>
        /// <param name="isCatiAdministrator"></param>
        /// <param name="isProsUser"></param>
        /// <param name="isConnectionSecure"></param>
        /// <exception cref="InternalErrorException">Supervisor tries to access CATI
        /// Supervisor without sufficient permissions.</exception>
        public static void Login(
            string name,
            string clientKey,
            string company,
            bool isCatiAdministrator,
            bool isProsUser,
            bool isConnectionSecure)
        {
            // We need to make company ID available ASAP in HttpContext as it is used everywhere.
            HttpContext.Current.User = new SupervisorPrincipal(name, clientKey, company, String.Empty, Tabs.None,
                isCatiAdministrator, isProsUser, isConnectionSecure);

            var evt = new InitUserTabPermissionsEvent();

            var authoringService = ServiceLocator.Resolve<IAuthoringService>();
            Tabs allowedTabs = authoringService.GetTabPermissions(name, BackendInstance.Current.CompanyId);

            evt.Details.AllowedTabs = allowedTabs.ToString();
            evt.Details.Mode = "CATI";
            evt.Finish();

            if (allowedTabs == Tabs.None)
            {
                throw new InternalErrorException(string.Format(Strings.SupervisorWithoutSufficientPermissions, name));
            }

            string companyName = BackendInstance.Current.CompanyName;

            HttpContext.Current.User = new SupervisorPrincipal(name, clientKey, company, companyName, allowedTabs,
                isCatiAdministrator, isProsUser, isConnectionSecure);
        }

        public static void SetupBackendInstance(int companyId)
        {
            var backendInstance = ServiceLocator.Resolve<IBackendInstanceFactory>()
                .Create(
                    companyId,
                    HostType.Supervisor);
            HttpContext.Current.Items["BackendInstance"] = backendInstance;
        }

        public static string BuildLogoffUrl()
        {
            var logoffUrl = UrlHelper.ModifyUrlProtocol(
                string.Format(
                    "{0}/authoring/Logoff.aspx?logoff=true",
                    ConfirmitConfiguration.ConfirmitUrl));

            return logoffUrl;
        }

        /*
        * The problem is we don't change session cookie when re-login in Authoring. We need to explicitly clean session in two scenarios:
        * 1. User has changed - Client key is defferent from previous one
        * 2. User the same, but he switched the company
        */
        public static void ClearSessionOnNewUserLogin()
        {
            var session = HttpContext.Current.Session;
            var identity = (SupervisorIdentity)SupervisorPrincipal.Current.Identity;

            var currentExplicitCompanyId = session["ExplicitCompanyId"];

            var companyId = GetCompanyIdFromCookie();
            var cookieIsValid = true;
            if (companyId <= 0)
            {
                cookieIsValid = false;
                companyId = int.Parse(identity.Company);
            }

            var companyHasChanged = currentExplicitCompanyId?.ToString() != companyId.ToString();
            if (companyHasChanged && (cookieIsValid || !string.IsNullOrEmpty(currentExplicitCompanyId?.ToString())))
            {
                // User has logged in under another login and so we need to clean session
                session.Clear();
            }
        }

        private static int GetCompanyIdFromCookie()
        {
            try
            {
                var stringEncryption =
                    new StringEncryption(ConfirmitConfiguration.GetStringValue("Aes128Key", string.Empty));
                var cookie = HttpContext.Current.Request.Cookies.Get("caticompany");
                if (cookie != null)
                {
                    var value = stringEncryption.DecryptString(cookie.Value);
                    var intValue = int.Parse(value);
                    return intValue;
                }

                return -1;
            }
            catch
            {
                return -1;
            }
        }

        // If there's an ExplicitCompanyId set we need to re-initialize identity company props and backend instance
        public static void SetupExplicitCompanyId()
        {
            var session = HttpContext.Current.Session;
            var identity = (SupervisorIdentity)SupervisorPrincipal.Current.Identity;

            if (session["ExplicitCompanyId"] != null)
            {
                var explicitCompanyId = (int)session["ExplicitCompanyId"];

                SetupBackendInstance(explicitCompanyId);

                identity.Company = explicitCompanyId.ToString();
                identity.m_CompanyName = BackendInstance.Current.CompanyName;
            }
        }

        public static void SaveExplicitVariablesToSession()
        {
            try
            {
                var session = HttpContext.Current.Session;
                var identity = (SupervisorIdentity) SupervisorPrincipal.Current.Identity;
                
                var companyId = GetCompanyIdFromCookie();

                if (companyId > 0)
                {
                    session["ExplicitCompanyId"] = companyId;
                }

                session["ExplicitClientKey"] = identity.ClientKey;
            }
            catch (Exception ex)
            {
                TraceHelper.TraceException(ex);
            }
        }
    }
}
