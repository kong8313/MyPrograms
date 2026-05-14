using System;
using System.Diagnostics;

namespace CustomDialerSimulator
{
    public class Logger
    {
        private readonly TextWriterTraceListener _traceListener;

        public Logger(string filePath)
        {
            _traceListener = new TextWriterTraceListener(filePath);
        }

        public void WriteLine(string text)
        {
            _traceListener.WriteLine(string.Format("{0}:\t{1}", DateTime.Now, text));
            _traceListener.Flush();
        }
    }
}
