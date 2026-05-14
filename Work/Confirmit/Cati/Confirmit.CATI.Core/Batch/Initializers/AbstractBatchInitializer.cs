using System;
using Confirmit.CATI.Core.Batch.Interfaces;

namespace Confirmit.CATI.Core.Batch.Initializers
{
    public abstract class AbstractBatchInitializer<TBatchParamaters> : IBatchInitializer
        where TBatchParamaters : BatchParameters
    {
        protected AbstractBatchInitializer()
        {
            SupportedBatchParametersType = typeof (TBatchParamaters);
        }

        public Type SupportedBatchParametersType { get; private set; }

        public void Initialize(IBatchUploader uploader, BatchParameters parameters)
        {
            Initialize(uploader, (TBatchParamaters)parameters);
        }

        public abstract void Initialize(IBatchUploader uploader, TBatchParamaters parameters);
    }
}
