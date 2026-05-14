using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.Repositories
{
    public class PersonDeferredMonitoringRepository : IPersonDeferredMonitoringRepository
    {
        private readonly ITimeService _timeService;

        public PersonDeferredMonitoringRepository(ITimeService timeService)
        {
            _timeService = timeService;
        }

        private readonly DateTime EmptyRecordTimeStamp = SqlDateTime.MinValue.Value;

        public BvPersonDeferredMonitoringPartEntity GetByIdWithCheck(int deferredRecordId, int interviewerId)
        {
            return BvPersonDeferredMonitoringPartAdapterEx.GetByIdWithCheck(deferredRecordId, interviewerId);
        }

        public BvPersonDeferredMonitoringPartEntity GetById(int deferredRecordId)
        {
            return BvPersonDeferredMonitoringPartAdapterEx.GetById(deferredRecordId);
        }

        public void Update([NotNull] BvPersonDeferredMonitoringEntity entity)
        {
            BvPersonDeferredMonitoringAdapterEx.Update(entity);
        }

        public void RemoveRecord(int deferredRecordId)
        {
            BvPersonDeferredMonitoringAdapterEx.RemoveRecord(deferredRecordId);
        }

        public BvPersonDeferredMonitoringPartEntity InsertEmptyDeferredRecord(
            int interviewerId,
            int surveySid,
            int interviewId,
            int? callId,
            int callCenterId,
            string respondentName,
            string telephoneNumber)
        {
            var record = new BvPersonDeferredMonitoringEntity
            {
                TimeStamp = _timeService.GetUtcNow(),
                ClientTimeUtc = EmptyRecordTimeStamp,
                ServerTimeUtc = EmptyRecordTimeStamp,
                HasAudio = false,
                InterviewID = interviewId,
                IsComplete = false,
                IsRecording = true,
                PersonSID = interviewerId,
                SurveySID = surveySid,
                RequestAudio = false,
                EventsFile = new byte[0],
                CallID = callId,
                CallCenterId = callCenterId,
                RespondentName = respondentName,
                TelephoneNumber = telephoneNumber,
                InterviewDuration = 0,
                RecordCreationTime = _timeService.GetUtcNow(),
                IsOldInterface = true
            };

            return BvPersonDeferredMonitoringAdapterEx.Insert(record);
        }

        public BvPersonDeferredMonitoringPartEntity GetByCallId(int callId)
        {
            return BvPersonDeferredMonitoringPartAdapterEx.GetByCallId(callId);
        }

        public bool IsEmptyRecord(BvPersonDeferredMonitoringPartEntity record)
        {
            return record.ClientTimeUtc == EmptyRecordTimeStamp;
        }

        public DateTime GetTimeStampByRecordId(int recordId)
        {
            return BvPersonDeferredMonitoringPartAdapterEx.GetByCondition(
                "[ID] = @ID",
                new SqlParameter("@ID", recordId)).SingleOrDefault().TimeStamp;
        }
    }
}