using System;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Installation.Common
{
    public class ConfirmitCatiEngine : IConfirmitCatiEngine
    {
        private readonly ILogger _logger;

        public ConfirmitCatiEngine(ILogger logger)
        {
            _logger = logger;
        }

        public string GetSchemeAndHostFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "http://localhost";
            }

            var uri = new Uri(url);
            return string.Format("{0}{1}{2}", uri.Scheme, Uri.SchemeDelimiter, uri.Authority);
        }

        public string GetConfirmParameterValue(string confirmDatabaseName, IDatabaseEngine databaseEngine, string parameterName)
        {
            var parameter = databaseEngine.ExecuteScalar<object>(confirmDatabaseName, string.Format("SELECT ConfigValue FROM CfgConfig WHERE ConfigName = '{0}'", parameterName));
            return parameter == null ? "" : parameter.ToString();
        }
    }
}
