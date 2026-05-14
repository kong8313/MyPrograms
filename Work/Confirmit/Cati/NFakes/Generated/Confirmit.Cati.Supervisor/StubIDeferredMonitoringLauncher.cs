using System;
using Confirmit.CATI.Common.Monitoring;
using Confirmit.CATI.Supervisor.Core.DeferredMonitoring;

namespace Confirmit.CATI.Supervisor.Classes.DeferredMonitoring.Fakes
{
    public class StubIDeferredMonitoringLauncher : IDeferredMonitoringLauncher 
    {
        private IDeferredMonitoringLauncher _inner;

        public StubIDeferredMonitoringLauncher()
        {
            _inner = null;
        }

        public IDeferredMonitoringLauncher Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate MonitoringLaunchInfo GetMonitoringLaunchInfoInt32StringStringDelegate(int recordId, string initialQuestion, string userName);
        public GetMonitoringLaunchInfoInt32StringStringDelegate GetMonitoringLaunchInfoInt32StringString;

        MonitoringLaunchInfo IDeferredMonitoringLauncher.GetMonitoringLaunchInfo(int recordId, string initialQuestion, string userName)
        {


            if (GetMonitoringLaunchInfoInt32StringString != null)
            {
                return GetMonitoringLaunchInfoInt32StringString(recordId, initialQuestion, userName);
            } else if (_inner != null)
            {
                return ((IDeferredMonitoringLauncher)_inner).GetMonitoringLaunchInfo(recordId, initialQuestion, userName);
            }

            return default(MonitoringLaunchInfo);
        }

    }
}