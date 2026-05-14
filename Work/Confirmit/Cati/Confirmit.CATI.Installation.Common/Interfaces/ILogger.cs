using System.Diagnostics;

namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface ILogger
    {
        void WriteLog(string message, params object[] parameters);

        void WriteLog(bool isPrintOnConsole, string message, params object[] parameters);

        void WriteLog(TraceEventType traceType, string message, params object[] parameters);

        void WriteLog(bool isPrintOnConsole, TraceEventType traceType, string message, params object[] parameters);
    }
}