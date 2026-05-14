using Confirmit.CATI.Core.Services.ApiClients.Models;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IRespondentsClient
    {
        int AddRespondent(string projectId, RespondentsInfo importDefinition);
    }
}