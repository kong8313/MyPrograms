using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIPersonGroupRepository : IPersonGroupRepository 
    {
        private IPersonGroupRepository _inner;

        public StubIPersonGroupRepository()
        {
            _inner = null;
        }

        public IPersonGroupRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvPersonGroupEntity GetByIdInt32Delegate(int sid);
        public GetByIdInt32Delegate GetByIdInt32;

        BvPersonGroupEntity IPersonGroupRepository.GetById(int sid)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(sid);
            } else if (_inner != null)
            {
                return ((IPersonGroupRepository)_inner).GetById(sid);
            }

            return default(BvPersonGroupEntity);
        }

        public delegate BvPersonGroupEntity TryGetByIdInt32Delegate(int sid);
        public TryGetByIdInt32Delegate TryGetByIdInt32;

        BvPersonGroupEntity IPersonGroupRepository.TryGetById(int sid)
        {


            if (TryGetByIdInt32 != null)
            {
                return TryGetByIdInt32(sid);
            } else if (_inner != null)
            {
                return ((IPersonGroupRepository)_inner).TryGetById(sid);
            }

            return default(BvPersonGroupEntity);
        }

        public delegate BvPersonGroupEntity GetByNameStringDelegate(string name);
        public GetByNameStringDelegate GetByNameString;

        BvPersonGroupEntity IPersonGroupRepository.GetByName(string name)
        {


            if (GetByNameString != null)
            {
                return GetByNameString(name);
            } else if (_inner != null)
            {
                return ((IPersonGroupRepository)_inner).GetByName(name);
            }

            return default(BvPersonGroupEntity);
        }

        public delegate BvPersonGroupEntity TryGetByNameStringDelegate(string name);
        public TryGetByNameStringDelegate TryGetByNameString;

        BvPersonGroupEntity IPersonGroupRepository.TryGetByName(string name)
        {


            if (TryGetByNameString != null)
            {
                return TryGetByNameString(name);
            } else if (_inner != null)
            {
                return ((IPersonGroupRepository)_inner).TryGetByName(name);
            }

            return default(BvPersonGroupEntity);
        }

        public delegate List<BvPersonGroupEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvPersonGroupEntity> IPersonGroupRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IPersonGroupRepository)_inner).GetAll();
            }

            return default(List<BvPersonGroupEntity>);
        }

        public delegate int InsertBvPersonGroupEntityDelegate(BvPersonGroupEntity personGroup);
        public InsertBvPersonGroupEntityDelegate InsertBvPersonGroupEntity;

        int IPersonGroupRepository.Insert(BvPersonGroupEntity personGroup)
        {


            if (InsertBvPersonGroupEntity != null)
            {
                return InsertBvPersonGroupEntity(personGroup);
            } else if (_inner != null)
            {
                return ((IPersonGroupRepository)_inner).Insert(personGroup);
            }

            return default(int);
        }

        public delegate void UpdateBvPersonGroupEntityDelegate(BvPersonGroupEntity personGroup);
        public UpdateBvPersonGroupEntityDelegate UpdateBvPersonGroupEntity;

        void IPersonGroupRepository.Update(BvPersonGroupEntity personGroup)
        {

            if (UpdateBvPersonGroupEntity != null)
            {
                UpdateBvPersonGroupEntity(personGroup);
            } else if (_inner != null)
            {
                ((IPersonGroupRepository)_inner).Update(personGroup);
            }
        }

    }
}