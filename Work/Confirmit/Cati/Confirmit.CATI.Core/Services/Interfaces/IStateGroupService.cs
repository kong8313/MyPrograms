using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IStateGroupService
    {
        List<BvSpState_ListEntity> GetStates(int sid);
        bool IsSystemState(BvStateEntity state);
    }
}