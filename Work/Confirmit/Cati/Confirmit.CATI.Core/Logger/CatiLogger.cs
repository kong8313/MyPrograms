using System;
using System.Diagnostics;

using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Logger
{
    public class CatiLogger : ILogger
    {
        public void Log(string text, TraceEventType severity)
        {
            switch (severity)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    Trace.TraceError(text);
                    break;
                case TraceEventType.Warning:
                    Trace.TraceWarning(text);
                    break;
                case TraceEventType.Information:
                    Trace.TraceInformation(text);
                    break;
                case TraceEventType.Verbose:
                    TraceHelper.TraceVerbose(text);
                    break;
            }
        }

        public void Log(Exception ex)
        {
            Trace.TraceError(ex.ToString());
        }
    }
}