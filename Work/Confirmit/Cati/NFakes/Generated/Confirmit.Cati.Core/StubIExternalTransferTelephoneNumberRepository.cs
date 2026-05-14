using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIExternalTransferTelephoneNumberRepository : IExternalTransferTelephoneNumberRepository 
    {
        private IExternalTransferTelephoneNumberRepository _inner;

        public StubIExternalTransferTelephoneNumberRepository()
        {
            _inner = null;
        }

        public IExternalTransferTelephoneNumberRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvSpTransfer_GetExternalListEntity> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<BvSpTransfer_GetExternalListEntity> IExternalTransferTelephoneNumberRepository.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IExternalTransferTelephoneNumberRepository)_inner).GetAll();
            }

            return default(List<BvSpTransfer_GetExternalListEntity>);
        }

        public delegate BvExternalTransferTelephoneNumberEntity TryGetByIdInt32Delegate(int id);
        public TryGetByIdInt32Delegate TryGetByIdInt32;

        BvExternalTransferTelephoneNumberEntity IExternalTransferTelephoneNumberRepository.TryGetById(int id)
        {


            if (TryGetByIdInt32 != null)
            {
                return TryGetByIdInt32(id);
            } else if (_inner != null)
            {
                return ((IExternalTransferTelephoneNumberRepository)_inner).TryGetById(id);
            }

            return default(BvExternalTransferTelephoneNumberEntity);
        }

        public delegate BvExternalTransferTelephoneNumberEntity TryGetByTelephoneNumberStringDelegate(string telNumber);
        public TryGetByTelephoneNumberStringDelegate TryGetByTelephoneNumberString;

        BvExternalTransferTelephoneNumberEntity IExternalTransferTelephoneNumberRepository.TryGetByTelephoneNumber(string telNumber)
        {


            if (TryGetByTelephoneNumberString != null)
            {
                return TryGetByTelephoneNumberString(telNumber);
            } else if (_inner != null)
            {
                return ((IExternalTransferTelephoneNumberRepository)_inner).TryGetByTelephoneNumber(telNumber);
            }

            return default(BvExternalTransferTelephoneNumberEntity);
        }

        public delegate int InsertBvExternalTransferTelephoneNumberEntityDelegate(BvExternalTransferTelephoneNumberEntity number);
        public InsertBvExternalTransferTelephoneNumberEntityDelegate InsertBvExternalTransferTelephoneNumberEntity;

        int IExternalTransferTelephoneNumberRepository.Insert(BvExternalTransferTelephoneNumberEntity number)
        {


            if (InsertBvExternalTransferTelephoneNumberEntity != null)
            {
                return InsertBvExternalTransferTelephoneNumberEntity(number);
            } else if (_inner != null)
            {
                return ((IExternalTransferTelephoneNumberRepository)_inner).Insert(number);
            }

            return default(int);
        }

        public delegate void UpdateBvExternalTransferTelephoneNumberEntityDelegate(BvExternalTransferTelephoneNumberEntity number);
        public UpdateBvExternalTransferTelephoneNumberEntityDelegate UpdateBvExternalTransferTelephoneNumberEntity;

        void IExternalTransferTelephoneNumberRepository.Update(BvExternalTransferTelephoneNumberEntity number)
        {

            if (UpdateBvExternalTransferTelephoneNumberEntity != null)
            {
                UpdateBvExternalTransferTelephoneNumberEntity(number);
            } else if (_inner != null)
            {
                ((IExternalTransferTelephoneNumberRepository)_inner).Update(number);
            }
        }

        public delegate void DeleteInt32Delegate(int id);
        public DeleteInt32Delegate DeleteInt32;

        void IExternalTransferTelephoneNumberRepository.Delete(int id)
        {

            if (DeleteInt32 != null)
            {
                DeleteInt32(id);
            } else if (_inner != null)
            {
                ((IExternalTransferTelephoneNumberRepository)_inner).Delete(id);
            }
        }

    }
}