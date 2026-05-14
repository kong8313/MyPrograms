using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using System.Reflection;

namespace Confirmit.CATI.Supervisor.Core.Management
{
    public class VersionInfoProvider
    {
        public static VersionInfo GetVersionInfo()
        {
            var supervisorVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var setupSettings = ServiceLocator.Resolve<ISetupSettings>();

            return new VersionInfo
            {
                DeployDate = setupSettings.ReleaseDate,
                ReleaseNumber = string.IsNullOrEmpty(setupSettings.ReleaseNumber) ? supervisorVersion : setupSettings.ReleaseNumber,
                SupervisorVersion = supervisorVersion,
                ConsolesVersion = setupSettings.InterviewerConsoleVersion
            };
        }
    }
}