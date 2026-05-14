using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Exception class contains a set of DialerParameterError objects. 
    /// It allows formatting several parameter errors to an only readable message for human supervisor.
    /// </summary>
    [Serializable]
    public class ParametersException : DialerException
    {
        /// <summary>
        /// Set of dialer parameter errors connected to this exception
        /// </summary>
        public IEnumerable<DialerParameterError> Errors;

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="errors">List of dialer parameter errors</param>
        public ParametersException(IEnumerable<DialerParameterError> errors)
            : base(DialerErrorCode.InvalidParameter, "One or more parameters are not valid") // Note: the Message property is overriden below
        {
            if (errors == null)
            {
               throw new ArgumentException("Errors collection is [null]"); 
            }

            if (!errors.Any())
            {
                throw new ArgumentException("Errors collection is [empty]");
            }

            Errors = errors;
        }

        /// <summary>
        /// Gets formatted string which contains all dialer parameter errors
        /// </summary>
        /// <returns>The dialer parameter errors short string representation</returns>
        public override string Message
        {
            get
            {
                var strBuilder = new StringBuilder();

                foreach (var error in Errors)
                {
                    strBuilder.AppendFormat("{0}: {1}.\n", error.Name, error.ErrorDescription);
                }

                return strBuilder.ToString();
            }
        }
    }
}