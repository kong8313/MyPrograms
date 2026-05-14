using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;

namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IPersonDeferredMonitoringRepository
    {
        BvPersonDeferredMonitoringPartEntity InsertEmptyDeferredRecord(int interviewerId,
            int surveySid,
            int interviewId,
            int? callId,
            int callCenterId,
            string respondentName,
            string telephoneNumber);

        BvPersonDeferredMonitoringPartEntity GetByCallId(int callId);

        bool IsEmptyRecord(BvPersonDeferredMonitoringPartEntity record);

        DateTime GetTimeStampByRecordId(int recordId);

        BvPersonDeferredMonitoringPartEntity GetByIdWithCheck(int deferredRecordId, int interviewerId);

        BvPersonDeferredMonitoringPartEntity GetById(int deferredRecordId);

        void Update([NotNull] BvPersonDeferredMonitoringEntity entity);

        void RemoveRecord(int deferredRecordId);
    }
}