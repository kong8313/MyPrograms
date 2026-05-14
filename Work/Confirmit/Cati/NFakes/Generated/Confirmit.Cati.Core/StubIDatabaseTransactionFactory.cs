using System;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces.Fakes
{
    public class StubIDatabaseTransactionFactory : IDatabaseTransactionFactory 
    {
        private IDatabaseTransactionFactory _inner;

        public StubIDatabaseTransactionFactory()
        {
            _inner = null;
        }

        public IDatabaseTransactionFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDatabaseTransactionScope CreateForCurrentInstanceDatabaseDatabaseTransactionOptionsDelegate(DatabaseTransactionOptions options);
        public CreateForCurrentInstanceDatabaseDatabaseTransactionOptionsDelegate CreateForCurrentInstanceDatabaseDatabaseTransactionOptions;

        IDatabaseTransactionScope IDatabaseTransactionFactory.CreateForCurrentInstanceDatabase(DatabaseTransactionOptions options)
        {


            if (CreateForCurrentInstanceDatabaseDatabaseTransactionOptions != null)
            {
                return CreateForCurrentInstanceDatabaseDatabaseTransactionOptions(options);
            } else if (_inner != null)
            {
                return ((IDatabaseTransactionFactory)_inner).CreateForCurrentInstanceDatabase(options);
            }

            return default(IDatabaseTransactionScope);
        }

        public delegate IDatabaseTransactionScope CreateForDefaultInstanceDatabaseDatabaseTransactionOptionsDelegate(DatabaseTransactionOptions options);
        public CreateForDefaultInstanceDatabaseDatabaseTransactionOptionsDelegate CreateForDefaultInstanceDatabaseDatabaseTransactionOptions;

        IDatabaseTransactionScope IDatabaseTransactionFactory.CreateForDefaultInstanceDatabase(DatabaseTransactionOptions options)
        {


            if (CreateForDefaultInstanceDatabaseDatabaseTransactionOptions != null)
            {
                return CreateForDefaultInstanceDatabaseDatabaseTransactionOptions(options);
            } else if (_inner != null)
            {
                return ((IDatabaseTransactionFactory)_inner).CreateForDefaultInstanceDatabase(options);
            }

            return default(IDatabaseTransactionScope);
        }

        public delegate IDatabaseTransactionScope CreateForConfirmlogDatabaseDatabaseTransactionOptionsDelegate(DatabaseTransactionOptions options);
        public CreateForConfirmlogDatabaseDatabaseTransactionOptionsDelegate CreateForConfirmlogDatabaseDatabaseTransactionOptions;

        IDatabaseTransactionScope IDatabaseTransactionFactory.CreateForConfirmlogDatabase(DatabaseTransactionOptions options)
        {


            if (CreateForConfirmlogDatabaseDatabaseTransactionOptions != null)
            {
                return CreateForConfirmlogDatabaseDatabaseTransactionOptions(options);
            } else if (_inner != null)
            {
                return ((IDatabaseTransactionFactory)_inner).CreateForConfirmlogDatabase(options);
            }

            return default(IDatabaseTransactionScope);
        }

        public delegate IDatabaseTransactionScope CreateForConfirmDatabaseDatabaseTransactionOptionsDelegate(DatabaseTransactionOptions options);
        public CreateForConfirmDatabaseDatabaseTransactionOptionsDelegate CreateForConfirmDatabaseDatabaseTransactionOptions;

        IDatabaseTransactionScope IDatabaseTransactionFactory.CreateForConfirmDatabase(DatabaseTransactionOptions options)
        {


            if (CreateForConfirmDatabaseDatabaseTransactionOptions != null)
            {
                return CreateForConfirmDatabaseDatabaseTransactionOptions(options);
            } else if (_inner != null)
            {
                return ((IDatabaseTransactionFactory)_inner).CreateForConfirmDatabase(options);
            }

            return default(IDatabaseTransactionScope);
        }

    }
}