using System.Diagnostics;
using Confirmit.CATI.Installation.Common.Interfaces;
using System;

namespace Confirmit.CATI.Installation.Common
{
    public class TraceLogger : ILogger
    {
        public void WriteLog(string message, params object[] parameters)
        {
            WriteLog(false, message, parameters);
        }

        public void WriteLog(bool isPrintOnConsole, string message, params object[] parameters)
        {
            WriteLog(isPrintOnConsole, TraceEventType.Information, message, parameters);
        }

        public void WriteLog(TraceEventType traceType, string message, params object[] parameters)
        {
            WriteLog(false, traceType, message, parameters);
        }

        public void WriteLog(bool isPrintOnConsole, TraceEventType traceType, string message, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                message = string.Format(message, parameters);
            }

            DateTime nowTime = DateTime.Now;
            Trace.TraceInformation(nowTime.ToLongTimeString() + "." + nowTime.Millisecond + ": " + traceType + ": " + message);
        }
    }
}