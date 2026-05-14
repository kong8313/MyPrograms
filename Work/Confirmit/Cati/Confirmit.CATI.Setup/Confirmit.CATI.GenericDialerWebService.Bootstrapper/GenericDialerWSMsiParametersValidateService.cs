using System.Diagnostics;
using System.Text;

using BootstrapperLibrary;
using BootstrapperLibrary.Interfaces;
using Confirmit.CATI.GenericDialerWebService.Bootstrapper.Properties;
using Confirmit.CATI.Installation.Common.Interfaces;
using Confirmit.Security.Crypto.Web;

namespace Confirmit.CATI.GenericDialerWebService.Bootstrapper
{
    class GenericDialerWSMsiParametersValidateService : DialerWSMsiParametersValidateService
    {
        private readonly GenericDialerWSMsiParameters _parameters;
        private readonly ICertificateEngine _certificateEngine;

        public GenericDialerWSMsiParametersValidateService(GenericDialerWSMsiParameters parameters, ILogger logger, IBootstrapperEngine bootstrapperEngine, IObjectFactory objectFactory) 
            : base(parameters, logger, bootstrapperEngine, objectFactory)
        {
            _parameters = parameters;
            _certificateEngine = objectFactory.CreateCertificateEngineObject(objectFactory.CreateDialogservice());
        }

        public override void ValidateParameters()
        {
            base.ValidateParameters();

            // Do additional validations for Generic Dialer WS installation
            var errInfo = new StringBuilder();

            if (CurrentInstallationSpecification.CurrentGenericDialerInstallationType == GenericDialerInstallationType.Generic)
            {
                errInfo.AppendLine(CheckParameter("GENERIC_DIALER_TYPE", _parameters.GenericDialerType));
                errInfo.AppendLine(_bootstrapperEngine.VerifyParameterValue("GENERIC_DIALER_TYPE", _parameters.GenericDialerType, new[] { "NotDefined", "InVade", "Sytel" }));
            }

            string checkResult = CheckParameter("BINDING_CONFIGURATION", _parameters.BindingConfiguration);
            errInfo.AppendLine(string.IsNullOrEmpty(checkResult)
                ? _bootstrapperEngine.VerifyParameterValue("BINDING_CONFIGURATION", _parameters.BindingConfiguration, new[] { "http", "https" })
                : checkResult);

            checkResult = CheckParameter("USE_AUTHORIZATION", _parameters.UseAuthorization);
            errInfo.AppendLine(string.IsNullOrEmpty(checkResult)
                ? _bootstrapperEngine.VerifyParameterValue("USE_AUTHORIZATION", _parameters.UseAuthorization, new[] { "1", string.Empty })
                : checkResult);

            errInfo.AppendLine(_bootstrapperEngine.VerifyParameterValue("CERTIFICATE_TYPE", _parameters.CertificateType, new[] { "Test", "Real" }));

            errInfo.AppendLine(_bootstrapperEngine.VerifyCertificateParameters(
                _certificateEngine, 
                _parameters.CertificateType, 
                _parameters.TestCertificateName, 
                _parameters.CertificatePath,
                EncryptionUsingMachineKey.Decrypt(DataProtection.All, _parameters.EncryptedCertificatePassword)));

            errInfo.AppendLine(CheckParameter("AUTHORIZATION_KEY", _parameters.AuthorizationKey));

            errInfo.AppendLine(CheckParameter("DIALER_ID", _parameters.DialerId));

            string errInfoStr = _bootstrapperEngine.RemoveExtraLineFeeds(errInfo);
            if (!string.IsNullOrEmpty(errInfoStr))
            {
                throw new MessageException(errInfoStr, TraceEventType.Warning);
            }

            _confirmitCATIValidator.ValidateNotNegativeIntParameter(_parameters.DialerId, "DIALER_ID");
        }

        public override void CheckPrerequisites()
        {
            _prereqChecker.VerifyIsFramework462Installed();
        }

        private string CheckParameter(string parameterName, string parameterValue)
        {
            if (parameterValue == null)
            {
                return string.Format(Resources.ParameterIsNotDefinedFormat, parameterName);
            }

            return string.Empty;
        }
    }
}
