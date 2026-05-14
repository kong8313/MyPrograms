using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when expected results or behavior found in the system that user should
    /// know about. It does not indicate problems in the system.
    /// </summary>
    [Serializable]
    public class UserMessageException : CatiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class.
        /// </summary>
        public UserMessageException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UserMessageException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public UserMessageException(string message, string messageKey)
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
        public UserMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UserMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                MessageKey = info.GetString("MessageKey");
            }
        }

        /// <summary>
        /// Gets or sets the resource key of the error message.
        /// </summary>
        public string MessageKey { get; set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("MessageKey", MessageKey);
        }

        /// <summary>
        /// Constructs the <see cref="FaultException"/> based on current exception details.
        /// </summary>
        /// <returns></returns>
        public virtual FaultException ToFault()
        {
            return new FaultException<UserMessageExceptionDetails>(
                new UserMessageExceptionDetails {Message = Message, MessageKey = MessageKey},
                Message);
        }

        public override System.Collections.IDictionary Data
        {
            get
            {
                return null;
            }
        }
    }

    /// <summary>
    /// The details of the user message fault. Used to pass exception details via WCF.
    /// </summary>
    public class UserMessageExceptionDetails
    {
        /// <summary>
        /// Gets or sets the text of the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the resource key of the error message.
        /// </summary>
        public string MessageKey { get; set; }

        /// <summary>
        /// Constructs the <see cref="UserMessageException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public virtual UserMessageException ToException()
        {
            return new UserMessageException(Message, MessageKey);
        }
    }
}