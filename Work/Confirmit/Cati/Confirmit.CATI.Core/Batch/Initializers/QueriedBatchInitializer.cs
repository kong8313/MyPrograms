using Confirmit.CATI.Core.Batch.Interfaces;

namespace Confirmit.CATI.Core.Batch.Initializers
{
    internal class QueriedBatchInitializer : AbstractBatchInitializer<QueriedBatchParameters>
    {
        public override void Initialize(IBatchUploader uploader, QueriedBatchParameters parameters)
        {
            uploader.UploadFromDatabase(parameters.SqlQuery);
        }
    }
}
