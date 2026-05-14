using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface6<T1, T2> : ITestInterface6<T1, T2>  where T1 : ICloneable, ITestInterface<T1>, new()  where T2 : new() 
    {
        private ITestInterface6<T1, T2> _inner;

        public StubITestInterface6()
        {
            _inner = null;
        }

        public ITestInterface6<T1, T2> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void BarDelegate();
        public BarDelegate Bar;

        void ITestInterface6<T1, T2>.Bar()
        {

            if (Bar != null)
            {
                Bar();
            } else if (_inner != null)
            {
                ((ITestInterface6<T1, T2>)_inner).Bar();
            }
        }

    }
}