using System.Data;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services.Interfaces;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services
{
    public class ContextInfoService : IContextInfoService
    {
        public void WriteContextInfo(int operationId, OperationType operationType, int callcenterId, int its = 0, DialingMode dialMode = 0)
        {
            var query = GetContextInfoSql(operationId, operationType, callcenterId, its, dialMode);
            new DatabaseEngine().ExecuteNonQuery(query, CommandType.Text);
        }

        public static string GetContextInfoSql(int operationId, OperationType operationType, int callcenterId, int its = 0, DialingMode dialMode = 0)
        {
            //in some cases we pass NotDefined=-1 to ITS 
            its = its == -1 ? 0 : its;
            var query = string.Format(
                @"DECLARE @Context VARBINARY(128)
                SET @Context = CONVERT(VARBINARY(128), '{0},{1},{2},{3},{4}')
                SET CONTEXT_INFO @Context", its, operationId, (int) operationType, callcenterId, (int) dialMode);
            return query;
        }

        public void ResetContextInfo()
        {
            var query =  @"SET CONTEXT_INFO 0x";
            new DatabaseEngine().ExecuteNonQuery(query, CommandType.Text);
        }
    }
}
