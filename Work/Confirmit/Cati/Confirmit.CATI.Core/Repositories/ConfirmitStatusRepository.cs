using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Repositories
{
    public static class ConfirmitStatusRepository
    {
        public static InterviewStatus GetByConfirmitStatus(string status)
        {
            var result = new InterviewStatus { Code = (int)CallOutcome.Error, Name = CallOutcome.Error.ToString() };

            var itsStatusEntity = BvConfirmitStatusAdapter.GetByCondition(
                "[StatusCode_Cnf] = @Status_CF", 
                new SqlParameter("@Status_CF", (object)status ?? DBNull.Value)).FirstOrDefault();

            if (itsStatusEntity != null)
            {
                result.Code = itsStatusEntity.StatusCode_BvFEE;
                result.Name = itsStatusEntity.StatusName_Cnf;
            }

            return result;
        }
    }
}
