using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIDialersRepository : IDialersRepository 
    {
        private IDialersRepository _inner;

        public StubIDialersRepository()
        {
            _inner = null;
        }

        public IDialersRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvDialersEntity GetByIdInt32Delegate(int id);
        public GetByIdInt32Delegate GetByIdInt32;

        BvDialersEntity IDialersRepository.GetById(int id)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(id);
            } else if (_inner != null)
            {
                return ((IDialersRepository)_inner).GetById(id);
            }

            return default(BvDialersEntity);
        }

        public delegate void UpdateBvDialersEntityBooleanDelegate(BvDialersEntity dialerEntity, bool useNotification);
        public UpdateBvDialersEntityBooleanDelegate UpdateBvDialersEntityBoolean;

        void IDialersRepository.Update(BvDialersEntity dialerEntity, bool useNotification)
        {

            if (UpdateBvDialersEntityBoolean != null)
            {
                UpdateBvDialersEntityBoolean(dialerEntity, useNotification);
            } else if (_inner != null)
            {
                ((IDialersRepository)_inner).Update(dialerEntity, useNotification);
            }
        }

        public delegate List<BvDialersEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvDialersEntity> IDialersRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IDialersRepository)_inner).GetAll();
            }

            return default(List<BvDialersEntity>);
        }

        public delegate bool IsAnyDialerConfiguredDelegate();
        public IsAnyDialerConfiguredDelegate IsAnyDialerConfigured;

        bool IDialersRepository.IsAnyDialerConfigured()
        {


            if (IsAnyDialerConfigured != null)
            {
                return IsAnyDialerConfigured();
            } else if (_inner != null)
            {
                return ((IDialersRepository)_inner).IsAnyDialerConfigured();
            }

            return default(bool);
        }

        public delegate int? GetNextAvailableDialerInt32DialTypeInt32Delegate(int surveyId, DialType dialType, int callCenterId);
        public GetNextAvailableDialerInt32DialTypeInt32Delegate GetNextAvailableDialerInt32DialTypeInt32;

        int? IDialersRepository.GetNextAvailableDialer(int surveyId, DialType dialType, int callCenterId)
        {


            if (GetNextAvailableDialerInt32DialTypeInt32 != null)
            {
                return GetNextAvailableDialerInt32DialTypeInt32(surveyId, dialType, callCenterId);
            } else if (_inner != null)
            {
                return ((IDialersRepository)_inner).GetNextAvailableDialer(surveyId, dialType, callCenterId);
            }

            return default(int?);
        }

        public delegate BvDialersEntity AddDialerBvDialersEntityDelegate(BvDialersEntity dialer);
        public AddDialerBvDialersEntityDelegate AddDialerBvDialersEntity;

        BvDialersEntity IDialersRepository.AddDialer(BvDialersEntity dialer)
        {


            if (AddDialerBvDialersEntity != null)
            {
                return AddDialerBvDialersEntity(dialer);
            } else if (_inner != null)
            {
                return ((IDialersRepository)_inner).AddDialer(dialer);
            }

            return default(BvDialersEntity);
        }

        public delegate void DeleteInt32Delegate(int dialerId);
        public DeleteInt32Delegate DeleteInt32;

        void IDialersRepository.Delete(int dialerId)
        {

            if (DeleteInt32 != null)
            {
                DeleteInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialersRepository)_inner).Delete(dialerId);
            }
        }

    }
}