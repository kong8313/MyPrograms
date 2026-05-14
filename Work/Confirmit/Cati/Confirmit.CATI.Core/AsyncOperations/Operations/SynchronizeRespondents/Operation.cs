using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.SynchronizeRespondents
{
    public class Operation : IAsyncOperation
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISystemSettings _systemSettings;
        private readonly IRespondentsSynchronizationProcessor _respondentsSynchronizationProcessor;

        public Operation(ISurveyRepository surveyRepository, ISystemSettings systemSettings, IRespondentsSynchronizationProcessor respondentsSynchronizationProcessor)
        {
            _surveyRepository = surveyRepository;
            _systemSettings = systemSettings;
            _respondentsSynchronizationProcessor = respondentsSynchronizationProcessor;
        }

        public IOperationDescriptor Descriptor
        {
            get { return new Descriptor(); }
        }

        private Parameters DeserializeParameters(string parameters)
        {
            var serializer = new XmlSerializer(typeof(Parameters));

            using (var reader = new StringReader(parameters))
            {
                return (Parameters)serializer.Deserialize(reader);
            }
        }

        public AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, string serializedParameters, IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            progressLogger.AppendText(entity.Id, "Start", sw.Elapsed, false);

            var parameters = DeserializeParameters(serializedParameters);

            var survey = _surveyRepository.GetById(parameters.SurveyId);

            var evt = new SynchronizeRespondentsEvent(parameters.SurveyId, survey.Name, entity);

            using (new EventDetailsScope(evt.Details))
            {
                var context = new RespondentsSynchronizationContext
                {
                    Survey = survey,
                    AddedRecords = 0,
                    DeletedRecords = 0,
                    ExtendedStatus = (int) CallOutcome.SynchronizedSample,
                    PartitionSize = _systemSettings.AsyncOperation.AddSamplePortionSize,
                    EventDetails = evt.Details,
                    TimeZoneReolver = new TimezoneResolver(),
                    OperationId = entity.Id
                };

                context.EventDetails.AddTiming("RespondentsSynchronizationCycle");
                _respondentsSynchronizationProcessor.SynchronizeRespondents(context, cancellationToken);

                evt.Details.CreatedRecords = context.AddedRecords;
                evt.Details.DeletedRecords = context.DeletedRecords;

                evt.Finish();

                var result = new AsyncOperationResult
                    { ProcessedItemsCount = context.AddedRecords + context.DeletedRecords, State = AsyncOperationState.Completed };

                progressLogger.AppendText(entity.Id, $"Complete. Created {context.AddedRecords} interviews. Deleted {context.DeletedRecords} interviews.", sw.Elapsed, true);

                return result;
            }
        }
    }
}
