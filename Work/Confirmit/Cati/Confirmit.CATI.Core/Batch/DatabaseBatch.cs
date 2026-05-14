using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Batch
{
    internal class DatabaseBatch : IDatabaseBatch, IBatchUploader
    {
        private bool _disposed;

        public DatabaseBatch()
        {
            int batch;
            //Unused rudimental param for stored procedure :)
            BvSpCreateTransferBatchIDAdapter.ExecuteNonQuery(0, out batch);
            Id = batch;
            Size = 0;
        }

        public int Size { get; set; }

        public int Id { get; private set; }

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
            if (!_disposed)
            {
                if (disposing)
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

                _disposed = true;
            }
        }

        public void Clear()
        {
            BvSpDeleteTransferAdapter.ExecuteNonQuery(Id); 
            this.Size = 0;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// </summary>
        ~DatabaseBatch()
        {
            Trace.TraceError("DatabaseBatch dispose was not performed manually");
            Dispose(false);
        }

        public void UploadFromDatabase(string sqlQuery, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(BackendInstance.Current.ConnectionString))
            {
                connection.Open();

                var commandText = string.Format("INSERT INTO BvTransferArrays SELECT {0}, id FROM ( {1} ) items; SELECT @@ROWCOUNT", Id, sqlQuery);

                Size = new DatabaseEngine().ExecuteScalar<int>(commandText, CommandType.Text, parameters);
            }
        }

        public void UploadFromMemory(IEnumerable<int> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            var transferArraysTable = BvTransferArraysAdapter.CreateDataTable();
            var transferArraysEntity = new BvTransferArraysEntity
            {
                BatchID = Id,
            };

            int itemsCount = 0;

            foreach (var item in items)
            {
                transferArraysEntity.ItemID = item;

                BvTransferArraysAdapter.SaveEntity2DataTable(
                    transferArraysTable,
                    transferArraysEntity);

                itemsCount++;
            }

            using (var connection = new SqlConnection(BackendInstance.Current.ConnectionString))
            {
                connection.Open();

                using (var bulk = new SqlBulkCopy(connection)
                {
                    BatchSize = 10000,
                    BulkCopyTimeout = 60 * 60,
                    DestinationTableName = transferArraysTable.TableName
                })
                {
                    bulk.WriteToServer(transferArraysTable);
                }
            }

            Size = itemsCount;
        }
    }
}
