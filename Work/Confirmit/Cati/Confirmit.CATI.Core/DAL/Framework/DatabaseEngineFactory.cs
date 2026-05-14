using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.DAL.Framework
{
    public class DatabaseEngineFactory : IDatabaseEngineFactory
    {
        private readonly IConnectionStrings _connectionStrings;

        public DatabaseEngineFactory(IConnectionStrings connectionStrings)
        {
            _connectionStrings = connectionStrings;
        }

        public IDatabaseEngine CreateForCurrentInstanceDatabase()
        {
            return new DatabaseEngine();
        }

        public IDatabaseEngine CreateForDefaultInstanceDatabase()
        {
            return new DatabaseEngine(_connectionStrings.DefaultInstanceConnectionString);
        }

        public IDatabaseEngine CreateForConfirmlogDatabase()
        {
            return new DatabaseEngine(_connectionStrings.ConfirmlogConnectionString);
        }

        public IDatabaseEngine CreateForConfirmDatabase()
        {
            return new DatabaseEngine(_connectionStrings.ConfirmConnectionString);
        }

        public IDatabaseEngine CreateForCustomConnectionProvider(IConnectionProvider customConnectionProvider)
        {
            return new DatabaseEngine(customConnectionProvider);
        }
    }
}