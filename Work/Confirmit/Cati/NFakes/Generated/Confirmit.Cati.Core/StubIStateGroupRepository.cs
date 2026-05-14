using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIStateGroupRepository : IStateGroupRepository 
    {
        private IStateGroupRepository _inner;

        public StubIStateGroupRepository()
        {
            _inner = null;
        }

        public IStateGroupRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvStateGroupEntity GetDefaultDelegate();
        public GetDefaultDelegate GetDefault;

        BvStateGroupEntity IStateGroupRepository.GetDefault()
        {


            if (GetDefault != null)
            {
                return GetDefault();
            } else if (_inner != null)
            {
                return ((IStateGroupRepository)_inner).GetDefault();
            }

            return default(BvStateGroupEntity);
        }

    }
}