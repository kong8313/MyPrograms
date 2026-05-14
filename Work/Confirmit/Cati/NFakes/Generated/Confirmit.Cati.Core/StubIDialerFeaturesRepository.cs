using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIDialerFeaturesRepository : IDialerFeaturesRepository 
    {
        private IDialerFeaturesRepository _inner;

        public StubIDialerFeaturesRepository()
        {
            _inner = null;
        }

        public IDialerFeaturesRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvDialerFeaturesEntity> GetAllInt32Delegate(int dialerId);
        public GetAllInt32Delegate GetAllInt32;

        List<BvDialerFeaturesEntity> IDialerFeaturesRepository.GetAll(int dialerId)
        {


            if (GetAllInt32 != null)
            {
                return GetAllInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerFeaturesRepository)_inner).GetAll(dialerId);
            }

            return default(List<BvDialerFeaturesEntity>);
        }

        public delegate void UpdateOrInsertBvDialerFeaturesEntityDelegate(BvDialerFeaturesEntity dialerFeaturesEntity);
        public UpdateOrInsertBvDialerFeaturesEntityDelegate UpdateOrInsertBvDialerFeaturesEntity;

        void IDialerFeaturesRepository.UpdateOrInsert(BvDialerFeaturesEntity dialerFeaturesEntity)
        {

            if (UpdateOrInsertBvDialerFeaturesEntity != null)
            {
                UpdateOrInsertBvDialerFeaturesEntity(dialerFeaturesEntity);
            } else if (_inner != null)
            {
                ((IDialerFeaturesRepository)_inner).UpdateOrInsert(dialerFeaturesEntity);
            }
        }

        public delegate void DeleteInt32StringDelegate(int dialerId, string name);
        public DeleteInt32StringDelegate DeleteInt32String;

        void IDialerFeaturesRepository.Delete(int dialerId, string name)
        {

            if (DeleteInt32String != null)
            {
                DeleteInt32String(dialerId, name);
            } else if (_inner != null)
            {
                ((IDialerFeaturesRepository)_inner).Delete(dialerId, name);
            }
        }

        public delegate void DeleteAllInt32Delegate(int dialerId);
        public DeleteAllInt32Delegate DeleteAllInt32;

        void IDialerFeaturesRepository.DeleteAll(int dialerId)
        {

            if (DeleteAllInt32 != null)
            {
                DeleteAllInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerFeaturesRepository)_inner).DeleteAll(dialerId);
            }
        }

    }
}