using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIActiveSupervisorService : IActiveSupervisorService 
    {
        private IActiveSupervisorService _inner;

        public StubIActiveSupervisorService()
        {
            _inner = null;
        }

        public IActiveSupervisorService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int CleanActiveSupervisorsTimeSpanDelegate(TimeSpan expirationTime);
        public CleanActiveSupervisorsTimeSpanDelegate CleanActiveSupervisorsTimeSpan;

        int IActiveSupervisorService.CleanActiveSupervisors(TimeSpan expirationTime)
        {


            if (CleanActiveSupervisorsTimeSpan != null)
            {
                return CleanActiveSupervisorsTimeSpan(expirationTime);
            } else if (_inner != null)
            {
                return ((IActiveSupervisorService)_inner).CleanActiveSupervisors(expirationTime);
            }

            return default(int);
        }

    }
}