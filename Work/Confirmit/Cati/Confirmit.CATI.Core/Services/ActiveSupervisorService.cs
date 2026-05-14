using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using System;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Services
{
    public class ActiveSupervisorService : IActiveSupervisorService
    {
        public int CleanActiveSupervisors(TimeSpan expirationTime)
        {
            var expirationDate = DateTime.UtcNow - expirationTime;

            var deletedRows = BvSupervisorsActiveAdapter.DeleteByConditionAndOutput("[LastActiveTime] < @ExpirationDate", new SqlParameter("@ExpirationDate", expirationDate));
            return deletedRows.Count;
        }
    }
}
