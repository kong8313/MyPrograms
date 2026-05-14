using System;
using System.Text;
using System.Threading;
using System.Web;
using System.Diagnostics;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Logger
{
    public abstract class CatiTraceListener : TraceListener
    {
        protected string ServerName
        {
            get;
            private set;
        }

        protected CatiTraceListener()
        {
            ServerName = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>().MachineName;
        }

        public static void FallbackLog(string text)
        {
            Console.Error.Write(text?.Replace("\r", "\\r").Replace("\n", "\\n"));
        }

        protected static StringBuilder BuildMessage(string format, object[] args)
        {
            var sb = new StringBuilder();
            if (args == null)
            {
                sb.Append(format);
            }
            else if (string.IsNullOrEmpty(format))
            {
                sb = BuildMessage(args);
            }
            else
            {
                sb.AppendFormat(format, args);
            }
            
            return sb;
        }

        protected static StringBuilder BuildMessage(object[] data)
        {
            var sb = new StringBuilder();
            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (i != 0)
                    {
                        sb.Append(", ");
                    }

                    if (data[i] != null)
                    {
                        sb.Append(data[i]);
                    }
                }
            }
            
            return sb;
        }

        /// <summary>
        /// Some kind of internal trace filter.
        /// </summary>
        /// <param name="message">The trace message.</param>
        /// <returns>true to trace the specified event; otherwise, false.</returns>
        protected static bool ShouldTrace(string message)
        {
            return CatiTraceListenerFilter.ShouldTrace(message);
        }

        protected static bool ShouldTraceErrorAsWarning(string message)
        {
            return CatiTraceListenerFilter.ShouldTraceErrorAsWarning(message);
        }

        protected static string GetEventTypeName(string source, string eventType)
        {
            if (Enum.IsDefined(typeof(ClientErrorSource), source))
            {
                // If error came from the client (cati or monitoring consoles)
               return source;
            }
            
            // if this is backend or supervisor error
            return eventType;
        }
    }
}