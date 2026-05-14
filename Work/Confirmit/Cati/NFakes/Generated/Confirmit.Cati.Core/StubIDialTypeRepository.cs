using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIDialTypeRepository : IDialTypeRepository 
    {
        private IDialTypeRepository _inner;

        public StubIDialTypeRepository()
        {
            _inner = null;
        }

        public IDialTypeRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvDialTypeEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvDialTypeEntity> IDialTypeRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IDialTypeRepository)_inner).GetAll();
            }

            return default(List<BvDialTypeEntity>);
        }

    }
}