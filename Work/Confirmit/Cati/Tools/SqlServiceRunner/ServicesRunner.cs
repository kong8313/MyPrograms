using System;
using System.ServiceProcess;

namespace SqlServiceRunner
{
    public class ServicesRunner
    {
        private readonly Logger _logger;

        public ServicesRunner(Logger logger)
        {
            _logger = logger;
        }

        public void VerifyAndRun()
        {
            string serviceName = "MSSQL";
            string serviceAgentName = "SQLAgent";

            foreach (var serviceController in ServiceController.GetServices())
            {
                if (serviceController.ServiceName.StartsWith(serviceName) ||
                    serviceController.ServiceName.StartsWith(serviceAgentName))
                {
                    if (serviceController.Status == ServiceControllerStatus.Stopped &&
                        serviceController.StartType == ServiceStartMode.Automatic)
                    {
                        try
                        {
                            _logger.WriteLog($"Starting service {serviceController.DisplayName}");
                            serviceController.Start();
                            serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                            _logger.WriteLog($"Service {serviceController.DisplayName} was started successfully");
                        }
                        catch (Exception ex)
                        {
                            _logger.WriteLog($"Error starting service {serviceController.DisplayName}: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}