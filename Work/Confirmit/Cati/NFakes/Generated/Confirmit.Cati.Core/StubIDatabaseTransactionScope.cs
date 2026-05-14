using System;
using Confirmit.CATI.Core.DAL.Framework;

namespace Confirmit.CATI.Core.DAL.Framework.Fakes
{
    public class StubIDatabaseTransactionScope : IDatabaseTransactionScope 
    {
        private IDatabaseTransactionScope _inner;

        public StubIDatabaseTransactionScope()
        {
            _inner = null;
        }

        public IDatabaseTransactionScope Inner
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

        public delegate void CommitDelegate();
        public CommitDelegate Commit;

        void IDatabaseTransactionScope.Commit()
        {

            if (Commit != null)
            {
                Commit();
            } else if (_inner != null)
            {
                ((IDatabaseTransactionScope)_inner).Commit();
            }
        }

    }
}