using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface9 : ITestInterface9 
    {
        private ITestInterface9 _inner;

        public StubITestInterface9()
        {
            _inner = null;
        }

        public ITestInterface9 Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void FooStringDelegate(string @event);
        public FooStringDelegate FooString;

        void ITestInterface9.Foo(string @event)
        {

            if (FooString != null)
            {
                FooString(@event);
            } else if (_inner != null)
            {
                ((ITestInterface9)_inner).Foo(@event);
            }
        }

    }
}