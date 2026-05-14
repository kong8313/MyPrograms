using System;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Class for common operations with exceptions.
    /// </summary>
    public class ExceptionManager
    {
        /// <summary>
        /// Returns a new ArgumentException for the specified argument name.
        /// </summary>
        /// <param name="argumentName">Name of the invalid argument.</param>
        public static ArgumentException NewArgumentException(string argumentName)
        {
            return new ArgumentException("Invalid argument value", argumentName);
        }

        /// <summary>
        /// Returns a new ArgumentNullException for the specified argument name.
        /// </summary>
        /// <param name="argumentName">Name of the invalid argument.</param>
        public static ArgumentException NewArgumentNullException(string argumentName)
        {
            return new ArgumentNullException(argumentName);
        }


        /// <summary>
        /// Returns a new InternalErrorException with the specified message.
        /// </summary>
        /// <param name="message">Internal error message</param>
        public static InternalErrorException NewInternalErrorException(string message)
        {
            return new InternalErrorException(message);
        }

        public static InternalErrorException NewInternalErrorException(string message, params object[] pars)
        {
            return new InternalErrorException(string.Format(message, pars));
        }

        /// <summary>
        /// Returns a new UserMessageException with the specified message.
        /// </summary>
        /// <param name="message">User message</param>
        public static UserMessageException NewUserMessageException(string message)
        {
            return new UserMessageException(message);
        }
        
    }
}