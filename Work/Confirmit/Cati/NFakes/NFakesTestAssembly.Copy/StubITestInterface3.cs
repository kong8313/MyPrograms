using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface3 : ITestInterface3 
    {
        private ITestInterface3 _inner;

        public StubITestInterface3()
        {
            _inner = null;
        }

        public ITestInterface3 Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void Foo2Delegate();
        public Foo2Delegate Foo2;

        void ITestInterface3.Foo2()
        {

            if (Foo2 != null)
            {
                Foo2();
            } else if (_inner != null)
            {
                ((ITestInterface3)_inner).Foo2();
            }
        }

        public delegate void Foo3Delegate();
        public Foo3Delegate Foo3;

        void ITestInterface3.Foo3()
        {

            if (Foo3 != null)
            {
                Foo3();
            } else if (_inner != null)
            {
                ((ITestInterface3)_inner).Foo3();
            }
        }

    }
}