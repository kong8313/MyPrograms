using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Reflection;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Core.Misc
{
    /// <summary>
    /// Clas needed to cache ProcessName/ProcessId and rest process properties,
    /// needed because getting ProcessName takes huge amount of CPU.
    /// </summary>
    public class ProcessAndEnvironmentInfo : IProcessAndEnvironmentInfo
    {
        public string ProcessName { get; private set; }

        public int ProcessId { get; private set; }

        public string MachineName { get; private set; }

        public string Version { get; private set; }

        public string Changeset { get; private set; }

        public ProcessAndEnvironmentInfo()
        {
            var process = Process.GetCurrentProcess();

            ProcessId = process.Id;

            ProcessName = process.ProcessName;

            MachineName = BootstrapConfig.IsContainerEnvironment ? Dns.GetHostName() : Environment.MachineName;

            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            Version = version.ToString();

            Changeset = (version.Revision + 65536).ToString(CultureInfo.InvariantCulture);
        }
    }
}