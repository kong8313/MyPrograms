using System;
using System.Runtime.InteropServices;

namespace Confirmit.CATI.WindowsServiceTools
{
    public class Win32Api
    {
        //
        // constants taken from Platform SDK (WinSvc.h)
        public const Int32 ServiceWin32OwnProcess = 0x00000010;
        public const Int32 ServiceAutoStart = 0x00000002;
        public const Int32 ServiceDemandStart = 0x00000003;
        public const Int32 ServiceErrorNormal = 0x00000001;
        public const Int32 ServiceControlStop = 0x00000001;
        public const Int32 ServiceDisabled = 0x00000004;
        public const Int32 ServiceConfigFailureActions = 0x00000002;
        public const Int32 ServiceNoChange = unchecked((int)0xffffffff);
        public const Int32 SCStatusProcessInfo = 0;
        public const Int32 FailureRestartDelay = 0;

        public enum ServiceState
        {
            ServiceStopped = 0x00000001,
            ServiceStartPending = 0x00000002,
            ServiceStopPending = 0x00000003,
            ServiceRunning = 0x00000004,
            ServiceContinuePending = 0x00000005,
            ServicePausePending = 0x00000006,
            ServicePaused = 0x00000007
        }

        public enum SCActionType
        {
            SCActionNone = 0,
            SCActionRestart = 1,
            SCActionReboot = 2,
            SCActionRunCommand = 3
        }

        public enum ServiceConfig2InfoLevel
        {
            ServiceConfigDescription = 0x00000001, // The lpBuffer parameter is a pointer to a SERVICE_DESCRIPTION structure.
            ServiceConfigFailureActions = 0x00000002, // The lpBuffer parameter is a pointer to a ServiceFailureActions structure.
            ServiceConfigDelayedAutoStartInfo = 0x00000003
        }

        [Flags]
        public enum ScmAccess
        {
            StandardRightsRequired = 0xF0000,
            SCManagerConnect = 0x00001,
            SCManagerCreateService = 0x00002,
            SCManagerEnumerateService = 0x00004,
            SCManagerLock = 0x00008,
            SCManagerQueryLockStatus = 0x00010,
            SCManagerModifyBootConfig = 0x00020,
            SCManagerAllAccess = StandardRightsRequired |
                SCManagerConnect |
                SCManagerCreateService |
                SCManagerEnumerateService |
                SCManagerLock |
                SCManagerQueryLockStatus |
                SCManagerModifyBootConfig
        }

        [Flags]
        public enum ServiceAccess
        {
            StandardRightsRequired = 0xF0000,
            ServiceQueryConfig = 0x00001,
            ServiceChangeConfig = 0x00002,
            ServiceQueryStatus = 0x00004,
            ServiceEnumerateDependents = 0x00008,
            ServiceStart = 0x00010,
            ServiceStop = 0x00020,
            ServicePauseContinue = 0x00040,
            ServiceInterrogate = 0x00080,
            ServiceUserDefinedControl = 0x00100,
            ServiceAllAccess = (StandardRightsRequired |
                ServiceQueryConfig |
                ServiceChangeConfig |
                ServiceQueryStatus |
                ServiceEnumerateDependents |
                ServiceStart |
                ServiceStop |
                ServicePauseContinue |
                ServiceInterrogate |
                ServiceUserDefinedControl)
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public Int32 dwServiceType;
            public ServiceState dwCurrentState;
            public Int32 dwControlsAccepted;
            public Int32 dwWin32ExitCode;
            public Int32 dwServiceSpecificExitCode;
            public Int32 dwCheckPoint;
            public Int32 dwWaitHint;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct QueryServiceConfigStruct
        {
            public Int32 ServiceType;
            public Int32 StartType;
            public Int32 ErrorControl;
            public IntPtr BinaryPathName;
            public IntPtr LoadOrderGroup;
            public Int32 TagID;
            public IntPtr Dependencies;
            public IntPtr StartName;
            public IntPtr DisplayName;
        }              

        [StructLayout(LayoutKind.Sequential)]
        public struct SCAction
        {
            [MarshalAs(UnmanagedType.U4)]
            public SCActionType Type;
            [MarshalAs(UnmanagedType.U4)]
            public Int32 Delay;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceFailureActions
        {
            [MarshalAs(UnmanagedType.U4)]
            public Int32 dwResetPeriod;
            [MarshalAs(UnmanagedType.LPStr)]
            public String lpRebootMsg;
            [MarshalAs(UnmanagedType.LPStr)]
            public String lpCommand;
            [MarshalAs(UnmanagedType.U4)]
            public Int32 cActions;
            public IntPtr lpsaActions;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatusProcess
        {
            public Int32 dwServiceType;
            public Int32 dwCurrentState;
            public Int32 dwControlsAccepted;
            public Int32 dwWin32ExitCode;
            public Int32 dwServiceSpecificExitCode;
            public Int32 dwCheckPoint;
            public Int32 dwWaitHint;
            public Int32 dwProcessId;
            public Int32 dwServiceFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ServiceDelayedAutoStartInfo
        {
            public bool fDelayedAutostart;
        }

        //
        // imported functions
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeServiceHandle OpenSCManager(string machineName, string databaseName, ScmAccess dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "CreateServiceW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeServiceHandle CreateService(
            SafeServiceHandle hSCManager,
            string lpServiceName,
            string lpDisplayName,
            ServiceAccess dwDesiredAccess,
            Int32 dwServiceType,
            Int32 dwStartType,
            Int32 dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            Int32 lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeServiceHandle OpenService(SafeServiceHandle hSCManager, string lpServiceName, ServiceAccess dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DeleteService(SafeServiceHandle hService);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool QueryServiceStatus(SafeServiceHandle hService, out ServiceStatus serviceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool QueryServiceStatusEx(SafeServiceHandle hService, Int32 infoLevel, out ServiceStatusProcess lpBuffer, Int32 cbBufSize, out Int32 pcbBytesNeeded);

        public static bool QueryServiceStatusEx(SafeServiceHandle hService, ref ServiceStatusProcess statusProcess)
        {
            Int32 requiredSize;
            return QueryServiceStatusEx(hService, SCStatusProcessInfo, out statusProcess, Marshal.SizeOf(statusProcess), out requiredSize);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool ControlService(SafeServiceHandle hService, Int32 controlCode, out ServiceStatus serviceStatus);

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig2")]
        public static extern int ChangeServiceConfig2(SafeServiceHandle hService, ServiceConfig2InfoLevel dwInfoLevel, IntPtr lpInfo);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ChangeServiceConfig(
            SafeServiceHandle service,
            int serviceType,
            int startType,
            int errorControl,
            [MarshalAs(UnmanagedType.LPTStr)] string binaryPathName,
            [MarshalAs(UnmanagedType.LPTStr)] string loadOrderGroup,
            IntPtr tagID,
            [MarshalAs(UnmanagedType.LPTStr)] string dependencies,
            [MarshalAs(UnmanagedType.LPTStr)] string startName,
            [MarshalAs(UnmanagedType.LPTStr)] string password,
            [MarshalAs(UnmanagedType.LPTStr)] string displayName);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int QueryServiceConfig(SafeServiceHandle service, IntPtr queryServiceConfig, int bufferSize, ref int bytesNeeded);
        
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);
        public delegate bool SetConsoleCtrlEventHandler(CtrlType sig);
        public enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
    }
}
