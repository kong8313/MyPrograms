using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services.Database.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Database
{
    public class DatabaseStatisticService : IDatabaseStatisticService
    {
        public void UpdateStatistic(string tableName)
        {
            string query = String.Format(@"UPDATE STATISTICS [{0}]", tableName);

            new DatabaseEngine().ExecuteNonQuery(query, CommandType.Text);
        }
    }
}
