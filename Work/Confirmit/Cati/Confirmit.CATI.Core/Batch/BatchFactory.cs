using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.Batch.Interfaces;

namespace Confirmit.CATI.Core.Batch
{
    internal class BatchFactory : IBatchFactory
    {
        private readonly Dictionary<Type, IBatchInitializer> _initializers;

        public BatchFactory(IBatchInitializer[] initializers)
        {
            _initializers = initializers.ToDictionary(x => x.SupportedBatchParametersType);
        }

        public IDatabaseBatch CreateEmptyDatabaseBatch()
        {
            return new DatabaseBatch();
        }

        public IDatabaseBatch CreateDatabaseBatch(BatchParameters parameters)
        {
            return CreateBatch<DatabaseBatch>(parameters);
        }

        public IMemoryBatch CreateMemoryBatch(BatchParameters parameters)
        {
            return CreateBatch<MemoryBatch>(parameters);
        }

        private TBatch CreateBatch<TBatch>(BatchParameters parameters) 
            where TBatch : IBatchUploader, IDisposable, new()
        {
            var batch = new TBatch();

            try
            {
                _initializers[parameters.GetType()].Initialize(batch, parameters);
            }
            catch
            {
                batch.Dispose();
                throw;
            }

            return batch;
        }

        
    }
}
