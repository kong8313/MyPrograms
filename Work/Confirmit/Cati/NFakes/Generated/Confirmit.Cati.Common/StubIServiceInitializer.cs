using System;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Common.ServiceLocation.Fakes
{
    public class StubIServiceInitializer : IServiceInitializer 
    {
        private IServiceInitializer _inner;

        public StubIServiceInitializer()
        {
            _inner = null;
        }

        public IServiceInitializer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeDelegate();
        public InitializeDelegate Initialize;

        void IServiceInitializer.Initialize()
        {

            if (Initialize != null)
            {
                Initialize();
            } else if (_inner != null)
            {
                ((IServiceInitializer)_inner).Initialize();
            }
        }

        public delegate void CleanupDelegate();
        public CleanupDelegate Cleanup;

        void IServiceInitializer.Cleanup()
        {

            if (Cleanup != null)
            {
                Cleanup();
            } else if (_inner != null)
            {
                ((IServiceInitializer)_inner).Cleanup();
            }
        }

    }
}