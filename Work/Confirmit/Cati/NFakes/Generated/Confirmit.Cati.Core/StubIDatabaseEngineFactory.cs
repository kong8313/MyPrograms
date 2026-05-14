using System;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces.Fakes
{
    public class StubIDatabaseEngineFactory : IDatabaseEngineFactory 
    {
        private IDatabaseEngineFactory _inner;

        public StubIDatabaseEngineFactory()
        {
            _inner = null;
        }

        public IDatabaseEngineFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDatabaseEngine CreateForCurrentInstanceDatabaseDelegate();
        public CreateForCurrentInstanceDatabaseDelegate CreateForCurrentInstanceDatabase;

        IDatabaseEngine IDatabaseEngineFactory.CreateForCurrentInstanceDatabase()
        {


            if (CreateForCurrentInstanceDatabase != null)
            {
                return CreateForCurrentInstanceDatabase();
            } else if (_inner != null)
            {
                return ((IDatabaseEngineFactory)_inner).CreateForCurrentInstanceDatabase();
            }

            return default(IDatabaseEngine);
        }

        public delegate IDatabaseEngine CreateForDefaultInstanceDatabaseDelegate();
        public CreateForDefaultInstanceDatabaseDelegate CreateForDefaultInstanceDatabase;

        IDatabaseEngine IDatabaseEngineFactory.CreateForDefaultInstanceDatabase()
        {


            if (CreateForDefaultInstanceDatabase != null)
            {
                return CreateForDefaultInstanceDatabase();
            } else if (_inner != null)
            {
                return ((IDatabaseEngineFactory)_inner).CreateForDefaultInstanceDatabase();
            }

            return default(IDatabaseEngine);
        }

        public delegate IDatabaseEngine CreateForConfirmlogDatabaseDelegate();
        public CreateForConfirmlogDatabaseDelegate CreateForConfirmlogDatabase;

        IDatabaseEngine IDatabaseEngineFactory.CreateForConfirmlogDatabase()
        {


            if (CreateForConfirmlogDatabase != null)
            {
                return CreateForConfirmlogDatabase();
            } else if (_inner != null)
            {
                return ((IDatabaseEngineFactory)_inner).CreateForConfirmlogDatabase();
            }

            return default(IDatabaseEngine);
        }

        public delegate IDatabaseEngine CreateForConfirmDatabaseDelegate();
        public CreateForConfirmDatabaseDelegate CreateForConfirmDatabase;

        IDatabaseEngine IDatabaseEngineFactory.CreateForConfirmDatabase()
        {


            if (CreateForConfirmDatabase != null)
            {
                return CreateForConfirmDatabase();
            } else if (_inner != null)
            {
                return ((IDatabaseEngineFactory)_inner).CreateForConfirmDatabase();
            }

            return default(IDatabaseEngine);
        }

        public delegate IDatabaseEngine CreateForCustomConnectionProviderIConnectionProviderDelegate(IConnectionProvider connectionProvider);
        public CreateForCustomConnectionProviderIConnectionProviderDelegate CreateForCustomConnectionProviderIConnectionProvider;

        IDatabaseEngine IDatabaseEngineFactory.CreateForCustomConnectionProvider(IConnectionProvider connectionProvider)
        {


            if (CreateForCustomConnectionProviderIConnectionProvider != null)
            {
                return CreateForCustomConnectionProviderIConnectionProvider(connectionProvider);
            } else if (_inner != null)
            {
                return ((IDatabaseEngineFactory)_inner).CreateForCustomConnectionProvider(connectionProvider);
            }

            return default(IDatabaseEngine);
        }

    }
}