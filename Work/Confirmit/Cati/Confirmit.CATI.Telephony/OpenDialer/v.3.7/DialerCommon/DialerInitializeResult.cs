using ConfirmitDialerInterface;

namespace DialerCommon
{
    public class DialerInitializeResult
    {
        public DialerErrorCode DialerErrorCode { get; }
        public string ErrorMessage { get; }

        public DialerInitializeResult(DialerErrorCode dialerErrorCode, string errorMessage = null)
        {
            DialerErrorCode = dialerErrorCode;
            ErrorMessage = errorMessage;
        }
    }
}