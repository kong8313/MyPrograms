using System.Data.SqlClient;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces
{
    public interface ITransactedConnectionProvider : IConnectionProvider
    {
        SqlTransaction BeginTransaction(string transactionName);
    }
}