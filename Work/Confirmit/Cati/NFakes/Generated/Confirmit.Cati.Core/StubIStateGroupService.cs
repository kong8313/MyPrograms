using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIStateGroupService : IStateGroupService 
    {
        private IStateGroupService _inner;

        public StubIStateGroupService()
        {
            _inner = null;
        }

        public IStateGroupService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvSpState_ListEntity> GetStatesInt32Delegate(int sid);
        public GetStatesInt32Delegate GetStatesInt32;

        List<BvSpState_ListEntity> IStateGroupService.GetStates(int sid)
        {


            if (GetStatesInt32 != null)
            {
                return GetStatesInt32(sid);
            } else if (_inner != null)
            {
                return ((IStateGroupService)_inner).GetStates(sid);
            }

            return default(List<BvSpState_ListEntity>);
        }

        public delegate bool IsSystemStateBvStateEntityDelegate(BvStateEntity state);
        public IsSystemStateBvStateEntityDelegate IsSystemStateBvStateEntity;

        bool IStateGroupService.IsSystemState(BvStateEntity state)
        {


            if (IsSystemStateBvStateEntity != null)
            {
                return IsSystemStateBvStateEntity(state);
            } else if (_inner != null)
            {
                return ((IStateGroupService)_inner).IsSystemState(state);
            }

            return default(bool);
        }

    }
}