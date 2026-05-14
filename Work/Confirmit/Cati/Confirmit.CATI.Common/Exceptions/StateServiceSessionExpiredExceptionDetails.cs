namespace Confirmit.CATI.Common.Exceptions
{
    public class StateServiceSessionExpiredExceptionDetails : UserMessageExceptionDetails
    {
        public override UserMessageException ToException()
        {
            return new StateServiceSessionExpiredException();
        }
    }
}
