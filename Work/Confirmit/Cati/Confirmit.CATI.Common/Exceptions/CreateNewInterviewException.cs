using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when console user has unsupported operational system
    /// </summary>
    public class CreateNewInterviewException : UserMessageException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestFailedException"/> class.
        /// </summary>
        public CreateNewInterviewException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CreateNewInterviewException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestFailedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CreateNewInterviewException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected CreateNewInterviewException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Constructs the <see cref="FaultException"/> based on current exception details.
        /// </summary>
        /// <returns></returns>
        public override FaultException ToFault()
        {
            return new FaultException<CreateNewInterviewExceptionDetails>(
                new CreateNewInterviewExceptionDetails { Message = Message },
                Message);
        }
    }

    /// <summary>
    /// The details of the user message fault. Used to pass exception details via WCF.
    /// </summary>
    public class CreateNewInterviewExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="UserMessageException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new CreateNewInterviewException(Message);
        }
    }
}