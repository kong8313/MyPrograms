using Confirmit.Configuration.Bootstrap;
using Confirmit.Identity.Sdk.Clients;

namespace Confirmit.CATI.Core.Services
{
    public class TrustedSubsystemClientSecretProvider : IClientSecretProvider
    {
        public string ClientId => BootstrapConfig.Authentication.TrustedSubsystem.ClientId;

        public string Secret => BootstrapConfig.Authentication.TrustedSubsystem.ClientSecret;
    }
}