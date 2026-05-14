using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IPersonGroupRepository
    {
        [NotNull]
        BvPersonGroupEntity GetById(int sid);

        [CanBeNull]
        BvPersonGroupEntity TryGetById(int sid);

        [NotNull]
        BvPersonGroupEntity GetByName(string name);

        [CanBeNull]
        BvPersonGroupEntity TryGetByName(string name);

        List<BvPersonGroupEntity> GetAll();

        int Insert(BvPersonGroupEntity personGroup);
        void Update(BvPersonGroupEntity personGroup);
    }
}
