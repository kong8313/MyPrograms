using Confirmit.CATI.Installation.Common.Interfaces;

namespace BootstrapperLibrary
{
    public class ReadingInstallationParameters
    {
        public IDatabaseEngine DatabaseEngine { get; private set; }

        public string DeployLogin { get; private set; }

        public string DeployPassword { get; private set; }

        public string InstallLocation { get; private set; }

        public string IsOverrideConfig { get; private set; }

        public ReadingInstallationParameters(string installLocation)
            : this(installLocation, null, string.Empty, string.Empty, string.Empty)
        {
        }

        public ReadingInstallationParameters(IDatabaseEngine databaseEngine, string deployLogin, string deployPassword)
            : this(string.Empty, databaseEngine, string.Empty, deployLogin, deployPassword)
        {
        }

        public ReadingInstallationParameters(string installLocation, string isOverrideConfig)
            : this(installLocation, null, isOverrideConfig, string.Empty, string.Empty)
        {
        }

        public ReadingInstallationParameters(string installLocation, IDatabaseEngine databaseEngine, string isOverrideConfig, string deployLogin, string deployPassword)
        {
            DatabaseEngine = databaseEngine;
            InstallLocation = installLocation;
            IsOverrideConfig = isOverrideConfig;
            DeployLogin = deployLogin;
            DeployPassword = deployPassword;
        }
    }
}