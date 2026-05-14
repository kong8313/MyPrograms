using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when unexpected results or behavior found in the system.  If it is
    /// thrown - there is a problem in the system and it should be fixed.
    /// </summary>
    [Serializable]
    public class InternalErrorException : CatiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
        /// </summary>
        public InternalErrorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalErrorException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InternalErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalErrorException"/> class
        /// with the specified error message and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InternalErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalErrorException"/> class.
        /// </summary>
        /// <param name="info">The 
        /// <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.</param>
        /// <param name="context">The 
        /// <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains
        /// contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/>
        /// parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The
        /// class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). 
        /// </exception>
        protected InternalErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}