using System;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IServiceDiscoveryClientProxy
    {
        Uri GetService(string serviceId);
    }
}
