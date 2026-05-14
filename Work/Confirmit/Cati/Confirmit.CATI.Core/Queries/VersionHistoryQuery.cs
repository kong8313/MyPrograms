using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Core.Queries
{
    public class VersionHistoryQuery
    {
        public List<BvVersionHistoryEntity> GetAll()
        { 
            return BvVersionHistoryAdapter.GetAll();
        }

        [NotNull]
        public BvVersionHistoryEntity GetById(int logId)
        {
            var versionHistoryEntity = BvVersionHistoryAdapter.GetByCondition(
                "[Id] = @DbLogId",
                new SqlParameter("@DbLogId", logId)).FirstOrDefault();

            if (versionHistoryEntity == null)
            {
                throw new InternalErrorException(string.Format("DB update log message with ID '{0}' does not exist.", logId));
            }

            return versionHistoryEntity;
        }
    }
}