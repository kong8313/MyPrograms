using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.BvCallHandlerLibrary;
using BvCallHandlerLibrary;

namespace Confirmit.CATI.Core.BvCallHandlerLibrary.Fakes
{
    public class StubIDialerInstanceFactory : IDialerInstanceFactory 
    {
        private IDialerInstanceFactory _inner;

        public StubIDialerInstanceFactory()
        {
            _inner = null;
        }

        public IDialerInstanceFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDialerInstance CreateBvDialersEntityDelegate(BvDialersEntity dialerEntity);
        public CreateBvDialersEntityDelegate CreateBvDialersEntity;

        IDialerInstance IDialerInstanceFactory.Create(BvDialersEntity dialerEntity)
        {


            if (CreateBvDialersEntity != null)
            {
                return CreateBvDialersEntity(dialerEntity);
            } else if (_inner != null)
            {
                return ((IDialerInstanceFactory)_inner).Create(dialerEntity);
            }

            return default(IDialerInstance);
        }

    }
}