using Confirmit.CATI.Core.Batch.Interfaces;

namespace Confirmit.CATI.Core.Batch.Initializers
{
    internal class SelectedBatchInitializer : AbstractBatchInitializer<SelectedBatchParameters>
    {
        public override void Initialize(IBatchUploader uploader, SelectedBatchParameters parameters)
        {
            uploader.UploadFromMemory(parameters.Items);
        }
    }
}
