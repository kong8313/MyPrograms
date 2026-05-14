using System;

namespace Confirmit.CATI.Core.Batch.Interfaces
{
    public interface IBatchInitializer
    {
        Type SupportedBatchParametersType { get; }
        void Initialize(IBatchUploader uploader, BatchParameters parameters);
    }
}