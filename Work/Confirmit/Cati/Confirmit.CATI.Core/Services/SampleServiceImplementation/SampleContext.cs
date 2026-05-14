using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.SchedulingScriptNotificationServiceImplementation;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public class SampleContext
    {
        public BvSurveyEntity Survey { get; set; }
        public int BatchId { get; set; }
        public ProcessSampleMode ProcessSampleMode { get; set; }
        public SchedulingMode SchedulingMode { get; set; }
        public int[] IgnoredItsByFcd { get; set; }
        public int RecordsCount { get; set; }
        public int AddedRecords { get; set; }
        public int PartitionSize { get; set; }
        public IEventDetails EventDetails { get; set; }
        public TimezoneResolver TimeZoneReolver { get; set; }
        public ISampleDataStorageRepository SampleDataStorageRepository { get; set; }
        public IRespondentBatchObtainer RespondentBatchObtainer { get; set; }
        public List<SchedulingScriptNotificatorExceptionDescription> SchedulingScriptNotificatorExceptions { get; set; }
        public SampleProcessingStateContainer StateContainer { get; set; }

        public SampleContext()
        {
            SchedulingScriptNotificatorExceptions = new List<SchedulingScriptNotificatorExceptionDescription>(); 
        }
    }
}