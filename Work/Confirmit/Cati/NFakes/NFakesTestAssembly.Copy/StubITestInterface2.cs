using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface2 : ITestInterface2 
    {
        private ITestInterface2 _inner;

        public StubITestInterface2()
        {
            _inner = null;
        }

        public ITestInterface2 Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void Foo1Delegate();
        public Foo1Delegate Foo1;

        void ITestInterface2.Foo1()
        {

            if (Foo1 != null)
            {
                Foo1();
            } else if (_inner != null)
            {
                ((ITestInterface2)_inner).Foo1();
            }
        }

        public delegate void Foo2Delegate();
        public Foo2Delegate Foo2;

        void ITestInterface2.Foo2()
        {

            if (Foo2 != null)
            {
                Foo2();
            } else if (_inner != null)
            {
                ((ITestInterface2)_inner).Foo2();
            }
        }

    }
}