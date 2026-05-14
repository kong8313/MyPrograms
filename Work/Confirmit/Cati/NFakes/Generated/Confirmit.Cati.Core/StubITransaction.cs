using System;
using Confirmit.CATI.Core.Transactions;

namespace Confirmit.CATI.Core.Transactions.Fakes
{
    public class StubITransaction : ITransaction 
    {
        private ITransaction _inner;

        public StubITransaction()
        {
            _inner = null;
        }

        public ITransaction Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecuteDelegate();
        public ExecuteDelegate Execute;

        void ITransaction.Execute()
        {

            if (Execute != null)
            {
                Execute();
            } else if (_inner != null)
            {
                ((ITransaction)_inner).Execute();
            }
        }

    }
}