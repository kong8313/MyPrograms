using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Represents exception that occurs if scheduling script validation fails
    /// </summary>
    public class SchedulingScriptSyntaxErrorException : UserMessageException
    {
         #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingScriptSyntaxErrorException"/> class.
        /// </summary>
        public SchedulingScriptSyntaxErrorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SchedulingScriptSyntaxErrorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageException"/> class
        /// with the specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="messageKey">The resource key of the error message</param>
        public SchedulingScriptSyntaxErrorException(string message, string messageKey)
            : base(message)
        {
            MessageKey = messageKey;
        }

        public SchedulingScriptSyntaxErrorException(string message,  CompilerErrorCollection errorDetails)
            : this(message)
        {
            this.ErrorDetails = errorDetails;
        }       

        #endregion

        public CompilerErrorCollection ErrorDetails
        {
            get; set;
        }

        public override FaultException ToFault()
        {
            return new FaultException<SchedulingScriptSyntaxErrorExceptionDetails>(
                new SchedulingScriptSyntaxErrorExceptionDetails { Message = Message, MessageKey = MessageKey, ErrorDetails = ErrorDetails },
                Message);
        }
    }

    /// <summary>
    /// Details of SchedulingScriptSyntaxErrorException fault.
    /// </summary>
    [KnownType(typeof(CompilerError))]
    public class SchedulingScriptSyntaxErrorExceptionDetails : UserMessageExceptionDetails
    {
        public CompilerErrorCollection ErrorDetails
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs the <see cref="PredictiveSurveyWithoutDialerException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new SchedulingScriptSyntaxErrorException(Message, ErrorDetails);
        }
    }
}
