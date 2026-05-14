using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.BlackList
{
    public interface IBlackListService
    {
        void AddNumber(BvTelephoneBlacklistEntity entity);

        void UpdateNumber(string oldNumber, BvTelephoneBlacklistEntity entity);

        void ImportNumbers(IEnumerable<string> numbers);
    }
}
