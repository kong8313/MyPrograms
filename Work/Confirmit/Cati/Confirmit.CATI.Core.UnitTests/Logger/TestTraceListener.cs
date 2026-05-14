using System.Diagnostics;

namespace Confirmit.CATI.Core.UnitTests.Logger
{
    public class TestTraceListener : TraceListener
    {
        public static int TraceEventCount { get; set; }

        public TestTraceListener()
        {
            TraceEventCount = 0;
        }

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType severity, int id, string message)
        {
            TraceEventCount++;
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEventCount++;
        }
    }
}