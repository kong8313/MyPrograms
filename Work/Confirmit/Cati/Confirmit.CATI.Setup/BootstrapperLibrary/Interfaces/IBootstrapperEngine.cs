using System.Diagnostics;
using System.Text;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary.Interfaces
{
    public interface IBootstrapperEngine
    {
        bool IsQuietMode { get; set; }

        string SideBySideName { get; }

        /// <summary>
        /// Show message box and write information to console
        /// </summary>
        /// <param name="text">Text to show</param>
        /// <param name="severity">Trace event type</param>
        void ShowMessageBox(string text, TraceEventType severity);

        /// <summary>
        /// Run installation process and wait, while it finishes
        /// </summary>
        /// <param name="msiFilePath">Path to msi file</param>
        /// <param name="msiFolderName">Path to folder contains msi file</param>
        /// <param name="passiveParamStr">Parameters for passive installation</param>
        void RunInstallation(string msiFilePath, string msiFolderName, string passiveParamStr);

         /// <summary>
        /// Verify, that current value is one from acceptable values
        /// </summary>
        /// <param name="parameter">Verification parameter name</param>
        /// <param name="value">Verification value</param>
        /// <param name="acceptableValues">Acceptable values list</param>
        /// <returns></returns>
        string VerifyParameterValue(string parameter, string value, string[] acceptableValues);

        /// <summary>
        /// Verify, that all parameters are defined
        /// </summary>
        /// <param name="installLocation">Install location</param>
        string VerifyInstallLocation(string installLocation);

        /// <summary>
        /// Parse command line parameters with update support
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <param name="msiParametersStringCreator">Msi parameter string creator object</param>
        /// <param name="updateInformation">Update information object</param>
        /// <param name="objectFactory">Object factory</param>
        /// <param name="installedProductSearcher">Installed product searcher object</param>
        /// <returns></returns>
        CommandLineParseResult ParseUpdateCommandLineParameters(
           string[] args, IMsiParametersStringCreator msiParametersStringCreator, 
           UpdateInformation updateInformation, IObjectFactory objectFactory,
           IInstalledProductSearcher installedProductSearcher);

        /// <summary>
        /// Verify certificate parameters for CATI and Generic Dialer WS installations 
        /// </summary>
        /// <param name="certificateEngine">Certificate engine object</param>
        /// <param name="certificateType">Type of certificate (CERTIFICATE_TYPE parameter)</param>
        /// <param name="testCertificateName">Test certificate name (TEST_CERTIFICATE_NAME parameter)</param>
        /// <param name="certificatePath">Path to certificate file (CERTIFICATE_PATH parameter)</param>
        /// <param name="certificatePassword">Password of certificate (CERTIFICATE_PASSWORD parameter)</param>
        /// <returns></returns>
        string VerifyCertificateParameters(ICertificateEngine certificateEngine, string certificateType, string testCertificateName, string certificatePath, string certificatePassword);

        /// <summary>
        /// Run uninstall of the old version
        /// </summary>
        /// <param name="msiFolderPath">Path to folder with msi file</param>
        /// <param name="installedProductSearcher">Object with information about installed product</param>
        void RunUninstall(string msiFolderPath, IInstalledProductSearcher installedProductSearcher);

        /// <summary>
        /// Remove extra line feeds from the string
        /// </summary>
        /// <param name="text">String with information</param>
        /// <returns></returns>
        string RemoveExtraLineFeeds(StringBuilder text);

        /// <summary>
        /// Verify, that the OS system type is suitable for the installation system type
        /// </summary>
        /// <param name="updateInformation">Information about update process</param>
        /// <returns></returns>
        bool IsSystemTypeCorrect(UpdateInformation updateInformation);
    }
}
