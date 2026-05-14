using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.DeleteRespondents
{
    class Operation : IAsyncOperation
    {
        private readonly IInterviewService _interviewService;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;

        public Operation(IInterviewService interviewService, IDatabaseLockTimeouts databaseLockTimeouts)
        {
            _interviewService = interviewService;
            _databaseLockTimeouts = databaseLockTimeouts;
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
            var parameters = DeserializeParameters(serializedParameters);
            var evt = new DeleteRespondentsAsyncEvent(parameters.SurveyId, parameters.ProjectId, parameters, entity);
            var result = new AsyncOperationResult { ProcessedItemsCount = 1, State = AsyncOperationState.Completed };

            using (
                var dbLock = DatabaseLockService.CreateLock(
                    DatabaseLockTimeoutsAndRecourceNames.ScheduleResourceName,
                    "ManagementService.DeleteRespondents",
                    _databaseLockTimeouts.DefaultLockTimeoutInMs))
            {
                if (!dbLock.TryEnterLock())
                {
                    throw new Exception("Cannot enter database lock. Async operation DeleteRespondents falied.");
                }

                _interviewService.DeleteRespondents(parameters.SurveyId, parameters.RespondentIds, cancellationToken);
            }

            evt.Finish();

            return result;
        }
    }
}