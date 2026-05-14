using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.ConfigureClusteredQuota
{
    public class Operation : ICallsManagementBatchedOperation 
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IQuotaClusteringSyncService _clusteredQuotaSyncService;

        public Operation(ISurveyRepository surveyRepository, IQuotaClusteringSyncService clusteredQuotaSyncService)
        {
            _surveyRepository = surveyRepository;
            _clusteredQuotaSyncService = clusteredQuotaSyncService;
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

            var survey = _surveyRepository.GetById(parameters.SurveyId);

            var evt = new ConfigureClusteredQuotaEvent(parameters.SurveyId, survey.Name, parameters, entity);
            
            var result = new AsyncOperationResult { ProcessedItemsCount = 1, State = AsyncOperationState.Completed };

            if (!String.IsNullOrEmpty(survey.ClusteredQuotaName))
            {
                _clusteredQuotaSyncService.InitializeCallsAndCounters(survey, cancellationToken);
            }
            else
            {
                _clusteredQuotaSyncService.ResetCallsAndCounters(survey, cancellationToken);
            }
            
            evt.Finish();

            return result;
        }



        public void ProcessSubBatch(ICallsManagementBatchedOperationBase operation, Batch.IDatabaseBatch subBatch, object state, BvAsyncOperationQueueEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
