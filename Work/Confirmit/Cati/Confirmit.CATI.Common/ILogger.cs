using System;
using System.Diagnostics;

namespace Confirmit.CATI.Common
{
    /// <summary>
    /// Base interface for logger classes.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// When implemented by derived class - logs the specified message with the specified severity.
        /// </summary>
        /// <param name="text">The message to log.</param>
        /// <param name="severity">The severity.</param>
        void Log(string text, TraceEventType severity);
        void Log(Exception ex);
    }
}
