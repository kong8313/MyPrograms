using System.Diagnostics;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface ILogger
    {
        void WriteLog(string message, params object[] parameters);

        void WriteLog(TraceEventType traceType, string message, params object[] parameters);
    }
}