using System;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Dialer exception class
    /// </summary>
    public class DialerException : Exception
    {
        /// <summary>
        /// Error code
        /// </summary>
        public DialerErrorCode ErrorCode { get; private set; }

        //TODO: We don't need the empty constructor. But there are some dependencies at the moment (ParametersException).
        //      It's need to be checked that the constructor removal is safe
        /// <summary>
        /// Obsolete. Should not be used.
        /// </summary>
        public DialerException()
            : this(DialerErrorCode.UnknownError, "Exception message is not specified")
        {
        }

        /// <summary>
        /// Constructor to be used if there is no proper error code in the <see cref="DialerErrorCode"/> enumeration.
        /// </summary>
        /// <param name="exceptionMessage">Exception message</param>
        public DialerException(string exceptionMessage)
            : this(DialerErrorCode.UnknownError, exceptionMessage)
        {
        }

        /// <summary>
        /// Constructor to be used if there is proper error code in the <see cref="DialerErrorCode"/> enumeration.
        /// </summary>
        /// <param name="errorCode">Error code. See <see cref="DialerErrorCode"/>.</param>
        /// <param name="exceptionMessage">Exception message</param>
        public DialerException(DialerErrorCode errorCode, string exceptionMessage)
            : base(exceptionMessage)
        {
            //TODO: Should we check that errorCode is not equal to DialerErrorCode.Success ???
            ErrorCode = errorCode;
        }
    }
}
