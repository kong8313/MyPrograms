using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubICallGroupRepository : ICallGroupRepository 
    {
        private ICallGroupRepository _inner;

        public StubICallGroupRepository()
        {
            _inner = null;
        }

        public ICallGroupRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvCallGroupEntity GetStringDelegate(string name);
        public GetStringDelegate GetString;

        BvCallGroupEntity ICallGroupRepository.Get(string name)
        {


            if (GetString != null)
            {
                return GetString(name);
            } else if (_inner != null)
            {
                return ((ICallGroupRepository)_inner).Get(name);
            }

            return default(BvCallGroupEntity);
        }

        public delegate BvCallGroupEntity GetInt32Delegate(int callGroupId);
        public GetInt32Delegate GetInt32;

        BvCallGroupEntity ICallGroupRepository.Get(int callGroupId)
        {


            if (GetInt32 != null)
            {
                return GetInt32(callGroupId);
            } else if (_inner != null)
            {
                return ((ICallGroupRepository)_inner).Get(callGroupId);
            }

            return default(BvCallGroupEntity);
        }

        public delegate void InsertBvCallGroupEntityDelegate(BvCallGroupEntity callGroup);
        public InsertBvCallGroupEntityDelegate InsertBvCallGroupEntity;

        void ICallGroupRepository.Insert(BvCallGroupEntity callGroup)
        {

            if (InsertBvCallGroupEntity != null)
            {
                InsertBvCallGroupEntity(callGroup);
            } else if (_inner != null)
            {
                ((ICallGroupRepository)_inner).Insert(callGroup);
            }
        }

        public delegate void UpdateBvCallGroupEntityDelegate(BvCallGroupEntity callGroup);
        public UpdateBvCallGroupEntityDelegate UpdateBvCallGroupEntity;

        void ICallGroupRepository.Update(BvCallGroupEntity callGroup)
        {

            if (UpdateBvCallGroupEntity != null)
            {
                UpdateBvCallGroupEntity(callGroup);
            } else if (_inner != null)
            {
                ((ICallGroupRepository)_inner).Update(callGroup);
            }
        }

        public delegate void DeleteInt32Delegate(int groupId);
        public DeleteInt32Delegate DeleteInt32;

        void ICallGroupRepository.Delete(int groupId)
        {

            if (DeleteInt32 != null)
            {
                DeleteInt32(groupId);
            } else if (_inner != null)
            {
                ((ICallGroupRepository)_inner).Delete(groupId);
            }
        }

        public delegate List<BvCallGroupEntity> GetAllGroupsDelegate();
        public GetAllGroupsDelegate GetAllGroups;

        List<BvCallGroupEntity> ICallGroupRepository.GetAllGroups()
        {


            if (GetAllGroups != null)
            {
                return GetAllGroups();
            } else if (_inner != null)
            {
                return ((ICallGroupRepository)_inner).GetAllGroups();
            }

            return default(List<BvCallGroupEntity>);
        }

    }
}