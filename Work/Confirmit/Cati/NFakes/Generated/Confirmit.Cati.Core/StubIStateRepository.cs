using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIStateRepository : IStateRepository 
    {
        private IStateRepository _inner;

        public StubIStateRepository()
        {
            _inner = null;
        }

        public IStateRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvStateEntity GetByItsAndStateGroupIdInt32Int32Delegate(int its, int stateGroupId);
        public GetByItsAndStateGroupIdInt32Int32Delegate GetByItsAndStateGroupIdInt32Int32;

        BvStateEntity IStateRepository.GetByItsAndStateGroupId(int its, int stateGroupId)
        {


            if (GetByItsAndStateGroupIdInt32Int32 != null)
            {
                return GetByItsAndStateGroupIdInt32Int32(its, stateGroupId);
            } else if (_inner != null)
            {
                return ((IStateRepository)_inner).GetByItsAndStateGroupId(its, stateGroupId);
            }

            return default(BvStateEntity);
        }

    }
}