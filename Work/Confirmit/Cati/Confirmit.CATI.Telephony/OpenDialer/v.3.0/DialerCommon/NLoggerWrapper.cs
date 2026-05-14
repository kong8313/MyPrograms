using System;
using System.Diagnostics;
using ConfirmitDialerInterface;
using NLog;
using ILogger = ConfirmitDialerInterface.ILogger;

namespace DialerCommon
{
    public class NLoggerWrapper : ILogger
    {
        private readonly Logger _logger;

        //TODO: move it to the settings
        private const string SourceCodeLocationDelimiter = ": ";

        public NLoggerWrapper(string source, DateTime assemblyStartTime)
        {
            // Next to lines is the attempt to use a logEvent in order to form the log file name.
            // This comment can be useful if we will have to get rid of 'GDC' (see below).
            //var logEvent = new LogEventInfo { Message = "", Level = LogLevel.Info, TimeStamp = DateTime.Now, Properties = { new KeyValuePair<object, object>("appStartTime", DateTime.Now.ToString("yyyyMMddHHmmss")) } };
            //_logger.Log(logEvent); 

            // Set log creation datetime to the 'LogFileStartTime' variable that will be available to use 
            // in the config file as log file name suffix
            var startTimeStr = string.Format("{0:yyyyMMdd}T{0:HHmmss.fffzzz}", assemblyStartTime).Replace(":", string.Empty);

#pragma warning disable 612,618
            GDC.Set("LogFileStartTime", startTimeStr);
#pragma warning restore 612,618

            _logger = LogManager.GetLogger(source);
        }

        public NLoggerWrapper(string source)
            : this(source, DateTime.Now)
        {
        }

        public void WriteLine(TraceEventType logLevel, string sourceCodeLocation, string message)
        {
            LogLevel nlogLogLevel;

            switch (logLevel)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    nlogLogLevel = LogLevel.Error;
                    break;

                case TraceEventType.Warning:
                    nlogLogLevel = LogLevel.Warn;
                    break;

                case TraceEventType.Information:
                    nlogLogLevel = LogLevel.Info;
                    break;

                // TraceEventType.Verbose and others
                default:
                    nlogLogLevel = LogLevel.Trace;
                    break;
            }

            Log(nlogLogLevel, sourceCodeLocation, () => message);
        }

        public void Error(string sourceCodeLocation, string message, params object[] args)
        {
            Log(LogLevel.Error, sourceCodeLocation, message, args);
        }

        public void Error(string sourceCodeLocation, Func<string> messageFunc)
        {
            Log(LogLevel.Error, sourceCodeLocation, messageFunc);
        }

        public void Warning(string sourceCodeLocation, string message, params object[] args)
        {
            Log(LogLevel.Warn, sourceCodeLocation, message, args);
        }

        public void Warning(string sourceCodeLocation, Func<string> messageFunc)
        {
            Log(LogLevel.Warn, sourceCodeLocation, messageFunc);
        }

        public void Info(string sourceCodeLocation, string message, params object[] args)
        {
            Log(LogLevel.Info, sourceCodeLocation, message, args);
        }

        public void Info(string sourceCodeLocation, Func<string> messageFunc)
        {
            Log(LogLevel.Info, sourceCodeLocation, messageFunc);
        }

        public void Verbose(string sourceCodeLocation, string message, params object[] args)
        {
            Log(LogLevel.Trace, sourceCodeLocation, message, args);
        }

        public void Verbose(string sourceCodeLocation, Func<string> messageFunc)
        {
            Log(LogLevel.Trace, sourceCodeLocation, messageFunc);
        }

        public ILogger NewLogger(string source)
        {
            return new NLoggerWrapper(source);
        }

        private string AssembleLogString(string sourceCodeLocation, Func<string> messageFunc)
        {
            return sourceCodeLocation + SourceCodeLocationDelimiter + messageFunc();
        }

        private void Log(LogLevel logLevel, string sourceCodeLocation, string message, object[] args)
        {
            try
            {
                _logger.Log(
                    logLevel, () => AssembleLogString(sourceCodeLocation, () => string.Format(message, args)));
            }
            catch (Exception ex)
            {
                Error(
                    "NLoggerWrapper.Log",
                    "{0} /// logLevel={1}, sourceCodeLocation={2}, message={3}, args=[{4}], StackTrace: {5}",
                    ex,
                    logLevel,
                    sourceCodeLocation,
                    message,
                    (args == null) ? "null" : string.Join(",", args),
                    (new StackTrace(true)).ToString());
            }
        }

        private void Log(LogLevel logLevel, string sourceCodeLocation, Func<string> messageFunc)
        {
            try
            {
                _logger.Log(logLevel, () => AssembleLogString(sourceCodeLocation, messageFunc));
            }
            catch (Exception ex)
            {
                Error(
                    "NLoggerWrapper.Log",
                    "{0} /// logLevel={1}, sourceCodeLocation={2}, StackTrace: {3}",
                    ex, logLevel, sourceCodeLocation, (new StackTrace(true)).ToString());
            }
        }
    }
}
