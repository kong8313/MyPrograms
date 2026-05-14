using System;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces.Fakes
{
    public class StubIDatabaseConnectionProviderFactory : IDatabaseConnectionProviderFactory 
    {
        private IDatabaseConnectionProviderFactory _inner;

        public StubIDatabaseConnectionProviderFactory()
        {
            _inner = null;
        }

        public IDatabaseConnectionProviderFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate ITransactedConnectionProvider CreateConnectionProviderForConfirmlogDatabaseDelegate();
        public CreateConnectionProviderForConfirmlogDatabaseDelegate CreateConnectionProviderForConfirmlogDatabase;

        ITransactedConnectionProvider IDatabaseConnectionProviderFactory.CreateConnectionProviderForConfirmlogDatabase()
        {


            if (CreateConnectionProviderForConfirmlogDatabase != null)
            {
                return CreateConnectionProviderForConfirmlogDatabase();
            } else if (_inner != null)
            {
                return ((IDatabaseConnectionProviderFactory)_inner).CreateConnectionProviderForConfirmlogDatabase();
            }

            return default(ITransactedConnectionProvider);
        }

    }
}