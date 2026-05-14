using System;
using Confirmit.Logging;

namespace Confirmit.CATI.Core.Logger.Kibana
{
    public class KibanaLogWriter : ILogWriter
    {
        public void Write(LogLevel logLevel, string message, CustomField[] eventFields, string loggerName = "KibanaLogWriter")
        {
            try
            {
                LogFactory.EnsureConfigurationIsLoaded();

                var logger = LogFactory.GetLogger(loggerName);

                logger.LogCustom(logLevel, () => message, eventFields);
            }
            catch (Exception ex)
            {
                CatiTraceListener.FallbackLog(
                    $"Error writing to kibana log:\r\n{ex}\r\n\r\n" +
                    $"Original log message:\r\n[{logLevel}]: {message}\r\n");
            }
        }
    }
}