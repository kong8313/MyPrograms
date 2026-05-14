using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Services.Interfaces.Database
{
    interface ITableRowService
    {
        DataTable SelectRow(string tableName, string[] keyColumns, SqlParameter keys);
    }
}
