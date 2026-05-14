using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Permissions;

using Microsoft.Win32.SafeHandles;

namespace Confirmit.CATI.WindowsServiceTools
{
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public sealed class SafeServiceHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        // Called by P/Invoke marshaler 
        public SafeServiceHandle()
            : base(true)
        {
        }

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        override protected bool ReleaseHandle()
        {
            try
            {
                return CloseServiceHandle(handle);
            }
            catch
            {
                return false;
            }
        }
    }    
}
