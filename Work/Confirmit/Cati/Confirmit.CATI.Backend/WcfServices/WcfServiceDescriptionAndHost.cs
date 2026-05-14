using System.ServiceModel;

namespace Confirmit.CATI.Backend.WcfServices
{
    internal class WcfServiceDescriptionAndHost
    {
        public IWcfServiceDescription Description
        {
            get; set;
        }

        public ServiceHost Host
        {
            get; set;
        }
    }
}
