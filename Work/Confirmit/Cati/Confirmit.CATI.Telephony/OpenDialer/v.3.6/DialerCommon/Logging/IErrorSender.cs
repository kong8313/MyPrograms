using System.Collections.Generic;
using Confirmit.CATI.Common.Contracts.ErrorReportingService;

namespace DialerCommon.Logging
{
    public interface IErrorSender
    {        
        void SendErrorMessages(IErrorReportingService x, IEnumerable<ErrorMessage> messages);
    }
}
