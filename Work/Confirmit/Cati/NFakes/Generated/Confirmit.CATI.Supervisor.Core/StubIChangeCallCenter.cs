using System;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Supervisor.Core.CallCenters.Fakes
{
    public class StubIChangeCallCenter : IChangeCallCenter 
    {
        private IChangeCallCenter _inner;

        public StubIChangeCallCenter()
        {
            _inner = null;
        }

        public IChangeCallCenter Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ChangeInt32Delegate(int callCenterId);
        public ChangeInt32Delegate ChangeInt32;

        void IChangeCallCenter.Change(int callCenterId)
        {

            if (ChangeInt32 != null)
            {
                ChangeInt32(callCenterId);
            } else if (_inner != null)
            {
                ((IChangeCallCenter)_inner).Change(callCenterId);
            }
        }

    }
}