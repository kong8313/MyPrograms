using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Xml;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Installation.Common
{
    public class ConfigsEngine
    {
        private readonly ILogger _logger;
        const string EncryptionProviderName = "RSAProtectedConfigurationProvider";

        public ConfigsEngine(ILogger logger)
        {
            _logger = logger;
        }


        private void SetAttribute(
            XmlDocument xmlDocument, string xpath, string attributeName, string attributeValue, XmlNamespaceManager nsmgr, bool doNotLogValue = false, string attributeNamespace = null)
        {
            _logger.WriteLog("Add attribute {0} with value {1} to node {2}", attributeName, doNotLogValue ? "***" : attributeValue, xpath);

            var selectedNode = (XmlElement)xmlDocument.SelectSingleNode(xpath, nsmgr);

            if (selectedNode != null)
            {
                if (string.IsNullOrEmpty(attributeNamespace))
                {
                    selectedNode.SetAttribute(attributeName, attributeValue);
                }
                else
                {
                    selectedNode.SetAttribute(attributeName, attributeNamespace, attributeValue);
                }
                
                _logger.WriteLog("Succeded");
            }
        }

        private void RemoveAttribute(XmlDocument xmlDocument, string xpath, string attributeName, XmlNamespaceManager nsmgr)
        {
            _logger.WriteLog("Remove attribute {0} from node {1}", attributeName, xpath);

            var selectedNode = (XmlElement)xmlDocument.SelectSingleNode(xpath, nsmgr);

            if (selectedNode != null)
            {
                selectedNode.RemoveAttribute(attributeName);
                _logger.WriteLog("Succeded");
            }
        }

        private void AddNode(XmlDocument xmlDocument, string xpath, string newNodeName, XmlNamespaceManager nsmgr)
        {
            _logger.WriteLog("Add node {0} to node {1}", newNodeName, xpath);

            XmlNode selectedNode = xmlDocument.SelectSingleNode(xpath, nsmgr);

            if (selectedNode != null)
            {
                selectedNode.AppendChild(xmlDocument.CreateElement(newNodeName));
                _logger.WriteLog("Succeded");
            }
        }


        private void RemoveNode(XmlDocument xmlDocument, string xpath, XmlNamespaceManager nsmgr)
        {
            _logger.WriteLog("Remove node {0}", xpath);

            XmlNode node2Remove = xmlDocument.SelectSingleNode(xpath, nsmgr);

            if (node2Remove != null && node2Remove.ParentNode != null)
            {
                node2Remove.ParentNode.RemoveChild(node2Remove);
                _logger.WriteLog("Succeded");
            }
        }


        private string GetAttributeValue(XmlDocument xmlDocument, string xpath, string attributeName, XmlNamespaceManager nsmgr)
        {
            _logger.WriteLog("Get attribute {0} from node {1}", attributeName, xpath);

            var selectedNode = (XmlElement)xmlDocument.SelectSingleNode(xpath, nsmgr);

            if (selectedNode != null)
            {
                string attributeValue = selectedNode.GetAttribute(attributeName);
                _logger.WriteLog("Succeded");
                return attributeValue;
            }

            return string.Empty;
        }


        /// <summary>
        /// Configure Supervisor config file
        /// </summary>
        /// <param name="supervisorLocation">Supervisor install location</param>
        /// <param name="catiConnectionString">Default database connection string</param>
        /// <param name="sessionStateMode">Session state mode: InProc/SQLServer/Redis</param>
        /// <param name="redisHostName">Redis host name</param>
        /// <param name="redisPassword">Redis password</param>
        /// <param name="sessionStateConnectionString">Session state connection string</param>
        /// <param name="sessionStateCookieName">Session state cookie name</param>
        /// <param name="confirmitKeepSessionAspxUrl">Confirmit keep session aspx url</param>
        /// <param name="igResFolderName">ig_res folder name</param>
        /// <param name="confirmitLogPath">Path for kibana logging</param>
        public void ConfigureSupervisorConfig(
            string supervisorLocation,
            string catiConnectionString,
            string sessionStateMode,
            string redisHostName,
            string redisPassword,
            string sessionStateConnectionString,
            string sessionStateCookieName,
            string confirmitKeepSessionAspxUrl,
            string igResFolderName, 
            string confirmitLogPath)
        {
            _logger.WriteLog("Begin ConfigureSupervisorConfig");

            try
            {
                string configPath = Path.Combine(supervisorLocation, "Web.config");

                if (sessionStateMode == "Redis")
                {
                    string configContent = File.ReadAllText(configPath);

                    configContent = configContent.Replace("<!--BeginNotRedisSection-->", "<!--BeginNotRedisSection");
                    configContent = configContent.Replace("<!--EndNotRedisSection-->", "EndNotRedisSection-->");

                    configContent = configContent.Replace("<!--BeginRedisSection", "<!--BeginRedisSection-->");
                    configContent = configContent.Replace("EndRedisSection-->", "<!--EndRedisSection-->");

                    File.WriteAllText(configPath, configContent);
                }

                var xmlDocument = new XmlDocument
                {
                    PreserveWhitespace = true
                };
                xmlDocument.Load(configPath);

                var nsMgr = new XmlNamespaceManager(xmlDocument.NameTable);

                if (sessionStateMode != "InProc")
                {
                    SetAttribute(xmlDocument, "//configuration/system.web/sessionState", "cookieName", sessionStateCookieName, nsMgr);
                }

                if (sessionStateMode != "Redis")
                {
                    SetAttribute(xmlDocument, "//configuration/system.web/sessionState", "mode", sessionStateMode, nsMgr);
                }

                if (sessionStateMode == "InProc")
                {
                    RemoveAttribute(xmlDocument, "//configuration/system.web/sessionState", "cookieName", nsMgr);

                    RemoveAttribute(xmlDocument, "//configuration/system.web/sessionState", "sqlConnectionString", nsMgr);
                }
                else if (sessionStateMode == "Redis")
                {
                    var redisConnectionString = string.Format("{0},password={1},ssl=False,abortConnect=False", redisHostName, redisPassword);
                    SetAttribute(xmlDocument, "//configuration/system.web/sessionState/providers/add[@name='RedisSessionStateProvider']", "connectionString", redisConnectionString, nsMgr, true);
                }
                else if (sessionStateMode == "SQLServer")
                {
                    SetAttribute(xmlDocument, "//configuration/system.web/sessionState", "sqlConnectionString", sessionStateConnectionString, nsMgr, true);
                }

                SetAttribute(xmlDocument, "//configuration/Telerik.Reporting/Cache/Providers/Provider/Parameters/Parameter[@name='ConnectionString']", "value", catiConnectionString, nsMgr, true);

                SetAttribute(xmlDocument, "//configuration/appSettings/add[@key='ConfirmitKeepSessionAspxUrl']", "value", confirmitKeepSessionAspxUrl, nsMgr);

                SetAttribute(xmlDocument, "//configuration/infragistics.web", "styleSetPath", "~/" + igResFolderName, nsMgr);

                SetAttribute(xmlDocument, "//configuration/appSettings/add[@key='Logging.Path']", "value", confirmitLogPath, nsMgr);

                SetAttribute(xmlDocument, "//configuration/appSettings/add[@key='DebugMode']", "value", "false", nsMgr);

                xmlDocument.PreserveWhitespace = false;
                xmlDocument.Save(configPath);
            }
            finally
            {
                _logger.WriteLog("End ConfigureSupervisorConfig");
            }
        }


        private static string RemoveComment(string pattern, string configText)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.Replace(configText, string.Empty);
        }


        /// <summary>
        /// Configure Backend config file
        /// </summary>
        /// <param name="installLocation">Backend install location</param>
        /// <param name="isLoadBalancedEnvironment">true - if accelerator use, false - otherwise</param>
        /// <param name="confirmitLogPath">Path for kibana logging</param>
        public void ConfigureBackendConfig(
            string installLocation,
            bool isLoadBalancedEnvironment,
            string confirmitLogPath)
        {
            _logger.WriteLog("Begin ConfigureBackendConfig");

            try
            {
                string configPath = Path.Combine(installLocation, "Confirmit.CATI.Backend.exe.config");

                var xmlDocument = new XmlDocument
                {
                    PreserveWhitespace = true
                };
                xmlDocument.Load(configPath);

                var nsMgr = new XmlNamespaceManager(xmlDocument.NameTable);

                SetAttribute(xmlDocument, "//configuration/appSettings/add[@key='Logging.Path']", "value", confirmitLogPath, nsMgr);

                xmlDocument.PreserveWhitespace = false;
                xmlDocument.Save(configPath);

                if (!isLoadBalancedEnvironment)
                {
                    return;
                }

                string configText = File.ReadAllText(configPath);

                const string useLabelBegin = "<!--UseSSLAccelerator_Part{0}_Begin-->";
                const string useLabelEnd = "<!--UseSSLAccelerator_Part{0}_End-->";
                const string doNotUseLabelBegin = "<!--Don'tUseSSLAccelerator_Part{0}_Begin-->";
                const string doNotUseLabelEnd = "<!--Don'tUseSSLAccelerator_Part{0}_End-->";

                configText = RemoveComment(string.Format(useLabelBegin, "1") + @"\s*\r\n\s*<!--", configText);
                configText = RemoveComment(string.Format(useLabelBegin, "2") + @"\s*\r\n\s*<!--", configText);
                configText = RemoveComment(@"-->\s*\r\n\s*" + string.Format(useLabelEnd, "1"), configText);
                configText = RemoveComment(@"-->\s*\r\n\s*" + string.Format(useLabelEnd, "2"), configText);

                configText = configText.Replace(string.Format(doNotUseLabelBegin, "1"), "<!--");
                configText = configText.Replace(string.Format(doNotUseLabelBegin, "2"), "<!--");
                configText = configText.Replace(string.Format(doNotUseLabelEnd, "1"), "-->");
                configText = configText.Replace(string.Format(doNotUseLabelEnd, "2"), "-->");

                File.WriteAllText(configPath, configText);
            }
            finally
            {
                _logger.WriteLog("End ConfigureBackendConfig");
            }
        }

        /// <summary>
        /// Find and comment all parameters like the following: <add name="traceListenerName"/>
        /// </summary>
        /// <param name="traceListenerName">Name of trace listener</param>
        /// <param name="configText">Content of config file</param>
        /// <returns></returns>
        private string CommentListeners(string traceListenerName, string configText)
        {
            int n = 0;
            do
            {
                n = configText.IndexOf(traceListenerName, n, StringComparison.Ordinal);
                if (n == -1)
                {
                    break;
                }

                int startIndex = n - 1;
                while (configText[startIndex] != '<')
                {
                    startIndex--;
                }

                int endIndex = n + traceListenerName.Length;
                while (configText[endIndex] != '>')
                {
                    endIndex++;
                }

                n = endIndex + 1;
                string traceListenerParameter = configText.Substring(startIndex, endIndex - startIndex + 1);

                if (traceListenerParameter.Replace(" ", "") == "<addname=\"" + traceListenerName + "\"/>")
                {
                    configText = configText.Substring(0, startIndex) + "<!--" + traceListenerParameter + "-->" + configText.Substring(endIndex + 1);
                    n += 7;
                }
            }
            while (true);


            return configText;
        }


        /// <summary>
        /// Disable file logging
        /// </summary>
        /// <param name="installLocation">Installation location</param>
        public void DisableFileLogging(
            string installLocation)
        {
            _logger.WriteLog("Begin ConfigureTciConfig");

            try
            {
                string configPath = Path.Combine(installLocation, "web.config");
                const string fileTraceListenerName = "DialerLogFileListener";

                string configText = File.ReadAllText(configPath);

                configText = CommentListeners(fileTraceListenerName, configText);

                File.WriteAllText(configPath, configText);
            }
            finally
            {
                _logger.WriteLog("End ConfigureTciConfig");
            }
        }


        public MachineConfigProperties GetMachineConfigProperties(string machineConfigPath)
        {
            _logger.WriteLog("Begin GetMachineConfigProperties");
            try
            {
                const string processModelXPath = "//configuration/system.web/processModel";
                const string httpRuntimeXPath = "//configuration/system.web/httpRuntime";

                var xmlDocument = new XmlDocument
                {
                    PreserveWhitespace = true
                };
                xmlDocument.Load(machineConfigPath);

                var nsMgr = new XmlNamespaceManager(xmlDocument.NameTable);

                string minWorkerThreads = GetAttributeValue(xmlDocument, processModelXPath, "minWorkerThreads", nsMgr);
                string maxWorkerThreads = GetAttributeValue(xmlDocument, processModelXPath, "maxWorkerThreads", nsMgr);
                string minIoThreads = GetAttributeValue(xmlDocument, processModelXPath, "minIoThreads", nsMgr);
                string maxIoThreads = GetAttributeValue(xmlDocument, processModelXPath, "maxIoThreads", nsMgr);
                string minFreeThreads = GetAttributeValue(xmlDocument, httpRuntimeXPath, "minFreeThreads", nsMgr);
                string minLocalRequestFreeThreads = GetAttributeValue(xmlDocument, httpRuntimeXPath, "minLocalRequestFreeThreads", nsMgr);

                return new MachineConfigProperties(MachineConfigChangingState.DoNotChange, minWorkerThreads, maxWorkerThreads, minIoThreads, maxIoThreads, minFreeThreads, minLocalRequestFreeThreads);
            }
            finally
            {
                _logger.WriteLog("End GetMachineConfigProperties");
            }
        }


        public void ConfigureMachineConfig(string machineConfigPath, MachineConfigProperties machineConfigProperties, string currentVersion)
        {
            _logger.WriteLog("Begin ConfigureMachineConfig");

            try
            {
                if (machineConfigProperties.MachineConfigChanging == MachineConfigChangingState.DoNotChange)
                {
                    _logger.WriteLog("MachineConfigChanging is DoNotChange. Do nothing.");
                    return;
                }

                string saveMachineConfigPath = string.Format("{0}_{1}_{2}.save", machineConfigPath, currentVersion, DateTime.Now.ToString("yyyy.MM.dd_hh_mm_ss"));
                if (!File.Exists(saveMachineConfigPath))
                {
                    _logger.WriteLog("Saving current machine config to {0}", saveMachineConfigPath);
                    File.Copy(machineConfigPath, saveMachineConfigPath);
                }

                const string processModelXPath = "//configuration/system.web/processModel";
                const string httpRuntimeXPath = "//configuration/system.web/httpRuntime";

                var xmlDocument = new XmlDocument
                {
                    PreserveWhitespace = true
                };
                xmlDocument.Load(machineConfigPath);

                var nsMgr = new XmlNamespaceManager(xmlDocument.NameTable);

                RemoveNode(xmlDocument, processModelXPath, nsMgr);
                RemoveNode(xmlDocument, httpRuntimeXPath, nsMgr);
                AddNode(xmlDocument, "//configuration/system.web", "processModel", nsMgr);

                if (machineConfigProperties.MachineConfigChanging == MachineConfigChangingState.SetCustomSettings)
                {
                    SetAttribute(xmlDocument, processModelXPath, "autoConfig", "false", nsMgr);
                    SetAttribute(xmlDocument, processModelXPath, "maxWorkerThreads", machineConfigProperties.MaxWorkerThreads, nsMgr);
                    SetAttribute(xmlDocument, processModelXPath, "maxIoThreads", machineConfigProperties.MaxIoThreads, nsMgr);
                    SetAttribute(xmlDocument, processModelXPath, "minWorkerThreads", machineConfigProperties.MinWorkerThreads, nsMgr);
                    SetAttribute(xmlDocument, processModelXPath, "minIoThreads", machineConfigProperties.MinIoThreads, nsMgr);

                    AddNode(xmlDocument, "//configuration/system.web", "httpRuntime", nsMgr);
                    SetAttribute(xmlDocument, httpRuntimeXPath, "minFreeThreads", machineConfigProperties.MinFreeThreads, nsMgr);
                    SetAttribute(xmlDocument, httpRuntimeXPath, "minLocalRequestFreeThreads", machineConfigProperties.MinLocalRequestFreeThreads, nsMgr);
                }
                else if (machineConfigProperties.MachineConfigChanging == MachineConfigChangingState.SetAutoConfigTrue)
                {
                    SetAttribute(xmlDocument, processModelXPath, "autoConfig", "true", nsMgr);
                }

                xmlDocument.PreserveWhitespace = false;
                xmlDocument.Save(machineConfigPath);
            }
            finally
            {
                _logger.WriteLog("End ConfigureMachineConfig");
            }
        }

        private Configuration OpenConfigFile(string configPath, string supervisorAlias)
        {
            var configFile = new FileInfo(configPath);
            var vdm = new VirtualDirectoryMapping(configFile.DirectoryName, true, configFile.Name);
            var wcfm = new WebConfigurationFileMap();
            wcfm.VirtualDirectories.Add("/", vdm);
            return WebConfigurationManager.OpenMappedWebConfiguration(wcfm, "/", supervisorAlias);
        }

        public void EncryptSection(string configPath, string supervisorAlias, string sectionName)
        {
            _logger.WriteLog("Begin EncryptSection");

            try
            {
                Configuration currentConfig = OpenConfigFile(configPath, supervisorAlias);
                ConfigurationSection section = currentConfig.GetSection(sectionName);

                if (section == null)
                {
                    throw new Exception("Section '" + sectionName + "' was not found");
                }

                if (!section.SectionInformation.IsProtected)
                {
                    _logger.WriteLog("Encrypt section '{0}' in config '{1}'", sectionName, configPath);
                    section.SectionInformation.ProtectSection(EncryptionProviderName);
                    currentConfig.Save();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(TraceEventType.Error, ex.ToString());
                throw new Exception(string.Format("Cannot encrypt section '{0}' in web.config", sectionName), ex);
            }
            finally
            {
                _logger.WriteLog("End EncryptSection");
            }
        }
    }
}
