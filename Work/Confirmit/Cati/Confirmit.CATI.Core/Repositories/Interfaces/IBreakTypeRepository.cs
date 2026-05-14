using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IBreakTypeRepository
    {
        List<BvBreakTypeEntity> GetAll();

        BvBreakTypeEntity TryGetById(int id);

        void Insert(BvBreakTypeEntity entity);

        void Update(BvBreakTypeEntity entity);

        void Delete(List<int> ids);
    }
}