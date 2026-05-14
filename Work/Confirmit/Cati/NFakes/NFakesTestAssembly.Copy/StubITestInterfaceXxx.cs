using System;

namespace NFakesTestAssembly.Fakes
{
    public class StubITestInterfaceXxx : ITestInterfaceXxx 
    {
        private ITestInterfaceXxx _inner;

        public StubITestInterfaceXxx()
        {
            _inner = null;
        }

        public ITestInterfaceXxx Inner
        {
            set {_inner = value;} get {return _inner;}
        }

    }
}