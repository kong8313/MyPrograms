using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Web.Administration;

namespace Confirmit.CATI.IntegrationTests.Tests.InstallationCommon.Tools
{
    public class IisEngineTestHelper : IDisposable
    {
        private class VirtualFolderInfo
        {
            public string ApplicationName { get; private set; }
            public string VirtualFolderPath { get; private set; }
            public string PhysicalPath { get; private set; }

            public VirtualFolderInfo(string applicationName, string virtualFolderPath, string physicalPath)
            {
                ApplicationName = applicationName;
                VirtualFolderPath = virtualFolderPath;
                PhysicalPath = physicalPath;
            }
        }

        const string DefaultSiteName = IisTests.DefaultSiteName;

        public string RootPhysicalPath { get; private set; }
        private readonly List<KeyValuePair<string, string>> _createdApplications;
        private readonly List<VirtualFolderInfo> _createdVirtualFolders;
        private readonly List<string> _createdFolders;
        private readonly List<string> _createdPages;

        public IisEngineTestHelper()
        {
            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites[0];
                Application app = site.Applications["/"];

                RootPhysicalPath = Environment.ExpandEnvironmentVariables(app.VirtualDirectories["/"].PhysicalPath);
            }

            _createdApplications = new List<KeyValuePair<string, string>>();
            _createdVirtualFolders = new List<VirtualFolderInfo>();
            _createdFolders = new List<string>();
            _createdPages = new List<string>();
        }

        public void CreateApplication(string applicationPath, string physicalPath)
        {
            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites[0];

                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }

                site.Applications.Add(applicationPath, physicalPath);
                serverManager.CommitChanges();
                _createdApplications.Add(new KeyValuePair<string, string>(applicationPath, physicalPath));
            }
        }

        public void CreateVirtualFolder(string applicationName, string virtualFolderPath, string physicalPath)
        {
            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites[0];
                Application app = string.IsNullOrEmpty(applicationName) ? site.Applications[0] : site.Applications[applicationName];
                if (app == null)
                {
                    throw new Exception(string.Format("Root folder '{0}' does not exists", applicationName));
                }

                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }

                app.VirtualDirectories.Add(virtualFolderPath, physicalPath);
                serverManager.CommitChanges();
                _createdVirtualFolders.Add(new VirtualFolderInfo(app.Path, virtualFolderPath, physicalPath));
            }
        }

        public void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                _createdFolders.Add(folderPath);
            }
        }

        public void CreatePage(string pagePath)
        {
            string pageFolderPath = Path.GetDirectoryName(pagePath) ?? string.Empty;

            if (!Directory.Exists(pageFolderPath))
            {
                Directory.CreateDirectory(pageFolderPath);
            }

            File.WriteAllText(pagePath, "Test page");
            _createdPages.Add(pagePath);
        }

        public string GetCacheControlMode(string testVirtualDirectoryName)
        {
            using (var serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetWebConfiguration(DefaultSiteName + testVirtualDirectoryName);
                ConfigurationSection staticContentSection = config.GetSection("system.webServer/staticContent");

                ConfigurationElement clientCacheElement = staticContentSection.GetChildElement("clientCache");
                return clientCacheElement["cacheControlMode"].ToString();
            }
        }

        public string GetCacheControlMaxAge(string testVirtualDirectoryName)
        {
            using (var serverManager = new ServerManager())
            {
                Configuration config = serverManager.GetWebConfiguration(DefaultSiteName + testVirtualDirectoryName);
                ConfigurationSection staticContentSection = config.GetSection("system.webServer/staticContent");

                ConfigurationElement clientCacheElement = staticContentSection.GetChildElement("clientCache");
                return clientCacheElement["cacheControlMaxAge"].ToString();
            }
        }

        public void Dispose()
        {
            foreach (var createdPage in _createdPages)
            {
                if (File.Exists(createdPage))
                {
                    File.Delete(createdPage);
                    Thread.Sleep(100);
                }
            }

            foreach (var createdFolder in _createdFolders)
            {
                RemoveDirectory(createdFolder);                
            }

            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites[0];

                foreach (var createdVirtualFolder in _createdVirtualFolders)
                {
                    Application app = site.Applications[createdVirtualFolder.ApplicationName];
                    if(app.VirtualDirectories[createdVirtualFolder.VirtualFolderPath] != null)
                    {
                        app.VirtualDirectories[createdVirtualFolder.VirtualFolderPath].Delete();
                    }

                    RemoveDirectory(createdVirtualFolder.PhysicalPath);
                }
                

                foreach (var createdApplication in _createdApplications)
                {
                    site.Applications[createdApplication.Key].Delete();

                    RemoveDirectory(createdApplication.Value);
                }

                serverManager.CommitChanges();
            }
        }

        private void RemoveDirectory(string path)
        {
            while (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(ex.ToString());
                }
            }
        }
    }
}