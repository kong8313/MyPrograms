using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIScheduleErrorRepository : IScheduleErrorRepository 
    {
        private IScheduleErrorRepository _inner;

        public StubIScheduleErrorRepository()
        {
            _inner = null;
        }

        public IScheduleErrorRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InsertBvScheduleErrorEntityDelegate(BvScheduleErrorEntity entity);
        public InsertBvScheduleErrorEntityDelegate InsertBvScheduleErrorEntity;

        void IScheduleErrorRepository.Insert(BvScheduleErrorEntity entity)
        {

            if (InsertBvScheduleErrorEntity != null)
            {
                InsertBvScheduleErrorEntity(entity);
            } else if (_inner != null)
            {
                ((IScheduleErrorRepository)_inner).Insert(entity);
            }
        }

        public delegate int GetErrorsCountByScheduleIDInt32Delegate(int scheduleId);
        public GetErrorsCountByScheduleIDInt32Delegate GetErrorsCountByScheduleIDInt32;

        int IScheduleErrorRepository.GetErrorsCountByScheduleID(int scheduleId)
        {


            if (GetErrorsCountByScheduleIDInt32 != null)
            {
                return GetErrorsCountByScheduleIDInt32(scheduleId);
            } else if (_inner != null)
            {
                return ((IScheduleErrorRepository)_inner).GetErrorsCountByScheduleID(scheduleId);
            }

            return default(int);
        }

        public delegate BvScheduleErrorEntity GetByRowNumberInt32Int32Delegate(int rowNumber, int scheduleId);
        public GetByRowNumberInt32Int32Delegate GetByRowNumberInt32Int32;

        BvScheduleErrorEntity IScheduleErrorRepository.GetByRowNumber(int rowNumber, int scheduleId)
        {


            if (GetByRowNumberInt32Int32 != null)
            {
                return GetByRowNumberInt32Int32(rowNumber, scheduleId);
            } else if (_inner != null)
            {
                return ((IScheduleErrorRepository)_inner).GetByRowNumber(rowNumber, scheduleId);
            }

            return default(BvScheduleErrorEntity);
        }

        public delegate List<BvScheduleErrorEntity> GetByScheduleIdInt32Delegate(int scheduleId);
        public GetByScheduleIdInt32Delegate GetByScheduleIdInt32;

        List<BvScheduleErrorEntity> IScheduleErrorRepository.GetByScheduleId(int scheduleId)
        {


            if (GetByScheduleIdInt32 != null)
            {
                return GetByScheduleIdInt32(scheduleId);
            } else if (_inner != null)
            {
                return ((IScheduleErrorRepository)_inner).GetByScheduleId(scheduleId);
            }

            return default(List<BvScheduleErrorEntity>);
        }

        public delegate List<BvScheduleErrorEntity> GetNotSentErrorsDelegate();
        public GetNotSentErrorsDelegate GetNotSentErrors;

        List<BvScheduleErrorEntity> IScheduleErrorRepository.GetNotSentErrors()
        {


            if (GetNotSentErrors != null)
            {
                return GetNotSentErrors();
            } else if (_inner != null)
            {
                return ((IScheduleErrorRepository)_inner).GetNotSentErrors();
            }

            return default(List<BvScheduleErrorEntity>);
        }

        public delegate void SetNotificationSentIEnumerableOfInt32Delegate(IEnumerable<int> ids);
        public SetNotificationSentIEnumerableOfInt32Delegate SetNotificationSentIEnumerableOfInt32;

        void IScheduleErrorRepository.SetNotificationSent(IEnumerable<int> ids)
        {

            if (SetNotificationSentIEnumerableOfInt32 != null)
            {
                SetNotificationSentIEnumerableOfInt32(ids);
            } else if (_inner != null)
            {
                ((IScheduleErrorRepository)_inner).SetNotificationSent(ids);
            }
        }

        public delegate void DeleteOldErrorsBvScheduleErrorEntityInt32Delegate(BvScheduleErrorEntity lastErrorToDelete, int scheduleId);
        public DeleteOldErrorsBvScheduleErrorEntityInt32Delegate DeleteOldErrorsBvScheduleErrorEntityInt32;

        void IScheduleErrorRepository.DeleteOldErrors(BvScheduleErrorEntity lastErrorToDelete, int scheduleId)
        {

            if (DeleteOldErrorsBvScheduleErrorEntityInt32 != null)
            {
                DeleteOldErrorsBvScheduleErrorEntityInt32(lastErrorToDelete, scheduleId);
            } else if (_inner != null)
            {
                ((IScheduleErrorRepository)_inner).DeleteOldErrors(lastErrorToDelete, scheduleId);
            }
        }

    }
}