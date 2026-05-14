using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Confirmit.CATI.DialerWebServices.CustomAction.Properties;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using CustomActionLibrary;

using DialerCommon;

using Microsoft.Deployment.WindowsInstaller;
using SessionCustomAction;

namespace Confirmit.CATI.DialerWebServices.CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult VerifyLoggingSettingsCustomAction(Session session)
        {
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            setupEngine.Logger.WriteLog("Begin VerifyLoggingSettingsCustomAction");
            
            bool isLoggingToFileEnabled = session["IS_FILE_LOGGING_ENABLED"] == "1";

            try
            {
                session["TEST_RESULT"] = "Success";

                if (!isLoggingToFileEnabled)
                {
                    session["TEST_RESULT"] = DialogResult.Yes == TopMostMessageBox.Show("Are you sure, you want to disable any logging?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DialogResult.Yes)
                        ? "Success"
                        : "Error";
                }
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, "Warning", MessageBoxIcon.Warning);
                session["TEST_RESULT"] = "Error";
            }
            finally
            {
                setupEngine.Logger.WriteLog("End VerifyLoggingSettingsCustomAction");
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// If set full name of custom action - then an error will occur
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult FillComboBoxesAndGetParamsFromMachineConfigCA(Session session)
        {
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            var iisEngine = new IISEngine(setupEngine.Logger);
            var configEngine = new ConfigsEngine(setupEngine.Logger);
            setupEngine.Logger.WriteLog("Begin FillComboBoxesAndGetParametersFromMachineConfigCustomAction");

            try
            {
                session.Database.Execute("Delete from ComboBox where Property='APP_POOL_NAME'");
                session.Database.Execute("Delete from ComboBox where Property='WEB_SITE_NAME'");

                setupEngine.Logger.WriteLog("APP_POOL_NAME and WEB_SITE_NAME properties was removed from ComboBox table");

                string insertString = session.Database.Tables["ComboBox"].SqlInsertString + " TEMPORARY";
                int order = 1;
                setupEngine.Logger.WriteLog("insertString={0}\r\norder ={1}", insertString, order);
                foreach (string appPoolName in iisEngine.GetAppPools())
                {
                    session.Database.Execute(insertString, new Record("APP_POOL_NAME", order++, appPoolName, appPoolName));
                }

                foreach (string webSiteName in iisEngine.GetWebSites())
                {
                    session.Database.Execute(insertString, new Record("WEB_SITE_NAME", order++, webSiteName, webSiteName));
                }

                session["WEB_SITE_NAME"] = "Default Web Site";

                MachineConfigProperties machineConfigProperties = configEngine.GetMachineConfigProperties(setupEngine.GetMachineConfigPath());
                session["MACHINE_CONFIG_CHANGING"] = machineConfigProperties.MachineConfigChanging.ToString();

                session["MIN_WORKET_THREADS_CURRENT_VALUE_LABEL"] = setupEngine.GetCurrectValueLabel(machineConfigProperties.MinWorkerThreads);
                session["MAX_WORKER_THREADS_CURRENT_VALUE_LABEL"] = setupEngine.GetCurrectValueLabel(machineConfigProperties.MaxWorkerThreads);
                session["MIN_IO_THREADS_CURRENT_VALUE_LABEL"] = setupEngine.GetCurrectValueLabel(machineConfigProperties.MinIoThreads);
                session["MAX_IO_THREADS_CURRENT_VALUE_LABEL"] = setupEngine.GetCurrectValueLabel(machineConfigProperties.MaxIoThreads);
                session["MIN_FREE_THREADS_CURRENT_VALUE_LABEL"] = setupEngine.GetCurrectValueLabel(machineConfigProperties.MinFreeThreads);
                session["MIN_LOCAL_REQUEST_FREE_THREADS_CURRENT_VALUE_LABEL"] = setupEngine.GetCurrectValueLabel(machineConfigProperties.MinLocalRequestFreeThreads);
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, "Warning", MessageBoxIcon.Warning);
            }
            finally
            {
                setupEngine.Logger.WriteLog("End FillComboBoxesAndGetParametersFromMachineConfigCustomAction");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ValidateConfigSettingsForGenericInstallationCA(Session session)
        {
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            setupEngine.Logger.WriteLog("Begin ValidateConfigSettingsForGenericInstallationCA");

            string dialerId = session["DIALER_ID"];

            var commonValidator = new CommonValidator();

            try
            {
                commonValidator.ValidateNotNegativeIntParameter(dialerId, "dialer ID");

                session["TEST_RESULT"] = "Success";
            }
            catch (Exception ex)
            {
                session["TEST_RESULT"] = "Error";
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, "Warning", MessageBoxIcon.Warning);
            }
            finally
            {
                setupEngine.Logger.WriteLog("End ValidateConfigSettingsForGenericInstallationCA");
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// Validate, that certificate was selected correctly
        /// </summary>
        /// <param name="session">Session variable</param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult ValidateCertificateNamesAvailabilityCustomAction(Session session)
        {
            var setupEngine = new SetupEngine(new InstallationLogger(session));
            setupEngine.Logger.WriteLog("Begin ValidateCertificateNamesAvailabilityCustomAction");

            try
            {
                string certificateType = session["CERTIFICATE_TYPE"];
                string testCertificateName = session["TEST_CERTIFICATE_NAME"];
                string certificatePath = session["CERTIFICATE_PATH"];
                string certificatePassword = session["CERTIFICATE_PASSWORD"];

                if (certificateType == "Test" && string.IsNullOrEmpty(testCertificateName.Trim()))
                {
                    throw new Exception(Resources.InvalidTestCertificateName);
                }

                if (certificateType == "Real")
                {
                    var certificateValidator = new CertificateValidator(setupEngine.Logger, certificatePath, certificatePassword);

                    ValidationState validationState = certificateValidator.Validate();

                    if (validationState != ValidationState.Valid)
                    {
                        TopMostMessageBox.Show(certificateValidator.GetValidationWarning(validationState), session["ProductName"], MessageBoxIcon.Warning);
                    }
                }

                session["TEST_RESULT"] = "Success";
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, session["ProductName"], MessageBoxIcon.Information);
                session["TEST_RESULT"] = "Error";
            }
            finally
            {
                setupEngine.Logger.WriteLog("End ValidateCertificateNamesAvailabilityCustomAction");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult SelectProcDumpExecutablePathCustomAction(Session session)
        {
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            setupEngine.Logger.WriteLog("Begin SelectProcDumpExecutablePathCustomAction");

            try
            {
                FileBrowser.SelectObject(setupEngine.Logger, session, DialogType.Open, "PROCDUMP_FILE_PATH", session["PROCDUMP_FILE_PATH"], "Specify path to ProcDump.exe file");
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, "Warning", MessageBoxIcon.Warning);
            }
            finally
            {
                setupEngine.Logger.WriteLog("End SelectProcDumpExecutablePathCustomAction");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult SelectProcDumpLogFolderCustomAction(Session session)
        {
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            setupEngine.Logger.WriteLog("Begin SelectProcDumpLogFolderCustomAction");

            try
            {
                string defaultDirectory = session["PROCDUMP_LOG_FOLDER_PATH"];
                if (string.IsNullOrEmpty(defaultDirectory))
                {
                    try
                    {
                        defaultDirectory = Path.GetDirectoryName(session["PROCDUMP_FILE_PATH"]);
                    }
                    catch
                    {
                        defaultDirectory = string.Empty;
                    }
                }

                FileBrowser.SelectObject(setupEngine.Logger, session, DialogType.SelectFolder, "PROCDUMP_LOG_FOLDER_PATH", defaultDirectory, "Specify path to folder with dumps of ProcDump.exe utility");
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, "Warning", MessageBoxIcon.Warning);
            }
            finally
            {
                setupEngine.Logger.WriteLog("End SelectProcDumpLogFolderCustomAction");
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult VerifyProcDumpParametersCustomAction(Session session)
        {
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            setupEngine.Logger.WriteLog("Begin VerifyProcDumpParametersCustomAction");

            var dumpCreationMode = (DumpCreationOptions)Enum.Parse(typeof(DumpCreationOptions), session["DUMP_CREATION_OPTIONS"]);
            string procDumpFilePath = session["PROCDUMP_FILE_PATH"];
            string procDumpLogFolder = session["PROCDUMP_LOG_FOLDER_PATH"];
            try
            {
                if (dumpCreationMode != DumpCreationOptions.CreateDump)
                {
                    session["TEST_RESULT"] = "Success";
                    return ActionResult.Success;
                }

                if (string.IsNullOrEmpty(procDumpFilePath) || string.IsNullOrEmpty(procDumpLogFolder))
                {
                    throw new Exception("You should specify both paths.");
                }

                string selectedFileName = Path.GetFileName(procDumpFilePath);

                if (selectedFileName.ToLower() != "procdump.exe" || !File.Exists(procDumpFilePath))
                {
                    throw new Exception("You must specify a path to ProcDump.exe file.");
                }

                if (!Directory.Exists(procDumpLogFolder))
                {
                    throw new Exception("Folder with dumps of ProcDump.exe utility is incorrect.");
                }

                session["TEST_RESULT"] = "Success";
            }
            catch (Exception ex)
            {
                session["TEST_RESULT"] = "Error";
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, "Warning", MessageBoxIcon.Warning);
            }
            finally
            {
                setupEngine.Logger.WriteLog("End VerifyProcDumpParametersCustomAction");
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// Check that real certificate name exists
        /// </summary>
        /// <param name="session">Session variable</param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult SelectPathToCertificateCustomAction(Session session)
        {
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            setupEngine.Logger.WriteLog("Begin SelectPathToCertificateCustomAction");

            try
            {
                FileBrowser.SelectObject(setupEngine.Logger, session, DialogType.Open, "CERTIFICATE_PATH", string.Empty, "Select Certificate File");

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                session["FATAL_ERROR"] = "\n\n" + ex.Message + "\n\n";
                return ActionResult.Failure;
            }
            finally
            {
                setupEngine.Logger.WriteLog("End SelectPathToCertificateCustomAction");
            }
        }

        /// <summary>
        /// Get full domain name
        /// </summary>
        /// <param name="session">Session variable</param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult GetFullDomainNameCustomAction(Session session)
        {
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            setupEngine.Logger.WriteLog("Begin GetFullDomainNameCustomAction");

            try
            {
                session["TEST_CERTIFICATE_NAME"] = setupEngine.GetFullComputerName();
                setupEngine.Logger.WriteLog("Full domain name={0}", session["TEST_CERTIFICATE_NAME"]);

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                session["FATAL_ERROR"] = "\n\n" + ex.Message + "\n\n";
                return ActionResult.Failure;
            }
            finally
            {
                setupEngine.Logger.WriteLog("End GetFullDomainNameCustomAction");
            }
        }

        [CustomAction]
        public static ActionResult BeforeWixInstallationCustomAction(Session session)
        {
            TopMostMessageBox.IsQuietMode = session["QUIET_MODE"] == "TRUE";
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            var iisEngine = new IISEngine(setupEngine.Logger);
            setupEngine.Logger.WriteLog("Begin BeforeWixInstallationCustomAction");
            var sessionStuff = new SessionSetupEngine(session);

            string installLocation = session["INSTALL_LOCATION"];

            string productName = session["ProductName"];
            string dialerWebSiteName = session["WEB_SITE_NAME"];
            string dialerWsType = session["DIALER_WS_TYPE"];

            bool isUpgradeFound = !string.IsNullOrEmpty(session["UPGRADE_FOUND"]);
            string authorizationKey = session["AUTHORIZATION_KEY"];

            try
            {
                session["ARPINSTALLLOCATION"] = installLocation;

                if (string.IsNullOrEmpty(session["WEB_SITE_ID"]))
                {
                    session["WEB_SITE_ID"] = iisEngine.GetWebSiteId(dialerWebSiteName);
                }

                setupEngine.Logger.WriteLog("WEB_SITE_ID={0}", session["WEB_SITE_ID"]);

                if (dialerWsType.StartsWith("GENERIC"))
                {
                    sessionStuff.DefineEncryptedAndDecryptParameters("CERTIFICATE_PASSWORD", "ENCRYPTED_CERTIFICATE_PASSWORD");

                    using (var ecryptor = new DialerAuthorizationKeyEncryptor())
                    {
                        session["ENCRYPTED_AUTHORIZATION_KEY"] = ecryptor.EncryptString(authorizationKey);
                        ecryptor.Clear();
                    }

                    session["USE_AUTHORIZATION_TRUE_OR_FALSE"] = session["USE_AUTHORIZATION"] == "1" ? "True" : "False";
                }

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, productName, MessageBoxIcon.Warning);
                return ActionResult.Failure;
            }
            finally
            {
                setupEngine.Logger.WriteLog("End BeforeWixInstallationCustomAction");
            }
        }

        [CustomAction]
        public static ActionResult AfterWixInstallationCustomAction(Session session)
        {
            TopMostMessageBox.IsQuietMode = session["QUIET_MODE"] == "TRUE";
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session));
            var sessionStuff = new SessionSetupEngine(session);
            var configEngine = new ConfigsEngine(setupEngine.Logger);
            var iisEngine = new IISEngine(setupEngine.Logger);
            var performanceCounterInitializer = new PerformanceCounterInitializerHelper();

            setupEngine.Logger.WriteLog("Begin AfterWixInstallationCustomAction");

            string installLocation = session["INSTALL_LOCATION"];
            string productName = session["ProductName"];
            string currentVersion = session["ProductVersion"];
            bool isWin64 = !string.IsNullOrEmpty(session["Msix64"]);
            bool isUpgradeFound = !string.IsNullOrEmpty(session["UPGRADE_FOUND"]);

            string dialerAppPoolName = session["APP_POOL_NAME"];
            bool isNeedSetRecyclingValueToZero = session["IS_SET_RECYCLING_VALUE_TO_ZERO"] == "1";
            string dialerAliaseName = session["ALIAS_EDIT"];
            string dialerSiteId = session["WEB_SITE_ID"];
            
            bool isLoggingToFileEnabled = session["IS_FILE_LOGGING_ENABLED"] == "1";

            string dialerWsType = session["DIALER_WS_TYPE"];
            string certificateType = session["CERTIFICATE_TYPE"];
            string testCertificateName = session["TEST_CERTIFICATE_NAME"];
            string certificatePath = session["CERTIFICATE_PATH"];
            string certificatePassword = session["CERTIFICATE_PASSWORD"];

            var dumpCreationMode = (DumpCreationOptions)Enum.Parse(typeof(DumpCreationOptions), session["DUMP_CREATION_OPTIONS"]);
            string procDumpFilePath = session["PROCDUMP_FILE_PATH"];
            string procDumpLogFolderPath = session["PROCDUMP_LOG_FOLDER_PATH"];
            string procDumpAdditionalParameters = session["PROCDUMP_ADDITIONAL_PARAMETERS"];
            string dumpCmdFilePath = Path.Combine(session["INSTALL_LOCATION"], "dumpCreator.cmd");

            const string caName = "AfterWixInstallationCustomAction";

            var machineConfigProperties = new MachineConfigProperties(
                (MachineConfigChangingState)Enum.Parse(typeof(MachineConfigChangingState), session["MACHINE_CONFIG_CHANGING"]),
                session["MIN_WORKER_THREADS"],
                session["MAX_WORKER_THREADS"],
                session["MIN_IO_THREADS"],
                session["MAX_IO_THREADS"],
                session["MIN_FREE_THREADS"],
                session["MIN_LOCAL_REQUEST_FREE_THREADS"]);

            try
            {
                if (dialerWsType.StartsWith("GENERIC"))
                {
                    sessionStuff.ChangeProgressStatus(caName, "Create performance counters");
                    performanceCounterInitializer.InitializeDialerServiceCounters();

                    if (dialerWsType == "GENERIC_LTU_SIMULATOR")
                    {
                        performanceCounterInitializer.InitializeSimulatorDialerDriverCounters();
                    }

                    sessionStuff.ChangeProgressStatus(caName, "Installing test certificates and configuring http listener");
                    try
                    {
                        string certificateThumbprint = setupEngine.InstallCertificateIfNeeded(
                            installLocation,
                            certificateType,
                            testCertificateName,
                            certificatePath,
                            certificatePassword);

                        setupEngine.ConfigureCertificateForIIS(Convert.ToInt32(dialerSiteId), certificateThumbprint);
                    }
                    catch (Exception ex)
                    {
                        setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                        TopMostMessageBox.Show(
                            "An error occurs by certificate installation: \r\n" +
                            ex.Message +
                            "\r\nYou should install certificate and configure http listener by hand. Installation will be continued.",
                            productName,
                            MessageBoxIcon.Warning);
                    }
                }

                if (isNeedSetRecyclingValueToZero)
                {
                    iisEngine.SetRecyclingValueToZero(dialerAppPoolName);
                }                

                setupEngine.ConfigureIIS(iisEngine, dialerSiteId, dialerAliaseName, dialerAppPoolName, isWin64, productName);

                if (!isUpgradeFound)
                {
                    if (!isLoggingToFileEnabled)
                    {
                        configEngine.DisableFileLogging(installLocation);
                    }

                    if (machineConfigProperties.MachineConfigChanging != MachineConfigChangingState.DoNotChange)
                    {
                        configEngine.ConfigureMachineConfig(setupEngine.GetMachineConfigPath(), machineConfigProperties, currentVersion);
                    }

                    if (dumpCreationMode != DumpCreationOptions.DoNotModifyCurrentOptions)
                    {
                        setupEngine.ConfigureOrphaning(iisEngine, dumpCreationMode, dialerAppPoolName, dumpCmdFilePath, procDumpFilePath, procDumpLogFolderPath, procDumpAdditionalParameters, currentVersion);
                    }
                }

                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
                TopMostMessageBox.Show(ex.Message, productName, MessageBoxIcon.Warning);
                return ActionResult.Failure;
            }
            finally
            {
                setupEngine.Logger.WriteLog("End AfterWixInstallationCustomAction");
            }
        }

        [CustomAction]
        public static ActionResult UninstallEventSourceAndSaveConfigCustomAction(Session session)
        {
            TopMostMessageBox.IsQuietMode = session.CustomActionData["QUIET_MODE"] == "TRUE";
            var setupEngine = new DialerWSSetupEngine(new InstallationLogger(session, true));

            string version = session.CustomActionData["Version"];
            string installLocation = session.CustomActionData["INSTALL_LOCATION"];

            try
            {
                setupEngine.Logger.WriteLog("Begin UninstallEventSourceAndSaveConfigCustomAction");

                setupEngine.SaveConfig(version, Path.Combine(installLocation, "Web.config"), installLocation);
            }
            catch (Exception ex)
            {
                setupEngine.Logger.WriteLog(TraceEventType.Error, ex.ToString());
            }
            finally
            {
                setupEngine.Logger.WriteLog("End UninstallEventSourceAndSaveConfigCustomAction");
            }

            return ActionResult.Success;
        }
    }
}
