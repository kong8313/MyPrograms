using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.SurveyArchiveServiceImplementation;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.RestoreSurvey
{
    public class Operation : IAsyncOperation
    {
        private readonly SurveyArchiveService _surveyArchiveService;

        public IOperationDescriptor Descriptor
        {
            get { return new Descriptor(); }
        }

        public Operation(SurveyArchiveService surveyArchiveService)
        {
            _surveyArchiveService = surveyArchiveService;
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
            var evt = new RestoreSurveyFromArchiveEvent();

            var parameters = DeserializeParameters(serializedParameters);
            evt.Details.Parameters = parameters;

            var warnings = _surveyArchiveService.Restore(parameters.SurveyId, parameters.Data, cancellationToken);

            evt.Details.Parameters.Data = null;
            evt.Save();

            var result = new AsyncOperationResult {ProcessedItemsCount = 1, State = AsyncOperationState.Completed };
            if (!string.IsNullOrEmpty(warnings))
            {
                result.Warnings = new List<string>(new[] {warnings});
            }

            return result;
        }
    }
}
