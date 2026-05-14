using System;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces
{
    public interface IConnectionProvider : IDisposable
    {
        SqlConnection Connection { get; }

        SqlTransaction Transaction { get; }
    }
}
