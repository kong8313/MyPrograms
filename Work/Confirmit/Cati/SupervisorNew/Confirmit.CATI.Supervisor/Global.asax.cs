using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.ConfirmitClientKey;
using Confirmit.CATI.Core.Security;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.Core.Services.PersonServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Filters;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.ServiceRegistration;
using Confirmit.CATI.Supervisor.ServiceRegistration;
using System;
using System.Configuration;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.BvCallHandlerLibrary;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.ScheduleDom;
using Confirmit.CATI.Supervisor.Classes.Auth;
using Confirmit.CATI.Supervisor.Core.AccessToken;
using Confirmit.CATI.Supervisor.Core.Security;
using Confirmit.Configuration.Bootstrap;
using Firmglobal.Framework.Security;
using ServiceLocator = Confirmit.CATI.Common.ServiceLocation.ServiceLocator;

namespace Confirmit.CATI.Supervisor
{
    public class Global : HttpApplication
    {
        private const string MicrosoftOfficeExistenceDiscovery = "Microsoft Office Existence Discovery";
        private const string TelerikReportEventObjectId = "__TelerikReportEventObect__";

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs reArgs)
        {
            string reArgsShortName = new AssemblyName(reArgs.Name).Name;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyShortName = assembly.GetName().Name;
                if (assemblyShortName == reArgsShortName)
                {
                    return assembly;
                }
            }

            return null;
        }

        protected void Application_Start(Object sender, EventArgs e)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += OnAssemblyResolve;

            var serviceLocator = new ServiceLocator();
            serviceLocator.Initialize();

            IServicesRegistryInitializer serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);
            serviceRegistryInitializer.RegisterRegistries(new IServiceLocatorRegistry[]
                                                              {
                                                                  new BackendRegistry(),
                                                                  new TelephonyRegistry(),
                                                                  new SupervisorRegistry(),
                                                                  new SecurityRegistry(),
                                                                  new PersonServiceRegistry(),
                                                                  new SupervisorFilterRegistry(),
                                                                  new TimezoneRegistry(),
                                                                  new SupervisorCallCentersRegistry(),
                                                                  new SupervisorCoreRegistry(),
                                                                  new AsyncOperationRegistry(),
                                                                  new MiscRegistry(),
                                                                  new SchedulingRegistry()
                                                              });

            var serviceceRegistrator = ServiceLocator.Resolve<IServiceRegistrator>();
            serviceceRegistrator.Register<ISideBySideManager, SideBySideManager>();
            serviceceRegistrator.RegisterSingleton<IProcessAndEnvironmentInfo, ProcessAndEnvironmentInfo>();
            serviceceRegistrator.Register<IConfirmitClientKeyProvider, SupervisorConfirmitClientKeyProvider>();
            serviceceRegistrator.RegisterSingleton<IUrlProvider, UrlProvider>();
            new SystemSettingSupervisorRegistrator().RegisterTypes(serviceceRegistrator);

            TraceHelper.RemoveNonContainerTraceListeners();

            InitConnectionStringForTelerikCache();
        }

        private static void InitConnectionStringForTelerikCache()
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;

            // Telerik requires DB connection string for cache to be specified in config file.
            // We use this approach to make 'ConfigurationManager.ConnectionStrings' editable and add a new entry in runtime
            // because we cannot edit config file when running in container.
            if (ConfigurationManager.ConnectionStrings["TelerikReportingCacheConnectionString"] == null)
            {
                var fieldInfo = typeof(ConfigurationElementCollection)
                    .GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null) return;

                fieldInfo.SetValue(ConfigurationManager.ConnectionStrings, false);

                ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings(
                    "TelerikReportingCacheConnectionString", new DbLibProvider().CatiDefaultConnectionString,
                    "System.Data.SqlClient"));

                // Make 'ConfigurationManager.ConnectionStrings' read-only again.
                fieldInfo.SetValue(ConfigurationManager.ConnectionStrings, true);
            }
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            // When opening documents from a URL location in Microsoft Office 2007,
            // the Office library can make an HTTP HEAD request to the web server for the opening URL.
            // This request is sent with a User-Agent set to "Microsoft Office Existence Discovery".
            // It results in errors in event log because Office does not have authentication cookie.
            // See http://blogs.msdn.com/vsofficedeveloper/pages/Office-Existence-Discovery-Protocol.aspx
            if (Request.UserAgent == MicrosoftOfficeExistenceDiscovery)
            {
                Response.End();
            }

            if (Request.UserLanguages != null && Request.UserLanguages.Length > 0)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }

            AuthorizationManager.SetupBackendInstance(0);
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            if (Server.GetLastError() is ThreadAbortException)
                Server.ClearError();

            int pageIndex;
            if (IsTelerikReportRequest() && Request.QueryString["PageIndex"] != null &&
                Int32.TryParse(Request.QueryString["PageIndex"], out pageIndex) && pageIndex == 0)
            {
                var evt = Context.Items[TelerikReportEventObjectId] as BuildTelerikReportEvent;
                if (evt != null)
                {
                    evt.Finish();
                }
            }
        }

        /// <summary>
        /// Occurs when request is being authenticated. 
        /// </summary>
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (CheckIfCorrectRequest() ||
                CheckIfErrorPageRequest() ||
                CheckIfHealthzRequest() ||
                CheckIfMetricsRequest() ||
                CheckIfRedirectRequest() ||
                TryAuthorizeWithAccessToken())
            {
                return;
            }

            HttpContext.Current.Response.Redirect("~/ReloadPage.aspx");
        }


        private bool CheckIfRedirectRequest()
        {
            var isRedirectPage = HttpContext.Current.Request.Path.EndsWith("ReloadPage.aspx", StringComparison.OrdinalIgnoreCase);

            HttpContext.Current.SkipAuthorization = isRedirectPage;
            return isRedirectPage;
        }

        private bool CheckIfHealthzRequest()
        {
            var isHealthzRequest =
                HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath?.StartsWith("~/healthz/",
                    StringComparison.OrdinalIgnoreCase) ?? false;

            HttpContext.Current.SkipAuthorization = isHealthzRequest;
            return isHealthzRequest;
        }

        private bool CheckIfMetricsRequest()
        {
            var isMetricsRequest =
                string.Equals(HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath, "~/metrics",
                    StringComparison.OrdinalIgnoreCase);

            HttpContext.Current.SkipAuthorization = isMetricsRequest;
            return isMetricsRequest;
        }

        protected void Application_AcquireRequestState(Object sender, EventArgs e)
        {
            // Here we do 2 things:
            //     1. Store explicit company id in session for using in further requests if it is passed in the URL
            //     2. Change company from the default for the user to the one stored in the session
            //
            // NOTE: We allow to switch company for the PROS users only, and won't do anything for normal users
            if ((HttpContext.Current.Session == null) || CheckIfRedirectRequest())
            {
                return;
            }

            if (ExceptionTraceHelper.IsErrorPage())
            {
                return;
            }

            AuthorizationManager.ClearSessionOnNewUserLogin();
            AuthorizationManager.SaveExplicitVariablesToSession();
            AuthorizationManager.SetupExplicitCompanyId();
        }

        private bool CheckIfErrorPageRequest()
        {
            var isErrorPage = ExceptionTraceHelper.IsErrorPage();
            HttpContext.Current.SkipAuthorization = isErrorPage;
            return isErrorPage;
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (IsTelerikReportRequest())
            {
                Server.ScriptTimeout = ServiceLocator.Resolve<ISystemSettings>().Reports.ReportGenerationTimeout;
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Context.Items[TelerikReportEventObjectId] = new BuildTelerikReportEvent(
                    Request.RawUrl,
                    Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : string.Empty);
            }
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            ExceptionTraceHelper.ShowServerError();
        }

        protected void Session_End(Object sender, EventArgs e)
        {
        }

        protected void Application_End(Object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Stops processing of a request if it is an async POST request with empty body. It might be caused by IE bug:
        /// http://support.microsoft.com/kb/895954/en-us
        /// </summary>
        /// <remarks>
        /// In activity views we keep all view state data on the server side (including selected items in surveys filter).
        /// The page only contains the session key that is sent back to the server with each request and is used to find correct view state data.
        /// This way of keeping view state helps us to reduce size of activity view pages.
        /// But if AJAX request have been posted to the server without request body (that contains this session key)
        /// view state for this page is being cleared (it works as if activity view page has been re-opened).
        /// So any filters, sorting and expanded rows will be lost.
        /// </remarks>
        private bool CheckIfCorrectRequest()
        {
            if (Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) &&
                Request.AppRelativeCurrentExecutionFilePath == "~/ScriptResource.axd")
                return true;

            if (Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && IsAsyncPostBackRequest(Request) && Request.Form.Count == 0)
            {
                var headers = new StringBuilder();
                foreach (var key in Request.Headers.AllKeys)
                {
                    headers.AppendLine(string.Format("{0} = {1}", key, Request.Headers[key]));
                }
                Trace.TraceWarning("Update panel POST request with empty body has been received.\r\nRequest headers:\r\n{0}", headers);

                Response.StatusCode = 204;
                Response.End();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value that indicates whether the current postback is being executed in partial-rendering mode.
        /// </summary>
        /// <remarks>Borrowed from PageRequestManager class</remarks>
        private static bool IsAsyncPostBackRequest(HttpRequest request)
        {
            // Detect the header for async postbacks. A header can appear
            // multiple times, and each header entry can contain a comma-separated 
            // list of values. ASP.NET doesn't split the comma-separated values for
            // us so we have to do it.

            // We used to use the Pragma header but some browsers, such as Opera, 
            // do not support sending it through XMLHttpRequest. Instead we use a
            // custom header, X-MicrosoftAjax. 
            string[] headerValues = request.Headers.GetValues("X-MicrosoftAjax");
            if (headerValues != null)
            {
                for (int i = 0; i < headerValues.Length; i++)
                {
                    string[] headerContents = headerValues[i].Split(',');
                    for (int j = 0; j < headerContents.Length; j++)
                    {
                        if (headerContents[j].Trim() == "Delta=true")
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsTelerikReportRequest()
        {
            var result = Request.Path.Contains("Telerik.ReportViewer.axd") && Request.QueryString["instanceID"] != null;
            return result;
        }

        private Tabs GetTabPermissions(ConfirmitPrincipal principal)
        {
            var result = Tabs.None;

            //If user has SystemCatiAdministrate or SystemProjectAdministrate permission, all tabs are allowed.
            if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCatiAdministrate) || 
                principal.IsInRole(SystemPermissions.SystemCatiSupervisorAdmin) ||
                principal.IsInRole(SystemPermissions.AccountRead) ||
                principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemProjectAdministrate))
            {
                result = Tabs.SurveyManagement | Tabs.InterviewerManagement | Tabs.Resources | Tabs.Scheduling |
                    Tabs.Reports | Tabs.ActvivityViews | Tabs.RecordedInterviews;
            }
            //otherwise check permissions for each tab
            else
            {
                if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCatiSuperviseSurveys))
                    result |= Tabs.SurveyManagement;
                if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCatiSuperviseInterviewers))
                    result |= Tabs.InterviewerManagement;
                if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCatiSuperviseResources))
                    result |= Tabs.Resources;
                if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCatiSuperviseScheduling))
                    result |= Tabs.Scheduling;
                if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCatiSuperviseReports))
                    result |= Tabs.Reports;
                if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCatiSuperviseActivity) ||
                    principal.IsInRole(SystemPermissions.SystemCatiSuperviseMonitor))
                    result |= Tabs.ActvivityViews;
                if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCatiSuperviseRecorded))
                    result |= Tabs.RecordedInterviews;
            }
            if (principal.IsInRole(ProjectAuthorization.SystemPermissions.SystemCapiSupervisor))
                result |= Tabs.ProductivityReport;
            return result;
        }

        /// <summary>
        /// When Supervisor is called from some external service. 
        /// Retrive an access token from header and use it to identify user by Identity service.
        /// </summary>
        /// <returns></returns>
        private bool TryAuthorizeWithAccessToken()
        {
            var identityService = ServiceLocator.Resolve<ISupervisorIdentityService>();
            var accessToken = identityService.GetActualAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                Trace.TraceWarning("Authorization failed: No access token found in the 'Authorization' header, and the 'catiidp' cookie is missing. The application will attempt to reload.");
                return false;
            }

            try
            {
                var identity = AsyncTaskRunner.RunSync(() => identityService.GetConfirmitIdentity(accessToken));
                
                var principal = new ConfirmitPrincipal(
                    new ClaimsPrincipal(identity).FindAll(identity.RoleClaimType).Select(x => x.Value), identity);

                var allowedTabs = GetTabPermissions(principal);

                if (allowedTabs == Tabs.None)
                {
                    ExceptionTraceHelper.ShowErrorPage("You do not have access to the CATI Supervisor");
                }

                HttpContext.Current.User = new SupervisorPrincipal(principal, allowedTabs);

                AuthorizationManager.SetupBackendInstance(principal.ConfirmitIdentity.CompanyId);
                
                new AccessTokenService().SetAccessToken(accessToken);

                return true;
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e, "Authorization with access token failed");
                return false;
            }
        }

        /// <summary>
        /// This method returns the correct relative path when installing
        /// the portal on a root web site instead of virtual directory
        /// </summary>
        public static string GetApplicationPath(HttpRequest request)
        {
            string path = string.Empty;
            if (request.ApplicationPath != "/")
                path = request.ApplicationPath;

            return path;
        }
    }
}
