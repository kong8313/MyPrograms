using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIBreakTypeRepository : IBreakTypeRepository 
    {
        private IBreakTypeRepository _inner;

        public StubIBreakTypeRepository()
        {
            _inner = null;
        }

        public IBreakTypeRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvBreakTypeEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvBreakTypeEntity> IBreakTypeRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IBreakTypeRepository)_inner).GetAll();
            }

            return default(List<BvBreakTypeEntity>);
        }

        public delegate BvBreakTypeEntity TryGetByIdInt32Delegate(int id);
        public TryGetByIdInt32Delegate TryGetByIdInt32;

        BvBreakTypeEntity IBreakTypeRepository.TryGetById(int id)
        {


            if (TryGetByIdInt32 != null)
            {
                return TryGetByIdInt32(id);
            } else if (_inner != null)
            {
                return ((IBreakTypeRepository)_inner).TryGetById(id);
            }

            return default(BvBreakTypeEntity);
        }

        public delegate void InsertBvBreakTypeEntityDelegate(BvBreakTypeEntity entity);
        public InsertBvBreakTypeEntityDelegate InsertBvBreakTypeEntity;

        void IBreakTypeRepository.Insert(BvBreakTypeEntity entity)
        {

            if (InsertBvBreakTypeEntity != null)
            {
                InsertBvBreakTypeEntity(entity);
            } else if (_inner != null)
            {
                ((IBreakTypeRepository)_inner).Insert(entity);
            }
        }

        public delegate void UpdateBvBreakTypeEntityDelegate(BvBreakTypeEntity entity);
        public UpdateBvBreakTypeEntityDelegate UpdateBvBreakTypeEntity;

        void IBreakTypeRepository.Update(BvBreakTypeEntity entity)
        {

            if (UpdateBvBreakTypeEntity != null)
            {
                UpdateBvBreakTypeEntity(entity);
            } else if (_inner != null)
            {
                ((IBreakTypeRepository)_inner).Update(entity);
            }
        }

        public delegate void DeleteListOfInt32Delegate(List<int> ids);
        public DeleteListOfInt32Delegate DeleteListOfInt32;

        void IBreakTypeRepository.Delete(List<int> ids)
        {

            if (DeleteListOfInt32 != null)
            {
                DeleteListOfInt32(ids);
            } else if (_inner != null)
            {
                ((IBreakTypeRepository)_inner).Delete(ids);
            }
        }

    }
}