using System;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    public class DotNetVersion
    {
        public string Release { get; set; }
        public string Servicing { get; set; }
        public string Version { get; set; }
    }

    public class ConsoleDescription
    {
        public string MachineName { get; set; }

        public string ConsoleVersion { get; set; }

        public string LocalTimezoneName { get; set; }

        public DateTime LocalTime { get; set; }

        /// <summary>
        /// Comma separated list of console machine IP's
        /// </summary>
        public string IPAddresses { get; set; }

        public int ProcessId { get; set; }

        public string InternetExplorerVersion { get; set; }

        public string OperatingSystemDescription { get; set; }

        public string OperatingSystemVersion { get; set; }

        public DotNetVersion DotNetVersion { get; set; }
    }
}
