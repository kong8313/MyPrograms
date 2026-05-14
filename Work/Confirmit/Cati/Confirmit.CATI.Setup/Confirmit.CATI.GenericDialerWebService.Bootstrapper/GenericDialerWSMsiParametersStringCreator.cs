using BootstrapperLibrary;
using BootstrapperLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.GenericDialerWebService.Bootstrapper
{
    public class GenericDialerWSMsiParametersStringCreator : IMsiParametersStringCreator
    {
        private readonly GenericDialerWSMsiParameters _parameters;
        private readonly ILogger _logger;
        private readonly IBootstrapperEngine _bootstrapperEngine;
        private readonly IParametersValidateService _parametersValidateService;
        private readonly IParametersReader _parametersReader;

        public GenericDialerWSMsiParametersStringCreator(
            GenericDialerWSMsiParameters parameters, ILogger logger, IBootstrapperEngine bootstrapperEngine,
            IParametersValidateService parametersValidateService, IParametersReader parametersReader)
        {
            _parameters = parameters;
            _logger = logger;
            _bootstrapperEngine = bootstrapperEngine;
            _parametersValidateService = parametersValidateService;
            _parametersReader = parametersReader;
        }

        public string CreateInstallationParametersString(ReadingInstallationParameters readingInstallationParameters)
        {
            _parametersReader.ReadParameters(readingInstallationParameters);

            _parametersValidateService.ValidateParameters();

            return _parameters.GenerateInstallationParametersString(_bootstrapperEngine.IsQuietMode);
        }
    }
}
