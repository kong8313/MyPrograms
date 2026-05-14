using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface8<T> : ITestInterface8<T>  where T : class 
    {
        private ITestInterface8<T> _inner;

        public StubITestInterface8()
        {
            _inner = null;
        }

        public ITestInterface8<T> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate T Foo1Delegate();
        public Foo1Delegate Foo1;

        T ITestInterface8<T>.Foo1()
        {


            if (Foo1 != null)
            {
                return Foo1();
            } else if (_inner != null)
            {
                return ((ITestInterface8<T>)_inner).Foo1();
            }

            return default(T);
        }

    }
}