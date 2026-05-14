using System.IO;
using System.Xml;

namespace Confirmit.CATI.IntegrationTests.Tests.InstallationCommon.Tools
{
    public class ConfigVerifier
    {
        private readonly string _configPath;
        private XmlDocument _xmlDocument;
        private XmlNamespaceManager _xmlNamespaceManager;

        public ConfigVerifier(string configPath)
        {
            _configPath = configPath;

            File.Copy(configPath, configPath + ".save", true);
        }

        private void LoadSupervisorConfig()
        {
            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(_configPath);
            _xmlNamespaceManager = new XmlNamespaceManager(_xmlDocument.NameTable);
        }

        private void LoadBackendConfig()
        {
            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(_configPath);
            _xmlNamespaceManager = new XmlNamespaceManager(_xmlDocument.NameTable);
        }

        private void LoadConsoleManifestFile()
        {
            _xmlDocument = new XmlDocument();
            _xmlDocument.Load(_configPath);

            _xmlNamespaceManager = new XmlNamespaceManager(_xmlDocument.NameTable);
            _xmlNamespaceManager.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
            _xmlNamespaceManager.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");
        }

        private string GetAttributeValue(string xpath, string attributeName)
        {
            var selectedNode = (XmlElement)_xmlDocument.SelectSingleNode(xpath, _xmlNamespaceManager);

            if (selectedNode != null)
            {
                return selectedNode.GetAttribute(attributeName);
            }

            return string.Empty;
        }

        public void RestoreConfig()
        {
            File.Copy(_configPath + ".save", _configPath, true);
            File.Delete(_configPath + ".save");
        }
        
        public string GetCatiConnectionString()
        {
            LoadSupervisorConfig();

            return GetAttributeValue("//configuration/Telerik.Reporting/Cache/Providers/Provider/Parameters/Parameter[@name='ConnectionString']", "value");
        }

        public string GetSessionStateMode()
        {
            LoadSupervisorConfig();

            return GetAttributeValue("//configuration/system.web/sessionState", "mode");
        }

        public string GetRedisConnectionString()
        {
            LoadSupervisorConfig();

            return GetAttributeValue("//configuration/system.web/sessionState/providers/add[@name='RedisSessionStateProvider']", "connectionString");
        }

        public string GetSessionStateConnectionString()
        {
            LoadSupervisorConfig();

            return GetAttributeValue("//configuration/system.web/sessionState", "sqlConnectionString");
        }

        public string GetSessionStateCookieName()
        {
            LoadSupervisorConfig();

            return GetAttributeValue("//configuration/system.web/sessionState", "cookieName");
        }

        public string GetConfirmitKeepSessionAspxUrl()
        {
            LoadSupervisorConfig();

            return GetAttributeValue("//configuration/appSettings/add[@key='ConfirmitKeepSessionAspxUrl']", "value");
        }

        public string GetIgResFolderName()
        {
            LoadSupervisorConfig();

            return GetAttributeValue("//configuration/infragistics.web", "styleSetPath");
        }

        public string GetConfirmitLogPath()
        {
            LoadSupervisorConfig();

            return GetAttributeValue("//configuration/appSettings/add[@key='Logging.Path']", "value");
        }

        public bool IsSSLEnabled()
        {
            string configContent = File.ReadAllText(_configPath);

            if (configContent.Contains("UseSSLAccelerator_Part") || configContent.Contains("Don'tUseSSLAccelerator_Part"))
            {
                return false;
            }

            return true;
        }

        public string GetDeploymentProviderFromApplicationFile()
        {
            LoadConsoleManifestFile();

            return GetAttributeValue("//asmv1:assembly/asmv2:deployment/asmv2:deploymentProvider", "codebase");
        }

        public string GetAssemblyIdentityFromApplicationFile()
        {
            LoadConsoleManifestFile();

            return GetAttributeValue("//asmv1:assembly/asmv1:assemblyIdentity", "name");
        }

        public string GetShortcutInfoFromApplicationFile()
        {
            LoadConsoleManifestFile();

            return GetAttributeValue("//asmv1:assembly/asmv2:deployment", "co.v1:createDesktopShortcut");
        }

        public string GetDescriptionFromApplicationFile()
        {
            LoadConsoleManifestFile();

            return GetAttributeValue("//asmv1:assembly/asmv1:description", "asmv2:product");
        }

        public string GetAssemblyIdentityFromManifestFile()
        {
            LoadConsoleManifestFile();

            return GetAttributeValue("//asmv1:assembly/asmv1:assemblyIdentity", "name");
        }

        public string GetAssemblyIdentityInEntryPointFromManifestFile()
        {
            LoadConsoleManifestFile();

            return GetAttributeValue("//asmv1:assembly/asmv2:entryPoint/asmv2:assemblyIdentity", "name");
        }

        public string GetCommandLineFileInEntryPointFromManifestFile()
        {
            LoadConsoleManifestFile();

            return GetAttributeValue("//asmv1:assembly/asmv2:entryPoint/asmv2:commandLine", "file");
        }

        public string GetAssemblyIdentityInDependencyFromManifestFile(string assemblyIdentityName)
        {
            LoadConsoleManifestFile();

            return GetAttributeValue(string.Format("//asmv1:assembly/asmv2:dependency/asmv2:dependentAssembly[@codebase='{0}']/asmv2:assemblyIdentity", assemblyIdentityName), "name");
        }

        public string GetConfigSizeFromManifestFile(string assemblyIdentityName)
        {
            LoadConsoleManifestFile();

            return GetAttributeValue(string.Format("//asmv1:assembly/asmv2:file[@name='{0}.config']", assemblyIdentityName), "size");
        }
    }
}