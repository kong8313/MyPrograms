using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.PersonLogin
{
    public interface ILicenseService
    {
         void CheckLicense(AgentType agentType);
    }
}