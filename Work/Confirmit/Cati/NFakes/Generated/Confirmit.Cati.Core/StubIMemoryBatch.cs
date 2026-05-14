using System;
using Confirmit.CATI.Core.Batch;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Batch.Fakes
{
    public class StubIMemoryBatch : IMemoryBatch 
    {
        private IMemoryBatch _inner;

        public StubIMemoryBatch()
        {
            _inner = null;
        }

        public IMemoryBatch Inner
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

        private IEnumerable<int> _Items;
        public Func<IEnumerable<int>> ItemsGet;
        public Action<IEnumerable<int>> ItemsSetIEnumerableOfInt32;

        IEnumerable<int> IMemoryBatch.Items
        {
            get
            {
                if (ItemsGet != null)
                {
                    return ItemsGet();
                } else if (_inner != null)
                {
                    return ((IMemoryBatch)_inner).Items;
                }

                if (ItemsSetIEnumerableOfInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Items;
                }

                return default(IEnumerable<int>);
            }

        }

    }
}