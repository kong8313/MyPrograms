using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    public interface ICallOperationsProvider
    {
        List<CallOperation> GetAll();
    }
}
