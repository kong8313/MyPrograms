using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IEditCallsQueryProvider
    {
        string GetQuery(
            int surveySid,
            int batchId,
            DateTime? timeToCall,
            DateTime? timeToExpire,
            int? callState,
            int? callPriority,
            int? shiftType,
            int? extendedStatus,
            byte? dialingMode,
            int fcdBehaviorAlgorithm,
            string whereCondition,
            int stateGroupId);

        IEnumerable<SqlParameter> GetSqlParameters(int surveySid,
            int batchId,
            DateTime? timeToCall,
            DateTime? timeToExpire,
            int? callState,
            int? callPriority,
            int? shiftType,
            int? extendedStatus,
            byte? dialingMode,
            int fcdBehaviorAlgorithm,
            int stateGroupId);
    }
}