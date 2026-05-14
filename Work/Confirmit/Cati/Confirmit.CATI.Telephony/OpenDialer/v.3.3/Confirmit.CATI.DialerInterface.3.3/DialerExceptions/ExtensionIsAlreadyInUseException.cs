using System;

namespace ConfirmitDialerInterface
{
    [Serializable]
    public class ExtensionIsAlreadyInUseException : DialerException
    {
        public ExtensionIsAlreadyInUseException(string exceptionMessage) :
            base(DialerErrorCode.ResourceAlreadyInUse, exceptionMessage)
        {
        }
    }
}
