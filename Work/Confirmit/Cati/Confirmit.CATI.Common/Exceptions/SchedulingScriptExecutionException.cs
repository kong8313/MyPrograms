using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// This exception occurs then a scheduling script is executed and something goes wrong. 
    /// E.g. it cannot find a survey variable or convert a value and so on.
    /// </summary>
    public class SchedulingScriptExecutionException : UserMessageException
    {

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingScriptExecutionException"/> class.
        /// </summary>
        public SchedulingScriptExecutionException ()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingScriptExecutionException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SchedulingScriptExecutionException (string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingScriptExecutionException"/> class
        /// with the specified error message and the message key.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public SchedulingScriptExecutionException(string message, string messageKey)
            : base(message, messageKey)
        {
        }


        #endregion
    }

}
