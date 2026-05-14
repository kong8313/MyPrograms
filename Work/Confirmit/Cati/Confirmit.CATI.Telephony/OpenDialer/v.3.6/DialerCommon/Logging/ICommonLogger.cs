using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;

namespace DialerCommon.Logging
{
    public interface  ICommonLogger: ILogger
    {
        LogFileGetter LogFileGetter { get; }

        void InitReportingWsTraceListener();

        void Error(int companyId, string sourceCodeLocation, string message, params object[] args);
    }
}
    