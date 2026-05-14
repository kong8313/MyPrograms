using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.CATI.Installation.Common.Properties;
using Microsoft.Web.Administration;
using Application = Microsoft.Web.Administration.Application;

namespace Confirmit.CATI.Installation.Common
{
    public enum DumpCreationOptions
    {
        DoNotModifyCurrentOptions,
        DoNotCreateDump,
        CreateDump
    }

    public class IISEngine
    {
        /// <summary>
        /// cacheControlMode
        /// </summary>
        private const string CacheControlModeAttributeName = "cacheControlMode";

        /// <summary>
        /// cacheControlMaxAge
        /// </summary>
        private const string CacheControlMaxAgeAttributeName = "cacheControlMaxAge";

        /// <summary>
        /// UseMaxAge
        /// </summary>
        private const string MaxAgeCacheMode = "UseMaxAge";

        /// <summary>
        /// DisableCache
        /// </summary>
        private const string DisableCacheMode = "DisableCache";

        private readonly ILogger _logger;

        public IISEngine(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Get all app pools
        /// </summary>
        /// <returns></returns>
        public List<string> GetAppPools()
        {
            var appPoolNames = new List<string>();

            using (var sm = new ServerManager())
            {
                appPoolNames.AddRange(sm.ApplicationPools.Select(appPoolName => appPoolName.Name));
            }

            return appPoolNames;
        }

        /// <summary>
        /// Get names of the all sites
        /// </summary>
        /// <returns></returns>
        public List<string> GetWebSites()
        {
            var siteNames = new List<string>();

            using (var sm = new ServerManager())
            {
                siteNames.AddRange(sm.Sites.Select(site => site.Name));
            }

            return siteNames;
        }

        /// <summary>
        /// Get the site ID for selected Web Site Name
        /// </summary>
        /// <param name="siteName">Web Site Name</param>
        /// <returns></returns>
        public string GetWebSiteId(string siteName)
        {
            using (var sm = new ServerManager())
            {
                foreach (var site in sm.Sites)
                {
                    if (site.Name == siteName)
                    {
                        return site.Id.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            throw new Exception(string.Format(Resources.WebSiteIsNotFound, siteName));
        }

        public void CreateAlias(string siteName, string applicationName, string appPoolName, string appPath)
        {
            using (var sm = new ServerManager())
            {
                Site site = sm.Sites[siteName];

                if (site.Applications["/" + applicationName] != null)
                {
                    _logger.WriteLog("IIS alias '{0}' already exists. Update PhysicalPath and ApplicationPoolName", applicationName);

                    site.Applications["/" + applicationName].VirtualDirectories[0].PhysicalPath = appPath;
                    site.Applications["/" + applicationName].ApplicationPoolName = appPoolName;
                    sm.CommitChanges();

                    return;
                }

                Application application = site.Applications.Add("/" + applicationName, appPath);
                application.ApplicationPoolName = appPoolName;

                sm.CommitChanges();
            }
        }

        public void RemoveAlias(string siteName, string applicationName)
        {
            using (var sm = new ServerManager())
            {
                Site site = sm.Sites[siteName];
                Application app = site.Applications["/" + applicationName];
                if (app == null)
                {
                    _logger.WriteLog("IIS alias '{0}' doesn't exists. Do nothing", applicationName);
                    return;
                }

                site.Applications.Remove(app);

                sm.CommitChanges();
            }
        }

        public void RemoveVirtualDirectory(string siteName, string directoryName)
        {
            directoryName = directoryName.Trim('/');

            using (var sm = new ServerManager())
            {
                Site site = sm.Sites[siteName];
                Application app = site.Applications[0];

                VirtualDirectory virtDirectory = app.VirtualDirectories.FirstOrDefault(x => x.Path == "/" + directoryName);
                if (virtDirectory != null)
                {
                    virtDirectory.Delete();
                    sm.CommitChanges();
                }
            }
        }

        public void ConfigureVirtualDirectories(string siteName, string virtualDirectoriesTree, string appPath)
        {
            virtualDirectoriesTree = "/" + virtualDirectoriesTree.Replace('\\', '/').Trim('/');

            using (var sm = new ServerManager())
            {
                Site site = sm.Sites[siteName];
                Application app = site.Applications[0];

                string[] virtualDirectoryNames = virtualDirectoriesTree.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                string virtualDirectoryPath = string.Empty;
                string emptyFolderPath = CreateEmptyFolder();
                for (int i = 0; i < virtualDirectoryNames.Length - 1; i++)
                {
                    virtualDirectoryPath += "/" + virtualDirectoryNames[i];
                    if (!IsVirtualDirectoryExist(app.VirtualDirectories, virtualDirectoryPath))
                    {
                        app.VirtualDirectories.Add(virtualDirectoryPath, emptyFolderPath);
                    }
                }
                
                if (IsVirtualDirectoryExist(app.VirtualDirectories, virtualDirectoriesTree))
                {
                    ChangevirtualDirectoryPhysicalPath(app.VirtualDirectories, virtualDirectoriesTree, appPath);
                }
                else
                {
                    app.VirtualDirectories.Add(virtualDirectoriesTree, appPath);
                }

                sm.CommitChanges();
            }

            DisableContentCaching(siteName, virtualDirectoriesTree);
        }

        private string CreateEmptyFolder()
        {
            string emptyFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CatiEmptyFolder");                
            if (!Directory.Exists(emptyFolderPath))
            {
                Directory.CreateDirectory(emptyFolderPath);
            }

            return emptyFolderPath;
        }


        public void DisableContentCaching(string siteName, string virtualDirectory)
        {
            if (!virtualDirectory.StartsWith("/"))
            {
                virtualDirectory = "/" + virtualDirectory;
            }

            using (var serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetWebConfiguration(siteName + virtualDirectory);
                ConfigurationSection staticContentSection = config.GetSection("system.webServer/staticContent");

                ConfigurationElement clientCacheElement = staticContentSection.GetChildElement("clientCache");
                clientCacheElement[CacheControlModeAttributeName] = DisableCacheMode;
                
                serverManager.CommitChanges();
            }
        }

        public void SetMaxAgeContentExpirationForSpecifiedFolders(string siteName, string virtualDirectory, string[] foldersForContentExpiration, TimeSpan maxAgeTime)
        {
            if (!virtualDirectory.StartsWith("/"))
            {
                virtualDirectory = "/" + virtualDirectory;
            }

            using (var serverManager = new ServerManager())
            {
                foreach (string folder in foldersForContentExpiration)
                {
                    Configuration config = serverManager.GetWebConfiguration(siteName + virtualDirectory + "/" + folder);
                    ConfigurationSection staticContentSection = config.GetSection("system.webServer/staticContent");

                    ConfigurationElement clientCacheElement = staticContentSection.GetChildElement("clientCache");
                    clientCacheElement[CacheControlModeAttributeName] = MaxAgeCacheMode;
                    clientCacheElement[CacheControlMaxAgeAttributeName] = maxAgeTime;
                }

                serverManager.CommitChanges();
            }
        }

        private void ChangevirtualDirectoryPhysicalPath(IEnumerable<VirtualDirectory> virtualDirectoryCollection, string virtualDirectoriesTree, string appPath)
        {
            foreach (var virtualDirectory in virtualDirectoryCollection)
            {
                if (virtualDirectory.Path.ToLowerInvariant() == virtualDirectoriesTree.ToLowerInvariant())
                {
                    virtualDirectory.PhysicalPath = appPath;
                }
            }
        }

        private bool IsVirtualDirectoryExist(IEnumerable<VirtualDirectory> virtualDirectories, string virtualDirectoriesTree)
        {
            return virtualDirectories.Any(virtualDirectory => virtualDirectory.Path.ToLowerInvariant() == virtualDirectoriesTree.ToLowerInvariant());
        }

        public void CreateAppPool(string appPoolName)
        {
            using (var sm = new ServerManager())
            {
                ApplicationPool appPool = sm.ApplicationPools[appPoolName] ?? sm.ApplicationPools.Add(appPoolName);

                appPool.ManagedRuntimeVersion = "v4.0";
                appPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                sm.CommitChanges();
            }
        }

        public void RemoveEmptyAppPool(string appPoolName)
        {
            using (var sm = new ServerManager())
            {
                ApplicationPool appPool = sm.ApplicationPools[appPoolName];
                if (appPool == null || DoesAppPoolHaveAssociatedApplication(sm, appPoolName))
                {
                    return;
                }

                sm.ApplicationPools.Remove(appPool);
                sm.CommitChanges();
            }
        }

        private bool DoesAppPoolHaveAssociatedApplication(ServerManager sm, string appPoolName)
        {
            return sm.Sites.SelectMany(site => site.Applications).Any(app => app.ApplicationPoolName == appPoolName);
        }

        /// <summary>
        /// Set recycling value to zero for selected app pool
        /// </summary>
        /// <param name="appPoolName">App Pool Name</param>
        public void SetRecyclingValueToZero(string appPoolName)
        {
            using (var sm = new ServerManager())
            {
                sm.ApplicationPools[appPoolName].Recycling.PeriodicRestart.Time = new TimeSpan(0);
                sm.CommitChanges();
            }
        }

        /// <summary>
        /// Set orphan properties for application pool       
        /// </summary>
        /// <param name="dumpCreationMode">if true - enable orphan worker process, otherwise - disable it</param>        
        /// <param name="appPoolName">App pool name</param>
        /// <param name="dumpCmdFilePath">Path to dumpCreator.cmd file</param>
        public void SetOrphaningForAppPool(DumpCreationOptions dumpCreationMode, string appPoolName, string dumpCmdFilePath)
        {
            using (var sm = new ServerManager())
            {
                if (dumpCreationMode == DumpCreationOptions.CreateDump)
                {
                    _logger.WriteLog("Set orphan properties for {0} to True", appPoolName);
                    sm.ApplicationPools[appPoolName].Failure.OrphanWorkerProcess = true;
                    sm.ApplicationPools[appPoolName].Failure.OrphanActionExe = dumpCmdFilePath;
                    sm.ApplicationPools[appPoolName].Failure.OrphanActionParams = "%1%";
                    sm.CommitChanges();
                }
                else if (dumpCreationMode == DumpCreationOptions.DoNotCreateDump)
                {
                    _logger.WriteLog("Set orphan properties for {0} to False", appPoolName);
                    sm.ApplicationPools[appPoolName].Failure.OrphanWorkerProcess = false;
                    sm.ApplicationPools[appPoolName].Failure.OrphanActionExe = string.Empty;
                    sm.ApplicationPools[appPoolName].Failure.OrphanActionParams = string.Empty;
                    sm.CommitChanges();
                }
            }
        }

        public void DisableRunningOf32BitApplicationsForAppPools(string productName, string appPoolName)
        {
            using (var sm = new ServerManager())
            {
                _logger.WriteLog("appPoolName={0}", appPoolName);

                // Set Enable32bitAppOnWin64 property for AppPools to false if it is needed
                // This is settings for all IIS. We need to forbid a running of 32 application, 
                // because we want to use aspnet_regiis x64.
                // This change doesn't change settings for real pools
                _logger.WriteLog("\"Enable32BitAppOnWin64\" for IIS={0}", sm.ApplicationPoolDefaults.Enable32BitAppOnWin64);

                if (sm.ApplicationPoolDefaults.Enable32BitAppOnWin64)
                {
                    sm.ApplicationPoolDefaults.Enable32BitAppOnWin64 = false;
                    sm.CommitChanges();
                    TopMostMessageBox.Show(Resources.RunningOf32BitApllicationsDisabledForIIS, productName, MessageBoxIcon.Information);
                }

                // Set Enable32bitAppOnWin64 property for cp pool to false if it is needed
                // We forbid a run 32 bit application on app pool, that contains our web 
                // application (Supervisor)
                _logger.WriteLog("\"Enable32BitAppOnWin64\" for {0}={1}", appPoolName, sm.ApplicationPools[appPoolName].Enable32BitAppOnWin64);

                if (sm.ApplicationPools[appPoolName].Enable32BitAppOnWin64)
                {
                    sm.ApplicationPools[appPoolName].Enable32BitAppOnWin64 = false;
                    sm.CommitChanges();
                    TopMostMessageBox.Show(string.Format(Resources.RunningOf32BitApllicationsDisabledForAppPool, appPoolName), productName, MessageBoxIcon.Information);
                }
            }
        }
    }
}