using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when login to dialer is executed in manual mode for predictive survey.
    /// </summary>
    public class SpellCheckerLanguageIsNotSupportedException : UserMessageException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class.
        /// </summary>
        public SpellCheckerLanguageIsNotSupportedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellCheckerLanguageIsNotSupportedException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SpellCheckerLanguageIsNotSupportedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellCheckerLanguageIsNotSupportedException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public SpellCheckerLanguageIsNotSupportedException(string message, string messageKey)
            : base(message)
        {
            MessageKey = messageKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellCheckerLanguageIsNotSupportedException"/> class. 
        /// with the specified error message and inner exception.
        /// </summary>
        /// <param name="message">
        /// The  error message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public SpellCheckerLanguageIsNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SpellCheckerLanguageIsNotSupportedException(SerializationInfo info, StreamingContext context)
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
            return new FaultException<SpellCheckerLanguageIsNotSupportedExceptionDetails>(
                new SpellCheckerLanguageIsNotSupportedExceptionDetails() { Message = Message, MessageKey = MessageKey },
                Message);
        }
    }

    /// <summary>
    /// Details of SurveyInManualDialingMode fault.
    /// </summary>
    public class SpellCheckerLanguageIsNotSupportedExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="SurveyInManualDialingModeException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new SpellCheckerLanguageIsNotSupportedException(Message, MessageKey);
        }
    }
}