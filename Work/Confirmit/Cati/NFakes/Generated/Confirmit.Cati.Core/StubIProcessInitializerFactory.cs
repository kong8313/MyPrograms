using System;
using Confirmit.CATI.Core.AsynchronousTrigger.ProcessInitializers.Factory;
using Confirmit.CATI.Core.AsynchronousTrigger.ProcessInitializers;

namespace Confirmit.CATI.Core.AsynchronousTrigger.ProcessInitializers.Factory.Fakes
{
    public class StubIProcessInitializerFactory : IProcessInitializerFactory 
    {
        private IProcessInitializerFactory _inner;

        public StubIProcessInitializerFactory()
        {
            _inner = null;
        }

        public IProcessInitializerFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IProcessInitializer CreateDelegate();
        public CreateDelegate Create;

        IProcessInitializer IProcessInitializerFactory.Create()
        {


            if (Create != null)
            {
                return Create();
            } else if (_inner != null)
            {
                return ((IProcessInitializerFactory)_inner).Create();
            }

            return default(IProcessInitializer);
        }

    }
}