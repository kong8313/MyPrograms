using System;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIDeferredMonitoringRepository : IDeferredMonitoringRepository 
    {
        private IDeferredMonitoringRepository _inner;

        public StubIDeferredMonitoringRepository()
        {
            _inner = null;
        }

        public IDeferredMonitoringRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<BvSpGetDeferredMonitoringListPageEntity> GetPagesStringPagingArgsInt32Int32OutDelegate(string personLogin, PagingArgs pagingArgs, int timezoneId, out int totalCount);
        public GetPagesStringPagingArgsInt32Int32OutDelegate GetPagesStringPagingArgsInt32Int32Out;

        List<BvSpGetDeferredMonitoringListPageEntity> IDeferredMonitoringRepository.GetPages(string personLogin, PagingArgs pagingArgs, int timezoneId, out int totalCount)
        {
            totalCount = default(int);


            if (GetPagesStringPagingArgsInt32Int32Out != null)
            {
                return GetPagesStringPagingArgsInt32Int32Out(personLogin, pagingArgs, timezoneId, out totalCount);
            } else if (_inner != null)
            {
                return ((IDeferredMonitoringRepository)_inner).GetPages(personLogin, pagingArgs, timezoneId, out totalCount);
            }

            return default(List<BvSpGetDeferredMonitoringListPageEntity>);
        }

        public delegate BvPersonDeferredMonitoringEntity TryGetByIdInt64Delegate(long deferredRecordId);
        public TryGetByIdInt64Delegate TryGetByIdInt64;

        BvPersonDeferredMonitoringEntity IDeferredMonitoringRepository.TryGetById(long deferredRecordId)
        {


            if (TryGetByIdInt64 != null)
            {
                return TryGetByIdInt64(deferredRecordId);
            } else if (_inner != null)
            {
                return ((IDeferredMonitoringRepository)_inner).TryGetById(deferredRecordId);
            }

            return default(BvPersonDeferredMonitoringEntity);
        }

        public delegate List<BvPersonDeferredMonitoringEntity> TryGetByInterviewIdInt64Int64Delegate(long surveySid, long interviewId);
        public TryGetByInterviewIdInt64Int64Delegate TryGetByInterviewIdInt64Int64;

        List<BvPersonDeferredMonitoringEntity> IDeferredMonitoringRepository.TryGetByInterviewId(long surveySid, long interviewId)
        {


            if (TryGetByInterviewIdInt64Int64 != null)
            {
                return TryGetByInterviewIdInt64Int64(surveySid, interviewId);
            } else if (_inner != null)
            {
                return ((IDeferredMonitoringRepository)_inner).TryGetByInterviewId(surveySid, interviewId);
            }

            return default(List<BvPersonDeferredMonitoringEntity>);
        }

        public delegate List<BvPersonDeferredMonitoringEntity> GetAllSavedRecordsDelegate();
        public GetAllSavedRecordsDelegate GetAllSavedRecords;

        List<BvPersonDeferredMonitoringEntity> IDeferredMonitoringRepository.GetAllSavedRecords()
        {


            if (GetAllSavedRecords != null)
            {
                return GetAllSavedRecords();
            } else if (_inner != null)
            {
                return ((IDeferredMonitoringRepository)_inner).GetAllSavedRecords();
            }

            return default(List<BvPersonDeferredMonitoringEntity>);
        }

        public delegate void UpdateRecordBvPersonDeferredMonitoringEntityDelegate(BvPersonDeferredMonitoringEntity entity);
        public UpdateRecordBvPersonDeferredMonitoringEntityDelegate UpdateRecordBvPersonDeferredMonitoringEntity;

        void IDeferredMonitoringRepository.UpdateRecord(BvPersonDeferredMonitoringEntity entity)
        {

            if (UpdateRecordBvPersonDeferredMonitoringEntity != null)
            {
                UpdateRecordBvPersonDeferredMonitoringEntity(entity);
            } else if (_inner != null)
            {
                ((IDeferredMonitoringRepository)_inner).UpdateRecord(entity);
            }
        }

    }
}