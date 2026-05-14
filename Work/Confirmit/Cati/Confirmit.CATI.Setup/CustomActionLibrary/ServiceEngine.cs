using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.WindowsServiceTools;
using CustomActionLibrary.Properties;

namespace CustomActionLibrary
{
    public class ServiceEngine : IDisposable
    {
        private readonly ServiceController _serviceController;

        private readonly SetupEngine _setupEngine;

        private readonly WinServiceTools _winServiceTools;

        /// <summary>
        /// Max delay for stopping service, sec
        /// </summary>
        private readonly int _delay;

        public bool IsServiceMarkedForDelete
        {
            get
            {
                return _winServiceTools.IsServiceMarkedForDelete(_serviceController.ServiceName);
            }
        }

        /// <summary>
        /// Class for work with service
        /// </summary>
        /// <param name="setupEngine">SetupEngine object</param>
        /// <param name="serviceName">Selected service</param>
        public ServiceEngine(SetupEngine setupEngine, string serviceName) :
            this(setupEngine, serviceName, 180)
        {
        }


        /// <summary>
        /// Class for work with service
        /// </summary>
        /// <param name="setupEngine">SetupEngine</param>
        /// <param name="serviceName">Selected service</param>
        /// <param name="delay">Time for delay</param>
        public ServiceEngine(SetupEngine setupEngine, string serviceName, int delay)
        {
            _delay = delay;
            _serviceController = new ServiceController(serviceName, Environment.MachineName);
            _setupEngine = setupEngine;
            _winServiceTools = new WinServiceTools();
        }

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _serviceController.Dispose();
                _winServiceTools.Dispose();
            }

            _disposed = true;
        }


        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }


        /// <summary>
        /// Set service startup type (disabled, manual and etc.)
        /// </summary>
        /// <param name="type">Startup type</param>
        private void ChangeServiceStartup(int type)
        {
            _setupEngine.Logger.WriteLog("Begin ChangeServiceStatus");

            try
            {
                if (_winServiceTools.IsServiceMarkedForDelete(_serviceController.ServiceName))
                {
                    _setupEngine.Logger.WriteLog(_serviceController.DisplayName + " is marked for delete");
                    throw new Exception(Resources.MarkedForDeletionService);
                }

                _setupEngine.Logger.WriteLog("Run ChangeServiceConfig");
                _winServiceTools.ChangeServiceStartup(_serviceController.ServiceName, type);
                _serviceController.Refresh();
            }
            finally
            {
                _setupEngine.Logger.WriteLog("End ChangeServiceStatus");
            }
        }

        /// <summary>
        /// Disable service
        /// </summary>
        private void DisableService()
        {
            _setupEngine.Logger.WriteLog("Begin DisableService {0}", _serviceController.DisplayName);
            try
            {
                ChangeServiceStartup(Win32Api.ServiceDisabled);
            }
            finally
            {
                _setupEngine.Logger.WriteLog("End DisableService");
            }
        }


        /// <summary>
        /// Start service
        /// </summary>
        public void StartService()
        {
            _setupEngine.Logger.WriteLog("Begin StartService {0}", _serviceController.DisplayName);
            try
            {
                if (_winServiceTools.IsServiceMarkedForDelete(_serviceController.ServiceName))
                {
                    _setupEngine.Logger.WriteLog("This service is marked for delete");
                    throw new Exception(Resources.MarkedForDeletionService);
                }

                WinServiceTools.StartService(_serviceController.ServiceName, _delay, true);
            }
            finally
            {
                _setupEngine.Logger.WriteLog("End StartService");
            }
        }


        /// <summary>
        /// Stop service
        /// </summary>
        private void StopService()
        {
            _setupEngine.Logger.WriteLog("Begin StopService {0}", _serviceController.DisplayName);
            try
            {
                if (_winServiceTools.IsServiceMarkedForDelete(_serviceController.ServiceName))
                {
                    _setupEngine.Logger.WriteLog("This service is marked for delete");
                    throw new Exception(Resources.MarkedForDeletionService);
                }

                do
                {
                    try
                    {
                        WinServiceTools.StopService(_serviceController.ServiceName, _delay);
                    }
                    catch (Exception ex)
                    {
                        _setupEngine.Logger.WriteLog("An error occured during stopping of service:\r\n" + ex);

                        if (TopMostMessageBox.Show(string.Format(Resources.QuestionAboutWaitingOfNotStoppedService, _serviceController.DisplayName), "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DialogResult.No) == DialogResult.No)
                        {
                            break;
                        }
                    }

                    _serviceController.Refresh();
                }
                while (_serviceController.Status != ServiceControllerStatus.Stopped);

            }
            finally
            {
                _setupEngine.Logger.WriteLog("End StopService");
            }
        }


        /// <summary>
        /// Remove service
        /// </summary>
        public void RemoveService()
        {
            _setupEngine.Logger.WriteLog("Begin RemoveService {0}", _serviceController.DisplayName);

            try
            {
                DisableService();
                StopService();

                _setupEngine.Logger.WriteLog("Run DeleteService");
                _winServiceTools.UnregisterService(_serviceController.ServiceName);
            }
            finally
            {
                _setupEngine.Logger.WriteLog("End RemoveService");
            }
        }

        public void WaitUntilAllCatiServicesStart(string instanceCatiServiceNamePrefix)
        {
            _setupEngine.Logger.WriteLog("Begin WaitUntilServicesStarted. instanceCatiServiceNamePrefix={0}", instanceCatiServiceNamePrefix);
            const int maxWaitTime = 900;
            int waitTime = 0;

            try
            {
                bool needToWait = true;
                while (needToWait)
                {
                    Thread.Sleep(1000);

                    var catiServices = ServiceController.GetServices().Where(x => x.ServiceName.StartsWith(instanceCatiServiceNamePrefix)).ToList();
                    try
                    {
                        waitTime++;
                        if (waitTime >= maxWaitTime)
                        {
                            var notStartedServices = catiServices.Where(catiService => catiService.Status != ServiceControllerStatus.Running);
                            string notStartedServicesInfo = notStartedServices.Aggregate(string.Empty, (current, catiService) => current + ("\r\n" + catiService.ServiceName));
                            throw new Exception(string.Format("CATI services were not started during {0} min. Not runned services:{1}", maxWaitTime / 60, notStartedServicesInfo));
                        }

                        needToWait = catiServices.Any(x => x.Status != ServiceControllerStatus.Running);
                    }
                    finally
                    {
                        catiServices.ForEach(x => x.Dispose());
                    }
                }
            }
            finally
            {
                _setupEngine.Logger.WriteLog("End WaitUntilServicesStarted. maxWaitTime={0} sec, waitTime={1} sec", maxWaitTime, waitTime);
            }
        }
    }
}
