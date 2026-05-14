using System;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.DAL.Framework.Interfaces.Fakes
{
    public class StubIConnectionProvider : IConnectionProvider 
    {
        private IConnectionProvider _inner;

        public StubIConnectionProvider()
        {
            _inner = null;
        }

        public IConnectionProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void DisposeDelegate();
        public DisposeDelegate Dispose;

        void IDisposable.Dispose()
        {

            if (Dispose != null)
            {
                Dispose();
            } else if (_inner != null)
            {
                ((IDisposable)_inner).Dispose();
            }
        }

        private SqlConnection _Connection;
        public Func<SqlConnection> ConnectionGet;
        public Action<SqlConnection> ConnectionSetSqlConnection;

        SqlConnection IConnectionProvider.Connection
        {
            get
            {
                if (ConnectionGet != null)
                {
                    return ConnectionGet();
                } else if (_inner != null)
                {
                    return ((IConnectionProvider)_inner).Connection;
                }

                if (ConnectionSetSqlConnection == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Connection;
                }

                return default(SqlConnection);
            }

        }

        private SqlTransaction _Transaction;
        public Func<SqlTransaction> TransactionGet;
        public Action<SqlTransaction> TransactionSetSqlTransaction;

        SqlTransaction IConnectionProvider.Transaction
        {
            get
            {
                if (TransactionGet != null)
                {
                    return TransactionGet();
                } else if (_inner != null)
                {
                    return ((IConnectionProvider)_inner).Transaction;
                }

                if (TransactionSetSqlTransaction == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Transaction;
                }

                return default(SqlTransaction);
            }

        }

    }
}