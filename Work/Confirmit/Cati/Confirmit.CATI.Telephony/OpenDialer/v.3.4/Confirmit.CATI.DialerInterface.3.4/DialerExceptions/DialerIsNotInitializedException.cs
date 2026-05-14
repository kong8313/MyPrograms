using System;

namespace ConfirmitDialerInterface
{
    [Serializable]
    public class DialerIsNotInitializedException : DialerException
    {
        public DialerIsNotInitializedException(string exceptionMessage) :
            base(exceptionMessage)
        {
        }
    }
}
