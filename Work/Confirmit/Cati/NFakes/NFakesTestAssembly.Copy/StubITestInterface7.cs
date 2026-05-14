using System;
using NFakesTestAssembly;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterface7<T1> : ITestInterface7<T1>  where T1 : struct 
    {
        private ITestInterface7<T1> _inner;

        public StubITestInterface7()
        {
            _inner = null;
        }

        public ITestInterface7<T1> Inner
        {
            set {_inner = value;} get {return _inner;}
        }

    }
}