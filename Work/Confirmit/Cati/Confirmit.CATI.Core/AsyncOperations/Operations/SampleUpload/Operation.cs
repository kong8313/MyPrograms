using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.SampleUpload
{
    public class Operation : AsyncOperation<Descriptor, Parameters>
    {
        private readonly ISampleService _sampleService;
        private readonly ISurveyRepository _surveyRepository;
        
        public Operation(ISampleService sampleService, ISurveyRepository surveyRepository)
        {
            _sampleService = sampleService;
            _surveyRepository = surveyRepository;
        }

        public override BaseAsyncOperationManagementActivityEvent<Parameters> CreateEvent(BvAsyncOperationQueueEntity entity, Parameters parameters)
        {
            return new SampleUploadEvent(parameters.SurveyId, parameters.ProjectId, parameters, entity);
        }

        public override AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity,
            Parameters parameters,
            IAsyncOperationProgressLogger progressLogger,
            BaseAsyncOperationManagementActivityEvent<Parameters> evt, CancellationToken cancellationToken)
        {
            Action<string> taskLog = message => progressLogger.AppendText(entity.Id, message, evt.Duration, true);
            Action<int, int, int> updateProgress = 
                (totalItemsCount, succeededItemsCount, failedItemsCount) => progressLogger.UpdateProgress(entity.Id, totalItemsCount, succeededItemsCount, failedItemsCount);
            var result = new AsyncOperationResult { ProcessedItemsCount = 1, State = AsyncOperationState.Completed };

            var survey = _surveyRepository.GetByName(parameters.ProjectId);
            using (new EventDetailsScope(evt.Details))
            {
                try
                {
                    _sampleService.ProcessSample(survey, parameters.BatchId, parameters.ProcessSampleMode,
                        parameters.SchedulingMode, taskLog, updateProgress, result, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(ex);
                    result.State = AsyncOperationState.Failed;

                    SampleService.SetState(parameters.BatchId, parameters.ProcessSampleMode, ProcessSampleAsyncResult.Error, "Error! " + ex);

                    Trace.TraceError($"ProcessSample operation with batchID = {parameters.BatchId}, " +
                                     $"processSampleMode = {parameters.ProcessSampleMode} schedulingMode = {parameters.SchedulingMode} has failed " +
                                     $"on Survey with SID = {survey.Name} ({survey.Description}). Exception: {ex}");
                    taskLog("Error! " + ex);
                }
            }

            return result;
        }
    }
}
