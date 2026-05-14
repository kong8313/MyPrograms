using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.SampleUpload
{
    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public string ProjectId { get; set; }
        public int BatchId { get; set; }
        public ProcessSampleMode ProcessSampleMode { get; set; }
        public SchedulingMode SchedulingMode { get; set; }
    }
}
