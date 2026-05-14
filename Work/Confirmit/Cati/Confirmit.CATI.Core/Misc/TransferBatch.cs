using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.Core.Misc
{
    /// <summary>
    /// Represents Fusion transfer batch identifier. This identifier is used by Fusion
    /// for batch operations (for example activating list of interviews). It's allocate
    /// some unmanaged resources in Fusion database, so those resources should be
    /// released ASAP.
    /// </summary>
    public class TransferBatch : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes new instance of FusionTransferBatch class.
        /// </summary>
        protected TransferBatch()
        {
            int batch;
            //Unused rudimental param for stored procedure :)
            BvSpCreateTransferBatchIDAdapter.ExecuteNonQuery(0, out batch);
            Value = batch;
            BatchSize = 0;
        }

        public TransferBatch(int batchId)
        {
            Value = batchId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Batch identifier.
        /// </summary>
        public int Value
        {
            get;
            protected set;
        }

        public int BatchSize
        {
            get; 
            private set;
        }

        #endregion

        #region IDisposable Members

        private bool m_Disposed;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged
        /// resources; <c>false</c> to release only unmanaged resources.</param>
        /// <remarks>
        /// Method executes in two distinct scenarios. If disposing equals true, the method
        /// has been called directly or indirectly by a user's code. Managed and unmanaged
        /// resources can be disposed. If disposing equals false, the method has been
        /// called by the runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        ///</remarks>
        private void Dispose(bool disposing)
        {
            if(!m_Disposed)
            {
                if(disposing)
                {
                    // release managed resources here.
                }

                try
                {
                    // release unmanaged resources.
                    Clear();
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }

                m_Disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="TransferBatch"/> is reclaimed by garbage collection.
        /// </summary>
        ~TransferBatch()
        {
            Trace.TraceError("TransferBatch dispose was not performed manually");
            Dispose(false);
        }

        #endregion

        #region Methods

        public static TransferBatch Create()
        {
            return new TransferBatch();
        }

        /// <summary>
        /// Insert multiple items into BvTransferArray
        /// </summary>
        /// <param name="items">Item identificators</param>
        public void Insert(IEnumerable<int> items)
        {            
            var transferArraysTable = BvTransferArraysAdapter.CreateDataTable();
            var transferArraysEntity = new BvTransferArraysEntity()
            {
                BatchID = Value,
            };

            foreach ( var item in items )
            {
                transferArraysEntity.ItemID = item;

                BvTransferArraysAdapter.SaveEntity2DataTable( 
                    transferArraysTable, 
                    transferArraysEntity );
            }

            using ( var connection = new SqlConnection( BackendInstance.Current.ConnectionString ) )
            {
                connection.Open();

                using (var bulk = new SqlBulkCopy(connection)
                    {
                        BatchSize = 10000,
                        BulkCopyTimeout = 60*60,
                        DestinationTableName = transferArraysTable.TableName
                    })
                {
                    bulk.WriteToServer(transferArraysTable);
                }
            }

            BatchSize = items.Count();
        }

        #endregion

        /// <summary>
        /// Move specific count of items to specific batch
        /// </summary>
        /// <param name="destination">destination batch</param>
        /// <param name="count">count items which should be moved to destination batch</param>
        /// <returns>count of items which was moved to destination batch. If there are not items in source batch, then returns 0</returns>
        private int MoveTo(TransferBatch destination, int count)
        {
            return MoveTo( Value, destination.Value, count);
        }

        private static int MoveTo(int sourceBatchId, int destinationBatchId, int count)
        {
            int retVal;
            BvSpTransferArray_MoveAdapter.ExecuteNonQuery(sourceBatchId, destinationBatchId, count, out retVal);
            return retVal;
        }

        public void Clear()
        {
            BvSpDeleteTransferAdapter.ExecuteNonQuery(Value);
        }

        public void SetBatchSize(int size)
        {
            BatchSize = size;
        }
    }
}