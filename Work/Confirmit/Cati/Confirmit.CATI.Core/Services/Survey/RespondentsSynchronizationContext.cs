using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;

namespace Confirmit.CATI.Core.Services.Survey
{
    public class RespondentsSynchronizationContext
    {
        public BvSurveyEntity Survey { get; set; }
        public int ExtendedStatus { get; set; }
        public int RecordsCount { get; set; }
        public int AddedRecords { get; set; }
        public int DeletedRecords { get; set; }
        public int PartitionSize { get; set; }
        public IEventDetails EventDetails { get; set; }
        public TimezoneResolver TimeZoneReolver { get; set; }
        public int OperationId { get; set; }
    }
}