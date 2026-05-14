using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public abstract class AsyncOperationWithoutEvent<TDescriptor, TParameters> : IAsyncOperation
        where TDescriptor : IOperationDescriptor, new()
        where TParameters : IAsyncOperationParameters
    {
        public IOperationDescriptor Descriptor
        {
            get { return new TDescriptor(); }
        }

        private TParameters DeserializeParameters(string parameters)
        {
            var serializer = new XmlSerializer(typeof(TParameters));

            using (var reader = new StringReader(parameters))
            {
                return (TParameters)serializer.Deserialize(reader);
            }
        }

        public AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity, string serializedParameters, IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken)
        {
            var parameters = DeserializeParameters(serializedParameters);

            return Execute(entity, parameters, progressLogger, cancellationToken);
        }

        public abstract AsyncOperationResult Execute(BvAsyncOperationQueueEntity entity,
            TParameters parameters,
            IAsyncOperationProgressLogger progressLogger, CancellationToken cancellationToken);
    }
}