using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerNotFoundException : CatiException
    {
        public DialerNotFoundException(int dialerId)
            : base(string.Format("Dialer with dialerId = {0} not found.", dialerId))
        {
        }
    }
}