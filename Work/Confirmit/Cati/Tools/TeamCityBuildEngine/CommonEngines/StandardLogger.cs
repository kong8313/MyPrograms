using System;
using System.Diagnostics;
using Microsoft.Build.Utilities;
using TeamCityBuildEngine.Interfaces;

namespace TeamCityBuildEngine.CommonEngines
{
    public class StandardLogger : ILogger
    {
        private readonly TaskLoggingHelper _log;

        public StandardLogger(TaskLoggingHelper log)
        {
            _log = log;
        }        

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

            DateTime nowTime = DateTime.Now;
            _log.LogMessage(nowTime.ToLongTimeString() + "." + nowTime.Millisecond + ": " + traceType + ": " + message);
        }
    }
}
