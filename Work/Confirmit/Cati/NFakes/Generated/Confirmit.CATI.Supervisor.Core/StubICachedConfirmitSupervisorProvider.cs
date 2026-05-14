using System;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using System.Collections.Generic;
using Confirmit.CATI.Core.AuthoringService;

namespace Confirmit.CATI.Supervisor.Core.CallCenters.Fakes
{
    public class StubICachedConfirmitSupervisorProvider : ICachedConfirmitSupervisorProvider 
    {
        private ICachedConfirmitSupervisorProvider _inner;

        public StubICachedConfirmitSupervisorProvider()
        {
            _inner = null;
        }

        public ICachedConfirmitSupervisorProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<CatiSupervisor> GetConfirmitCatiSupervisorsDelegate();
        public GetConfirmitCatiSupervisorsDelegate GetConfirmitCatiSupervisors;

        IEnumerable<CatiSupervisor> ICachedConfirmitSupervisorProvider.GetConfirmitCatiSupervisors()
        {


            if (GetConfirmitCatiSupervisors != null)
            {
                return GetConfirmitCatiSupervisors();
            } else if (_inner != null)
            {
                return ((ICachedConfirmitSupervisorProvider)_inner).GetConfirmitCatiSupervisors();
            }

            return default(IEnumerable<CatiSupervisor>);
        }

        public delegate void ClearCacheDelegate();
        public ClearCacheDelegate ClearCache;

        void ICachedConfirmitSupervisorProvider.ClearCache()
        {

            if (ClearCache != null)
            {
                ClearCache();
            } else if (_inner != null)
            {
                ((ICachedConfirmitSupervisorProvider)_inner).ClearCache();
            }
        }

    }
}