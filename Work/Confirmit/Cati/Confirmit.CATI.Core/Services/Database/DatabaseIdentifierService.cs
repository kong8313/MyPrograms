using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Services.Database.Interfaces;

namespace Confirmit.CATI.Core.Services.Database
{
    public class DatabaseIdentifierService : IDatabaseIdentifierService
    {
        private readonly IDatabaseEngineFactory _databaseEngineFactory;

        private readonly ConcurrentDictionary<string, string> _identifier2EscapedIdentifier = new ConcurrentDictionary<string, string>();

        public DatabaseIdentifierService(IDatabaseEngineFactory databaseEngineFactory)
        {
            _databaseEngineFactory = databaseEngineFactory;
        }

        public string GetEscapedIdentifier(string identifier)
        {
            string escapedIdentifier;

            if (!_identifier2EscapedIdentifier.TryGetValue(identifier, out escapedIdentifier))
            {
                escapedIdentifier = _databaseEngineFactory.CreateForCurrentInstanceDatabase().ExecuteScalar<string>(
                    "SELECT QUOTENAME(@identifier)",
                    CommandType.Text,
                    new SqlParameter("@identifier", identifier));

                _identifier2EscapedIdentifier.TryAdd(identifier, escapedIdentifier);
            }

            return escapedIdentifier;
        }
    }
}