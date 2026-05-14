using System;
using Confirmit.CATI.Backend.WebApiServices;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubIDatabaseContextFactory : IDatabaseContextFactory 
    {
        private IDatabaseContextFactory _inner;

        public StubIDatabaseContextFactory()
        {
            _inner = null;
        }

        public IDatabaseContextFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDatabaseContext CreateDatabaseContextStringDelegate(string connectionString);
        public CreateDatabaseContextStringDelegate CreateDatabaseContextString;

        IDatabaseContext IDatabaseContextFactory.CreateDatabaseContext(string connectionString)
        {


            if (CreateDatabaseContextString != null)
            {
                return CreateDatabaseContextString(connectionString);
            } else if (_inner != null)
            {
                return ((IDatabaseContextFactory)_inner).CreateDatabaseContext(connectionString);
            }

            return default(IDatabaseContext);
        }

    }
}