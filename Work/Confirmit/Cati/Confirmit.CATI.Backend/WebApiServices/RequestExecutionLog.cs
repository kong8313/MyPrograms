using System.Collections.Generic;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class RequestExecutionLog : IRequestExecutionLog
    {
        private readonly List<string> _entries = new List<string>(); 

        public void AddEntry(string entry)
        {
            _entries.Add(entry);
        }

        public IEnumerable<string> GetEntries()
        {
            return _entries;
        }
    }
}