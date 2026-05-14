using System;

namespace Confirmit.CATI.Common.Exceptions
{
    /// <summary>
    /// Occures during scheduling script execution and contains rule, subrule and action numbers in which exception was thrown.
    /// It does not indicate problems in the system.
    /// </summary>
    public class SchedulingExecutionException : UserMessageException
    {
        /// <summary>
        /// Gets or sets the number of rule where error happened.
        /// </summary>
        public int RuleNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of subrule where error happened.
        /// </summary>
        public int SubRuleNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of action which caused error.
        /// </summary>
        public int ActionNumber { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingExecutionException"/> class.
        /// </summary>
        public SchedulingExecutionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingExecutionException"/> class
        /// with the specified error message and details where error happened
        /// </summary>
        /// <param name="message">The error message.</param>
        public SchedulingExecutionException(string message, Exception innerException, int ruleId, int subRuleId, int actionId)
            : base(message, innerException)
        {
            RuleNumber = ruleId;
            SubRuleNumber = subRuleId;
            ActionNumber = actionId;
        }
    }
}
