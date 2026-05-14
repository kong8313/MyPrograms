using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occurs when Confirmit quota definition is not in sync with the database (for example when quota name has been changed
    /// and survey has not been relaunched).
    /// </summary>
    public class QuotaNotInSyncException : UserMessageException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaNotInSyncException"/> class.
        /// </summary>
        public QuotaNotInSyncException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaNotInSyncException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public QuotaNotInSyncException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaNotInSyncException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public QuotaNotInSyncException(string message, string messageKey)
            : base(message)
        {
            MessageKey = messageKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuotaNotInSyncException"/> class
        /// with the specified error message and inner exception.
        /// </summary>
        /// <param name="message">The  error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public QuotaNotInSyncException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected QuotaNotInSyncException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                MessageKey = info.GetString("MessageKey");
            }
        }

        #endregion
    }
}