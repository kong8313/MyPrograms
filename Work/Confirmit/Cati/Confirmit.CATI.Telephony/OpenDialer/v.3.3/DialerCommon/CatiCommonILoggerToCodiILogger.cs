using System;
using System.Diagnostics;

using ILoggerCatiCommon = Confirmit.CATI.Common.ILogger;
using ILoggerCodi = ConfirmitDialerInterface.ILogger;

namespace DialerCommon
{
    public class CatiCommonILoggerToCodiILogger : ILoggerCatiCommon
    {
        private readonly ILoggerCodi _openDialerInterfaceLogger;

        public CatiCommonILoggerToCodiILogger(ILoggerCodi openDialerInterfaceLogger)
        {
            _openDialerInterfaceLogger = openDialerInterfaceLogger;
        }

        public void Log(string text, TraceEventType severity)
        {
            var sourceCodeLocation = new StackFrame(1, true).GetMethod().Name;

            switch (severity)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    _openDialerInterfaceLogger.Error(sourceCodeLocation, text);
                    break;

                case TraceEventType.Warning:
                    _openDialerInterfaceLogger.Warning(sourceCodeLocation, text);
                    break;

                case TraceEventType.Information:
                    _openDialerInterfaceLogger.Info(sourceCodeLocation, text);
                    break;

                // TraceEventType.Verbose and others
                default:
                    _openDialerInterfaceLogger.Verbose(sourceCodeLocation, text);
                    break;
            }
        }

        public void Log(Exception ex)
        {
            var sourceCodeLocation = new StackFrame(1, true).GetMethod().Name;
            _openDialerInterfaceLogger.Error(sourceCodeLocation, ex.ToString());
        }
    }
}