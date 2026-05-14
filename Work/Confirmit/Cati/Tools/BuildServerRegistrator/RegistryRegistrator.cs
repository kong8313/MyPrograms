using Microsoft.Win32;

namespace BuildServerRegistrator
{
    public class RegistryRegistrator
    {
        public void Register(ConfigParameters serverConfigurations, DefaultCredentialsMaker defaultCredentialsMaker)
        {
            using (RegistryKey registry = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FIRM"))
            {
                registry.SetValue("SqlEngine", "MsSql");
            }

            using (RegistryKey registry = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FIRM\ConFIRM"))
            {
                registry.SetValue("SQLServerName", serverConfigurations.SqlServerName);
            }

            using (RegistryKey registry = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FIRM\ConFIRM\SqlSettings"))
            {
                registry.SetValue("SurveyC", defaultCredentialsMaker.GetEncryptedSurveyCCredentials());
            }
            
            using (RegistryKey registry = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\FIRM"))
            {
                registry.SetValue("SqlEngine", "MsSql");
            }

            using (RegistryKey registry = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\FIRM\ConFIRM"))
            {
                registry.SetValue("SQLServerName", serverConfigurations.SqlServerName);
            }

            using (RegistryKey registry = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\FIRM\ConFIRM\SqlSettings"))
            {
                registry.SetValue("SurveyC", defaultCredentialsMaker.GetEncryptedSurveyCCredentials());
            }
        }
    }
}
