using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using CustomActionLibrary.Properties;

namespace CustomActionLibrary
{
    /// <summary>
    /// Class with function for CustomAction class
    /// </summary>
    public class SetupEngine
    {
        protected readonly string SystemRoot;
        public ILogger Logger { get; private set; }

        private const string TestRootCertificateName = "Confirmit CATI Root Test Certificate Sha256";

        public IExternalInvoker ExternalInvoker { get; private set; }

        public SetupEngine(ILogger logger)
            : this(logger, new ExternalInvoker(logger))
        {

        }
        public SetupEngine(ILogger logger, IExternalInvoker externalInvoker)            
        {
            Logger = logger;
            SystemRoot = Environment.GetEnvironmentVariable("SYSTEMROOT") ?? string.Empty;
            ExternalInvoker = externalInvoker;
        }

        private string GetConfigBackupsPath(string installLocation)
        {
            return Path.Combine(installLocation, "ConfigBackups");
        }

        private string GetConfigSavePath(string version, string fileOrFolderName, string installLocation)
        {
            string saveFileOrFolderName = string.Format("{0}_{1}_{2}", version, DateTime.Now.ToString("yyyy-MM-dd_HH-mm"), fileOrFolderName);
            string backupFolder = GetConfigBackupsPath(installLocation);

            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            return Path.Combine(backupFolder, saveFileOrFolderName);
        }

        private void CreateBackupFolderIfNeeded(string installLocation)
        {
            string backupFolder = GetConfigBackupsPath(installLocation);

            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }
        }

        /// <summary>
        /// Save config file
        /// </summary>
        /// <param name="version">Current version of product</param>
        /// <param name="configPath">Path to config file to save</param>
        /// <param name="installLocation">Install location</param>
        public void SaveConfig(string version, string configPath, string installLocation)
        {
            if (!File.Exists(configPath))
            {
                TopMostMessageBox.Show(string.Format(Resources.FileIsNotFound, configPath), "Error", MessageBoxIcon.Error);
                return;
            }

            string configSavePath = GetConfigSavePath(version, Path.GetFileName(configPath), installLocation);

            CreateBackupFolderIfNeeded(installLocation);
            File.Copy(configPath, configSavePath);
        }

        /// <summary>
        /// Save data of file to hard disk
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="data">File data</param>
        /// <param name="installLocation">Backend install location</param>
        /// <returns></returns>
        private string SaveResourse(string fileName, byte[] data, string installLocation)
        {
            Logger.WriteLog("Begin SaveResourse\r\n FileName={0}", fileName);

            try
            {
                string path = Path.Combine(installLocation, fileName);
                string directoryName = Path.GetDirectoryName(path) ?? string.Empty;
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                File.WriteAllBytes(path, data);
                return path;
            }
            finally
            {
                Logger.WriteLog("End SaveResourse");
            }
        }


        /// <summary>
        /// Install Confirmit CATI Root Test Certificate
        /// </summary>
        private void InstallTestRootCertificate()
        {
            var certificateStoreWorker = new CertificateStoreWorker(this, StoreName.Root, StoreLocation.LocalMachine, TestRootCertificateName, TestRootCertificateName);

            if (certificateStoreWorker.GetCertificatesCount() == 0)
            {
                Logger.WriteLog("Add {0} certificate to Root/LocalMachine certificate store", TestRootCertificateName);

                certificateStoreWorker.InstallCertificate(Resources.ConfirmitCATIRootTestCertificate_cer);

                Logger.WriteLog("{0} certificate installed successfully", TestRootCertificateName);
            }
            else
            {
                Logger.WriteLog("{0} certificate wasn't installed, because certificate store already has a certificate with the same 'Issued to' and 'Issued by' parameters", TestRootCertificateName);
            }
        }

        /// <summary>
        /// Installs test certificates if needed
        /// For the details see following page: 
        /// http://www.codeplex.com/wikipage?ProjectName=WCFSecurity&title=How%20To%20-%20Create%20and%20Install%20Temporary%20Certificates%20in%20WCF%20for%20Message%20Security%20During%20Development&referringTitle=How%20Tos
        /// </summary>
        /// <param name="installLocation">Backend install location</param>
        /// <param name="certificateType">Type of certificate: test or real</param>
        /// <param name="testCertificateName">Certifiacte name of test certificate</param>
        /// <param name="certificatePath">Path to certificate file</param>
        /// <param name="certificatePassword">Certificate password</param>
        public string InstallCertificateIfNeeded(
            string installLocation,
            string certificateType,
            string testCertificateName,
            string certificatePath,
            string certificatePassword)
        {
            Logger.WriteLog("Begin InstallCertificateIfNeeded");

            try
            {
                string certificateThumbprint;

                if (certificateType == "Test")
                {
                    InstallTestRootCertificate();

                    var certificateStoreWorker = new CertificateStoreWorker(this, StoreName.My, StoreLocation.LocalMachine, testCertificateName, TestRootCertificateName);
                    if (certificateStoreWorker.GetCertificatesCount() == 0)
                    {
                        Logger.WriteLog("Create and Install {0} certificate to My/LocalMachine certificate store", testCertificateName);

                        //
                        // Save all resources
                        //
                        string rootCerFilePath = SaveResourse("TestCertificates\\" + TestRootCertificateName + ".cer", Resources.ConfirmitCATIRootTestCertificate_cer, installLocation);
                        string rootPvkFilePath = SaveResourse("TestCertificates\\" + TestRootCertificateName + ".pvk", Resources.ConfirmitCATIRootTestCertificate_pvk, installLocation);
                        string makecertFilePath = SaveResourse("Tools\\makecert.exe", Resources.makecert, installLocation);

                        //
                        // Create test SSL certificate and install it to My/LocalMachine certificate store.
                        // All done by one call to the makecert.
                        // 
                        string testCertificateFilePath = Path.Combine(installLocation, "TestCertificates\\" + testCertificateName + ".cer");

                        int tryNumber = 3;
                        while (true)
                        {
                            try
                            {
                                ExternalInvoker.Invoke("\"" + makecertFilePath + "\"", "-a SHA256 -len 4096 -eku 1.3.6.1.5.5.7.3.1 -sk ConfirmitCATISSLTestCertificateKeyNameSha256 -iv \"" + rootPvkFilePath + "\" -n \"CN=" + testCertificateName + "\" -ic \"" + rootCerFilePath + "\" -sr localmachine -ss my -sky exchange -pe \"" + testCertificateFilePath + "\"");

                                Logger.WriteLog("{0} certificate installed successfully", testCertificateName);
                                break;
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLog(TraceEventType.Error, ex.ToString());
                                tryNumber--;
                                if (tryNumber == 0 ||
                                    DialogResult.No == TopMostMessageBox.Show(string.Format(Resources.IncorrectPasswordWithAttemptsCountQuestion, tryNumber), "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, DialogResult.No))
                                {
                                    throw;
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.WriteLog("{0} certificate wasn't installed, because certificate store already has a certificate with the same 'Issued to' and 'Issued by' parameters", testCertificateName);
                    }

                    //
                    // Get installed certificate thumbprint
                    //
                    certificateThumbprint = certificateStoreWorker.GetCertificateThumbprint();

                    Logger.WriteLog("{0} certificate thumbprint {1}", testCertificateName, certificateThumbprint);
                }
                else
                {
                    var cert = new X509Certificate2(certificatePath, certificatePassword);
                    var certificateStoreWorker = new CertificateStoreWorker(this, StoreName.My, StoreLocation.LocalMachine, cert.SubjectName.Name, cert.IssuerName.Name);

                    if (certificateStoreWorker.GetCertificatesCount() == 0)
                    {
                        Logger.WriteLog("Install '{0}' certificate to My/LocalMachine certificate store", certificatePath);

                        certificateStoreWorker.InstallCertificate(cert);
                    }
                    else
                    {
                        Logger.WriteLog("'{0}' certificate wasn't installed, because certificate store already has a certificate with the same 'Issued to' and 'Issued by' parameters", certificatePath);
                    }

                    certificateThumbprint = cert.Thumbprint;
                }

                return certificateThumbprint;
            }
            finally
            {
                Logger.WriteLog("End InstallCertificateIfNeeded");
            }
        }

        /// <summary>
        /// Get full computer domain name
        /// </summary>
        /// <returns></returns>
        public string GetFullComputerName()
        {
            Logger.WriteLog("Begin GetFullComputerName");

            try
            {
                var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                if (string.IsNullOrEmpty(ipProperties.DomainName))
                {
                    return ipProperties.HostName;
                }
                else
                {
                    return string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
                }
            }
            finally
            {
                Logger.WriteLog("End GetFullComputerName");
            }
        }
    }
}

