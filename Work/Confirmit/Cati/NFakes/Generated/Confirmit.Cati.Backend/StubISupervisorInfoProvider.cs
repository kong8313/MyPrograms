using System;
using Confirmit.CATI.Backend.WebApiServices;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubISupervisorInfoProvider : ISupervisorInfoProvider 
    {
        private ISupervisorInfoProvider _inner;

        public StubISupervisorInfoProvider()
        {
            _inner = null;
        }

        public ISupervisorInfoProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CatiSupervisorInfo GetInfoDelegate();
        public GetInfoDelegate GetInfo;

        CatiSupervisorInfo ISupervisorInfoProvider.GetInfo()
        {


            if (GetInfo != null)
            {
                return GetInfo();
            } else if (_inner != null)
            {
                return ((ISupervisorInfoProvider)_inner).GetInfo();
            }

            return default(CatiSupervisorInfo);
        }

    }
}