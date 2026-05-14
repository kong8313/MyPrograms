using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.RereadSurveyReplicatedData
{
    public class Operation : IAsyncOperation
    {
        private readonly IReplicationService _replicationService;

        public Operation(IReplicationService replicationService)
        {
            _replicationService = replicationService;
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
            var result = new AsyncOperationResult { State = AsyncOperationState.Completed };

            var parameters = DeserializeParameters(serializedParameters);

            try
            {
                _replicationService.RereadSurveyReplicatedData(parameters.SurveyId, parameters.Reason, cancellationToken);
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex);
                result.State = AsyncOperationState.Failed;

                return result;
            }


            return result;
        }
    }
}
