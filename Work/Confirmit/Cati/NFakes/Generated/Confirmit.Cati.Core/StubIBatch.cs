using System;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.Batch.Fakes
{
    public class StubIBatch : IBatch 
    {
        private IBatch _inner;

        public StubIBatch()
        {
            _inner = null;
        }

        public IBatch Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _Size;
        public Func<int> SizeGet;
        public Action<int> SizeSetInt32;

        int IBatch.Size
        {
            get
            {
                if (SizeGet != null)
                {
                    return SizeGet();
                } else if (_inner != null)
                {
                    return ((IBatch)_inner).Size;
                }

                if (SizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Size;
                }

                return default(int);
            }

        }

    }
}