using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Services.TimeService;
using System.Data.SqlClient;
using System.Linq;

namespace Confirmit.CATI.Core.Services
{
    /// <summary>
    /// Repesents service class for BvPersonDeferredMonitoringEntity entity.
    /// </summary>
    public class DeferredMonitoringService : IDeferredMonitoringService
    {
        private readonly ITimeService _timeService;

        public DeferredMonitoringService(ITimeService timeService)
        {
            _timeService = timeService;
        }

        /// <summary>
        /// Returns starting file for given deferred record. It is xml string which contains data
        /// needed to start deferred monitoring at client side.
        /// </summary>
        /// <returns>XML string.</returns>
        public string GetStartFile(int recordId)
        {
            return BvSpGetDeferredMonitoringStartFileAdapter.ExecuteEntity(recordId).StartingFile;
        }

        public void AppendToEventsFile(int id, byte[] packet)
        {
            BvPersonDeferredMonitoringAdapterEx.AppendToEventsFile(id, packet);
        }

        public void CompleteRecord(int id, byte[] packet, bool hasAudio, bool requestAudio, bool updateDuration)
        {
            var currentEntity = BvPersonDeferredMonitoringAdapter.GetByCondition(
                "ID = @ID",
                new[] { new SqlParameter("@ID", id) }).SingleOrDefault();

            var duration = updateDuration ? (int)(_timeService.GetUtcNow() - currentEntity.RecordCreationTime).TotalSeconds : currentEntity.InterviewDuration;

            BvPersonDeferredMonitoringAdapterEx.CompleteDeferredMonitoringRecord(id, packet, hasAudio, requestAudio, duration);
        }
    }
}