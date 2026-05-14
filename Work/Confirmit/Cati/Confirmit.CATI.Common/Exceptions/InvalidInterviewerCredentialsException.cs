using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when interviewer specifies incorrect login name or password.
    /// </summary>
    public class InvalidInterviewerCredentialsException : UserMessageException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInterviewerCredentialsException"/> class.
        /// </summary>
        public InvalidInterviewerCredentialsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInterviewerCredentialsException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public InvalidInterviewerCredentialsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInterviewerCredentialsException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public InvalidInterviewerCredentialsException(string message, string messageKey)
            : base(message)
        {
            MessageKey = messageKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInterviewerCredentialsException"/> class
        /// with the specified error message and inner exception.
        /// </summary>
        /// <param name="message">The  error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidInterviewerCredentialsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidInterviewerCredentialsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        public override FaultException ToFault()
        {
            return new FaultException<InvalidInterviewerCredentialsExceptionDetails>(
                new InvalidInterviewerCredentialsExceptionDetails {Message = Message, MessageKey = MessageKey},
                Message);
        }
    }

    /// <summary>
    /// Details of InvalidInterviewerCredentials fault.
    /// </summary>
    public class InvalidInterviewerCredentialsExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="InvalidInterviewerCredentialsException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new InvalidInterviewerCredentialsException(Message, MessageKey);
        }
    }
}