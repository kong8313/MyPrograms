using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using BootstrapperLibrary.Interfaces;
using BootstrapperLibrary.Properties;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary
{
    public class Bootstrapper
    {
        private readonly IBootstrapperEngine _bootstrapperEngine;

        private readonly IObjectFactory _objectFactory;
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public Bootstrapper(IObjectFactory objectFactory, ILogger logger, IBootstrapperEngine bootstrapperEngine)
        {
            _objectFactory = objectFactory;
            _logger = logger;
            _bootstrapperEngine = bootstrapperEngine;
            _dialogService = objectFactory.CreateDialogservice();
        }

        public int Execute(
            string[] commandLineArgs, 
            byte[] msiSetupFileData, 
            string msiFolderPath,
            IMsiParametersStringCreator msiParametersStringCreator,
            IParametersValidateService parametersValidateService,
            UpdateInformation updateInformation)
        {
            try
            {
                parametersValidateService.CheckPrerequisites();

                if (!_bootstrapperEngine.IsSystemTypeCorrect(updateInformation))
                {
                    _logger.WriteLog(Resources.WrongSystemType);
                    _dialogService.Show(Resources.WrongSystemType, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 6;
                }

                string currentProductName = string.Format("{0} {1} ver.{2} {3}",
                    updateInformation.ProductNamePathBeforeSxSName,
                    _bootstrapperEngine.SideBySideName,
                    BootstrapperEngine.GetCurrentVersion(),
                    updateInformation.ProductType);
                string productNameMask = string.Format("{0} {1} ver.\\d+.\\d+.\\d+.\\d+ {2}",
                    updateInformation.ProductNamePathBeforeSxSName.Replace("(", "\\(").Replace(")", "\\)"),
                    _bootstrapperEngine.SideBySideName,
                    updateInformation.ProductType);

                _logger.WriteLog("currentProductName=" + currentProductName);
                _logger.WriteLog("productNameMask=" + productNameMask);

                IInstalledProductSearcher installedProductSearcher = _objectFactory.CreateInstalledProductsSearcherObject(currentProductName, productNameMask, _logger);

                CommandLineParseResult commandLineParseResult = _bootstrapperEngine.ParseUpdateCommandLineParameters(
                    commandLineArgs, msiParametersStringCreator, updateInformation, _objectFactory, installedProductSearcher);

                if (commandLineParseResult == null)
                {
                    return 0;
                }

                if (commandLineParseResult.Action != InstallationAction.Install && installedProductSearcher.IsProductAlreadyInstalled)
                {
                    _logger.WriteLog("Run uninstall");
                    _bootstrapperEngine.RunUninstall(msiFolderPath, installedProductSearcher);

                    _logger.WriteLog("Uninstall has finished successfully");

                    if (commandLineParseResult.Action == InstallationAction.Uninstall)
                    {
                        return 0;
                    }
                }

                string msiFileName = Path.GetFileNameWithoutExtension(commandLineArgs[0]) + ".msi";
                string msiFilePath = Path.Combine(msiFolderPath, msiFileName);
                File.WriteAllBytes(msiFilePath, msiSetupFileData);

                _bootstrapperEngine.RunInstallation(msiFilePath, msiFolderPath, commandLineParseResult.MsiPropertiesForUnattendedInstallation);

                return 0;
            }
            catch (PrerequisiteException ex)
            {
                _bootstrapperEngine.ShowMessageBox(ex.Message, TraceEventType.Error);
                _logger.WriteLog(TraceEventType.Error, ex.ToString());
                return 7;
            }
            catch (MessageException ex)
            {
                _bootstrapperEngine.ShowMessageBox(ex.Message, ex.Severity);
                _logger.WriteLog(ex.Severity, ex.ToString());
                return 8;
            }
            catch (ValidateException ex)
            {
                _bootstrapperEngine.ShowMessageBox(ex.Message, TraceEventType.Warning);
                _logger.WriteLog(TraceEventType.Warning, ex.ToString());
                return 9;
            }
            catch (Exception ex)
            {
                _bootstrapperEngine.ShowMessageBox(ex.ToString(), TraceEventType.Error);
                _logger.WriteLog(TraceEventType.Error, ex.ToString());
                return 10;
            }
        }
    }
}
