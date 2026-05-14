using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using BootstrapperLibrary.Interfaces;
using BootstrapperLibrary.Properties;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary
{
    public class BootstrapperEngine : IBootstrapperEngine
    {
        private readonly string _usageInfo;
        public bool IsQuietMode { get; set; }
        public string SideBySideName { get; }

        private readonly IExternalInvoker _externalInvoker;
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public BootstrapperEngine(IObjectFactory objectFactory, ILogger logger)
            : this(objectFactory, logger, Resources.StandardUsageInfo)
        {
        }

        public BootstrapperEngine(IObjectFactory objectFactory, ILogger logger, string usageInfo)
        {
            _usageInfo = usageInfo;
                 
            _externalInvoker = objectFactory.CreateExternalInvokerObject(logger, 0);            
            _logger = logger;
            _dialogService = objectFactory.CreateDialogservice();

            SideBySideName = GetSideBySideName();
        }

        private string GetSideBySideName()
        {
            string sxsName;
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(fileVersionInfo.ProductName))
            {
                string[] words = fileVersionInfo.ProductName.Split(' ');
                sxsName = words[3].TrimEnd('.');
            }
            else
            {
                sxsName = "Rel";
            }

            _logger.WriteLog("Side by side name is: {0}", sxsName);

            return sxsName;
        }

        /// <summary>
        /// Show message box
        /// </summary>
        /// <param name="text">Text to show</param>
        /// <param name="severity">Trace event type</param>
        public void ShowMessageBox(string text, TraceEventType severity)
        {
            if (IsQuietMode)
            {
                return;
            }

            if (severity == TraceEventType.Warning)
            {
                _dialogService.Show(text, Resources.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (severity == TraceEventType.Information)
            {
                _dialogService.Show(text, Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _dialogService.Show(text, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Verify, that current value is one from acceptable values
        /// </summary>
        /// <param name="parameterName">Verification parameter name</param>
        /// <param name="parameterValue">Verification value</param>
        /// <param name="acceptableValues">Acceptable values list</param>
        /// <returns></returns>
        public string VerifyParameterValue(string parameterName, string parameterValue, string[] acceptableValues)
        {
            return acceptableValues.Contains(parameterValue)
                ? string.Empty
                : string.Format(Resources.WrongParameterValueAndAcceptableValue,
                    parameterName,
                    parameterValue,
                    acceptableValues.Aggregate((rez, s) => rez + " | " + (s == string.Empty ? "<empty>" : s)));
        }

        /// <summary>
        /// Verify, that all parameters are defined
        /// </summary>
        /// <param name="installLocation">Install location</param>
        public string VerifyInstallLocation(string installLocation)
        {
            var errInfo = new StringBuilder();

            if (installLocation != null)
            {
                if (installLocation.Length > 2)
                {
                    string driveLetter = installLocation.Substring(0, 3);
                    if (Environment.GetLogicalDrives().All(drive => drive.ToLower() != driveLetter.ToLower()))
                    {
                        errInfo.AppendFormat(Resources.WrongInstallLocationParameterNoDrive, driveLetter);
                    }
                    else
                    {
                        errInfo.AppendFormat(Directory.Exists(installLocation)
                            ? CreateAndRemoveTestFile(installLocation)
                            : CreateAndRemoveInstallLocationFolder(installLocation));
                    }
                }
                else
                {
                    errInfo.AppendFormat(Resources.WrongInstallLocationParameterTooShort);
                }
            }

            return errInfo.ToString();
        }

        private string CreateAndRemoveInstallLocationFolder(string installLocationPath)
        {
            try
            {
                Directory.CreateDirectory(installLocationPath);
            }
            catch
            {
                return Resources.WrongInstallLocationParameterNoCreationRights;
            }

            string fileCreationResult = CreateAndRemoveTestFile(installLocationPath);

            if (fileCreationResult.Length > 0)
            {
                return fileCreationResult;
            }

            try
            {
                Directory.Delete(installLocationPath, true);
            }
            catch
            {
                return Resources.WrongInstallLocationParameterNoRemovingRights;
            }

            return string.Empty;
        }

        private string CreateAndRemoveTestFile(string installLocationPath)
        {
            string tempFileNamePath = Path.Combine(installLocationPath, Path.GetFileName(Path.GetTempFileName()));
            try
            {
                File.WriteAllText(tempFileNamePath, "test");
            }
            catch
            {
                return Resources.WrongInstallLocationParameterNoCreationRights;
            }

            try
            {
                File.Delete(tempFileNamePath);
            }
            catch
            {
                return Resources.WrongInstallLocationParameterNoRemovingRights;
            }

            return string.Empty;
        }

        /// <summary>
        /// Verify certificate parameters for CATI and Generic Dialer WS installations 
        /// </summary>
        /// <param name="certificateEngine">Certificate engine object</param>
        /// <param name="certificateType">Type of certificate</param>
        /// <param name="testCertificateName">Test certificate name</param>
        /// <param name="certificatePath">Path to certificate</param>
        /// <param name="certificatePassword">Password</param>
        /// <returns></returns>
        public string VerifyCertificateParameters(
            ICertificateEngine certificateEngine, string certificateType, string testCertificateName, string certificatePath, string certificatePassword)
        {
            if (certificateType == "Test" && string.IsNullOrEmpty(testCertificateName))
            {
                return Resources.TestCertificateNameParameterMustBeFilled;
            }

            if (certificateType == "Real")
            {
                return certificateEngine.VerifyCertificateFromFile(certificatePath, certificatePassword);
            }

            return string.Empty;
        }

        /// <summary>
        /// Parse update command line parameters
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="msiParametersStringCreator">Msi parameter string creator object</param>
        /// <param name="updateInformation">Update information object with specific objects for current bootstrapper</param>
        /// <param name="objectFactory">Factory object</param>
        /// <param name="installedProductSearcher">Installed product searcher object</param>
        /// <returns></returns>
        public CommandLineParseResult ParseUpdateCommandLineParameters(
           string[] args,
            IMsiParametersStringCreator msiParametersStringCreator,
            UpdateInformation updateInformation,
            IObjectFactory objectFactory,
            IInstalledProductSearcher installedProductSearcher)
        {
            var commandLineParseResult = new CommandLineParseResult();
            Version currentVersion = GetCurrentVersion();

            if (args.Length == 1)
            {
                return updateInformation.SelectActionForm.ShowForm(
                    _logger,
                    currentVersion,
                    installedProductSearcher,
                    objectFactory,
                    msiParametersStringCreator);
            }

            string installLocation = string.Empty;
            IsQuietMode = false;
            for (int i = 1; i < args.Length; i++)
            {
                if (IsHelpArgument(args[i]))
                {
                    throw new MessageException(_usageInfo, TraceEventType.Information);
                }

                if (IsQuietModeArgument(args[i]))
                {
                    IsQuietMode = true;
                    TopMostMessageBox.IsQuietMode = true;
                    continue;
                }

                switch (args[i].ToLowerInvariant())
                {
                    case "/update":
                    case "-update":
                        SetInstallationAction(commandLineParseResult, InstallationAction.Update);
                        break;
                    case "/installlocation":
                    case "-installlocation":
                        i++;
                        if (i >= args.Length || 
                            args[i].StartsWith("/") || args[i].StartsWith("-"))
                        {
                            throw new MessageException(Resources.WrongCountOfUpdateParameters + _usageInfo, TraceEventType.Warning);
                        }

                        installLocation = Path.GetFullPath(args[i]);
                        break;
                    case "/uninstall":
                    case "-uninstall":
                        SetInstallationAction(commandLineParseResult, InstallationAction.Uninstall);
                        return commandLineParseResult;
                    case "/ignoreversion":
                    case "-ignoreversion":
                        commandLineParseResult.IgnoreVersion = true;
                        break;

                    default:
                        throw new MessageException(string.Format(Resources.UnknownCommandLineParameter, args[i], _usageInfo), TraceEventType.Warning);
                }
            }

            if (string.IsNullOrEmpty(installLocation))
            {
                installLocation = installedProductSearcher.InstallLocation;
            }

            if (currentVersion <= installedProductSearcher.InstalledVersion && !commandLineParseResult.IgnoreVersion)
            {
                throw new MessageException("Installed version  is equal to or greater than the current one. Use '/ignoreversion' parameter to install your version", TraceEventType.Warning);
            }

            if (commandLineParseResult.Action == InstallationAction.Update)
            {
                var readingInstallationParameters = new ReadingInstallationParameters(installLocation);
                commandLineParseResult.MsiPropertiesForUnattendedInstallation = msiParametersStringCreator.CreateInstallationParametersString(readingInstallationParameters);
            }

            return commandLineParseResult;
        }

        private void SetInstallationAction(CommandLineParseResult commandLineParseResult, InstallationAction installationAction)
        {
            if (commandLineParseResult.Action == InstallationAction.None)
            {
                commandLineParseResult.Action = installationAction;
            }
            else if (commandLineParseResult.Action != installationAction)
            { 
                throw new MessageException(Resources.YouCannotUseBothUpdateAndUninstallParameters + _usageInfo, TraceEventType.Warning);
            }
        }       

        private bool IsHelpArgument(string argument)
        {
            argument = argument.ToLowerInvariant();
            return argument == "/?" || argument == "-?" || argument == "/h" || argument == "-h" || argument == "/help" || argument == "-help";
        }

        private bool IsQuietModeArgument(string argument)
        {
            argument = argument.ToLowerInvariant();
            return argument == "/q" || argument == "-q";
        }

        /// <summary>
        /// Run installation process and wait, while it finishes
        /// </summary>
        /// <param name="msiFilePath">Path to msi file</param>
        /// <param name="msiFolderName">Path to folder contains msi file</param>
        /// <param name="passiveParamStr">Parameters for passive installation</param>
        /// <returns></returns>
        public void RunInstallation(string msiFilePath, string msiFolderName, string passiveParamStr)
        {
            string msiexecArgs = $"/i \"{msiFilePath}\" /l*v \"{Path.Combine(msiFolderName, "LogFile.txt")}\" EXECUTABLE_PATH=\"{Application.ExecutablePath}\"{passiveParamStr}";

            if (string.IsNullOrEmpty(passiveParamStr))
            {
                _externalInvoker.Invoke("msiexec", msiexecArgs, false);
            }
            else
            {
                _externalInvoker.Invoke("msiexec", msiexecArgs, 500000);

                ShowMessageBox(Resources.InstallationHasFinishedSuccessfully, TraceEventType.Information);
            }
        }

        /// <summary>
        /// Run uninstall of old version
        /// </summary>
        /// <param name="msiFolderPath">Path to folder with msi file</param>
        /// <param name="installedProductSearcher">Object with information about installed product</param>
        public void RunUninstall(string msiFolderPath, IInstalledProductSearcher installedProductSearcher)
        {
            string msiexecArgs =
                $"/x {installedProductSearcher.ProductCode} /l*v \"{Path.Combine(msiFolderPath, "UninstallLogFile.txt")}\" /passive INSTALL_LOCATION=\"{installedProductSearcher.InstallLocation}\"";

            _externalInvoker.Invoke("msiexec", msiexecArgs, 500000);
        }

        /// <summary>
        /// Remove extra line feeds from the string
        /// </summary>
        /// <param name="text">String with information</param>
        /// <returns></returns>
        public string RemoveExtraLineFeeds(StringBuilder text)
        {
            string[] lines = text.ToString().Split(new [] { "\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            return string.Join("\r\n", lines);
        }

        /// <summary>
        /// Verify that the OS system type is suitable for the installation system type
        /// </summary>
        /// <param name="updateInformation">Information about update process</param>
        /// <returns></returns>
        public bool IsSystemTypeCorrect(UpdateInformation updateInformation)
        {
            if (updateInformation.ProductNamePathBeforeSxSName == "Confirmit CATI TCI Dialer")
            {
                return true;
            }

            if ((Environment.Is64BitOperatingSystem && updateInformation.ProductType == SystemType.x86) ||
                (!Environment.Is64BitOperatingSystem && updateInformation.ProductType == SystemType.x64))
            {
                _logger.WriteLog("System type of setup: " + updateInformation.ProductType);
                _logger.WriteLog("System type of Windows: " + (Environment.Is64BitOperatingSystem ? "x64" : "x86"));
                return false;
            }

            return true;
        }

        public static Version GetCurrentVersion()
        {
            return new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);
        }
    }
}
