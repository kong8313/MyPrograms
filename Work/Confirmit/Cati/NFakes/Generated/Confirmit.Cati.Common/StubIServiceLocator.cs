using System;
using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Common.ServiceLocation.Fakes
{
    public class StubIServiceLocator : IServiceLocator 
    {
        private IServiceLocator _inner;

        public StubIServiceLocator()
        {
            _inner = null;
        }

        public IServiceLocator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IServiceResolver CreateChildContainerDelegate();
        public CreateChildContainerDelegate CreateChildContainer;

        IServiceResolver IServiceLocator.CreateChildContainer()
        {


            if (CreateChildContainer != null)
            {
                return CreateChildContainer();
            } else if (_inner != null)
            {
                return ((IServiceLocator)_inner).CreateChildContainer();
            }

            return default(IServiceResolver);
        }

    }
}