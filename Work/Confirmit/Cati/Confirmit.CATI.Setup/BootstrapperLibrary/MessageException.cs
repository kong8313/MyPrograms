using System;
using System.Diagnostics;

namespace BootstrapperLibrary
{
    public class MessageException : Exception
    {
        private readonly TraceEventType _severity;

        public TraceEventType Severity
        { 
            get
            {
                return _severity;
            }
        }

        public MessageException(string message, TraceEventType severity)
            : base(message)
        {
            _severity = severity;
        }
    }
}
