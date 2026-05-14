using Confirmit.Security.Crypto;
using Microsoft.Win32;

namespace PreventLocking
{
    public class ConfirmitRegistrator
    {
        public void SetRegistry(string serverName, string surveyC)
        {
            SetRegistry(serverName, surveyC, string.Empty);
            SetRegistry(serverName, surveyC, @"Wow6432Node\");
        }

        private void SetRegistry(string serverName, string surveyC, string subKey)
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\{subKey}FIRM", true))
            {
                key.SetValue("SqlEngine", "MsSql");
            }

            using (RegistryKey key = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\{subKey}FIRM\ConFIRM", true))
            {
                key.SetValue("SQLServerName", serverName);
                key.SetValue("ConfirmitDatabaseServerSystemServerName", serverName);
            }

            using (RegistryKey key = Registry.LocalMachine.CreateSubKey($@"SOFTWARE\{subKey}FIRM\ConFIRM\SqlSettings", true))
            {
                var encryptedSurveyC = new CryptComp().Encrypt(surveyC);
                key.SetValue("SurveyC", encryptedSurveyC);
            }
        }

        public void GetRegistryParameters(out string serverName, out string surveyC)
        {
            serverName = null;
            surveyC = null;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FIRM\ConFIRM", true))
            {
                if (key != null)
                {
                    serverName = (string) key.GetValue("SQLServerName");
                }
            }

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FIRM\ConFIRM\SqlSettings", true))
            {
                if (key != null)
                {
                    var encryptedSurveyC = (string) key.GetValue("SurveyC");
                    surveyC = new CryptComp().Decrypt(encryptedSurveyC);
                }
            }
        }
    }
}