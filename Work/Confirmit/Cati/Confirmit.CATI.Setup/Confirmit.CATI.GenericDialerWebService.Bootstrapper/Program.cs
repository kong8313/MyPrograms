using System;
using System.IO;
using System.Windows.Forms;

using BootstrapperLibrary;
using BootstrapperLibrary.Interfaces;
using Confirmit.CATI.GenericDialerWebService.Bootstrapper.Properties;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.Security.Crypto.Web;

namespace Confirmit.CATI.GenericDialerWebService.Bootstrapper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <returns>
        /// 0 - All rigth, 10 - error of bootstrapper, other code - error of installation
        /// </returns>
        [STAThread]
        private static int Main()
        {
            LoadConfigFile();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string msiFolderPath = Path.Combine(Application.StartupPath, DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss"));
            Directory.CreateDirectory(msiFolderPath);

            ILogger logger = new FileAndConsoleLogger(Path.Combine(msiFolderPath, "Bootstrapper.log"));
            logger.WriteLog("Start bootstrapper actions");

            IObjectFactory objectFactory = new ObjectFactory();
            var genericDialerWsMsiParameters = new GenericDialerWSMsiParameters();

            IBootstrapperEngine bootstrapperEngine = new BootstrapperEngine(objectFactory, logger);
            IParametersValidateService genericDialerParametersValidateService = new GenericDialerWSMsiParametersValidateService(
                genericDialerWsMsiParameters, logger, bootstrapperEngine, objectFactory);
            IParametersReader genericDialerWsMsiParametersReader = new GenericDialerWSMsiParametersReader(
                genericDialerWsMsiParameters, logger, GetRegistryPath(bootstrapperEngine.SideBySideName));
            IMsiParametersStringCreator genericDialerParameterStringCreator = new GenericDialerWSMsiParametersStringCreator(
                genericDialerWsMsiParameters, logger, bootstrapperEngine, genericDialerParametersValidateService, genericDialerWsMsiParametersReader);

            var updateInformation = new UpdateInformation(GetProductNamePathBeforeSxSName(), CurrentInstallationSpecification.CurrentInstallationSystemType, new SelectActionForm());

            var bootstrapper = new BootstrapperLibrary.Bootstrapper(objectFactory, logger, bootstrapperEngine);

            int returnValue = bootstrapper.Execute(
                Environment.GetCommandLineArgs(),
                Resources.Setup,
                msiFolderPath,
                genericDialerParameterStringCreator,
                genericDialerParametersValidateService,
                updateInformation);

            logger.WriteLog("Finish bootstrapper actions");

            return returnValue;
        }

        private static string GetProductNamePathBeforeSxSName()
        { 
            switch(CurrentInstallationSpecification.CurrentGenericDialerInstallationType)
            {
                case GenericDialerInstallationType.Generic:
                    return "Confirmit CATI Generic Dialer Web Service";
                case GenericDialerInstallationType.SimulatorGeneric:
                    return "Confirmit CATI Simulator (G) Dialer Web Service";
                case GenericDialerInstallationType.LtuSimulatorGeneric:
                    return "Confirmit CATI LTU Simulator (G) Dialer Web Service";
                default:
                    throw new Exception("Crytical internal error: Unsupported installation type");
            }
        }

        private static string GetRegistryPath(string sideBySideName)
        {
            switch (CurrentInstallationSpecification.CurrentGenericDialerInstallationType)
            {
                case GenericDialerInstallationType.Generic:
                    return @"SOFTWARE\Confirmit\GENERIC_DialerWebService." + sideBySideName;
                case GenericDialerInstallationType.SimulatorGeneric:
                    return @"SOFTWARE\Confirmit\GENERIC_SIMULATOR_DialerWebService." + sideBySideName;
                case GenericDialerInstallationType.LtuSimulatorGeneric:
                    return @"SOFTWARE\Confirmit\GENERIC_LTU_SIMULATOR_DialerWebService." + sideBySideName;
                default:
                    throw new Exception("Crytical internal error: Unsupported installation type");
            }
        }

        private static void LoadConfigFile()
        {
            const string configFileContent = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <system.web>
    <machineKey validation=""SHA1"" validationKey=""DFD06FF3058574D6A2E7B592E33A80EDE007E596EE8A1754CC68ECE5B44EA9A8A7ABC3C53F0EA8BD90B26AE301977609F142578EF3EB1E05A592C92F354E3341"" decryptionKey=""FFEE834B2BB87C08D5CA27C65E0F2512B8EC3F0F3DAE0AFB""/>
  </system.web>
</configuration>";

            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "App.config");
            File.WriteAllText(configPath, configFileContent);

            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configPath);
            EncryptionUsingMachineKey.Encrypt(DataProtection.All, "test");
            File.Delete(configPath);
        }
    }
}
