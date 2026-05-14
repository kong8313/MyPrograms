using System;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public interface ISampleService
    {
        void AddSampleRecord(
            int batchId, int surveySid, ProcessSampleMode processSampleMode, ProcessSampleAsyncResult sampleState);
        
        void ProcessSample(BvSurveyEntity survey, int batchId, ProcessSampleMode processSampleMode, SchedulingMode mode,
            Action<string> taskLog, Action<int, int, int> updateProgress, AsyncOperationResult result, CancellationToken cancellationToken);

        ProcessSampleAsyncResult GetState(int batchId, ProcessSampleMode processSampleMode, out string stateDescription);
    }
}