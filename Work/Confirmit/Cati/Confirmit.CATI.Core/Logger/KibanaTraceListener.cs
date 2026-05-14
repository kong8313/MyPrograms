using System;
using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.Logging;

namespace Confirmit.CATI.Core.Logger
{
    public class KibanaTraceListener : CatiTraceListener
    {
        public override void TraceEvent(TraceEventCache eventCache, string source,
                                       TraceEventType severity, int id, string message)
        {
            WriteToKibanaLog(severity, message, source, id);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source,
                                        TraceEventType severity, int id,
                                        string format, params object[] args)
        {
            var message = BuildMessage(format, args).ToString();
            WriteToKibanaLog(severity, message, source, id);
        }

        public override void TraceData(TraceEventCache eventCache, string source,
                                       TraceEventType severity, int id, object data)
        {
            var message = data.ToString();
            WriteToKibanaLog(severity, message, source, id);
        }

        public override void TraceData(TraceEventCache eventCache, string source,
                                       TraceEventType severity, int id, params object[] data)
        {
            var message = BuildMessage(data).ToString();
            WriteToKibanaLog(severity, message, source, id);
        }

        public override void Write(string message)
        {
            WriteToKibanaLog(TraceEventType.Information, message, string.Empty, 0);
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        private void WriteToKibanaLog(TraceEventType severity, string text, string source, int id)
        {
            try
            {
                if (!ShouldTrace(text))
                {
                    return;
                }

                if (severity == TraceEventType.Error && ShouldTraceErrorAsWarning(text))
                {
                    severity = TraceEventType.Warning;
                }
                
                var logWriter = ServiceLocator.Resolve<ILogWriter>();
                var companyInfo = ServiceLocator.Resolve<ICompanyInfo>();
                var companyId = companyInfo.GetCompanyId(id, source);
                logWriter.Write(Map2LogLevel(severity), text, LogData.ToCustomFields(companyId));
            }
            catch (Exception ex)
            {
                FallbackLog(
                    $"Error writing to kibana log:\r\n{ex}\r\n\r\n" +
                    $"Original log message:\r\n[{severity},{id}]: {text}\r\n");
            }
        }

        private LogLevel Map2LogLevel(TraceEventType severity)
        {
            switch (severity)
            {
                case TraceEventType.Information:
                case TraceEventType.Verbose:
                    return LogLevel.Info;
                case TraceEventType.Warning:
                    return LogLevel.Warn;
                case TraceEventType.Error:
                    return LogLevel.Error;
                case TraceEventType.Critical:
                    return LogLevel.Fatal;
                default:
                    return LogLevel.Trace;
            }
        }
    }
}
