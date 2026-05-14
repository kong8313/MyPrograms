using System;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubIPersonDeferredMonitoringRepository : IPersonDeferredMonitoringRepository 
    {
        private IPersonDeferredMonitoringRepository _inner;

        public StubIPersonDeferredMonitoringRepository()
        {
            _inner = null;
        }

        public IPersonDeferredMonitoringRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvPersonDeferredMonitoringPartEntity InsertEmptyDeferredRecordInt32Int32Int32NullableOfInt32Int32StringStringDelegate(int interviewerId, int surveySid, int interviewId, int? callId, int callCenterId, string respondentName, string telephoneNumber);
        public InsertEmptyDeferredRecordInt32Int32Int32NullableOfInt32Int32StringStringDelegate InsertEmptyDeferredRecordInt32Int32Int32NullableOfInt32Int32StringString;

        BvPersonDeferredMonitoringPartEntity IPersonDeferredMonitoringRepository.InsertEmptyDeferredRecord(int interviewerId, int surveySid, int interviewId, int? callId, int callCenterId, string respondentName, string telephoneNumber)
        {


            if (InsertEmptyDeferredRecordInt32Int32Int32NullableOfInt32Int32StringString != null)
            {
                return InsertEmptyDeferredRecordInt32Int32Int32NullableOfInt32Int32StringString(interviewerId, surveySid, interviewId, callId, callCenterId, respondentName, telephoneNumber);
            } else if (_inner != null)
            {
                return ((IPersonDeferredMonitoringRepository)_inner).InsertEmptyDeferredRecord(interviewerId, surveySid, interviewId, callId, callCenterId, respondentName, telephoneNumber);
            }

            return default(BvPersonDeferredMonitoringPartEntity);
        }

        public delegate BvPersonDeferredMonitoringPartEntity GetByCallIdInt32Delegate(int callId);
        public GetByCallIdInt32Delegate GetByCallIdInt32;

        BvPersonDeferredMonitoringPartEntity IPersonDeferredMonitoringRepository.GetByCallId(int callId)
        {


            if (GetByCallIdInt32 != null)
            {
                return GetByCallIdInt32(callId);
            } else if (_inner != null)
            {
                return ((IPersonDeferredMonitoringRepository)_inner).GetByCallId(callId);
            }

            return default(BvPersonDeferredMonitoringPartEntity);
        }

        public delegate bool IsEmptyRecordBvPersonDeferredMonitoringPartEntityDelegate(BvPersonDeferredMonitoringPartEntity record);
        public IsEmptyRecordBvPersonDeferredMonitoringPartEntityDelegate IsEmptyRecordBvPersonDeferredMonitoringPartEntity;

        bool IPersonDeferredMonitoringRepository.IsEmptyRecord(BvPersonDeferredMonitoringPartEntity record)
        {


            if (IsEmptyRecordBvPersonDeferredMonitoringPartEntity != null)
            {
                return IsEmptyRecordBvPersonDeferredMonitoringPartEntity(record);
            } else if (_inner != null)
            {
                return ((IPersonDeferredMonitoringRepository)_inner).IsEmptyRecord(record);
            }

            return default(bool);
        }

        public delegate DateTime GetTimeStampByRecordIdInt32Delegate(int recordId);
        public GetTimeStampByRecordIdInt32Delegate GetTimeStampByRecordIdInt32;

        DateTime IPersonDeferredMonitoringRepository.GetTimeStampByRecordId(int recordId)
        {


            if (GetTimeStampByRecordIdInt32 != null)
            {
                return GetTimeStampByRecordIdInt32(recordId);
            } else if (_inner != null)
            {
                return ((IPersonDeferredMonitoringRepository)_inner).GetTimeStampByRecordId(recordId);
            }

            return default(DateTime);
        }

        public delegate BvPersonDeferredMonitoringPartEntity GetByIdWithCheckInt32Int32Delegate(int deferredRecordId, int interviewerId);
        public GetByIdWithCheckInt32Int32Delegate GetByIdWithCheckInt32Int32;

        BvPersonDeferredMonitoringPartEntity IPersonDeferredMonitoringRepository.GetByIdWithCheck(int deferredRecordId, int interviewerId)
        {


            if (GetByIdWithCheckInt32Int32 != null)
            {
                return GetByIdWithCheckInt32Int32(deferredRecordId, interviewerId);
            } else if (_inner != null)
            {
                return ((IPersonDeferredMonitoringRepository)_inner).GetByIdWithCheck(deferredRecordId, interviewerId);
            }

            return default(BvPersonDeferredMonitoringPartEntity);
        }

        public delegate BvPersonDeferredMonitoringPartEntity GetByIdInt32Delegate(int deferredRecordId);
        public GetByIdInt32Delegate GetByIdInt32;

        BvPersonDeferredMonitoringPartEntity IPersonDeferredMonitoringRepository.GetById(int deferredRecordId)
        {


            if (GetByIdInt32 != null)
            {
                return GetByIdInt32(deferredRecordId);
            } else if (_inner != null)
            {
                return ((IPersonDeferredMonitoringRepository)_inner).GetById(deferredRecordId);
            }

            return default(BvPersonDeferredMonitoringPartEntity);
        }

        public delegate void UpdateBvPersonDeferredMonitoringEntityDelegate(BvPersonDeferredMonitoringEntity entity);
        public UpdateBvPersonDeferredMonitoringEntityDelegate UpdateBvPersonDeferredMonitoringEntity;

        void IPersonDeferredMonitoringRepository.Update(BvPersonDeferredMonitoringEntity entity)
        {

            if (UpdateBvPersonDeferredMonitoringEntity != null)
            {
                UpdateBvPersonDeferredMonitoringEntity(entity);
            } else if (_inner != null)
            {
                ((IPersonDeferredMonitoringRepository)_inner).Update(entity);
            }
        }

        public delegate void RemoveRecordInt32Delegate(int deferredRecordId);
        public RemoveRecordInt32Delegate RemoveRecordInt32;

        void IPersonDeferredMonitoringRepository.RemoveRecord(int deferredRecordId)
        {

            if (RemoveRecordInt32 != null)
            {
                RemoveRecordInt32(deferredRecordId);
            } else if (_inner != null)
            {
                ((IPersonDeferredMonitoringRepository)_inner).RemoveRecord(deferredRecordId);
            }
        }

    }
}