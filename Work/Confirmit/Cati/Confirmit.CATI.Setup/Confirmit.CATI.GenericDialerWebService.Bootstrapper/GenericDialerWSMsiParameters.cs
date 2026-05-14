using System;
using System.Text;
using BootstrapperLibrary;

namespace Confirmit.CATI.GenericDialerWebService.Bootstrapper
{
    public class GenericDialerWSMsiParameters : DialerWSMsiParameters
    {
        public string CertificateType { get; set; }
        public string TestCertificateName { get; set; }
        public string CertificatePath { get; set; }
        public string EncryptedCertificatePassword { get; set; }
        public string AuthorizationKey { get; set; }
        public string BindingConfiguration { get; set; }
        public string UseAuthorization { get; set; }
        public string DialerId { get; set; }
        public string GenericDialerType { get; set; }

        public override string GenerateInstallationParametersString(bool isQuietMode)
        {
            var installationParametersString = new StringBuilder(base.GenerateInstallationParametersString(isQuietMode));

            installationParametersString.AppendFormat(" {0}=\"{1}\"", "CERTIFICATE_TYPE", CertificateType);
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "TEST_CERTIFICATE_NAME", TestCertificateName);
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "CERTIFICATE_PATH", CertificatePath);
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "ENCRYPTED_CERTIFICATE_PASSWORD", EncryptedCertificatePassword);
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "AUTHORIZATION_KEY", AuthorizationKey);
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "DIALER_WS_TYPE", GetDialerWsType());
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "BINDING_CONFIGURATION", BindingConfiguration);
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "USE_AUTHORIZATION", UseAuthorization);
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "DIALER_ID", DialerId);
            installationParametersString.AppendFormat(" {0}=\"{1}\"", "GENERIC_DIALER_TYPE", GenericDialerType);

            return installationParametersString.ToString();
        }

        private string GetDialerWsType()
        {
            switch (CurrentInstallationSpecification.CurrentGenericDialerInstallationType)
            {
                case GenericDialerInstallationType.Generic:
                    return "GENERIC";
                case GenericDialerInstallationType.SimulatorGeneric:
                    return "GENERIC_SIMULATOR";
                case GenericDialerInstallationType.LtuSimulatorGeneric:
                    return "GENERIC_LTU_SIMULATOR";
                default:
                    throw new Exception($"Critical internal error: Unsupported installation type: {CurrentInstallationSpecification.CurrentGenericDialerInstallationType}");
            }
        }
    }
}