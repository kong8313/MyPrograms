using ConfirmitDialerInterface;

namespace DialerCommon.Logging
{
    public interface  ICommonLogger: ILogger
    {
        void InitReportingWsTraceListener();

        void Error(int companyId, string sourceCodeLocation, string message, params object[] args);
    }
}
    