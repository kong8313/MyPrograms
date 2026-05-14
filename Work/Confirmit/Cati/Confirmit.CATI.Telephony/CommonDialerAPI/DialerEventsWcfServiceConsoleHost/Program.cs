using System;
using System.ServiceModel;
using Confirmit.CATI.Backend.WcfServices.External.DialerEventsHandlerService;
using Confirmit.CATI.Telephony.CommonDialerAPI.DialerEventsTestWcfService;

namespace DialerEventsWcfServiceConsoleHost
{
    class Program
    {
        public static readonly string baseAddress = "http://localhost";

        static void Main(string[] args)
        {
            using (var serviceHost = new ServiceHost(typeof(DialerEventsHandlerTestService)))//, new Uri(baseAddress)))
            {
                try
                {
                    serviceHost.Open();
                    Console.WriteLine("DialerEventsHandlerTestService was successfully hosted. Press [enter] to exit...");

                    var factory = new ChannelFactory<IDialerEventsHandlerService>("DialerEventsHandlerTestServiceEndpoint");
                    IDialerEventsHandlerService eventHandler = factory.CreateChannel();
                    eventHandler.NotifyUserState("dummy1", "dummy2", "dummy3", "dummy4", "dummy5", "dummy6");

                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred while hosting DialerEventsHandlerTestService. Press [enter] to exit...");
                    Console.WriteLine(ex.ToString());
                    Console.ReadLine();
                }
            }
        }
    }
}
