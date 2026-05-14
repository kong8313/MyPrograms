using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    public class PasswordDoesNotSatisfyRulesException : UserMessageException
    {
        public PasswordDoesNotSatisfyRulesException()
        {
        }

        public PasswordDoesNotSatisfyRulesException(string message) : base(message)
        {
        }

        public PasswordDoesNotSatisfyRulesException(string message, string messageKey)
            : base(message)
        {
            MessageKey = messageKey;
        }

        public PasswordDoesNotSatisfyRulesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PasswordDoesNotSatisfyRulesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override FaultException ToFault()
        {
            return new FaultException<PasswordDoesNotSatisfyRulesExceptionDetails>(
                new PasswordDoesNotSatisfyRulesExceptionDetails { Message = Message, MessageKey = MessageKey },
                Message);
        }
    }

    /// <summary>
    /// Details of PasswordDoesNotSatisfyRulesException fault.
    /// </summary>
    public class PasswordDoesNotSatisfyRulesExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="PasswordDoesNotSatisfyRulesException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new PasswordDoesNotSatisfyRulesException(Message, MessageKey);
        }
    }
}
