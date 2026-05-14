using System;
using System.ServiceModel;

using Confirmit.CATI.Common.Exceptions;

namespace DialerCommon.DialerExceptions
{
    [Serializable]
    public class DialerWsNotInitializedException : UserMessageException
    {
        /// <summary>
        /// Constructs the <see cref="FaultException"/> based on current exception details.
        /// </summary>
        /// <returns></returns>
        public override FaultException ToFault()
        {
            return new FaultException<DialerWsNotInitializedExceptionDetails>(
                new DialerWsNotInitializedExceptionDetails());
        }
    }

    public class DialerWsNotInitializedExceptionDetails : UserMessageExceptionDetails
    {
        /// <summary>
        /// Constructs the <see cref="DialerWsNotInitializedException"/> based on current details.
        /// </summary>
        /// <returns></returns>
        public override UserMessageException ToException()
        {
            return new DialerWsNotInitializedException();
        }
    }
}
