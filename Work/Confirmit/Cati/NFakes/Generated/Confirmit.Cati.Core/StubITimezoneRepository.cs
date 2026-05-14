using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubITimezoneRepository : ITimezoneRepository 
    {
        private ITimezoneRepository _inner;

        public StubITimezoneRepository()
        {
            _inner = null;
        }

        public ITimezoneRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvTimezoneEntity GetInt32Delegate(int timezoneId);
        public GetInt32Delegate GetInt32;

        BvTimezoneEntity ITimezoneRepository.Get(int timezoneId)
        {


            if (GetInt32 != null)
            {
                return GetInt32(timezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneRepository)_inner).Get(timezoneId);
            }

            return default(BvTimezoneEntity);
        }

        public delegate BvTimezoneEntity GetMasterTimezoneInt32Delegate(int timezoneId);
        public GetMasterTimezoneInt32Delegate GetMasterTimezoneInt32;

        BvTimezoneEntity ITimezoneRepository.GetMasterTimezone(int timezoneId)
        {


            if (GetMasterTimezoneInt32 != null)
            {
                return GetMasterTimezoneInt32(timezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneRepository)_inner).GetMasterTimezone(timezoneId);
            }

            return default(BvTimezoneEntity);
        }

        public delegate List<BvTimezoneEntity> GetActiveListDelegate();
        public GetActiveListDelegate GetActiveList;

        List<BvTimezoneEntity> ITimezoneRepository.GetActiveList()
        {


            if (GetActiveList != null)
            {
                return GetActiveList();
            } else if (_inner != null)
            {
                return ((ITimezoneRepository)_inner).GetActiveList();
            }

            return default(List<BvTimezoneEntity>);
        }

        public delegate List<BvTimezoneEntity> GetMasterListDelegate();
        public GetMasterListDelegate GetMasterList;

        List<BvTimezoneEntity> ITimezoneRepository.GetMasterList()
        {


            if (GetMasterList != null)
            {
                return GetMasterList();
            } else if (_inner != null)
            {
                return ((ITimezoneRepository)_inner).GetMasterList();
            }

            return default(List<BvTimezoneEntity>);
        }

        public delegate List<BvTimezoneEntity> GetMasterListFromDefaultDatabaseDelegate();
        public GetMasterListFromDefaultDatabaseDelegate GetMasterListFromDefaultDatabase;

        List<BvTimezoneEntity> ITimezoneRepository.GetMasterListFromDefaultDatabase()
        {


            if (GetMasterListFromDefaultDatabase != null)
            {
                return GetMasterListFromDefaultDatabase();
            } else if (_inner != null)
            {
                return ((ITimezoneRepository)_inner).GetMasterListFromDefaultDatabase();
            }

            return default(List<BvTimezoneEntity>);
        }

        public delegate void InsertMasterEntityBvTimezoneEntityDelegate(BvTimezoneEntity entity);
        public InsertMasterEntityBvTimezoneEntityDelegate InsertMasterEntityBvTimezoneEntity;

        void ITimezoneRepository.InsertMasterEntity(BvTimezoneEntity entity)
        {

            if (InsertMasterEntityBvTimezoneEntity != null)
            {
                InsertMasterEntityBvTimezoneEntity(entity);
            } else if (_inner != null)
            {
                ((ITimezoneRepository)_inner).InsertMasterEntity(entity);
            }
        }

        public delegate void UpdateMasterEntityBvTimezoneEntityBooleanDelegate(BvTimezoneEntity entity, bool isActiveTimezone);
        public UpdateMasterEntityBvTimezoneEntityBooleanDelegate UpdateMasterEntityBvTimezoneEntityBoolean;

        void ITimezoneRepository.UpdateMasterEntity(BvTimezoneEntity entity, bool isActiveTimezone)
        {

            if (UpdateMasterEntityBvTimezoneEntityBoolean != null)
            {
                UpdateMasterEntityBvTimezoneEntityBoolean(entity, isActiveTimezone);
            } else if (_inner != null)
            {
                ((ITimezoneRepository)_inner).UpdateMasterEntity(entity, isActiveTimezone);
            }
        }

        public delegate List<BvTimezoneEntity> GetCustomTimezonesInt32Delegate(int parentTimezoneId);
        public GetCustomTimezonesInt32Delegate GetCustomTimezonesInt32;

        List<BvTimezoneEntity> ITimezoneRepository.GetCustomTimezones(int parentTimezoneId)
        {


            if (GetCustomTimezonesInt32 != null)
            {
                return GetCustomTimezonesInt32(parentTimezoneId);
            } else if (_inner != null)
            {
                return ((ITimezoneRepository)_inner).GetCustomTimezones(parentTimezoneId);
            }

            return default(List<BvTimezoneEntity>);
        }

        public delegate void InsertCustomTimezoneBvTimezoneEntityDelegate(BvTimezoneEntity customTimezone);
        public InsertCustomTimezoneBvTimezoneEntityDelegate InsertCustomTimezoneBvTimezoneEntity;

        void ITimezoneRepository.InsertCustomTimezone(BvTimezoneEntity customTimezone)
        {

            if (InsertCustomTimezoneBvTimezoneEntity != null)
            {
                InsertCustomTimezoneBvTimezoneEntity(customTimezone);
            } else if (_inner != null)
            {
                ((ITimezoneRepository)_inner).InsertCustomTimezone(customTimezone);
            }
        }

        public delegate void UpdateCustomTimezoneBvTimezoneEntityDelegate(BvTimezoneEntity customTimezone);
        public UpdateCustomTimezoneBvTimezoneEntityDelegate UpdateCustomTimezoneBvTimezoneEntity;

        void ITimezoneRepository.UpdateCustomTimezone(BvTimezoneEntity customTimezone)
        {

            if (UpdateCustomTimezoneBvTimezoneEntity != null)
            {
                UpdateCustomTimezoneBvTimezoneEntity(customTimezone);
            } else if (_inner != null)
            {
                ((ITimezoneRepository)_inner).UpdateCustomTimezone(customTimezone);
            }
        }

    }
}