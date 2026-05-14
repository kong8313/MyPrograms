using System;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.Batch.Fakes
{
    public class StubIDatabaseBatch : IDatabaseBatch 
    {
        private IDatabaseBatch _inner;

        public StubIDatabaseBatch()
        {
            _inner = null;
        }

        public IDatabaseBatch Inner
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

        public delegate void ClearDelegate();
        public ClearDelegate Clear;

        void IDatabaseBatch.Clear()
        {

            if (Clear != null)
            {
                Clear();
            } else if (_inner != null)
            {
                ((IDatabaseBatch)_inner).Clear();
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

        private int _Id;
        public Func<int> IdGet;
        public Action<int> IdSetInt32;

        int IDatabaseBatch.Id
        {
            get
            {
                if (IdGet != null)
                {
                    return IdGet();
                } else if (_inner != null)
                {
                    return ((IDatabaseBatch)_inner).Id;
                }

                if (IdSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Id;
                }

                return default(int);
            }

        }

        private int _Size1;
        public Func<int> SizeGet1;
        public Action<int> SizeSetInt321;

        int IDatabaseBatch.Size
        {
            get
            {
                if (SizeGet1 != null)
                {
                    return SizeGet1();
                } else if (_inner != null)
                {
                    return ((IDatabaseBatch)_inner).Size;
                }

                if (SizeSetInt321 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Size1;
                }

                return default(int);
            }

            set
            {
                if (SizeSetInt321 != null)
                {
                    SizeSetInt321(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDatabaseBatch)_inner).Size = value;
                    return;
                }

                if (SizeGet1 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Size1 = value;
                }

            }
        }

    }
}