using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when login to dialer is executed in manual mode for predictive survey.
    /// </summary>
    public class ManualUserInPredictiveModeException : UserMessageException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class.
        /// </summary>
        public ManualUserInPredictiveModeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ManualUserInPredictiveModeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public ManualUserInPredictiveModeException(string message, string messageKey)
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
        public ManualUserInPredictiveModeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ManualUserInPredictiveModeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                MessageKey = info.GetString("MessageKey");
            }
        }

        #endregion

        public override FaultException ToFault()
        {
            return new FaultException<ManualUserInPredictiveModeExceptionDetails>(
                new ManualUserInPredictiveModeExceptionDetails {Message = Message, MessageKey = MessageKey},
                Message);
        }
    }

    /// <summary>
    /// Details of ManualUserInPredictiveMode fault.
    /// </summary>
    public class ManualUserInPredictiveModeExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="ManualUserInPredictiveModeException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new ManualUserInPredictiveModeException(Message, MessageKey);
        }
    }
}