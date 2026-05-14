using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.Contracts.ErrorReportingService;

namespace DialerCommon.Logging
{    
    public class DailerServiceErrorSender : IErrorSender
    {        
        public void SendErrorMessages(IErrorReportingService serviceInstance, IEnumerable<ErrorMessage> messages)
        {
            serviceInstance.SendDialerErrorMessages(messages);
        }
    }
}
