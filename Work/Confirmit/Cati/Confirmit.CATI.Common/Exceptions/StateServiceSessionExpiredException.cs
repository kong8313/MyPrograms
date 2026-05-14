using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    public class StateServiceSessionExpiredException : UserMessageException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class.
        /// </summary>
        public StateServiceSessionExpiredException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public StateServiceSessionExpiredException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public StateServiceSessionExpiredException(string message, string messageKey)
            : base(message)
        {
            MessageKey = messageKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message and inner exception.
        /// </summary>
        /// <param name="message">The  error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public StateServiceSessionExpiredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected StateServiceSessionExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                MessageKey = info.GetString("MessageKey");
            }
        }

        public override FaultException ToFault()
        {
            return new FaultException<StateServiceSessionExpiredExceptionDetails>(
                new StateServiceSessionExpiredExceptionDetails(),
                Message);
        }
    }
}
