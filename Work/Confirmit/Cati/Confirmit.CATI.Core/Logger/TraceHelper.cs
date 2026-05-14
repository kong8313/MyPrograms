using System;
using System.Diagnostics;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Core.Logger
{
    /// <summary>
    /// Contains additional methods for Trace class.
    /// </summary>
    public static class TraceHelper
    {
        /// <summary>
        /// Writes a verbose message to the trace listeners in the System.Diagnostics.Trace.Listeners collection.
        /// </summary>
        /// <param name="messageFormat">The message format.</param>
        /// <param name="args">The formatting arguments.</param>
        [StringFormatMethod("messageFormat")]
        public static void TraceVerbose(string messageFormat, params object[] args)
        {
            if (!ServiceLocator.Resolve<ISystemSettings>().Logging.TraceVerbose)
            {
                return;
            }

            var trace = new TraceEventCache();
            var source = BackendEventSource.Backend.ToString();
            foreach (TraceListener listener in Trace.Listeners)
            {
                if (args == null || args.Length == 0)
                {
                    listener.TraceEvent(trace, source, TraceEventType.Verbose, 0, messageFormat);
                }
                else
                {
                    listener.TraceEvent(trace, source, TraceEventType.Verbose, 0, messageFormat, args);
                }

                if (Trace.AutoFlush)
                {
                    listener.Flush();
                }
            }
        }

        /// <summary>
        /// Write exception to the trace listeners in the System.Diagnostics.Trace.Listeners collection.
        /// Node: UserMessageException is written to log as warning, otherwise exception is written to log as error
        /// </summary>
        /// <param name="exception">The exception to write.</param>
        /// <param name="comments">The comment to write.</param>
        public static void TraceException(Exception exception, string comments = null)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            string exceptionText = (comments ?? string.Empty) + Environment.NewLine + exception;

            if (exception is UserMessageException)
            {
                Trace.TraceWarning(exceptionText);
            }
            else
            {
                Trace.TraceError(exceptionText);
            }
        }

        /// <summary>
        /// Writes a client error message to the trace listeners in the System.Diagnostics.Trace.Listeners collection.
        /// </summary>
        /// <param name="companyId">The ID of the company where the error has occurred.</param>
        /// <param name="message">The error message.</param>
        /// <param name="errorSource">The source of the error (Console, Player, etc).</param>
        public static void TraceClientError(int companyId, string message, ClientErrorSource errorSource)
        {
            foreach (TraceListener listener in Trace.Listeners)
            {
                listener.TraceEvent(new TraceEventCache(), errorSource.ToString(), TraceEventType.Warning, companyId, message);

                if (Trace.AutoFlush)
                {
                    listener.Flush();
                }
            }
        }

        /// <summary>
        /// Removes trace listeners that are not useful in a container environment:
        /// <list type="bullet">
        ///   <item><see cref="EventLogTraceListener"/> — Windows Event Log is not available in K8s pods.</item>
        ///   <item><see cref="DefaultTraceListener"/> — produces plain text output to stdout that
        ///     duplicates structured JSON from <see cref="KibanaTraceListener"/> and confuses log aggregators like Loki.</item>
        ///   <item><see cref="ConsoleTraceListener"/> — same plain text duplication issue as DefaultTraceListener.</item>
        /// </list>
        /// Should be called once during application startup.
        /// </summary>
        public static void RemoveNonContainerTraceListeners()
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;

            var listenersToRemove = Trace.Listeners.OfType<TraceListener>()
                .Where(l => l is EventLogTraceListener || l is DefaultTraceListener || l is ConsoleTraceListener)
                .ToList();
            foreach (var listener in listenersToRemove)
            {
                Trace.Listeners.Remove(listener);
            }
        }
    }
}
