using System.Diagnostics;
using BootstrapperLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.Win32;

namespace BootstrapperLibrary
{
    public class DialerWSMsiParametersReader : IParametersReader
    {
        protected readonly string RegistryPath;
        private readonly DialerWSMsiParameters _parameters;
        protected readonly ILogger Logger;

        public DialerWSMsiParametersReader(DialerWSMsiParameters parameters, ILogger logger, string registryPath)
        {
            _parameters = parameters;
            Logger = logger;
            RegistryPath = registryPath;
        }

        public virtual void ReadParameters(ReadingInstallationParameters readingInstallationParameters)
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(RegistryPath);
            if (regKey == null)
            {
                throw new MessageException($"Registry path 'HKLM\\{RegistryPath}' is not found", TraceEventType.Warning);
            }

            string[] subRegKeys = regKey.GetValueNames();

            foreach (string subRegKey in subRegKeys)
            {
                switch (subRegKey)
                {
                    case "APP_POOL_NAME":
                        _parameters.AppPoolName = GetValue(regKey, subRegKey);
                        break;
                    case "WEB_SITE_ID":
                        _parameters.WebSiteId = GetValue(regKey, subRegKey);
                        break;
                    case "WEB_SITE_NAME":
                        _parameters.WebSiteName = GetValue(regKey, subRegKey);
                        break;
                    case "IS_SET_RECYCLING_VALUE_TO_ZERO":
                        _parameters.IsSetRecyclingValueToZero = GetValue(regKey, subRegKey);
                        break;
                    case "CATI_SERVER_ADDRESS":
                        _parameters.CatiServerAddress = GetValue(regKey, subRegKey);
                        break;
                    case "MONITOR_ROOT":
                        _parameters.MonitorRoot = GetValue(regKey, subRegKey);
                        break;
                    case "URL_ROOT":
                        _parameters.UrlRoot = GetValue(regKey, subRegKey);
                        break;
                    case "FILE_NAME_PATTERN":
                        _parameters.FileNamePattern = GetValue(regKey, subRegKey);
                        break;
                    case "IS_FILE_LOGGING_ENABLED":
                        _parameters.IsFileLoggingEnabled = GetValue(regKey, subRegKey);
                        break;
                    case "LOG_FILE_NAME":
                        _parameters.LogFileName = GetValue(regKey, subRegKey);
                        break;
                    case "LOG_FILE_PATH":
                        _parameters.LogFilePath = GetValue(regKey, subRegKey);
                        break;
                    case "MACHINE_CONFIG_CHANGING":
                        _parameters.MachineConfigChanging = GetValue(regKey, subRegKey);
                        break;
                    case "MIN_WORKER_THREADS":
                        _parameters.MinWorkerThreads = GetValue(regKey, subRegKey);
                        break;
                    case "MAX_WORKER_THREADS":
                        _parameters.MaxWorkerThreads = GetValue(regKey, subRegKey);
                        break;
                    case "MIN_IO_THREADS":
                        _parameters.MinIoThreads = GetValue(regKey, subRegKey);
                        break;
                    case "MAX_IO_THREADS":
                        _parameters.MaxIoThreads = GetValue(regKey, subRegKey);
                        break;
                    case "MIN_FREE_THREADS":
                        _parameters.MinFreeThreads = GetValue(regKey, subRegKey);
                        break;
                    case "MIN_LOCAL_REQUEST_FREE_THREADS":
                        _parameters.MinLocalRequestFreeThreads = GetValue(regKey, subRegKey);
                        break;
                    case "MAX_CONNECTION_COUNT":
                        _parameters.MaxConnectionCount = GetValue(regKey, subRegKey);
                        break;
                    case "DUMP_CREATION_OPTIONS":
                        _parameters.DumpCreationOptions = GetValue(regKey, subRegKey);
                        break;
                    case "PROCDUMP_FILE_PATH":
                        _parameters.ProcdumpFilePath = GetValue(regKey, subRegKey);
                        break;
                    case "PROCDUMP_LOG_FOLDER_PATH":
                        _parameters.ProcdumpLogFolderPath = GetValue(regKey, subRegKey);
                        break;
                    case "PROCDUMP_ADDITIONAL_PARAMETERS":
                        _parameters.ProcdumpAdditionalParameters = GetValue(regKey, subRegKey);
                        break;
                }
            }

            _parameters.InstallLocation = readingInstallationParameters.InstallLocation;
        }

        private string GetValue(RegistryKey regKey, string subRegKey)
        {
            return (string)regKey.GetValue(subRegKey);
        }
    }
}