using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIPersonRepository : IPersonRepository 
    {
        private IPersonRepository _inner;

        public StubIPersonRepository()
        {
            _inner = null;
        }

        public IPersonRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvPersonEntity GetByIdInt32Delegate(int sid);
        public GetByIdInt32Delegate GetByIdInt32;

        BvPersonEntity IPersonRepository.GetById(int sid)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(sid);
            } else if (_inner != null)
            {
                return ((IPersonRepository)_inner).GetById(sid);
            }

            return default(BvPersonEntity);
        }

        public delegate BvPersonEntity TryGetByIdInt32Delegate(int sid);
        public TryGetByIdInt32Delegate TryGetByIdInt32;

        BvPersonEntity IPersonRepository.TryGetById(int sid)
        {


            if (TryGetByIdInt32 != null)
            {
                return TryGetByIdInt32(sid);
            } else if (_inner != null)
            {
                return ((IPersonRepository)_inner).TryGetById(sid);
            }

            return default(BvPersonEntity);
        }

        public delegate BvPersonEntity GetByNameStringDelegate(string name);
        public GetByNameStringDelegate GetByNameString;

        BvPersonEntity IPersonRepository.GetByName(string name)
        {


            if (GetByNameString != null)
            {
                return GetByNameString(name);
            } else if (_inner != null)
            {
                return ((IPersonRepository)_inner).GetByName(name);
            }

            return default(BvPersonEntity);
        }

        public delegate BvPersonEntity TryGetByNameStringDelegate(string name);
        public TryGetByNameStringDelegate TryGetByNameString;

        BvPersonEntity IPersonRepository.TryGetByName(string name)
        {


            if (TryGetByNameString != null)
            {
                return TryGetByNameString(name);
            } else if (_inner != null)
            {
                return ((IPersonRepository)_inner).TryGetByName(name);
            }

            return default(BvPersonEntity);
        }

        public delegate List<BvPersonEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvPersonEntity> IPersonRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IPersonRepository)_inner).GetAll();
            }

            return default(List<BvPersonEntity>);
        }

        public delegate List<BvPersonEntity> GetByTypeAgentTypeDelegate(AgentType type);
        public GetByTypeAgentTypeDelegate GetByTypeAgentType;

        List<BvPersonEntity> IPersonRepository.GetByType(AgentType type)
        {


            if (GetByTypeAgentType != null)
            {
                return GetByTypeAgentType(type);
            } else if (_inner != null)
            {
                return ((IPersonRepository)_inner).GetByType(type);
            }

            return default(List<BvPersonEntity>);
        }

        public delegate int InsertBvPersonEntityDelegate(BvPersonEntity person);
        public InsertBvPersonEntityDelegate InsertBvPersonEntity;

        int IPersonRepository.Insert(BvPersonEntity person)
        {


            if (InsertBvPersonEntity != null)
            {
                return InsertBvPersonEntity(person);
            } else if (_inner != null)
            {
                return ((IPersonRepository)_inner).Insert(person);
            }

            return default(int);
        }

        public delegate void UpdateBvPersonEntityBooleanDelegate(BvPersonEntity person, bool updateCache);
        public UpdateBvPersonEntityBooleanDelegate UpdateBvPersonEntityBoolean;

        void IPersonRepository.Update(BvPersonEntity person, bool updateCache)
        {

            if (UpdateBvPersonEntityBoolean != null)
            {
                UpdateBvPersonEntityBoolean(person, updateCache);
            } else if (_inner != null)
            {
                ((IPersonRepository)_inner).Update(person, updateCache);
            }
        }

        public delegate void DeleteInt32BooleanDelegate(int sid, bool updateCache);
        public DeleteInt32BooleanDelegate DeleteInt32Boolean;

        void IPersonRepository.Delete(int sid, bool updateCache)
        {

            if (DeleteInt32Boolean != null)
            {
                DeleteInt32Boolean(sid, updateCache);
            } else if (_inner != null)
            {
                ((IPersonRepository)_inner).Delete(sid, updateCache);
            }
        }

    }
}