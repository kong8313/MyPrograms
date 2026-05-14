namespace Confirmit.CATI.Telephony
{
    public interface ILoggerHealthChecker
    {
        void Reset();
        void Check(int companyId, int dialerId);
        void ForcedCheck(int companyId, int dialerId);
    }
}