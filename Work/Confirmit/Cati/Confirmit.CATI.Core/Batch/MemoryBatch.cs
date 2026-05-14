using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.Batch.Interfaces;
using Confirmit.CATI.Core.DAL.Framework;

namespace Confirmit.CATI.Core.Batch
{
    class MemoryBatch : IMemoryBatch, IBatchUploader
    {
        public IEnumerable<int> Items { get; set; }

        public int Size
        {
            get { return Items.Count(); }
        }

        public void Dispose()
        {
            
        }

        public void UploadFromDatabase(string sqlQuery, params SqlParameter[] parameters)
        {
            Items = new DatabaseEngine().ExecuteScalarList<int>(sqlQuery, CommandType.Text, parameters);
        }

        public void UploadFromMemory(IEnumerable<int> items)
        {
            Items = items;
        }
    }
}
