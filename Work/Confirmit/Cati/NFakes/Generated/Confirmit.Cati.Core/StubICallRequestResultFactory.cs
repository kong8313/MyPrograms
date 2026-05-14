using System;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;

namespace Confirmit.CATI.Core.Services.CallDelivery.Interfaces.Fakes
{
    public class StubICallRequestResultFactory : ICallRequestResultFactory 
    {
        private ICallRequestResultFactory _inner;

        public StubICallRequestResultFactory()
        {
            _inner = null;
        }

        public ICallRequestResultFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CallRequestResult CreateILookupCallEntityDelegate(ILookupCallEntity call);
        public CreateILookupCallEntityDelegate CreateILookupCallEntity;

        CallRequestResult ICallRequestResultFactory.Create(ILookupCallEntity call)
        {


            if (CreateILookupCallEntity != null)
            {
                return CreateILookupCallEntity(call);
            } else if (_inner != null)
            {
                return ((ICallRequestResultFactory)_inner).Create(call);
            }

            return default(CallRequestResult);
        }

    }
}