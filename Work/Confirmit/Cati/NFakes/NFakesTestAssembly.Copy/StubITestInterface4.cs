using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface4 : ITestInterface4 
    {
        private ITestInterface4 _inner;

        public StubITestInterface4()
        {
            _inner = null;
        }

        public ITestInterface4 Inner
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

        public delegate void Foo2Delegate1();
        public Foo2Delegate1 Foo21;

        void ITestInterface3.Foo2()
        {

            if (Foo21 != null)
            {
                Foo21();
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