using System.Collections.Generic;
using System.Data.SqlClient;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Batch.Interfaces
{
    public interface IBatchUploader
    {
        void UploadFromDatabase(string sqlQuery, params SqlParameter[] parametrs);
        void UploadFromMemory(IEnumerable<int> items);
    }
}