using System;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIPersonSessionHistoryRepository : IPersonSessionHistoryRepository 
    {
        private IPersonSessionHistoryRepository _inner;

        public StubIPersonSessionHistoryRepository()
        {
            _inner = null;
        }

        public IPersonSessionHistoryRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int InsertStartSessionEventIConnectionProviderInt32Int32Delegate(IConnectionProvider connectionProvider, int callCenterId, int personId);
        public InsertStartSessionEventIConnectionProviderInt32Int32Delegate InsertStartSessionEventIConnectionProviderInt32Int32;

        int IPersonSessionHistoryRepository.InsertStartSessionEvent(IConnectionProvider connectionProvider, int callCenterId, int personId)
        {


            if (InsertStartSessionEventIConnectionProviderInt32Int32 != null)
            {
                return InsertStartSessionEventIConnectionProviderInt32Int32(connectionProvider, callCenterId, personId);
            } else if (_inner != null)
            {
                return ((IPersonSessionHistoryRepository)_inner).InsertStartSessionEvent(connectionProvider, callCenterId, personId);
            }

            return default(int);
        }

        public delegate void InsertStopSessionEventIConnectionProviderInt32Delegate(IConnectionProvider connectionProvider, int sessionId);
        public InsertStopSessionEventIConnectionProviderInt32Delegate InsertStopSessionEventIConnectionProviderInt32;

        void IPersonSessionHistoryRepository.InsertStopSessionEvent(IConnectionProvider connectionProvider, int sessionId)
        {

            if (InsertStopSessionEventIConnectionProviderInt32 != null)
            {
                InsertStopSessionEventIConnectionProviderInt32(connectionProvider, sessionId);
            } else if (_inner != null)
            {
                ((IPersonSessionHistoryRepository)_inner).InsertStopSessionEvent(connectionProvider, sessionId);
            }
        }

        public delegate IEnumerable<PersonSessionHistoryEntity> GetSessionEventsNullableOfInt32Int32NullableOfDateTimeNullableOfDateTimeDelegate(int? callCenterId, int companyId, DateTime? starTime, DateTime? endTime);
        public GetSessionEventsNullableOfInt32Int32NullableOfDateTimeNullableOfDateTimeDelegate GetSessionEventsNullableOfInt32Int32NullableOfDateTimeNullableOfDateTime;

        IEnumerable<PersonSessionHistoryEntity> IPersonSessionHistoryRepository.GetSessionEvents(int? callCenterId, int companyId, DateTime? starTime, DateTime? endTime)
        {


            if (GetSessionEventsNullableOfInt32Int32NullableOfDateTimeNullableOfDateTime != null)
            {
                return GetSessionEventsNullableOfInt32Int32NullableOfDateTimeNullableOfDateTime(callCenterId, companyId, starTime, endTime);
            } else if (_inner != null)
            {
                return ((IPersonSessionHistoryRepository)_inner).GetSessionEvents(callCenterId, companyId, starTime, endTime);
            }

            return default(IEnumerable<PersonSessionHistoryEntity>);
        }

    }
}