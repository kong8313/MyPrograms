using System.Collections.Generic;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public interface IRequestExecutionLog
    {
        void AddEntry(string entry);
        IEnumerable<string> GetEntries();
    }
}