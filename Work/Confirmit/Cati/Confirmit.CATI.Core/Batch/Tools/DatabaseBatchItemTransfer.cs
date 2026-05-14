using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Batch.Interfaces;

namespace Confirmit.CATI.Core.Batch.Tools
{
    public class DatabaseBatchItemTransfer : IDatabaseBatchItemTransfer
    {
        public bool TransferTo(IDatabaseBatch sourceBatch, IDatabaseBatch destinationBatch, int countItems)
        {
            int size;
            BvSpTransferArray_MoveAdapter.ExecuteNonQuery(sourceBatch.Id, destinationBatch.Id, countItems, out size);

            destinationBatch.Size += size;
            return size > 0;
        }
    }
}
