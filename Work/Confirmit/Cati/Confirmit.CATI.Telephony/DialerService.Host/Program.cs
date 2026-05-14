using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;

namespace DialerService.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var dialerServiceUri = new Uri("http://localhost/Temporary_Listen_Addresses/TestDialerService");

            var dialerService = new ServiceHost(typeof(Confirmit.CATI.Telephony.DialerService.DialerService), dialerServiceUri); ;

            // Start both services simultaneously to speed up tests execution.
            var asyncResult = dialerService.BeginOpen(null, null);

            if (asyncResult.AsyncWaitHandle.WaitOne(10000))
            {
                Trace.TraceInformation("Service is started");
            }
            else
            {
                Trace.TraceError("Service is not started");
            }

            Thread.Sleep(120000);
        }
    }
}
