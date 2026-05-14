using System;
using System.Data.SqlClient;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IConnectionProvider : IDisposable
    {
        SqlConnection Connection { get; }

        SqlTransaction Transaction { get; }
    }
}
