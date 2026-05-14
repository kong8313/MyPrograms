using System;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.Batch.Fakes
{
    public class StubIBatchFactory : IBatchFactory 
    {
        private IBatchFactory _inner;

        public StubIBatchFactory()
        {
            _inner = null;
        }

        public IBatchFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDatabaseBatch CreateDatabaseBatchBatchParametersDelegate(BatchParameters parameters);
        public CreateDatabaseBatchBatchParametersDelegate CreateDatabaseBatchBatchParameters;

        IDatabaseBatch IBatchFactory.CreateDatabaseBatch(BatchParameters parameters)
        {


            if (CreateDatabaseBatchBatchParameters != null)
            {
                return CreateDatabaseBatchBatchParameters(parameters);
            } else if (_inner != null)
            {
                return ((IBatchFactory)_inner).CreateDatabaseBatch(parameters);
            }

            return default(IDatabaseBatch);
        }

        public delegate IMemoryBatch CreateMemoryBatchBatchParametersDelegate(BatchParameters parameters);
        public CreateMemoryBatchBatchParametersDelegate CreateMemoryBatchBatchParameters;

        IMemoryBatch IBatchFactory.CreateMemoryBatch(BatchParameters parameters)
        {


            if (CreateMemoryBatchBatchParameters != null)
            {
                return CreateMemoryBatchBatchParameters(parameters);
            } else if (_inner != null)
            {
                return ((IBatchFactory)_inner).CreateMemoryBatch(parameters);
            }

            return default(IMemoryBatch);
        }

        public delegate IDatabaseBatch CreateEmptyDatabaseBatchDelegate();
        public CreateEmptyDatabaseBatchDelegate CreateEmptyDatabaseBatch;

        IDatabaseBatch IBatchFactory.CreateEmptyDatabaseBatch()
        {


            if (CreateEmptyDatabaseBatch != null)
            {
                return CreateEmptyDatabaseBatch();
            } else if (_inner != null)
            {
                return ((IBatchFactory)_inner).CreateEmptyDatabaseBatch();
            }

            return default(IDatabaseBatch);
        }

    }
}