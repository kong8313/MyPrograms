using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubISystemSettingRepository : ISystemSettingRepository 
    {
        private ISystemSettingRepository _inner;

        public StubISystemSettingRepository()
        {
            _inner = null;
        }

        public ISystemSettingRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetStringInt32Delegate(string settingSystemName, int companyId);
        public GetStringInt32Delegate GetStringInt32;

        string ISystemSettingRepository.Get(string settingSystemName, int companyId)
        {


            if (GetStringInt32 != null)
            {
                return GetStringInt32(settingSystemName, companyId);
            } else if (_inner != null)
            {
                return ((ISystemSettingRepository)_inner).Get(settingSystemName, companyId);
            }

            return default(string);
        }

        public delegate BvSystemSettingsEntity GetSettingForCurrentCompanyStringDelegate(string systemName);
        public GetSettingForCurrentCompanyStringDelegate GetSettingForCurrentCompanyString;

        BvSystemSettingsEntity ISystemSettingRepository.GetSettingForCurrentCompany(string systemName)
        {


            if (GetSettingForCurrentCompanyString != null)
            {
                return GetSettingForCurrentCompanyString(systemName);
            } else if (_inner != null)
            {
                return ((ISystemSettingRepository)_inner).GetSettingForCurrentCompany(systemName);
            }

            return default(BvSystemSettingsEntity);
        }

        public delegate IEnumerable<BvSystemSettingsEntity> GetAllSettingsForCurrentCompanyDelegate();
        public GetAllSettingsForCurrentCompanyDelegate GetAllSettingsForCurrentCompany;

        IEnumerable<BvSystemSettingsEntity> ISystemSettingRepository.GetAllSettingsForCurrentCompany()
        {


            if (GetAllSettingsForCurrentCompany != null)
            {
                return GetAllSettingsForCurrentCompany();
            } else if (_inner != null)
            {
                return ((ISystemSettingRepository)_inner).GetAllSettingsForCurrentCompany();
            }

            return default(IEnumerable<BvSystemSettingsEntity>);
        }

        public delegate void InsertSettingForCurrentCompanyBvSystemSettingsEntityDelegate(BvSystemSettingsEntity entity);
        public InsertSettingForCurrentCompanyBvSystemSettingsEntityDelegate InsertSettingForCurrentCompanyBvSystemSettingsEntity;

        void ISystemSettingRepository.InsertSettingForCurrentCompany(BvSystemSettingsEntity entity)
        {

            if (InsertSettingForCurrentCompanyBvSystemSettingsEntity != null)
            {
                InsertSettingForCurrentCompanyBvSystemSettingsEntity(entity);
            } else if (_inner != null)
            {
                ((ISystemSettingRepository)_inner).InsertSettingForCurrentCompany(entity);
            }
        }

        public delegate void UpdateSettingForCurrentCompanyBvSystemSettingsEntityDelegate(BvSystemSettingsEntity entity);
        public UpdateSettingForCurrentCompanyBvSystemSettingsEntityDelegate UpdateSettingForCurrentCompanyBvSystemSettingsEntity;

        void ISystemSettingRepository.UpdateSettingForCurrentCompany(BvSystemSettingsEntity entity)
        {

            if (UpdateSettingForCurrentCompanyBvSystemSettingsEntity != null)
            {
                UpdateSettingForCurrentCompanyBvSystemSettingsEntity(entity);
            } else if (_inner != null)
            {
                ((ISystemSettingRepository)_inner).UpdateSettingForCurrentCompany(entity);
            }
        }

        public delegate void DeleteSettingForCurrentCompanyStringDelegate(string systemName);
        public DeleteSettingForCurrentCompanyStringDelegate DeleteSettingForCurrentCompanyString;

        void ISystemSettingRepository.DeleteSettingForCurrentCompany(string systemName)
        {

            if (DeleteSettingForCurrentCompanyString != null)
            {
                DeleteSettingForCurrentCompanyString(systemName);
            } else if (_inner != null)
            {
                ((ISystemSettingRepository)_inner).DeleteSettingForCurrentCompany(systemName);
            }
        }

        public delegate BvSystemSettingsEntity GetSettingForDefaultCompanyStringDelegate(string systemName);
        public GetSettingForDefaultCompanyStringDelegate GetSettingForDefaultCompanyString;

        BvSystemSettingsEntity ISystemSettingRepository.GetSettingForDefaultCompany(string systemName)
        {


            if (GetSettingForDefaultCompanyString != null)
            {
                return GetSettingForDefaultCompanyString(systemName);
            } else if (_inner != null)
            {
                return ((ISystemSettingRepository)_inner).GetSettingForDefaultCompany(systemName);
            }

            return default(BvSystemSettingsEntity);
        }

        public delegate IEnumerable<BvSystemSettingsEntity> GetAllSettingsForDefaultCompanyDelegate();
        public GetAllSettingsForDefaultCompanyDelegate GetAllSettingsForDefaultCompany;

        IEnumerable<BvSystemSettingsEntity> ISystemSettingRepository.GetAllSettingsForDefaultCompany()
        {


            if (GetAllSettingsForDefaultCompany != null)
            {
                return GetAllSettingsForDefaultCompany();
            } else if (_inner != null)
            {
                return ((ISystemSettingRepository)_inner).GetAllSettingsForDefaultCompany();
            }

            return default(IEnumerable<BvSystemSettingsEntity>);
        }

        public delegate void InsertSettingForDefaultCompanyBvSystemSettingsEntityDelegate(BvSystemSettingsEntity entity);
        public InsertSettingForDefaultCompanyBvSystemSettingsEntityDelegate InsertSettingForDefaultCompanyBvSystemSettingsEntity;

        void ISystemSettingRepository.InsertSettingForDefaultCompany(BvSystemSettingsEntity entity)
        {

            if (InsertSettingForDefaultCompanyBvSystemSettingsEntity != null)
            {
                InsertSettingForDefaultCompanyBvSystemSettingsEntity(entity);
            } else if (_inner != null)
            {
                ((ISystemSettingRepository)_inner).InsertSettingForDefaultCompany(entity);
            }
        }

        public delegate void UpdateSettingForDefaultCompanyBvSystemSettingsEntityDelegate(BvSystemSettingsEntity entity);
        public UpdateSettingForDefaultCompanyBvSystemSettingsEntityDelegate UpdateSettingForDefaultCompanyBvSystemSettingsEntity;

        void ISystemSettingRepository.UpdateSettingForDefaultCompany(BvSystemSettingsEntity entity)
        {

            if (UpdateSettingForDefaultCompanyBvSystemSettingsEntity != null)
            {
                UpdateSettingForDefaultCompanyBvSystemSettingsEntity(entity);
            } else if (_inner != null)
            {
                ((ISystemSettingRepository)_inner).UpdateSettingForDefaultCompany(entity);
            }
        }

        public delegate void DeleteSettingForDefaultCompanyStringDelegate(string systemName);
        public DeleteSettingForDefaultCompanyStringDelegate DeleteSettingForDefaultCompanyString;

        void ISystemSettingRepository.DeleteSettingForDefaultCompany(string systemName)
        {

            if (DeleteSettingForDefaultCompanyString != null)
            {
                DeleteSettingForDefaultCompanyString(systemName);
            } else if (_inner != null)
            {
                ((ISystemSettingRepository)_inner).DeleteSettingForDefaultCompany(systemName);
            }
        }

    }
}