using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Threading;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using CustomActionLibrary;

namespace CatiInstallation
{
    /// <summary>
    /// Class with function for CustomAction class
    /// </summary>
    public class CatiSetupEngine : SetupEngine
    {
        public CatiSetupEngine(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Install Framework64\aspnet_regiis if it is needed
        /// Change IIS version to 4.0 for instaleld CP web  component
        /// </summary>
        public void ConfigureIISApplication(IISEngine iisEngine, string productName, string supervisorAppPoolName, 
            string supervisorSiteName, string supervisorVirtualDirectoryName, string supervisorLocation)
        {
            Logger.WriteLog("Begin ConfigureIISApplication");

            try
            {
                string regiis = Path.Combine(SystemRoot, @"Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe");
                // 
                // Install the right aspnet_regiis if it is needed
                // If asp isn't installed on IIS (or installed 32 bit version on 64 bit system) - 
                // we will install the needed asp
                //
                string[] frameworkVersionsInfo = ExternalInvoker.Invoke(regiis, "-lv").Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                bool isAspNetInstalled = frameworkVersionsInfo.Any(frameworkVersionInfo => frameworkVersionInfo.StartsWith("4.0.30319.0") && frameworkVersionInfo.Contains("Framework64"));

                if (!isAspNetInstalled)
                {
                    ExternalInvoker.Invoke(regiis, "-ir");
                }

                iisEngine.CreateAppPool(supervisorAppPoolName);

                iisEngine.CreateAlias(supervisorSiteName, supervisorVirtualDirectoryName, supervisorAppPoolName, supervisorLocation);

                iisEngine.DisableRunningOf32BitApplicationsForAppPools(productName, supervisorAppPoolName);
            }
            finally
            {
                Logger.WriteLog("End ConfigureIISApplication");
            }
        }

        /// <summary>
        /// Delete binding for 0.0.0.0:443
        /// Configure http.sys by netsh.exe
        /// </summary>
        public void DeleteBinding()
        {
            Logger.WriteLog("Begin DeleteBinding");
            try
            {
                ExternalInvoker.Invoke("netsh.exe", "http delete sslcert ipport=0.0.0.0:443");
            }
            catch (Exception ex)
            {
                // Need to suppress an exception, because we may to call this function, when binding isn't defined yet
                Logger.WriteLog(TraceEventType.Error, ex.ToString());
            }
            finally
            {
                Logger.WriteLog("End DeleteBinding");
            }
        }

        /// <summary>
        /// Configures http.sys using installed or selected certificate.
        /// For the details see following page: 
        /// http://www.codeplex.com/wikipage?ProjectName=WCFSecurity&title=How%20To%20-%20Create%20and%20Install%20Temporary%20Certificates%20in%20WCF%20for%20Message%20Security%20During%20Development&referringTitle=How%20Tos
        /// </summary>
        /// <param name="certificateThumbprint">Certificate thumbprint</param>
        public void ConfigureHttpListener(string certificateThumbprint)
        {
            const string appId = "{DF179E9A-676B-4153-AA92-E589568C7D41}";
            DeleteBinding();
            ExternalInvoker.Invoke("netsh.exe", "http add sslcert ipport=0.0.0.0:443 certhash=" + certificateThumbprint.Replace(" ", "") + " appid=" + appId);
        }

        /// <summary>
        /// Check if http listener is configured on 443 port
        /// </summary>
        /// <returns></returns>
        public bool IsHttpListenerRegistered()
        {
            var output = ExternalInvoker.Invoke("netsh.exe", "http show sslcert ipport = 0.0.0.0:443", null, -1, true, true);
            return output.Contains("0.0.0.0:443");
        }

        public string GetDefaultInstanceServiceName(string sideBySideName)
        {
            return "Confirmit.CATI.Backend." + sideBySideName;
        }

        public void CheckDatabaseCreationAbility(CatiDatabaseEngine catiDatabaseEngine, string dataFilePath, string logsFilePath, string catiDefaultDbRecoveryModel, string errorMessage)
        {
            string testDatabaseName = "CatiTestDatabase_" + new Random().Next(10000);

            Logger.WriteLog("Create '{0}' database. dataFilePath='{1}'. logsFilePath='{2}'. catiDefaultDbRecoveryModel='{3}'", testDatabaseName, dataFilePath, logsFilePath, catiDefaultDbRecoveryModel);
            try
            {
                catiDatabaseEngine.CreateDatabase(testDatabaseName, dataFilePath, logsFilePath, catiDefaultDbRecoveryModel);
                Logger.WriteLog("Successful creation");

                catiDatabaseEngine.ExecuteNonQuery(string.Format("DROP DATABASE {0}", testDatabaseName));
                Logger.WriteLog("Successful removal");
            }
            catch (Exception ex)
            {
                throw new Exception(errorMessage, ex);
            }
        }

        public void AddFileSecurity(string fileName, string account, FileSystemRights rights, AccessControlType controlType)
        {
            FileSecurity fSecurity = File.GetAccessControl(fileName);

            fSecurity.AddAccessRule(new FileSystemAccessRule(account, rights, controlType));

            File.SetAccessControl(fileName, fSecurity);
        }

        public void RemoveAllBackendServices(string sideBySideName)
        {
            foreach (var service in ServiceController.GetServices())
            {
                if (service.ServiceName.StartsWith(GetDefaultInstanceServiceName(sideBySideName), true, Thread.CurrentThread.CurrentCulture))
                {
                    using (var serviceEngine = new ServiceEngine(this, service.ServiceName))
                    {
                        serviceEngine.RemoveService();
                    }
                }
            }
        }

        public void StopAllCatiServices(string catiSqlServerName, string sideBySideName, string currentVersion)
        {
            var prefixName = "Confirmit.CATI.Backend." + sideBySideName;
            var serverName = Environment.MachineName;

            StopServices(serverName, prefixName, true);
            StopServices(serverName, prefixName, false);

            while (GetServicesFromServer(serverName, prefixName).Count > 0)
            {
                Thread.Sleep(500);
            }
        }

        private void StopServices(string serverName, string prefixName, bool stopDefaultServicesOnly)
        {
            List<ServiceController> servicesToStop = GetServicesFromServer(serverName, prefixName);

            foreach (ServiceController serviceController in servicesToStop)
            {
                var sc = new ServiceController(serviceController.ServiceName, serverName);

                if (stopDefaultServicesOnly)
                {
                    if (!serviceController.ServiceName.Contains("$"))
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 3, 0));
                        break;
                    }
                }
                else
                {
                    sc.Stop();
                }
            }
        }

        private List<ServiceController> GetServicesFromServer(string serverName, string prefixName)
        {
            var services = ServiceController.GetServices(serverName);
            return services.Where(service => service.Status != ServiceControllerStatus.Stopped && service.ServiceName.StartsWith(prefixName)).ToList();
        }

        public void StartAllCatiServicesAndWaitUntilTheyStarted(string sideBySideName, string isLoadBalancedEnvironment)
        {
            // Starting backend service     
            string defaultCatiServiceName = GetDefaultInstanceServiceName(sideBySideName);
            using (var serviceEngine = new ServiceEngine(this, defaultCatiServiceName))
            {
                serviceEngine.StartService();

                if (isLoadBalancedEnvironment == "True")
                {
                    serviceEngine.WaitUntilAllCatiServicesStart(defaultCatiServiceName + "$");
                    Thread.Sleep(30000);
                }
            }
        }
    }
}