using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.Web.Administration;

namespace Confirmit.CATI.Installation.Common
{
    public class IsAliveHtmEngine : IIsAliveHtmEngine
    {
        private readonly ILogger _logger;
        private readonly bool _ignoreIfIsAlivePageDoesNotExist;

        public IsAliveHtmEngine(ILogger logger)
            : this(logger, false)
        {
        }

        public IsAliveHtmEngine(ILogger logger, bool ignoreIfIsAlivePageDoesNotExist)
        {
            _logger = logger;
            _ignoreIfIsAlivePageDoesNotExist = ignoreIfIsAlivePageDoesNotExist;
        }

        public void VerifyAccesToPageByUrl(string urlAddress)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(urlAddress);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Wrong response status code:" + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!_ignoreIfIsAlivePageDoesNotExist)
                {
                    throw new ValidateException(string.Format("Page '{0}' is inaccesiible on {1}.", urlAddress, Environment.MachineName), ex);
                }

                _logger.WriteLog(TraceEventType.Warning, "An error occured during verifying of access to IsAlive.htm page:\r\n{0}\r\nIgnore this error because a variable 'Cati.LoadBalancer.IgnoreIfIsAlivePageDoesNotExist' is true", ex);
            }
        }

        public bool BackupIsAliveHtmFile(string isAlivePageUrl)
        {
            try
            {
                ChangeIsAliveHtmFileName(GetPhysicalPathToPage(isAlivePageUrl), true);
            }
            catch (Exception ex)
            {
                if (!_ignoreIfIsAlivePageDoesNotExist)
                {
                    throw;
                }

                _logger.WriteLog(TraceEventType.Warning, "An error occured during renaming (backup) of IsAlive.htm page:\r\n{0}\r\nIgnore this error because a variable 'Cati.LoadBalancer.IgnoreIfIsAlivePageDoesNotExist' is true", ex);
                return false;
            }

            return true;
        }

        public void RestoreIsAliveHtmFile(string isAlivePageUrl)
        {
            try
            {
                ChangeIsAliveHtmFileName(GetPhysicalPathToPage(isAlivePageUrl), false);
            }
            catch (Exception ex)
            {
                if (!_ignoreIfIsAlivePageDoesNotExist)
                {
                    throw;
                }

                _logger.WriteLog(TraceEventType.Warning, "An error occured during renaming (restore) of IsAlive.htm page (no file '___IsAlive.htm'):\r\n{0}\r\nIgnore this error because a variable 'Cati.LoadBalancer.IgnoreIfIsAlivePageDoesNotExist' is true", ex);
            }
        }

        private void ChangeIsAliveHtmFileName(string isAlivePagePath, bool doBackup)
        {
            _logger.WriteLog("Path to IsAlive.htm file: {0}", isAlivePagePath);

            string isAliveFolderPath = Path.GetDirectoryName(isAlivePagePath) ?? string.Empty;
            string isAlivePageName = Path.GetFileName(isAlivePagePath);
            string renamedIsAlivePath = Path.Combine(isAliveFolderPath, "___" + isAlivePageName);
            _logger.WriteLog("Path to renamed IsAlive.htm file: {0}", renamedIsAlivePath);

            if (doBackup)
            {
                if (!File.Exists(renamedIsAlivePath))
                {
                    File.Move(isAlivePagePath, renamedIsAlivePath);
                }

                // Remove isAlivePagePath if there were both files
                if (File.Exists(isAlivePagePath))
                {
                    File.Delete(isAlivePagePath);
                }
            }
            else
            {
                if (!File.Exists(isAlivePagePath))
                {
                    File.Move(renamedIsAlivePath, isAlivePagePath);
                }
            }
        }

        private string GetPhysicalPathToPage(string isAlivePageUrl)
        {
            isAlivePageUrl = CanonizeIsAlivePageUrl(isAlivePageUrl);

            string isAlivePageServerPath = GetServerPath(isAlivePageUrl);
            string isAlivePageName = GetPageName(isAlivePageUrl);

            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites[0];

                string isAliveApplicationPath = GetApplicationPath(isAlivePageServerPath, site);
                var application = site.Applications[isAliveApplicationPath];

                isAlivePageServerPath = MakeServerPathShorter(isAlivePageServerPath, isAliveApplicationPath.Length);
                string isAliveVirtaulDirectoryPath = GetVirtaulDirectoryPath(isAlivePageServerPath, application);
                var virtualDirectory = application.VirtualDirectories[isAliveVirtaulDirectoryPath];

                isAlivePageServerPath = MakeServerPathShorter(isAlivePageServerPath, isAliveVirtaulDirectoryPath.Length) + "/" + isAlivePageName;
                return Environment.ExpandEnvironmentVariables(Path.Combine(virtualDirectory.PhysicalPath, isAlivePageServerPath.TrimStart('/').Replace('/', '\\')));
            }
        }

        private string AddFirstSlash(string pageUrl)
        {
            if (!pageUrl.StartsWith("/"))
            {
                pageUrl = "/" + pageUrl;
            }

            return pageUrl;
        }

        private string GetServerPath(string isAlivePageUrl)
        {
            string serverPath = isAlivePageUrl.Substring(0, isAlivePageUrl.LastIndexOf('/'));
            if (string.IsNullOrEmpty(serverPath))
            {
                serverPath = "/";
            }

            return serverPath;
        }

        private string GetPageName(string isAlivePageUrl)
        {
            return isAlivePageUrl.Substring(isAlivePageUrl.LastIndexOf('/') + 1);
        }

        private string MakeServerPathShorter(string serverPath, int removeLength)
        {
            string newServerPath = serverPath.Substring(removeLength);
            return AddFirstSlash(newServerPath);
        }

        private string GetApplicationPath(string isAlivePageServerPath, Site site)
        {
            string applicationPath = isAlivePageServerPath;

            while (site.Applications.All(application => application.Path != applicationPath))
            {
                applicationPath = applicationPath.Substring(0, applicationPath.LastIndexOf('/'));

                if (string.IsNullOrEmpty(applicationPath))
                {
                    return "/";
                }
            }

            return applicationPath;
        }

        private string GetVirtaulDirectoryPath(string isAlivePageServerPath, Application application)
        {
            string virtualDirectoryPath = isAlivePageServerPath;

            while (application.VirtualDirectories.All(virtualDirectory => virtualDirectory.Path != virtualDirectoryPath))
            {
                virtualDirectoryPath = virtualDirectoryPath.Substring(0, virtualDirectoryPath.LastIndexOf('/'));

                if (string.IsNullOrEmpty(virtualDirectoryPath))
                {
                    return "/";
                }
            }

            return virtualDirectoryPath;
        }

        private string CanonizeIsAlivePageUrl(string isAlivePageUrl)
        {
            string newIsAlivePageUrl = isAlivePageUrl.Replace('\\', '/');
            return AddFirstSlash(newIsAlivePageUrl);
        }
    }
}