using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIIvrSettingsRepository : IIvrSettingsRepository 
    {
        private IIvrSettingsRepository _inner;

        public StubIIvrSettingsRepository()
        {
            _inner = null;
        }

        public IIvrSettingsRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvIvrSettingsEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvIvrSettingsEntity> IIvrSettingsRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IIvrSettingsRepository)_inner).GetAll();
            }

            return default(List<BvIvrSettingsEntity>);
        }

        public delegate BvIvrSettingsEntity TryGetByLanguageIdInt32Delegate(int languageId);
        public TryGetByLanguageIdInt32Delegate TryGetByLanguageIdInt32;

        BvIvrSettingsEntity IIvrSettingsRepository.TryGetByLanguageId(int languageId)
        {


            if (TryGetByLanguageIdInt32 != null)
            {
                return TryGetByLanguageIdInt32(languageId);
            } else if (_inner != null)
            {
                return ((IIvrSettingsRepository)_inner).TryGetByLanguageId(languageId);
            }

            return default(BvIvrSettingsEntity);
        }

        public delegate void InsertBvIvrSettingsEntityDelegate(BvIvrSettingsEntity entity);
        public InsertBvIvrSettingsEntityDelegate InsertBvIvrSettingsEntity;

        void IIvrSettingsRepository.Insert(BvIvrSettingsEntity entity)
        {

            if (InsertBvIvrSettingsEntity != null)
            {
                InsertBvIvrSettingsEntity(entity);
            } else if (_inner != null)
            {
                ((IIvrSettingsRepository)_inner).Insert(entity);
            }
        }

        public delegate void UpdateInt32BvIvrSettingsEntityDelegate(int languageId, BvIvrSettingsEntity entity);
        public UpdateInt32BvIvrSettingsEntityDelegate UpdateInt32BvIvrSettingsEntity;

        void IIvrSettingsRepository.Update(int languageId, BvIvrSettingsEntity entity)
        {

            if (UpdateInt32BvIvrSettingsEntity != null)
            {
                UpdateInt32BvIvrSettingsEntity(languageId, entity);
            } else if (_inner != null)
            {
                ((IIvrSettingsRepository)_inner).Update(languageId, entity);
            }
        }

        public delegate void DeleteListOfInt32Delegate(List<int> languageIds);
        public DeleteListOfInt32Delegate DeleteListOfInt32;

        void IIvrSettingsRepository.Delete(List<int> languageIds)
        {

            if (DeleteListOfInt32 != null)
            {
                DeleteListOfInt32(languageIds);
            } else if (_inner != null)
            {
                ((IIvrSettingsRepository)_inner).Delete(languageIds);
            }
        }

    }
}