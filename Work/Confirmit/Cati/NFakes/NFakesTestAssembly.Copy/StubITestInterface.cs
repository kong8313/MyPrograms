using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface<T1> : ITestInterface<T1> 
    {
        private ITestInterface<T1> _inner;

        public StubITestInterface()
        {
            _inner = null;
        }

        public ITestInterface<T1> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate T1 FooDelegate();
        public FooDelegate Foo;

        T1 ITestInterface<T1>.Foo()
        {


            if (Foo != null)
            {
                return Foo();
            } else if (_inner != null)
            {
                return ((ITestInterface<T1>)_inner).Foo();
            }

            return default(T1);
        }

    }
}