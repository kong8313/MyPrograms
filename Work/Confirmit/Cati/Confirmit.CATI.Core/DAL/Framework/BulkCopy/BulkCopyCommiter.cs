using System.Linq;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy
{
    public class BulkCopyCommiter : IBulkCopyCommiter
    {
        private readonly object _lock;
        private readonly IBulkCopy _bulkCopy;
        private readonly IBulkCopyEntityAccumulator<IInterviewerActivityEventBase> _interviewersAccumulator;
        private readonly IBulkCopyEntitySerializer<IInterviewerActivityEventBase> _interviewersSerializer;
        private readonly IConnectionStrings _connectionStrings;

        public BulkCopyCommiter(
            IBulkCopy bulkCopy,
            IBulkCopyEntityAccumulator<IInterviewerActivityEventBase> interviewersAccumulator,
            IBulkCopyEntitySerializer<IInterviewerActivityEventBase> interviewersSerializer,
            IConnectionStrings connectionStrings)
        {
            _lock = new object();

            _bulkCopy = bulkCopy;
            _interviewersAccumulator = interviewersAccumulator;
            _interviewersSerializer = interviewersSerializer;
            _connectionStrings = connectionStrings;
        }

        public void Commit()
        {
            var evt = new BulkCopyInterviewerActivityEventsEvent();

            lock (_lock)
            {
                var activityEvents = _interviewersAccumulator.GetAccumulatedEntitiesAndCleanAccumulator();

                // ReSharper disable PossibleMultipleEnumeration
                if (!activityEvents.Any())
                    return;

                var table4Bulk = _interviewersSerializer.Serialize(activityEvents);
                // ReSharper restore PossibleMultipleEnumeration

                evt.Details.AddTiming("Serialize Events");

                _bulkCopy.Copy(
                    _connectionStrings.ConfirmlogConnectionString,
                    table4Bulk);

                evt.Details.AddTiming("Commit Events");

                evt.Details.EventsCount = table4Bulk.Rows.Count;

            }

            evt.Save();
        }
    }
}