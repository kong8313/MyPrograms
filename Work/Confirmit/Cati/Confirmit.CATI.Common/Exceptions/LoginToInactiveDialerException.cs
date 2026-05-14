using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when unexpected results or behavior found in the system.  If it is
    /// thrown - there is a problem in the system and it should be fixed.
    /// </summary>
    [Serializable]
    public class LoginToInactiveDialerException : UserMessageException
    {
        /// <summary>
        /// Contains an inactive dialer ID to which user try to connect
        /// </summary>
        public int DialerId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginToInactiveDialerException"/> class.
        /// </summary>
        public LoginToInactiveDialerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginToInactiveDialerException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="dialerId">Inactive dialer ID to which user try to connect</param>
        public LoginToInactiveDialerException(string message, int dialerId)
            : base(message)
        {
            DialerId = dialerId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginToInactiveDialerException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        /// <param name="dialerId">Inactive dialer ID to which user try to connect</param>
        public LoginToInactiveDialerException(string message, string messageKey, int dialerId)
            : base(message)
        {
            MessageKey = messageKey;
            DialerId = dialerId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginToInactiveDialerException"/> class
        /// with the specified error message and inner exception.
        /// </summary>
        /// <param name="message">The  error message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="dialerId">Inactive dialer ID to which user try to connect</param>
        public LoginToInactiveDialerException(string message, int dialerId, Exception innerException)
            : base(message, innerException)
        {
            DialerId = dialerId;
        }

        protected LoginToInactiveDialerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                MessageKey = info.GetString("MessageKey");
            }
        }

        /// <summary>
        /// Constructs the <see cref="FaultException"/> based on current exception details.
        /// </summary>
        /// <returns></returns>
        public override FaultException ToFault()
        {
            return new FaultException<LoginToInactiveDialerExceptionDetails>(
                new LoginToInactiveDialerExceptionDetails { Message = Message, MessageKey = MessageKey, DialerId = DialerId},
                Message);
        }
    }


    /// <summary>
    /// The details of the user message fault. Used to pass exception details via WCF.
    /// </summary>
    public class LoginToInactiveDialerExceptionDetails: UserMessageExceptionDetails
    {
        public int DialerId { get; set; }

        /// <summary>
        /// Constructs the <see cref="LoginToInactiveDialerException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new LoginToInactiveDialerException(Message, MessageKey, DialerId);
        }
    }
}