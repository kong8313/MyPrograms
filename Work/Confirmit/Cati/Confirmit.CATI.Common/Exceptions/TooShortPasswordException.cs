using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    [Serializable]
    public class TooShortPasswordException : UserMessageException
    {
        /// <summary>
        /// Contains minimal allowed password length.
        /// This value can be shown to user so (s)he could compose correct password
        /// </summary>
        public int MinimalLength { get; set; }
        
        public TooShortPasswordException()
        {
        }

        public TooShortPasswordException(string message, int minimalLength) : base(message)
        {
            MinimalLength = minimalLength;
        }

        public TooShortPasswordException(string message, string messageKey, int minimalLength)
            : base(message)
        {
            MessageKey = messageKey;
            MinimalLength = minimalLength;
        }

        public TooShortPasswordException(string message, int minimalLength, Exception innerException)
            : base(message, innerException)
        {
            MinimalLength = minimalLength;
        }

        protected TooShortPasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                MinimalLength = info.GetInt32("MinimalLength");
            }
        }

        public override FaultException ToFault()
        {
            return new FaultException<TooShortPasswordExceptionDetails>(
                new TooShortPasswordExceptionDetails { Message = Message, MessageKey = MessageKey, MinimalLength = MinimalLength },
                Message);
        }
    }

    /// <summary>
    /// Details of TooShortPasswordException fault.
    /// </summary>
    public class TooShortPasswordExceptionDetails : UserMessageExceptionDetails
    {
        public int MinimalLength { get; set; }
        
        /// <summary>
        /// Constructs the <see cref="TooShortPasswordException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new TooShortPasswordException(Message, MessageKey, MinimalLength);
        }
    }
}