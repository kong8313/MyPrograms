using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface ICallGroupRepository
    {
        BvCallGroupEntity Get(string name);
        BvCallGroupEntity Get(int callGroupId);
        void Insert(BvCallGroupEntity callGroup);
        void Update(BvCallGroupEntity callGroup);
        void Delete(int groupId);
        List<BvCallGroupEntity> GetAllGroups();
    }
}