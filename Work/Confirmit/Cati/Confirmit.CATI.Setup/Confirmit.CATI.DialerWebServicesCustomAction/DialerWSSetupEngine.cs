using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Confirmit.CATI.DialerWebServices.CustomAction.Properties;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using CustomActionLibrary;

using Microsoft.Web.Administration;

using Binding = Microsoft.Web.Administration.Binding;

namespace Confirmit.CATI.DialerWebServices.CustomAction
{
    public class DialerWSSetupEngine : SetupEngine
    {

        public DialerWSSetupEngine(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Configure dumpCreator.cmd file
        /// </summary>
        /// <param name="iisEngine"></param>
        /// <param name="dumpCreationMode">Dump creation mode</param>        
        /// <param name="dialerAppPoolName">Dialer app pool name</param>
        /// <param name="dumpCmdFilePath">Path to dumpCreator.cmd file</param>
        /// <param name="procDumpFilePath">Path to ProcDump.exe utility</param>
        /// <param name="procDumpLogFolderPath">Path to dumps folder of ProcDump.exe utility</param>     
        /// <param name="procDumpAdditionalParameters">Additional parameters of ProcDump.exe utility</param>   
        /// <param name="currentVersion">Current version</param>        
        public void ConfigureOrphaning(
            IISEngine iisEngine, DumpCreationOptions dumpCreationMode, string dialerAppPoolName, string dumpCmdFilePath, 
            string procDumpFilePath, string procDumpLogFolderPath, string procDumpAdditionalParameters, string currentVersion)
        {
            iisEngine.SetOrphaningForAppPool(dumpCreationMode, dialerAppPoolName, dumpCmdFilePath);

            if (dumpCreationMode == DumpCreationOptions.CreateDump)
            {
                ConfigureDumpCmdFile(dumpCmdFilePath, procDumpFilePath, procDumpLogFolderPath, procDumpAdditionalParameters, currentVersion);
            }
        }

        /// <summary>
        /// Configure dumpCreator.cmd file
        /// </summary>
        /// <param name="dumpCmdFilePath">Path to dumpCreator.cmd file</param>
        /// <param name="procDumpFilePath">Path to ProcDump.exe utility</param>
        /// <param name="procDumpLogFolderPath">Path to dumps folder of ProcDump.exe utility</param>        
        /// <param name="procDumpAdditionalParameters">Additional parameters of ProcDump.exe utility</param>        
        /// <param name="currentVersion">Current version</param>        
        private static void ConfigureDumpCmdFile(string dumpCmdFilePath, string procDumpFilePath, string procDumpLogFolderPath, string procDumpAdditionalParameters, string currentVersion)
        {
            string fileContent = File.ReadAllText(dumpCmdFilePath);
            
            if (!procDumpLogFolderPath.EndsWith("\\"))
            {
                procDumpLogFolderPath += "\\";
            }

            if (!procDumpAdditionalParameters.Contains("accepteula"))
            {
                procDumpAdditionalParameters += " -accepteula";
            }
     
            fileContent = SetParameterValue(fileContent, "<ROOT_DUMP_FOLDER_PATH>", procDumpLogFolderPath);
            fileContent = SetParameterValue(fileContent, "<PROCDUMP_PATH>", procDumpFilePath);
            fileContent = SetParameterValue(fileContent, "<ADDITIONAL_PARAMETERS>", procDumpAdditionalParameters);
            fileContent = SetParameterValue(fileContent, "<CURRENT_VERSION>", currentVersion);

             File.WriteAllText(dumpCmdFilePath, fileContent);            
        }        

        private static string SetParameterValue(string fileContent, string searchString, string value)
        {
            int pos = fileContent.IndexOf(searchString, StringComparison.Ordinal);

            if (pos == -1)
            {
                throw new Exception(string.Format(Resources.WrongFormatOfDurmpCreatorCmdFile, searchString));
            }

            return fileContent.Substring(0, pos) + value + fileContent.Substring(pos + searchString.Length);
        }        

        /// <summary>
        /// Configure IIS
        /// </summary>
        /// <param name="iisEngine"></param>
        /// <param name="dialerSiteID">Site ID</param>
        /// <param name="dialerAliaseName">Aliase name</param>
        /// <param name="dialerAppPoolName">Dialer app pool name</param>
        /// <param name="isWin64">true if this is x64 installation, otherwise - false</param>        
        /// <param name="productName">Product name</param>
        public void ConfigureIIS(IISEngine iisEngine, string dialerSiteID, string dialerAliaseName, string dialerAppPoolName, bool isWin64, string productName)
        {
            Logger.WriteLog("Start ConfigureIIS");

            string x64Str = isWin64 ? "64" : string.Empty;
            string regiis = Path.Combine(SystemRoot, string.Format(@"Microsoft.NET\Framework{0}\v4.0.30319\aspnet_regiis.exe", x64Str));

            Logger.WriteLog("x64Str={0}\r\nregiis={1}\r\nEnvironment.OSVersion.Version.Major={2}", x64Str, regiis, Environment.OSVersion.Version.Major);

            if (!string.IsNullOrEmpty(x64Str))
            {
                iisEngine.DisableRunningOf32BitApplicationsForAppPools(productName, dialerAppPoolName);
            }

            //
            // Change IIS version to 4.0 for instaleld web service alias
            // If our web application has worked with asp 1.1. - it will work with asp 4.0 
            // If we have an error, we will register this version of IIS and try one more time
            //
            string regiisArgs = "-s W3SVC/" + dialerSiteID + "/ROOT/" + dialerAliaseName;
            try
            {
                ExternalInvoker.Invoke(regiis, regiisArgs, 10000);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(TraceEventType.Error, ex.ToString());

                ExternalInvoker.Invoke(regiis, "-ir");

                ExternalInvoker.Invoke(regiis, regiisArgs, 10000);
            }

            Logger.WriteLog("Finish ConfigureIIS");
        }

        /// <summary>
        /// Get path to machine.config file
        /// if x64 framework has installed on this computer - return path to machine config from x64 framework version,
        /// otherwise return path to machine config from x86 version
        /// </summary>
        /// <returns></returns>
        public string GetMachineConfigPath()
        {
            string winPath = Environment.GetEnvironmentVariable("windir") ?? @"c:\windows";
            string x86MachineConfigPath = Path.Combine(winPath, @"Microsoft.NET\Framework\v4.0.30319\CONFIG\machine.config");
            string x64MachineConfigPath = Path.Combine(winPath, @"Microsoft.NET\Framework64\v4.0.30319\CONFIG\machine.config");

            if (File.Exists(x64MachineConfigPath))
            {
                return x64MachineConfigPath;
            }

            return x86MachineConfigPath;
        }

        public string GetCurrectValueLabel(string currentValue)
        {
            return "\r\n(" + (string.IsNullOrEmpty(currentValue)
                ? "No current value)"
                : string.Format("Current value is {0})", currentValue));
        }


        /// <summary>
        /// Configure binding for https for web site with selected certificate
        /// </summary>
        /// <param name="dialerSiteID">Site ID</param>
        /// <param name="certificateHash">Certificate hash</param>
        public void ConfigureCertificateForIIS(int dialerSiteID, string certificateHash)
        {
            // These actions are splited, because there is a problem with certificate changing
            // We need to remove an existed binding and then add a new one
            // But "serverManager.CommitChanges();" can be used only once in "using (var serverManager = new ServerManager())"
            RemoveHttpsBinding(dialerSiteID);
            AddHttpsBinding(dialerSiteID, certificateHash);
        }

        private void RemoveHttpsBinding(int dialerSiteID)
        {
            using (var serverManager = new ServerManager())
            {
                Site site = serverManager.Sites.FirstOrDefault(s => s.Id == dialerSiteID);
                if (site != null)
                {
                    Binding binding = site.Bindings.FirstOrDefault(s => s.Protocol == "https");

                    if (binding != null)
                    {
                        site.Bindings.Remove(binding);
                        serverManager.CommitChanges();
                    }
                }
            }
        }

        private void AddHttpsBinding(int dialerSiteID, string certificateThumbprint)
        {
            using (var serverManager = new ServerManager())
            {
                Site site = serverManager.Sites.FirstOrDefault(s => s.Id == dialerSiteID);
                if (site != null)
                {
                    BindingCollection bindingCollection = site.Bindings;

                    Binding binding = site.Bindings.CreateElement("binding");
                    binding.Protocol = "https";
                    binding.BindingInformation = "*:443:";
                    binding.CertificateStoreName = "MY";
                    binding["certificateHash"] = certificateThumbprint;

                    bindingCollection.Add(binding);

                    serverManager.CommitChanges();
                }
            }
        }


        public string GetCertificateThumbprintForHttpsBinding(int webSiteId)
        {
            using (var serverManager = new ServerManager())
            {
                Site site = serverManager.Sites.FirstOrDefault(s => s.Id == webSiteId);
                if (site != null)
                {
                    Binding binding = site.Bindings.FirstOrDefault(s => s.Protocol == "https");

                    if (binding != null)
                    {
                        return binding["certificateHash"].ToString();
                    }
                }
            }

            return string.Empty;
        }
    }
}
