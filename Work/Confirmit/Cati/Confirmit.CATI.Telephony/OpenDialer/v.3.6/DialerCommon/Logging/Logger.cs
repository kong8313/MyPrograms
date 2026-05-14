using System;
using System.Diagnostics;
using System.Linq;
using DialerCommon;
using DialerCommon.Logging;
using DialerCommon.Logging.TraceListeners;

namespace Confirmit.CATI.Telephony
{
    public class Logger : ICommonLogger
    {
        private readonly TraceSource _traceSource;
        private readonly UtcOffsetString _utcOffsetString;
        private readonly Lazy<LogFileGetter> _lazyLogFileGetter;
        private readonly RequestId _requestId;
        public LogFileGetter LogFileGetter => _lazyLogFileGetter.Value;

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="sourceName"></param>

        public Logger(string sourceName) : this(sourceName, new UtcOffsetSource())
        {
            _requestId = null;
        }
        public Logger(string sourceName, RequestId requestId) : this(sourceName, new UtcOffsetSource())
        {
            _requestId = requestId;
        }

        /// <summary>
        /// This constructor is used for testing purposes only
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="utcOffsetSource"></param>
        public Logger(string sourceName, IUtcOffsetSource utcOffsetSource) : this(new TraceSource(sourceName), utcOffsetSource)
        {
        }

        /// <summary>
        /// This constructor is needed for testing purposes
        /// </summary>
        /// <param name="traceSource"></param>
        public Logger(TraceSource traceSource) : this(traceSource, new UtcOffsetSource())
        {
        }

        protected Logger(TraceSource traceSource, IUtcOffsetSource utcOffsetSource)
        {
            _utcOffsetString = new UtcOffsetString(new UtcOffsetSource());
            _traceSource = traceSource;
            _lazyLogFileGetter = new Lazy<LogFileGetter>(() => new LogFileGetter(_traceSource));
        }

        private string DateTimeAsString()
        {
            // Currently we use following format: YYYY-MM-DD hh:mm:ss.mmm
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public void WriteLine(TraceEventType logLevel, string comment, string message)
        {
            string logMessage = AssembleLogMessage(comment, message);

            try
            {
                _traceSource.TraceEvent(logLevel, 0, logMessage);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            {
                // We suppress the exception here because there is no way to report about it
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        public void Error(string sourceCodeLocation, string message, params object[] args)
        {
            Log(TraceEventType.Error, sourceCodeLocation, message, args);
        }

        public void Error(string sourceCodeLocation, Func<string> messageFunc)
        {
            Log(TraceEventType.Error, sourceCodeLocation, messageFunc);
        }

        public void Warning(string sourceCodeLocation, string message, params object[] args)
        {
            Log(TraceEventType.Warning, sourceCodeLocation, message, args);
        }

        public void Warning(string sourceCodeLocation, Func<string> messageFunc)
        {
            Log(TraceEventType.Warning, sourceCodeLocation, messageFunc);
        }

        public void Info(string sourceCodeLocation, string message, params object[] args)
        {
            Log(TraceEventType.Information, sourceCodeLocation, message, args);
        }

        public void Info(string sourceCodeLocation, Func<string> messageFunc)
        {
            Log(TraceEventType.Information, sourceCodeLocation, messageFunc);
        }

        public void Verbose(string sourceCodeLocation, string message, params object[] args)
        {
            Log(TraceEventType.Verbose, sourceCodeLocation, message, args);
        }

        public void Verbose(string sourceCodeLocation, Func<string> messageFunc)
        {
            Log(TraceEventType.Verbose, sourceCodeLocation, messageFunc);
        }

        public ConfirmitDialerInterface.ILogger NewLogger(string source)
        {
            return new Logger(source, _requestId);
        }

        private string AssembleLogMessage(string comment, string message)
        {
            var requestIdValue = _requestId != null ? $"[rid={_requestId.Value}]\t" : "";

            return $"{_utcOffsetString} {DateTimeAsString()}\t{comment}\t{requestIdValue}{message}";
        }

        private void Log(TraceEventType logLevel, string sourceCodeLocation, string message, object[] args)
        {
            try
            {
                WriteLine(logLevel, sourceCodeLocation, string.Format(message, args));
            }
            catch (Exception ex)
            {
                // We don't catch exception in the Error call below because of it's assumed that an exception thrown 
                // from the below code means that something is wrong with the code and the problem shouldn't be masked, but fixed.
                Error("Logger." + new StackFrame(1, true).GetMethod().Name,
                    "{0} /// sourceCodeLocation='{1}', message='{2}', args=[{3}], StackTrace: {4}",
                    ex,
                    sourceCodeLocation,
                    message ?? "null",
                    (args == null) ? "null" : string.Join(", ", args.Select(x => "'" + x.ToString() + "'")),
                    (new StackTrace(true)).ToString());
            }
        }

        private void Log(TraceEventType logLevel, string sourceCodeLocation, Func<string> messageFunc)
        {
            try
            {
                WriteLine(logLevel, sourceCodeLocation, messageFunc());
            }
            catch (Exception ex)
            {
                // We don't catch exception in the Error call below because of it's assumed that an exception thrown 
                // from the below code means that something is wrong with the code and the problem shouldn't be masked, but fixed.
                Error("Logger." + new StackFrame(1, true).GetMethod().Name,
                    "{0} /// sourceCodeLocation='{1}', messageFunc='{2}', StackTrace: {3}",
                    ex,
                    sourceCodeLocation,
                    messageFunc.Method,
                    (new StackTrace(true)).ToString());
            }
        }

        public void WriteErrorToFileTraceListenerOnly(string errorMessage)
        {
            var listener = _traceSource.Listeners.OfType<TextToLogFileTraceListener>().FirstOrDefault();

            if (listener != null)
            {
                listener.WriteLine(errorMessage);
            }
        }

        public void HealthTest(string comment, string message)
        {
            string logMessage = AssembleLogMessage(comment, message);

            Trace.TraceInformation(logMessage);
            _traceSource.TraceEvent(TraceEventType.Information, 0, logMessage);
        }

        public void InitReportingWsTraceListener()
        {
            var listener = _traceSource.Listeners.OfType<WsReportingTraceListener>().FirstOrDefault();

            if (listener == null)
            {
                listener = new WsReportingTraceListener(new DailerServiceErrorSender(), this)
                {
                    Filter = new EventTypeFilter(SourceLevels.Error)
                };

                _traceSource.Listeners.Add(listener);
            }
        }

        public void Error(int companyId, string sourceCodeLocation, string message, params object[] args)
        {
            SetCompanyId(companyId);

            WriteLine(TraceEventType.Error, sourceCodeLocation, string.Format(message, args));

            ResetCompanyId();
        }

        private void SetCompanyId(int companyId)
        {
            var listener = _traceSource.Listeners.OfType<WsReportingTraceListener>().FirstOrDefault();

            if (listener != null)
            {
                listener.SetCompanyId(companyId);
            }
        }

        private void ResetCompanyId()
        {
            SetCompanyId(0);
        }
    }
}
