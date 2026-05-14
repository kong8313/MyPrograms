using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface10 : ITestInterface10 
    {
        private ITestInterface10 _inner;

        public StubITestInterface10()
        {
            _inner = null;
        }

        public ITestInterface10 Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Parent.Nested FooDelegate();
        public FooDelegate Foo;

        Parent.Nested ITestInterface10.Foo()
        {


            if (Foo != null)
            {
                return Foo();
            } else if (_inner != null)
            {
                return ((ITestInterface10)_inner).Foo();
            }

            return default(Parent.Nested);
        }

    }
}