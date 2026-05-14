using System;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using System.Threading;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation.Fakes
{
    public class StubISampleService : ISampleService 
    {
        private ISampleService _inner;

        public StubISampleService()
        {
            _inner = null;
        }

        public ISampleService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AddSampleRecordInt32Int32ProcessSampleModeProcessSampleAsyncResultDelegate(int batchId, int surveySid, ProcessSampleMode processSampleMode, ProcessSampleAsyncResult sampleState);
        public AddSampleRecordInt32Int32ProcessSampleModeProcessSampleAsyncResultDelegate AddSampleRecordInt32Int32ProcessSampleModeProcessSampleAsyncResult;

        void ISampleService.AddSampleRecord(int batchId, int surveySid, ProcessSampleMode processSampleMode, ProcessSampleAsyncResult sampleState)
        {

            if (AddSampleRecordInt32Int32ProcessSampleModeProcessSampleAsyncResult != null)
            {
                AddSampleRecordInt32Int32ProcessSampleModeProcessSampleAsyncResult(batchId, surveySid, processSampleMode, sampleState);
            } else if (_inner != null)
            {
                ((ISampleService)_inner).AddSampleRecord(batchId, surveySid, processSampleMode, sampleState);
            }
        }

        public delegate void ProcessSampleBvSurveyEntityInt32ProcessSampleModeSchedulingModeActionOfStringActionOfInt32Int32Int32AsyncOperationResultCancellationTokenDelegate(BvSurveyEntity survey, int batchId, ProcessSampleMode processSampleMode, SchedulingMode mode, Action<string> taskLog, Action<int, int, int> updateProgress, AsyncOperationResult result, CancellationToken cancellationToken);
        public ProcessSampleBvSurveyEntityInt32ProcessSampleModeSchedulingModeActionOfStringActionOfInt32Int32Int32AsyncOperationResultCancellationTokenDelegate ProcessSampleBvSurveyEntityInt32ProcessSampleModeSchedulingModeActionOfStringActionOfInt32Int32Int32AsyncOperationResultCancellationToken;

        void ISampleService.ProcessSample(BvSurveyEntity survey, int batchId, ProcessSampleMode processSampleMode, SchedulingMode mode, Action<string> taskLog, Action<int, int, int> updateProgress, AsyncOperationResult result, CancellationToken cancellationToken)
        {

            if (ProcessSampleBvSurveyEntityInt32ProcessSampleModeSchedulingModeActionOfStringActionOfInt32Int32Int32AsyncOperationResultCancellationToken != null)
            {
                ProcessSampleBvSurveyEntityInt32ProcessSampleModeSchedulingModeActionOfStringActionOfInt32Int32Int32AsyncOperationResultCancellationToken(survey, batchId, processSampleMode, mode, taskLog, updateProgress, result, cancellationToken);
            } else if (_inner != null)
            {
                ((ISampleService)_inner).ProcessSample(survey, batchId, processSampleMode, mode, taskLog, updateProgress, result, cancellationToken);
            }
        }

        public delegate ProcessSampleAsyncResult GetStateInt32ProcessSampleModeStringOutDelegate(int batchId, ProcessSampleMode processSampleMode, out string stateDescription);
        public GetStateInt32ProcessSampleModeStringOutDelegate GetStateInt32ProcessSampleModeStringOut;

        ProcessSampleAsyncResult ISampleService.GetState(int batchId, ProcessSampleMode processSampleMode, out string stateDescription)
        {
            stateDescription = default(string);


            if (GetStateInt32ProcessSampleModeStringOut != null)
            {
                return GetStateInt32ProcessSampleModeStringOut(batchId, processSampleMode, out stateDescription);
            } else if (_inner != null)
            {
                return ((ISampleService)_inner).GetState(batchId, processSampleMode, out stateDescription);
            }

            return default(ProcessSampleAsyncResult);
        }

    }
}