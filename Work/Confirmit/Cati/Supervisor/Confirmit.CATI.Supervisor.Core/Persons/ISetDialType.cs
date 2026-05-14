using System.Collections.Generic;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
    public interface ISetDialType
    {
        void Set(DialType dialType, IEnumerable<int> personIds);
    }
}