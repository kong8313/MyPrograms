using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public interface ISupervisorInfoProvider
    {
        CatiSupervisorInfo GetInfo();
    }
}
