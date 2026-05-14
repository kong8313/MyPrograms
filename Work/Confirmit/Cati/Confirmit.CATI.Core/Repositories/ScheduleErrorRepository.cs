using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Repositories
{
    public class ScheduleErrorRepository : IScheduleErrorRepository
    {
        
        public ScheduleErrorRepository()
        {

        }

        public void DeleteOldErrors(BvScheduleErrorEntity lastErrorToDelete, int scheduleId)
        {
            if (lastErrorToDelete == null)
                return;

            var condition = "ScheduleID = @ScheduleID AND Timestamp <= @Timestamp";

            var sqlParams = new SqlParameter[] {
                new SqlParameter("ScheduleID", scheduleId),
                new SqlParameter("Timestamp", lastErrorToDelete.Timestamp)
            };

            BvScheduleErrorAdapter.DeleteByCondition(condition, sqlParams);
        }

        public BvScheduleErrorEntity GetByRowNumber(int rowNumber, int scheduleId)
        {
            var query = $@"
                {BvScheduleErrorAdapter.selectSql} 
                WHERE ScheduleID = @ScheduleID 
                Order By Timestamp 
                OFFSET @Offset ROWS 
                FETCH NEXT 1 ROWS ONLY";

            using (var reader = new DatabaseEngine().ExecuteReaderInNewConnection(query, 
                System.Data.CommandType.Text, 
                new SqlParameter("ScheduleID", scheduleId), 
                new SqlParameter("Offset", rowNumber - 1)))
            {
                return BvScheduleErrorAdapter.Read(reader);
            }
        }

        public int GetErrorsCountByScheduleID(int scheduleId)
        {
            var query = @"
                SELECT COUNT(*) 
                FROM dbo.[BvScheduleError] 
                WHERE ScheduleID = @ScheduleID";

            return new DatabaseEngine().ExecuteScalar<int>(query, new SqlParameter("ScheduleID", scheduleId));
        }

        public void Insert(BvScheduleErrorEntity entity)
        {
            BvScheduleErrorAdapter.Insert(entity);
        }

        public List<BvScheduleErrorEntity> GetByScheduleId(int scheduleId)
        {
            string condition = "ScheduleID = @ScheduleID";
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("ScheduleID", scheduleId)
            };

            return BvScheduleErrorAdapter.GetByCondition(condition, parameters);
        }

        public List<BvScheduleErrorEntity> GetNotSentErrors()
        {
            string condition = "NotificationSent = @NotificationSent";
            SqlParameter[] parameters = new SqlParameter[] {
                new SqlParameter("NotificationSent", false)
            };

            return BvScheduleErrorAdapter.GetByCondition(condition, parameters);
        }

        public void SetNotificationSent(IEnumerable<int> ids)
        {
            var query =
                @"
                UPDATE dbo.[BvScheduleError] SET
                [NotificationSent] = 1
                WHERE EXISTS(SELECT 1 FROM @ids WHERE Value = Id)";

            new DatabaseEngine().ExecuteNonQuery(query, BvIntArrayTypeAdapter.CreateSqlParameter("@ids", ids));
        }
    }
}
