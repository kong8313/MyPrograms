using System;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling.Fakes
{
    public class StubIIntersectable<T> : IIntersectable<T> 
    {
        private IIntersectable<T> _inner;

        public StubIIntersectable()
        {
            _inner = null;
        }

        public IIntersectable<T> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool HasIntersectionTDelegate(T obj);
        public HasIntersectionTDelegate HasIntersectionT;

        bool IIntersectable<T>.HasIntersection(T obj)
        {


            if (HasIntersectionT != null)
            {
                return HasIntersectionT(obj);
            } else if (_inner != null)
            {
                return ((IIntersectable<T>)_inner).HasIntersection(obj);
            }

            return default(bool);
        }

    }
}