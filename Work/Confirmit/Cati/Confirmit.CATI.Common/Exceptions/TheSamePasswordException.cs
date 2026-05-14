using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    public class TheSamePasswordException : UserMessageException
    {
        public TheSamePasswordException()
        {
        }

        public TheSamePasswordException(string message) : base(message)
        {
        }

        public TheSamePasswordException(string message, string messageKey)
            : base(message)
        {
            MessageKey = messageKey;
        }

        public TheSamePasswordException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TheSamePasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override FaultException ToFault()
        {
            return new FaultException<TheSamePasswordExceptionDetails>(
                new TheSamePasswordExceptionDetails { Message = Message, MessageKey = MessageKey },
                Message);
        }
    }

    /// <summary>
    /// Details of TheSamePasswordException fault.
    /// </summary>
    public class TheSamePasswordExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="TheSamePasswordException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new TheSamePasswordException(Message, MessageKey);
        }
    }
}
