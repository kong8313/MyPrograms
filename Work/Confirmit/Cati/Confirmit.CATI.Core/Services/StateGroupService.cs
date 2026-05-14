using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services
{
    public class StateGroupService : IStateGroupService
    {
        public List<BvSpState_ListEntity> GetStates(int sid)
        {
            using ( var rd = BvSpState_ListByGroupAdapter.ExecuteReader(sid) )
            {
                return BvSpState_ListAdapter.ReadList(rd);
            }
        }

        public bool IsSystemState(BvStateEntity state)
        {
            return state.StateID <= 30 || state.StateID >= 1000;
        }
    }
}