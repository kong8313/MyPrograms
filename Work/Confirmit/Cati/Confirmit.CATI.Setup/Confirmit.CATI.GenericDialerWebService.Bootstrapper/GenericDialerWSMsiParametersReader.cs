using System.Diagnostics;
using BootstrapperLibrary;
using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.Win32;

namespace Confirmit.CATI.GenericDialerWebService.Bootstrapper
{
    public class GenericDialerWSMsiParametersReader : DialerWSMsiParametersReader
    {
        private readonly GenericDialerWSMsiParameters _parameters;

        public GenericDialerWSMsiParametersReader(GenericDialerWSMsiParameters parameters, ILogger logger, string registryPath)
            : base(parameters, logger, registryPath)
        {
            _parameters = parameters;
        }

        public override void ReadParameters(ReadingInstallationParameters readingInstallationParameters)
        {
            base.ReadParameters(readingInstallationParameters);

            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(RegistryPath);
            if (regKey == null)
            {
                throw new MessageException(string.Format("Registry path 'HKLM\\{0}' is not found", RegistryPath), TraceEventType.Warning);
            }

            string[] subRegKeys = regKey.GetValueNames();

            foreach (string subRegKey in subRegKeys)
            {
                switch (subRegKey)
                {
                    case "CERTIFICATE_TYPE":
                        _parameters.CertificateType = GetValue(regKey, subRegKey);
                        break;
                    case "TEST_CERTIFICATE_NAME":
                        _parameters.TestCertificateName = GetValue(regKey, subRegKey);
                        break;
                    case "CERTIFICATE_PATH":
                        _parameters.CertificatePath = GetValue(regKey, subRegKey);
                        break;
                    case "ENCRYPTED_CERTIFICATE_PASSWORD":
                        _parameters.EncryptedCertificatePassword = GetValue(regKey, subRegKey);
                        break;
                    case "AUTHORIZATION_KEY":
                        _parameters.AuthorizationKey = GetValue(regKey, subRegKey);
                        break;
                    case "BINDING_CONFIGURATION":
                        _parameters.BindingConfiguration = GetValue(regKey, subRegKey);
                        break;
                    case "USE_AUTHORIZATION":
                        _parameters.UseAuthorization = GetValue(regKey, subRegKey);
                        break;
                    case "DIALER_ID":
                        _parameters.DialerId = GetValue(regKey, subRegKey);
                        break;
                    case "GENERIC_DIALER_TYPE":
                        _parameters.GenericDialerType = GetValue(regKey, subRegKey);
                        break;
                }
            }
        }

        private string GetValue(RegistryKey regKey, string subRegKey)
        {
            return (string)regKey.GetValue(subRegKey);
        }
    }
}