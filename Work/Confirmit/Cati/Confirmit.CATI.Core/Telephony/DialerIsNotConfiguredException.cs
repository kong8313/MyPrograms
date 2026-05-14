using System;

namespace Confirmit.CATI.Core.Telephony
{
    /// <summary>
    /// Is being generated if dialer is not proper configured. I.e. when it looks like we work with no dialer.
    /// </summary>
    public class DialerIsNotConfiguredException : Exception
    {
        public DialerIsNotConfiguredException(string message)
            : base(message)
        {
        }
    }
}