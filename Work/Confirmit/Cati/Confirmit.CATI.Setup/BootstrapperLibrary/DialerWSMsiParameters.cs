using System.Globalization;
using System.Text;

namespace BootstrapperLibrary
{
    public class DialerWSMsiParameters
    {
        public string InstallLocation { get; set; }
        public string AppPoolName { get; set; }
        public string WebSiteId { get; set; }
        public string WebSiteName { get; set; }
        public string IsSetRecyclingValueToZero { get; set; }
        public string CatiServerAddress { get; set; }
        public string MonitorRoot { get; set; }
        public string UrlRoot { get; set; }
        public string FileNamePattern { get; set; }
        public string IsFileLoggingEnabled { get; set; }
        public string LogFileName { get; set; }
        public string LogFilePath { get; set; }
        public string MachineConfigChanging { get; set; }
        public string MinWorkerThreads { get; set; }
        public string MaxWorkerThreads { get; set; }
        public string MinIoThreads { get; set; }
        public string MaxIoThreads { get; set; }
        public string MinFreeThreads { get; set; }
        public string MinLocalRequestFreeThreads { get; set; }
        public string MaxConnectionCount { get; set; }
        public string DumpCreationOptions { get; set; }
        public string ProcdumpFilePath { get; set; }
        public string ProcdumpLogFolderPath { get; set; }
        public string ProcdumpAdditionalParameters { get; set; }

        public virtual string GenerateInstallationParametersString(bool isQuietMode)
        {
            var paramStr = new StringBuilder(" /passive");
            paramStr.AppendFormat(" QUIET_MODE=\"{0}\"", isQuietMode.ToString(CultureInfo.InvariantCulture).ToUpper());

            paramStr.AppendFormat(" {0}=\"{1}\"", "INSTALL_LOCATION", InstallLocation);
            paramStr.AppendFormat(" {0}=\"{1}\"", "APP_POOL_NAME", AppPoolName);
            paramStr.AppendFormat(" {0}=\"{1}\"", "WEB_SITE_ID", WebSiteId);
            paramStr.AppendFormat(" {0}=\"{1}\"", "WEB_SITE_NAME", WebSiteName);
            paramStr.AppendFormat(" {0}=\"{1}\"", "IS_SET_RECYCLING_VALUE_TO_ZERO", IsSetRecyclingValueToZero);
            paramStr.AppendFormat(" {0}=\"{1}\"", "CATI_SERVER_ADDRESS", CatiServerAddress);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MONITOR_ROOT", MonitorRoot);
            paramStr.AppendFormat(" {0}=\"{1}\"", "URL_ROOT", UrlRoot);
            paramStr.AppendFormat(" {0}=\"{1}\"", "FILE_NAME_PATTERN", FileNamePattern);
            paramStr.AppendFormat(" {0}=\"{1}\"", "IS_FILE_LOGGING_ENABLED", IsFileLoggingEnabled);
            paramStr.AppendFormat(" {0}=\"{1}\"", "LOG_FILE_NAME", LogFileName);
            paramStr.AppendFormat(" {0}=\"{1}\"", "LOG_FILE_PATH", LogFilePath);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MACHINE_CONFIG_CHANGING", MachineConfigChanging);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MIN_WORKER_THREADS", MinWorkerThreads);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MAX_WORKER_THREADS", MaxWorkerThreads);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MIN_IO_THREADS", MinIoThreads);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MAX_IO_THREADS", MaxIoThreads);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MIN_FREE_THREADS", MinFreeThreads);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MIN_LOCAL_REQUEST_FREE_THREADS", MinLocalRequestFreeThreads);
            paramStr.AppendFormat(" {0}=\"{1}\"", "MAX_CONNECTION_COUNT", MaxConnectionCount);
            paramStr.AppendFormat(" {0}=\"{1}\"", "DUMP_CREATION_OPTIONS", DumpCreationOptions);
            paramStr.AppendFormat(" {0}=\"{1}\"", "PROCDUMP_FILE_PATH", ProcdumpFilePath);
            paramStr.AppendFormat(" {0}=\"{1}\"", "PROCDUMP_LOG_FOLDER_PATH", ProcdumpLogFolderPath);
            paramStr.AppendFormat(" {0}=\"{1}\"", "PROCDUMP_ADDITIONAL_PARAMETERS", ProcdumpAdditionalParameters);

            return paramStr.ToString();
        }
    }
}