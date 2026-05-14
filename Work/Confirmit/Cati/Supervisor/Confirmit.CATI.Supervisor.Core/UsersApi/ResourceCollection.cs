using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Confirmit.CATI.Supervisor.Core.UsersApi
{
    public class ResourceCollection<T> where T : class
    {
        public string ItemType { get; set; }

        public int ItemCount { get; set; }

        public IEnumerable<T> Items { get; set; }

        public Dictionary<string, string> Links { get; set; }
    }
}
