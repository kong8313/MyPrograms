using System;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Common.ServiceLocation.Fakes
{
    public class StubIServiceResolver : IServiceResolver 
    {
        private IServiceResolver _inner;

        public StubIServiceResolver()
        {
            _inner = null;
        }

        public IServiceResolver Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        T IServiceResolver.Resolve<T>()
        {


            return default(T);
        }

    }
}