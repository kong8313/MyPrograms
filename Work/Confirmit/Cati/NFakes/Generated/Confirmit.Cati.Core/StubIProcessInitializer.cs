using System;
using Confirmit.CATI.Core.AsynchronousTrigger.ProcessInitializers;

namespace Confirmit.CATI.Core.AsynchronousTrigger.ProcessInitializers.Fakes
{
    public class StubIProcessInitializer : IProcessInitializer 
    {
        private IProcessInitializer _inner;

        public StubIProcessInitializer()
        {
            _inner = null;
        }

        public IProcessInitializer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeDelegate();
        public InitializeDelegate Initialize;

        void IProcessInitializer.Initialize()
        {

            if (Initialize != null)
            {
                Initialize();
            } else if (_inner != null)
            {
                ((IProcessInitializer)_inner).Initialize();
            }
        }

        public delegate void UninitializeDelegate();
        public UninitializeDelegate Uninitialize;

        void IProcessInitializer.Uninitialize()
        {

            if (Uninitialize != null)
            {
                Uninitialize();
            } else if (_inner != null)
            {
                ((IProcessInitializer)_inner).Uninitialize();
            }
        }

    }
}