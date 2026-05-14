using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TeamCityBuildEngine.CommonEngines;
using TeamCityBuildEngine.Interfaces;
using ILogger = TeamCityBuildEngine.Interfaces.ILogger;

namespace TeamCityBuildEngine
{
    public class InstallationGuide : Task
    {
        private ILogger _logger;
        private string _errorMessage;
        private string _buildNumber;
        private string _sideBySideName;
        private string _branchName;
        private string _testlabInstallerPassword;
        private string[] _computers;

        public string BuildNumber
        {
            set { _buildNumber = value; }
        }

        public string SideBySideName
        {
            set { _sideBySideName = value; }
        }

        public string BranchName
        {
            set { _branchName = value; }
        }

        public string TestlabInstallerPassword
        {
            set { _testlabInstallerPassword = value; }
        }
        
        public string Computers
        {
            set
            {
                _computers = value.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        [Output]
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public override bool Execute()
        {
            _logger = new FileLogger(Path.GetFullPath("InstallationGuide.log"));
            IExternalExecutor externalInvoker = new ExternalExecutor(_logger);
            var copyist = new Copyist(_logger);

            try
            {
                _logger.WriteLog("Begin InstallationGuide Executing");
                _errorMessage = "Preparation";

                if (_computers == null || _computers.Length == 0)
                {
                    throw new Exception("Computers list is empty");
                }

                const string remoteInstallationFolderPath = @"c:\kits\RemoteInstallation";
                string psExecCredentials = $"/accepteula -u firm\\TestlabInstaller -p {_testlabInstallerPassword}";
                const string psExecPath = "PsExec\\PsExec.exe";

                _logger.WriteLog("Remove old installation folders from all servers");
                foreach (string computerName in _computers)
                {
                    _errorMessage = "Remove old installation folders from " + computerName + " computer";

                    string remoteInstallationFolderServerPath = @"\\" + computerName + remoteInstallationFolderPath.Substring(2);

                    foreach (string directoryPath in Directory.EnumerateDirectories(remoteInstallationFolderServerPath)) 
                    {
                        copyist.RemoveDirectory(directoryPath);
                    }
                }

                _logger.WriteLog("Copy new installation files to all computers");
                foreach (string computerName in _computers)
                {
                    foreach (string productName in GetInstallProductList(computerName))
                    {
                        _errorMessage = "Copy " + productName + " to " + computerName;
                        string sourcePath = productName == "Confirmit CATI TCI Dialer"
                            ? string.Format(@"..\assemblies\Installation\exe\x86\{0} {1} {2} x86.exe", productName, _sideBySideName, _buildNumber)
                            : string.Format(@"..\assemblies\Installation\exe\x64\{0} {1} {2} x64.exe", productName, _sideBySideName, _buildNumber);                        
                        copyist.CopyFile(sourcePath, string.Format("\\\\{0}{1}\\{2}.exe", computerName, remoteInstallationFolderPath.Substring(2), productName));
                    }
                }

                _logger.WriteLog("Run installations of all products on all computers");
                foreach (string computerName in _computers)
                {
                    foreach (string productName in GetInstallProductList(computerName))
                    {
                        _errorMessage = "Install " + productName + " on " + computerName;
                        string arguments = $"/i /s {psExecCredentials} \\\\{computerName} \"{Path.Combine(remoteInstallationFolderPath, productName + ".exe")}\" /q /ignoreVersion /update";

                        externalInvoker.Invoke(psExecPath, arguments, 300000);
                    }
                }
                
                _logger.WriteLog("End InstallationGuide Execution");

                _errorMessage = "Success";
                return true;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(TraceEventType.Error, ex.ToString());
                _errorMessage += ": " + ex.Message;
               
                throw;
            }
        }

        private string[] GetInstallProductList(string computerName)
        {
            if (_branchName == "master")
            {
                if (computerName.ToLowerInvariant() == "co-osl-tenta203")
                {
                    return new[]
                    {
                        "Confirmit CATI LTU Simulator (G) Dialer Web Service"
                    };
                }

                return new[]
                {
                    "Confirmit CATI Generic Dialer Web Service",
                    "Confirmit CATI Simulator (G) Dialer Web Service",
                    "Confirmit CATI LTU Simulator (G) Dialer Web Service"
                };
            }

            return new[]
            {
                "Confirmit CATI Generic Dialer Web Service",
                "Confirmit CATI LTU Simulator (G) Dialer Web Service"
            };
        }
    }
}
