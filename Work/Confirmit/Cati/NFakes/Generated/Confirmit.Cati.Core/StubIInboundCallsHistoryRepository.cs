using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIInboundCallsHistoryRepository : IInboundCallsHistoryRepository 
    {
        private IInboundCallsHistoryRepository _inner;

        public StubIInboundCallsHistoryRepository()
        {
            _inner = null;
        }

        public IInboundCallsHistoryRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InsertBvInboundCallsHistoryEntityDelegate(BvInboundCallsHistoryEntity entity);
        public InsertBvInboundCallsHistoryEntityDelegate InsertBvInboundCallsHistoryEntity;

        void IInboundCallsHistoryRepository.Insert(BvInboundCallsHistoryEntity entity)
        {

            if (InsertBvInboundCallsHistoryEntity != null)
            {
                InsertBvInboundCallsHistoryEntity(entity);
            } else if (_inner != null)
            {
                ((IInboundCallsHistoryRepository)_inner).Insert(entity);
            }
        }

        public delegate BvInboundCallsHistoryEntity GetByIdInt32Delegate(int id);
        public GetByIdInt32Delegate GetByIdInt32;

        BvInboundCallsHistoryEntity IInboundCallsHistoryRepository.GetById(int id)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(id);
            } else if (_inner != null)
            {
                return ((IInboundCallsHistoryRepository)_inner).GetById(id);
            }

            return default(BvInboundCallsHistoryEntity);
        }

    }
}