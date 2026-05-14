using System;
using System.Diagnostics;
using System.ServiceProcess;

using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.CATI.Core.Threading;

namespace Confirmit.CATI.Backend.Threads
{
    public class InstanceMonitoringThread : PeriodicalThread
    {
        public InstanceMonitoringThread()
            : base("InstanceMonitoringThread")
        {
        }

        public override TimeSpan StopTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(30);
            }
        }

        public override TimeSpan SleepTimeout
        {
            get
            {
                return TimeSpan.FromSeconds(60);
            }
        }

        protected override void DoWork(object parameter)
        {
            // For comments see InstanceManagementService.RegisterSchedulingServiceInstance
            lock (InstanceManagementLock.lockObject)
            {
                foreach (ServiceController service in ServiceController.GetServices())
                {
                    if (service.ServiceName.StartsWith(SideBySideManager.ServicePrefix))
                    {
                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            Trace.TraceError(
                                "{0} service is not started. Service status is {1}",
                                service.DisplayName,
                                service.Status.ToString());

                            if (service.Status == ServiceControllerStatus.Stopped)
                            {
                                try
                                {
                                    service.Start();
                                }
                                catch (Exception ex)
                                {
                                    Trace.TraceError(
                                   "Attempt to start service {0} has failed with message\r\n{1}",
                                   service.DisplayName,
                                   ex);
                                }
                            }
                        }
                    }
                }
            }
        } // lock (InstanceManagementLock.lockObject)
    }
}
