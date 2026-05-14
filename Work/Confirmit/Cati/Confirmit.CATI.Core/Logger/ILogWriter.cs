using Confirmit.Logging;

namespace Confirmit.CATI.Core.Logger
{
    public interface ILogWriter
    {
        void Write(LogLevel logLevel, string message, CustomField[] eventFields, string loggerName = "KibanaLogWriter");
    }
}