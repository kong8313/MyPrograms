using System;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Supervisor.Core.CallCenters.Fakes
{
    public class StubICallCenterProvider : ICallCenterProvider 
    {
        private ICallCenterProvider _inner;

        public StubICallCenterProvider()
        {
            _inner = null;
        }

        public ICallCenterProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int GetCurrentIdDelegate();
        public GetCurrentIdDelegate GetCurrentId;

        int ICallCenterProvider.GetCurrentId()
        {


            if (GetCurrentId != null)
            {
                return GetCurrentId();
            } else if (_inner != null)
            {
                return ((ICallCenterProvider)_inner).GetCurrentId();
            }

            return default(int);
        }

        public delegate BvCallCenterEntity GetCurrentDelegate();
        public GetCurrentDelegate GetCurrent;

        BvCallCenterEntity ICallCenterProvider.GetCurrent()
        {


            if (GetCurrent != null)
            {
                return GetCurrent();
            } else if (_inner != null)
            {
                return ((ICallCenterProvider)_inner).GetCurrent();
            }

            return default(BvCallCenterEntity);
        }

    }
}