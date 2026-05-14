using System;

namespace Confirmit.CATI.Supervisor.Core.Exceptions
{
    /// <summary>
    /// Dialler configuration exception.
    /// </summary>
    public class DiallerConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiallerConfigurationException"/> class.
        /// </summary>
        public DiallerConfigurationException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiallerConfigurationException"/> class.
        /// <param name="message">Exception message.</param>
        /// </summary>
        public DiallerConfigurationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiallerConfigurationException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public DiallerConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
