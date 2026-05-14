using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using Confirmit.CATI.Installation.Common.Interfaces;
using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility
{
    public class ProcessManager : IProcessManager
    {
        #region Implementation of IProcessManager

        public void KillProcessTree(Process process, ILogger logger)
        {
            if (process == null) return;

            var list = new List<Process>();
            GetProcessAndChildren(Process.GetProcesses(), process, list, 1);

            foreach (Process p in list)
            {
                try
                {
                    p.Kill();
                }
                catch (Exception ex)
                {
                    logger.WriteLog("En exception catched while killing a process: " + ex.Message);
                }
            }
        }

        #endregion

        private static int GetParentProcessId(Process p)
        {
            var parentId = 0;
            try
            {
                var mo = new ManagementObject("win32_process.handle='" + p.Id + "'");
                mo.Get();
                parentId = Convert.ToInt32(mo["ParentProcessId"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                parentId = 0;
            }
            return parentId;
        }

        private static void GetProcessAndChildren(Process[] plist, Process parent, List<Process> output, int indent)
        {
            foreach (Process p in plist)
            {
                if (GetParentProcessId(p) == parent.Id)
                {
                    GetProcessAndChildren(plist, p, output, indent + 1);
                }
            }
            output.Add(parent);
        }

    }
}