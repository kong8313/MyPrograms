using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using Confirmit.CATI.Common.Exceptions;

using TimeoutException = System.TimeoutException;

namespace Confirmit.CATI.WindowsServiceTools
{
    public class WinServiceTools : IDisposable
    {
        private const int DefaultServiceStartTimeout = 60;
        private const int DefaultServiceStopTimeout = 300;

        private readonly SafeServiceHandle _scManager;

        public WinServiceTools()
        {
            _scManager = Win32Api.OpenSCManager(Environment.MachineName, null, Win32Api.ScmAccess.SCManagerAllAccess);

            if (_scManager.IsInvalid)
            {
                throw new InternalErrorException(string.Format("Cannot open SCManager: {0}", Marshal.GetLastWin32Error()));
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scManager.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        } 

        
        public void RegisterService(
            string name,
            string displayName,
            string pathToBinary,
            string commandLine)
        {
            IntPtr actionsPtr = IntPtr.Zero;
            IntPtr failureActionsPtr = IntPtr.Zero;

            try
            {
                using (SafeServiceHandle service = Win32Api.CreateService(
                    _scManager,
                    name,
                    displayName,
                    Win32Api.ServiceAccess.ServiceAllAccess,
                    Win32Api.ServiceWin32OwnProcess,
                    Win32Api.ServiceAutoStart,
                    Win32Api.ServiceErrorNormal,
                    pathToBinary + " " + commandLine,
                    null,
                    0,
                    null,
                    null,
                    null))
                {
                    if (service.IsInvalid)
                    {
                        throw new InternalErrorException(string.Format("Cannot create service {0}. Error {1}", name, Marshal.GetLastWin32Error()));
                    }

                    //
                    // See http://blogs.msdn.com/anlynes/archive/2006/07/30/Using-.NET-Code-to-Set-a-Windows-Service-to-Automatically-Restart-on-Failure.aspx
                    // See http://blogs.msdn.com/anlynes/attachment/683192.ashx
                    //
                    actionsPtr = Marshal.AllocHGlobal((Marshal.SizeOf(typeof(Win32Api.SCAction)) * 3));
                    failureActionsPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32Api.ServiceFailureActions)));

                    var failureActions = new Win32Api.ServiceFailureActions
                    {
                        dwResetPeriod = 0,
                        lpRebootMsg = null,
                        lpCommand = null,
                        cActions = 3,
                        lpsaActions = actionsPtr
                    };

                    var failureAction = new Win32Api.SCAction
                    {
                        Type = Win32Api.SCActionType.SCActionRestart,
                        Delay = Win32Api.FailureRestartDelay
                    };

                    Marshal.StructureToPtr(failureAction, (IntPtr)((Int64)actionsPtr + Marshal.SizeOf(typeof(Win32Api.SCAction)) * 0), false);

                    Marshal.StructureToPtr(failureAction, (IntPtr)((Int64)actionsPtr + Marshal.SizeOf(typeof(Win32Api.SCAction)) * 1), false);

                    Marshal.StructureToPtr(failureAction, (IntPtr)((Int64)actionsPtr + Marshal.SizeOf(typeof(Win32Api.SCAction)) * 2), false);

                    Marshal.StructureToPtr(failureActions, failureActionsPtr, false);

                    // Make the change
                    int changeResult = Win32Api.ChangeServiceConfig2(service, Win32Api.ServiceConfig2InfoLevel.ServiceConfigFailureActions, failureActionsPtr);

                    // Check that the change occurred
                    if (changeResult == 0)
                    {
                        throw new InternalErrorException(string.Format("Cannot set service {0} failure actions. Error {1}", name, Marshal.GetLastWin32Error()));
                    }

                    var delayedStartInfo = new Win32Api.ServiceDelayedAutoStartInfo() { fDelayedAutostart = true };
                    var delayedStartInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Win32Api.ServiceDelayedAutoStartInfo)));
                    Marshal.StructureToPtr(delayedStartInfo, delayedStartInfoPtr, true);

                    // Make the change
                    changeResult = Win32Api.ChangeServiceConfig2(service, Win32Api.ServiceConfig2InfoLevel.ServiceConfigDelayedAutoStartInfo, delayedStartInfoPtr);

                    if (changeResult == 0)
                    {
                        throw new InternalErrorException(string.Format("Cannot set service {0} delayed start. Error {1}", name, Marshal.GetLastWin32Error()));
                    }
                }
            }
            finally
            {
                if (actionsPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(actionsPtr);
                }

                if (failureActionsPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(failureActionsPtr);
                }
            }
        }

        private static void Log(TraceEventType type, string message)
        {
            Console.WriteLine(message);

            switch (type)
            {
                case TraceEventType.Error:
                    Trace.TraceError(message);
                    break;
                case TraceEventType.Warning:
                    Trace.TraceWarning(message);
                    break;
                default:
                    Trace.TraceInformation(message);
                    break;
            }
        }

        public void UnregisterService(string serviceName)
        {
            int tryCnt = 1;
            var timer = new Stopwatch();
            do
            {
                try
                {
                    timer = Stopwatch.StartNew();
                    Log(TraceEventType.Information,$"Start removing service {serviceName}");

                    StopService(serviceName);
                    Log(TraceEventType.Information, $"Service {serviceName} was stopped successfully");

                    using (SafeServiceHandle service =
                        Win32Api.OpenService(_scManager, serviceName, Win32Api.ServiceAccess.ServiceAllAccess))
                    {
                        if (service.IsInvalid)
                        {
                            throw new InternalErrorException(string.Format("Cannot open service '{0}': {1}",
                                serviceName,
                                Marshal.GetLastWin32Error()));
                        }

                        if (!Win32Api.DeleteService(service))
                        {
                            throw new InternalErrorException(string.Format("Cannot delete service '{0}': {1}",
                                serviceName,
                                Marshal.GetLastWin32Error()));
                        }
                    }

                    WaitUntilServiceIsRemoved(serviceName);
                    
                    Log(TraceEventType.Information, $"Service {serviceName} was removed successfully in {timer.ElapsedMilliseconds}ms");

                    break;
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Warning, $"Attempt #{tryCnt} to remove service '{serviceName}' failed in {timer.ElapsedMilliseconds}ms with error: {ex}");
                    tryCnt++;
                }
            } 
            while (tryCnt < 4);

            if (tryCnt == 4)
            {
                throw new InternalErrorException($"Cannot completely remove service '{serviceName}': {Marshal.GetLastWin32Error()}");
            }
        }

        private void WaitUntilServiceIsRemoved(string serviceName)
        {
            const int maxSleepTime = 20000;
            const int sleepTime = 100;

            int waitTime = 0;
            do
            { 
                Thread.Sleep(sleepTime);
                waitTime += sleepTime;
            }
            while (waitTime < maxSleepTime && ServiceController.GetServices().Any(x => x.ServiceName == serviceName));

            if (waitTime == maxSleepTime)
            {
                Log(TraceEventType.Error, $"Service '{serviceName}' has not removed for {maxSleepTime} ms");
                throw new InternalErrorException(string.Format("Cannot completely remove service '{0}': {1}", serviceName, Marshal.GetLastWin32Error()));
            }

            Trace.TraceInformation("Waiting time after service removing: {0} ms", waitTime);
        }


        public int GetPIDByServiceName(string serviceName)
        {
            Trace.TraceInformation("get SQL server PID by service name");

            int pid;

            using (SafeServiceHandle service = Win32Api.OpenService(_scManager, serviceName, Win32Api.ServiceAccess.ServiceAllAccess))
            {
                if (service.IsInvalid)
                {
                    throw new InternalErrorException(string.Format("Cannot open service: {0}", Marshal.GetLastWin32Error()));
                }

                var info = new Win32Api.ServiceStatusProcess();

                if (!Win32Api.QueryServiceStatusEx(service, ref info))
                {
                    throw new InternalErrorException(string.Format("Cannot QueryServiceStatusEx: {0}", Marshal.GetLastWin32Error()));
                }

                pid = info.dwProcessId;
            }

            return pid;
        }


        /// <summary>
        /// Get service executable path
        /// </summary>
        /// <param name="serviceName">Service name</param>
        /// <returns></returns>
        public string GetServiceExecutablePath(string serviceName)
        {
            if (!IsServiceExist(serviceName))
            {
                throw new InternalErrorException("Service " + serviceName + " not exist");
            }

            var qscPtr = IntPtr.Zero;

            try
            {
                using (SafeServiceHandle service = Win32Api.OpenService(_scManager, serviceName, Win32Api.ServiceAccess.ServiceAllAccess))
                {
                    if (service.IsInvalid)
                    {
                        throw new InternalErrorException(string.Format("Cannot open service: {0}", Marshal.GetLastWin32Error()));
                    }

                    int bytesNeeded = 0;
                    int retCode = Win32Api.QueryServiceConfig(service, qscPtr, 0, ref bytesNeeded);
                    if (retCode == 0 && bytesNeeded == 0)
                    {
                        throw new InternalErrorException(string.Format("Cannot execute QueryServiceConfig operation: {0}", Marshal.GetLastWin32Error()));
                    }

                    qscPtr = Marshal.AllocCoTaskMem(bytesNeeded);
                    retCode = Win32Api.QueryServiceConfig(service, qscPtr, bytesNeeded, ref bytesNeeded);
                    if (retCode == 0)
                    {
                        throw new InternalErrorException(string.Format("Cannot execute QueryServiceConfig operation: {0}", Marshal.GetLastWin32Error()));
                    }

                    var qscs = (Win32Api.QueryServiceConfigStruct)Marshal.PtrToStructure(
                        qscPtr,
                        new Win32Api.QueryServiceConfigStruct().GetType());

                    return Marshal.PtrToStringAuto(qscs.BinaryPathName);
                }
            }
            finally
            {
                if (qscPtr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(qscPtr);
                }
            }
        }


        /// <summary>
        /// Set service executable path
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="executablePath"></param>
        public void SetServiceExecutablePath(string serviceName, string executablePath)
        {
            if (!IsServiceExist(serviceName))
            {
                return;
            }

            if (!File.Exists(executablePath.Replace("\"", "")))
            {
                return;
            }

            using (SafeServiceHandle service = Win32Api.OpenService(_scManager, serviceName, Win32Api.ServiceAccess.ServiceAllAccess))
            {
                if (service.IsInvalid)
                {
                    throw new InternalErrorException(string.Format("Cannot open service: {0}", Marshal.GetLastWin32Error()));
                }

                int retCode = Win32Api.ChangeServiceConfig(service, Win32Api.ServiceNoChange, Win32Api.ServiceNoChange, Win32Api.ServiceNoChange, executablePath, null, IntPtr.Zero, null, null, null, null);
                if (retCode == 0)
                {
                    throw new InternalErrorException(string.Format("Cannot execute QueryServiceConfig operation: {0}", Marshal.GetLastWin32Error()));
                }
            }
        }

        public bool IsServiceMarkedForDelete(string serviceName)
        {
            using (SafeServiceHandle service = Win32Api.OpenService(_scManager, serviceName, Win32Api.ServiceAccess.ServiceAllAccess))
            {
                int res = Win32Api.ChangeServiceConfig(service, Win32Api.ServiceNoChange, Win32Api.ServiceNoChange, Win32Api.ServiceNoChange, null, null, IntPtr.Zero, null, null, null, null);
                if (res != 0)
                {
                    return false;
                }

                res = Marshal.GetLastWin32Error();
                if (res == 1072)
                {
                    return true;
                }

                throw new Exception("An error occures during work with '" + serviceName + "' service.\r\nFunction name: ChangeServiceConfig\r\nAn error code: " + res);
            }
        }

        /// <summary>
        /// Set service startup type (disabled, manual and etc.)
        /// </summary>
        /// <param name="serviceName">Short service name</param>
        /// <param name="type">Startup type</param>
        public void ChangeServiceStartup(string serviceName, int type)
        {
            using (SafeServiceHandle service = Win32Api.OpenService(_scManager, serviceName, Win32Api.ServiceAccess.ServiceChangeConfig))
            {
                if (Win32Api.ChangeServiceConfig(service, Win32Api.ServiceNoChange, type, Win32Api.ServiceNoChange, null, null, IntPtr.Zero, null, null, null, null) == 0)
                {
                    throw new InternalErrorException(string.Format("ChangeServiceConfig failed {0}", Marshal.GetLastWin32Error()));
                }
            }
        }

        public ServiceControllerStatus GetServiceStatus(string serviceName)
        {
            using (ServiceController services = ServiceController.GetServices().First(x => x.ServiceName == serviceName))
            {
                return services.Status;
            }
        }

        public void EnableService(string serviceName)
        {
            ChangeServiceStartup(serviceName, Win32Api.ServiceAutoStart);
        }

        public void DisableService(string serviceName)
        {
            ChangeServiceStartup(serviceName, Win32Api.ServiceDisabled);
        }

        /// <summary>
        /// Return true if service exist, false - otherwise
        /// </summary>
        /// <param name="serviceName">Short service name</param>
        /// <returns></returns>
        public static bool IsServiceExist(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            return services.Any(sc => sc.DisplayName == serviceName);
        }


        public static void StopService(string serviceName)
        {
            StopService(serviceName, DefaultServiceStopTimeout);
        }

        public static void StopService(string serviceName, int serviceStopTimeout)
        {
            using (var scm = new ServiceController(serviceName))
            {
                Log(TraceEventType.Information, $"Service {serviceName} has status {scm.Status}");

                if (scm.Status == ServiceControllerStatus.Stopped)
                {
                    return;
                }

                if (scm.Status != ServiceControllerStatus.StartPending)
                {
                    scm.Refresh();
                    scm.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(DefaultServiceStartTimeout));
                }

                if (scm.Status != ServiceControllerStatus.StopPending)
                {
                    scm.Stop();
                }

                try
                {
                    scm.Refresh();
                    scm.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(serviceStopTimeout));
                }
                catch (TimeoutException ex)
                {
                    throw new InternalErrorException(string.Format("Timeout expired while waiting for service {0} stop. Exception {1}", serviceName, ex));
                }
            }
        }

        /// <summary>
        /// Synchronously starts the service with the specified name. 
        /// Does not return control until service is started.
        /// </summary>
        /// <param name="serviceName">Name of the service to start.</param>
        public static void StartService(string serviceName)
        {
            StartServices(new[] { serviceName }, DefaultServiceStartTimeout, false);
        }

        /// <summary>
        /// Synchronously starts the service with the specified name. 
        /// Does not return control until service is started.
        /// </summary>
        /// <param name="serviceName">Name of the service to start.</param>
        /// <param name="serviceStartTimeout">Service stop timeout</param>
        public static void StartService(string serviceName, int serviceStartTimeout)
        {
            StartServices(new[] { serviceName }, serviceStartTimeout, false);
        }

        /// <summary>
        /// Synchronously starts the service with the specified name. 
        /// Does not return control until service is started.
        /// </summary>
        /// <param name="serviceName">Name of the service to start.</param>
        /// <param name="serviceStartTimeout">Service stop timeout</param>
        /// <param name="isThrowErrors">if true - throw exceptions up</param>
        public static void StartService(string serviceName, int serviceStartTimeout, bool isThrowErrors)
        {
            StartServices(new[] { serviceName }, serviceStartTimeout, isThrowErrors);
        }

        /// <summary>
        /// Synchronously starts services with the specified names. 
        /// Does not return control until all services are started.
        /// </summary>
        /// <param name="serviceNames">Names of services to start.</param>
        public static void StartServices(string[] serviceNames)
        {
            StartServices(serviceNames, DefaultServiceStartTimeout, false);
        }

        /// <summary>
        /// Synchronously starts services with the specified names. 
        /// Does not return control until all services are started.
        /// </summary>
        /// <param name="serviceNames">Names of services to start.</param>
        /// <param name="serviceStartTimeout">Service stop timeout</param>
        public static void StartServices(string[] serviceNames, int serviceStartTimeout)
        {
            StartServices(serviceNames, serviceStartTimeout, false);
        }
        
        /// <summary>
        /// Synchronously starts services with the specified names. 
        /// Does not return control until all services are started.
        /// </summary>
        /// <param name="serviceNames">Names of services to start.</param>
        /// <param name="serviceStartTimeout">Service stop timeout</param>
        /// <param name="isThrowErrors">if true - throw exceptions up</param>
        public static void StartServices(string[] serviceNames, int serviceStartTimeout, bool isThrowErrors)
        {
            //
            // start services
            foreach (string serviceName in serviceNames)
            {
                try
                {
                    StartServiceNoWait(serviceName);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Service {0} failed to start: {1}", serviceName, ex);
                    if (isThrowErrors)
                    {
                        throw;
                    }
                }
            }

            //
            // wait while all services started
            foreach (string serviceName in serviceNames)
            {
                try
                {
                    WaitServiceStarted(serviceName, serviceStartTimeout);

                    Trace.TraceInformation("Service {0} started successfully.", serviceName);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());

                    if (isThrowErrors)
                    {
                        throw;
                    }
                }
            }
        }

        
        private static void StartServiceNoWait(string serviceName)
        {
            using (var scm = new ServiceController(serviceName))
            {
                if (scm.Status == ServiceControllerStatus.Running || scm.Status == ServiceControllerStatus.StartPending)
                    return;

                scm.Start();
            }
        }


        private static void WaitServiceStarted(string serviceName, int serviceStartTimeout)
        {
            using (var scm = new ServiceController(serviceName))
            {
                int waitAttempts = serviceStartTimeout;

                do
                {
                    //
                    // Verify service state every second in cycle.
                    //
                    try
                    {
                        scm.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(1));
                    }
                    catch (System.ServiceProcess.TimeoutException) { }

                    scm.Refresh();

                    if (scm.Status == ServiceControllerStatus.Running)
                    {
                        // Everything is fine. Service started, we can go out.
                        return;
                    }

                    if (scm.Status == ServiceControllerStatus.Stopped)
                    {
                        //
                        // Looks like service failed to start.
                        //
                        throw new InternalErrorException(string.Format("Service {0} failed to start and switched to the stopped state.", serviceName));
                    }
                }
                while (--waitAttempts != 0);

                throw new InternalErrorException(string.Format("Timeout expired while waiting for service {0} run.", serviceName));
            }
        }
    }
}
