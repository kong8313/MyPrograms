using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Confirmit.CATI.Common.Exceptions;

using ConfirmitDialerInterface;

namespace DialerCommon.DialerParameters
{
    [Serializable]
    public class DialerParametersException : UserMessageException
    {
        public IEnumerable<DialerParameterError> Errors;


        public DialerParametersException(ParametersException exception)
        {
            Errors = exception.Errors;
        }

        public DialerParametersException(IEnumerable<DialerParameterError> errors)
        {
            Errors = errors;
        }

        /// <summary>
        /// Gets formatted string which contains all dialer parameter errors
        /// </summary>
        /// <returns></returns>
        public string GetExceptionFormattedString()
        {
            var strBuilder = new StringBuilder();

            foreach (var error in Errors)
            {
                strBuilder.AppendFormat("{0}: {1}.\n", error.Name, error.ErrorDescription);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Gets formatted string which contains all dialer parameter errors
        /// with full error description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            foreach (var error in Errors)
            {
                strBuilder.AppendFormat("{0}: {1}.\n", error.Name, error.FullDescription);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Constructs the <see cref="FaultException"/> based on current exception details.
        /// </summary>
        /// <returns></returns>
        public override FaultException ToFault()
        {
            return new FaultException<DialerParametersExceptionDetails>(
                new DialerParametersExceptionDetails(Errors),
                GetExceptionFormattedString());
        }
    }

    public class DialerParametersExceptionDetails : UserMessageExceptionDetails
    {
        public List<DialerParameterError> Errors;

        public DialerParametersExceptionDetails()
        {
            // The empty constructor is needed as the class is being used in FaultException<>
        }

        public DialerParametersExceptionDetails(IEnumerable<DialerParameterError> errors)
        {
            Errors = errors.ToList();
        }

        public DialerParametersExceptionDetails(ParametersException exception)
        {
            Errors = exception.Errors.ToList();
        }

        /// <summary>
        /// Constructs the <see cref="DialerParametersException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new DialerParametersException(Errors);
        }
    }
}
