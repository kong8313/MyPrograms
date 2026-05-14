using System.Diagnostics;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace RunTestParallelUtility.UnitTests
{
    public class TempLogger : ILogger
    {
        public void WriteLog(string message, params object[] parameters)
        {
        }

        public void WriteLog(bool isPrintOnConsole, string message, params object[] parameters)
        {
        }

        public void WriteLog(TraceEventType traceType, string message, params object[] parameters)
        {
        }

        public void WriteLog(bool isPrintOnConsole, TraceEventType traceType, string message, params object[] parameters)
        {
        }
    }
}
