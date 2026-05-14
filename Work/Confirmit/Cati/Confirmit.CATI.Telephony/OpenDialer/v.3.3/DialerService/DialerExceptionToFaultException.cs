using System;
using System.ServiceModel;
using Confirmit.CATI.Telephony.DialerService.Contract;
using ConfirmitDialerInterface;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Telephony.DialerService
{
    class DialerExceptionToFaultException
    {
        public FaultException Convert(Exception ex)
        {
            var dialerException = (ex as DialerException) ??
                // Not a DialerException - put into newly created DialerException
                new DialerException(DialerErrorCode.Exception, ex.ToString());

            return Convert(dialerException);
        }

        private FaultException Convert(DialerException dialerException)
        {
            var parametersException = dialerException as ParametersException;
            if (parametersException != null)
            {
                return new FaultException<DialerParametersExceptionDetails>(
                    new DialerParametersExceptionDetails(parametersException), parametersException.Message);
            }

            // The default conversion is to FaultException<DialerExceptionDetail>
            return new FaultException<DialerExceptionDetail>(new DialerExceptionDetail(dialerException), dialerException.Message);
        }
    }
}
