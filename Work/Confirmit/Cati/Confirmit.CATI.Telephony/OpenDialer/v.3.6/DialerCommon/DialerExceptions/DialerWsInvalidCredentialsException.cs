using System;
using System.ServiceModel;

using Confirmit.CATI.Common.Exceptions;

namespace DialerCommon.DialerExceptions
{
    [Serializable]
    public class DialerWsInvalidCredentialsException : UserMessageException
    {
        public DialerWsInvalidCredentialsException(string message) :
            base(message)
        {
        }

        /// <summary>
        /// Constructs the <see cref="FaultException"/> based on current exception details.
        /// </summary>
        /// <returns></returns>
        public override FaultException ToFault()
        {
            return new FaultException<DialerWsInvalidCredentialsExceptionDetails>(
                new DialerWsInvalidCredentialsExceptionDetails { Message = this.Message } );
        }
    }

    public class DialerWsInvalidCredentialsExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="DialerWsInvalidCredentialsException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new DialerWsInvalidCredentialsException(Message);
        }
    }
}
