using System;
using System.Diagnostics;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class ConsoleLogger : ILogger
    {
        public void WriteLog(string message, params object[] parameters)
        {
            WriteLog(TraceEventType.Information, message, parameters);
        }

        public void WriteLog(TraceEventType traceType, string message, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                message = string.Format(message, parameters);
            }

            Console.WriteLine(message);
        }
    }
}