using System.Collections.Generic;
using Confirmit.CATI.Common.Types;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Paging;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ITelephoneBlacklistRepository
    {
        List<BvTelephoneBlacklistEntity> GetAll();

        List<BvTelephoneBlacklistEntity> GetPage(PagingArgs pageArguments, out int totalCount);

        BvTelephoneBlacklistEntity GetByDisplayPattern(string displayPattern);

        BvTelephoneBlacklistEntity GetById(int id);

        BvTelephoneBlacklistEntity GetByNumber(string telephoneNumber);

        int Insert(BvTelephoneBlacklistEntity entity);

        void Update(BvTelephoneBlacklistEntity entity);

        int DeleteAll();

        void Delete(IEnumerable<int> ids);

        Range<int> Import(List<BvTelephoneBlacklistEntity> entities);
    }
}
