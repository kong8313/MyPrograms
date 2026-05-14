using System;
using System.Diagnostics;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Dialer writes trace messages to Confirmit CATI log via this interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Write a trace message to Confirmit CATI log.
        /// </summary>
        /// <param name="logLevel">trace event type accrding to .NET documentation</param>
        /// <param name="comment">
        /// Comment (or preamble) which is placed at the beginning of the trace message
        /// It's recommended to place class and method name here like as "DummyClass.DummyMethod"
        /// </param>
        /// <param name="message">The message itself</param>
        [Obsolete]
        void WriteLine(TraceEventType logLevel, string comment, string message);

        /// <summary>
        /// Write an error message to Confirmit CATI log.
        /// </summary>
        /// <param name="sourceCodeLocation">Reference to the source code.
        /// It is placed at the beginning of the trace message.
        /// It's recommended to place class and method name here like "DummyClass.DummyMethod".
        /// </param>
        /// <param name="message">The message itself</param>
        /// <param name="args"></param>
        void Error(string sourceCodeLocation, string message, params object[] args);

        /// <summary>
        /// Write an error message to Confirmit CATI log using the message delegate function.
        /// The message delegate function allows to create more sophisticated log messages.
        /// </summary>
        /// <param name="sourceCodeLocation">Reference to the source code.
        /// It is placed at the beginning of the trace message.
        /// It's recommended to place class and method name here like "DummyClass.DummyMethod".
        /// </param>
        /// <param name="messageFunc">The message delegate function. </param>
        void Error(string sourceCodeLocation, Func<string> messageFunc);

        /// <summary>
        /// Write a warning message to Confirmit CATI log.
        /// </summary>
        /// <param name="sourceCodeLocation">Reference to the source code.
        /// It is placed at the beginning of the trace message.
        /// It's recommended to place class and method name here like "DummyClass.DummyMethod".
        /// </param>
        /// <param name="message">The message itself</param>
        /// <param name="args"></param>
        void Warning(string sourceCodeLocation, string message, params object[] args);

        /// <summary>
        /// Write a warning message to Confirmit CATI log using the message delegate function.
        /// The message delegate function allows to create more sophisticated log messages.
        /// </summary>
        /// <param name="sourceCodeLocation">Reference to the source code.
        /// It is placed at the beginning of the trace message.
        /// It's recommended to place class and method name here like "DummyClass.DummyMethod".
        /// </param>
        /// <param name="messageFunc">The message delegate function. </param>
        void Warning(string sourceCodeLocation, Func<string> messageFunc);

        /// <summary>
        /// Write an information message to Confirmit CATI log.
        /// </summary>
        /// <param name="sourceCodeLocation">Reference to the source code.
        /// It is placed at the beginning of the trace message.
        /// It's recommended to place class and method name here like "DummyClass.DummyMethod".
        /// </param>
        /// <param name="message">The message itself</param>
        /// <param name="args"></param>
        void Info(string sourceCodeLocation, string message, params object[] args);

        /// <summary>
        /// Write an information message to Confirmit CATI log using the message delegate function.
        /// The message delegate function allows to create more sophisticated log messages.
        /// </summary>
        /// <param name="sourceCodeLocation">Reference to the source code.
        /// It is placed at the beginning of the trace message.
        /// It's recommended to place class and method name here like "DummyClass.DummyMethod".
        /// </param>
        /// <param name="messageFunc">The message delegate function. </param>
        void Info(string sourceCodeLocation, Func<string> messageFunc);

        /// <summary>
        /// Write a verbose (debug) message to Confirmit CATI log.
        /// </summary>
        /// <param name="sourceCodeLocation">Reference to the source code.
        /// It is placed at the beginning of the trace message.
        /// It's recommended to place class and method name here like "DummyClass.DummyMethod".
        /// </param>
        /// <param name="message">The message itself</param>
        /// <param name="args"></param>
        void Verbose(string sourceCodeLocation, string message, params object[] args);

        /// <summary>
        /// Write a verbose (debug) message to Confirmit CATI log using the message delegate function.
        /// The message delegate function allows to create more sophisticated log messages.
        /// </summary>
        /// <param name="sourceCodeLocation">Reference to the source code.
        /// It is placed at the beginning of the trace message.
        /// It's recommended to place class and method name here like "DummyClass.DummyMethod".
        /// </param>
        /// <param name="messageFunc">The message delegate function. </param>
        void Verbose(string sourceCodeLocation, Func<string> messageFunc);

        /// <summary>
        /// Create a new logger in order to write messages to Confirmit CATI log with some new source name. 
        /// </summary>
        /// <param name="source">The source name</param>
        ILogger NewLogger(string source);
    }
}
