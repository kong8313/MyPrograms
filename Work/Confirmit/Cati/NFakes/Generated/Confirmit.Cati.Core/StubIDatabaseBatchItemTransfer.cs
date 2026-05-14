using System;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.Batch.Tools;

namespace Confirmit.CATI.Core.Batch.Tools.Fakes
{
    public class StubIDatabaseBatchItemTransfer : IDatabaseBatchItemTransfer 
    {
        private IDatabaseBatchItemTransfer _inner;

        public StubIDatabaseBatchItemTransfer()
        {
            _inner = null;
        }

        public IDatabaseBatchItemTransfer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool TransferToIDatabaseBatchIDatabaseBatchInt32Delegate(IDatabaseBatch sourceBatch, IDatabaseBatch destinationBatch, int countItems);
        public TransferToIDatabaseBatchIDatabaseBatchInt32Delegate TransferToIDatabaseBatchIDatabaseBatchInt32;

        bool IDatabaseBatchItemTransfer.TransferTo(IDatabaseBatch sourceBatch, IDatabaseBatch destinationBatch, int countItems)
        {


            if (TransferToIDatabaseBatchIDatabaseBatchInt32 != null)
            {
                return TransferToIDatabaseBatchIDatabaseBatchInt32(sourceBatch, destinationBatch, countItems);
            } else if (_inner != null)
            {
                return ((IDatabaseBatchItemTransfer)_inner).TransferTo(sourceBatch, destinationBatch, countItems);
            }

            return default(bool);
        }

    }
}