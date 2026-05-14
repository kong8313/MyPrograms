using System;
using Confirmit.CATI.Supervisor.Reports;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Reports.Fakes
{
    public class StubIInboundHandlerOperationsProvider : IInboundHandlerOperationsProvider 
    {
        private IInboundHandlerOperationsProvider _inner;

        public StubIInboundHandlerOperationsProvider()
        {
            _inner = null;
        }

        public IInboundHandlerOperationsProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<InboundHandlerOperation> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<InboundHandlerOperation> IInboundHandlerOperationsProvider.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((IInboundHandlerOperationsProvider)_inner).GetAll();
            }

            return default(List<InboundHandlerOperation>);
        }

    }
}