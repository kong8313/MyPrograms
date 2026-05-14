using System;
using System.Diagnostics;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Common.Fakes
{
    public class StubILogger : ILogger 
    {
        private ILogger _inner;

        public StubILogger()
        {
            _inner = null;
        }

        public ILogger Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void LogStringTraceEventTypeDelegate(string text, TraceEventType severity);
        public LogStringTraceEventTypeDelegate LogStringTraceEventType;

        void ILogger.Log(string text, TraceEventType severity)
        {

            if (LogStringTraceEventType != null)
            {
                LogStringTraceEventType(text, severity);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Log(text, severity);
            }
        }

        public delegate void LogExceptionDelegate(Exception ex);
        public LogExceptionDelegate LogException;

        void ILogger.Log(Exception ex)
        {

            if (LogException != null)
            {
                LogException(ex);
            } else if (_inner != null)
            {
                ((ILogger)_inner).Log(ex);
            }
        }

    }
}