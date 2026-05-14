using System;
using Confirmit.CATI.Supervisor.Core.Surveys;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Surveys.Fakes
{
    public class StubICallOperationsProvider : ICallOperationsProvider 
    {
        private ICallOperationsProvider _inner;

        public StubICallOperationsProvider()
        {
            _inner = null;
        }

        public ICallOperationsProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<CallOperation> GetAllDelegate();
        public GetAllDelegate GetAll;

        List<CallOperation> ICallOperationsProvider.GetAll()
        {


            if (GetAll != null)
            {
                return GetAll();
            } else if (_inner != null)
            {
                return ((ICallOperationsProvider)_inner).GetAll();
            }

            return default(List<CallOperation>);
        }

    }
}