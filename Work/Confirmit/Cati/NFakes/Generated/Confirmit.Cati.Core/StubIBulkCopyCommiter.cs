using System;
using Confirmit.CATI.Core.DAL.Framework.BulkCopy;

namespace Confirmit.CATI.Core.DAL.Framework.BulkCopy.Fakes
{
    public class StubIBulkCopyCommiter : IBulkCopyCommiter 
    {
        private IBulkCopyCommiter _inner;

        public StubIBulkCopyCommiter()
        {
            _inner = null;
        }

        public IBulkCopyCommiter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CommitDelegate();
        public CommitDelegate Commit;

        void IBulkCopyCommiter.Commit()
        {

            if (Commit != null)
            {
                Commit();
            } else if (_inner != null)
            {
                ((IBulkCopyCommiter)_inner).Commit();
            }
        }

    }
}