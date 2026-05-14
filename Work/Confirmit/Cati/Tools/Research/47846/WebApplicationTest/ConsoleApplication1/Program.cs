using System;
using System.Web;

using System.ServiceModel;
using System.ServiceModel.Description;


using WcfServiceLibrary;

namespace ConsoleApplication1
{
    class Program
    {

        public static ServiceHost CreateServiceHost
                (string service, Uri[] baseAddresses)
        {
            //return base.CreateServiceHost(service, baseAddresses);

            string httpsAddress = "https://" + baseAddresses[0].Host + baseAddresses[0].PathAndQuery;
            // The service parameter is ignored here because we know our service.
            ServiceHost host = new ServiceHost(typeof(WcfTestService),
                //new Uri("http://localhost:5555/1.svc"));
                //new Uri(httpsAddress));
                new[] { baseAddresses[0], new Uri(httpsAddress) });

            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            // define the service endpoints
            host.AddServiceEndpoint(typeof(IWcfTestService),
                binding, "urn:test:test",
                new Uri(httpsAddress));


            string mexAddress = baseAddresses[0].Host + baseAddresses[0].PathAndQuery;

            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;
            behavior.HttpGetUrl = new Uri("http://" + mexAddress + "/get");

            host.Description.Behaviors.Add(behavior);
            host.AddServiceEndpoint(typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexHttpBinding(), "urn:mex", new Uri("http://" + mexAddress + "/mex"));


            return host;

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Press Enter to start");
            Console.ReadLine();
            
            ServiceHost host = CreateServiceHost("1", 
                
                new[] {new Uri("http://localhost:4557/WcfTestService/Test/Test1")});

            host.Open();

            foreach (ServiceEndpoint se in host.Description.Endpoints)
            {
                Console.WriteLine("Endpoint details:");
                Console.WriteLine("Logical address: \t{0}", se.Address);
                Console.WriteLine("Physical address: \t{0}", se.ListenUri);
                Console.WriteLine("Binding: \t{0}", se.Binding.Name);
                Console.WriteLine("Contract: \t{0}", se.Contract.Name);
                Console.WriteLine();
            }


            Console.WriteLine("Press Enter to stop");
            Console.ReadLine();
            host.Close();
        }
    }
}
