using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when non-manual user tries to work with a predictive survey without dialer.
    /// </summary>
    public class PredictiveSurveyWithoutDialerException : UserMessageException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class.
        /// </summary>
        public PredictiveSurveyWithoutDialerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public PredictiveSurveyWithoutDialerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public PredictiveSurveyWithoutDialerException(string message, string messageKey)
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
        public PredictiveSurveyWithoutDialerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PredictiveSurveyWithoutDialerException(SerializationInfo info, StreamingContext context)
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
            return new FaultException<PredictiveSurveyWithoutDialerExceptionDetails>(
                new PredictiveSurveyWithoutDialerExceptionDetails { Message = Message, MessageKey = MessageKey },
                Message);
        }
    }

    /// <summary>
    /// Details of ManualUserInPredictiveMode fault.
    /// </summary>
    public class PredictiveSurveyWithoutDialerExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="PredictiveSurveyWithoutDialerException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new PredictiveSurveyWithoutDialerException(Message, MessageKey);
        }
    }
}