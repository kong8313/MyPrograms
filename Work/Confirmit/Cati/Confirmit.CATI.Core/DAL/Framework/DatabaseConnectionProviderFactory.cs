using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public class DatabaseConnectionProviderFactory: IDatabaseConnectionProviderFactory
    {
        private readonly IConnectionStrings _connectionStrings;

        public DatabaseConnectionProviderFactory(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public ITransactedConnectionProvider CreateConnectionProviderForConfirmlogDatabase()
        {
            return new RemoteConnectionProvider(_connectionStrings.ConfirmlogConnectionString);
        }
    }
}