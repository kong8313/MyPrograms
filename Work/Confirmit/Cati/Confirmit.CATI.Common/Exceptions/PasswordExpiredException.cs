using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    public class PasswordExpiredException : UserMessageException
    {
        public PasswordExpiredException()
        {
        }

        public PasswordExpiredException(string message)
            : base(message)
        {
        }

        public PasswordExpiredException(string message, string messageKey)
            : base(message)
        {
            MessageKey = messageKey;
        }

        public PasswordExpiredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PasswordExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override FaultException ToFault()
        {
            return new FaultException<PasswordExpiredExceptionDetails>(
                new PasswordExpiredExceptionDetails { Message = Message, MessageKey = MessageKey },
                Message);
        }
    }

    /// <summary>
    /// Details of PasswordExpiredException fault.
    /// </summary>
    public class PasswordExpiredExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="PasswordExpiredException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new PasswordExpiredException(Message, MessageKey);
        }
    }
}
